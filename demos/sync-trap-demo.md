# How a local branch got merged into dev by clicking Sync

*2026-09-22T10:02:18Z by Showboat 0.6.1*
<!-- showboat-id: d8790e3c-0c63-4522-806f-50d8a58bdb3a -->

A reproduction of a real incident. A feature branch was created off `origin/dev`, which silently set its **upstream to `dev` itself**. Clicking "Sync" in Visual Studio then did its two halves — pull from the upstream, push to the upstream — and the upstream was `dev`. The feature branch landed on `dev` as an ordinary fast-forward: no force, no rejection, nothing to notice.

Everything below is executed live against a throwaway GitHub repo. `demo/sync-trap-dev` plays the role of `dev`; `demo/sync-trap-feature` plays the role of the feature branch.

```bash
cd clone-a && git --version && git remote get-url origin
```

```output
git version 2.51.1.windows.1
https://github.com/realgpasternak/copilot-playground.git
```

## Setup: a shared integration branch

First, a branch to stand in for `dev`, pushed so it exists on the remote.

```bash
cd clone-a &&
git switch -c demo/sync-trap-dev origin/main --quiet &&
echo "shared integration branch" > shared.txt &&
git add shared.txt &&
git commit -q -m "demo: baseline on the shared branch" &&
git push -q -u origin demo/sync-trap-dev 2>/dev/null &&
git log --oneline -1
```

```output
94ec539 demo: baseline on the shared branch
```

## The trap is set at branch-creation time

Now create a feature branch the way everyone does it — off the remote integration branch. Note what git records as the upstream.

```bash
cd clone-a &&
git switch -c demo/sync-trap-feature origin/demo/sync-trap-dev --quiet &&
echo "--- what git recorded as this branch's upstream ---" &&
git config --get-regexp "^branch\.demo/sync-trap-feature\." &&
echo &&
echo "--- and the default for that behaviour ---" &&
echo "branch.autoSetupMerge = $(git config --get branch.autoSetupMerge || echo "(unset, defaults to: true)")"
```

```output
--- what git recorded as this branch's upstream ---
branch.demo/sync-trap-feature.remote origin
branch.demo/sync-trap-feature.merge refs/heads/demo/sync-trap-dev

--- and the default for that behaviour ---
branch.autoSetupMerge = (unset, defaults to: true)
```

That is the whole bug, already present before a single line of work is written. `branch.<feature>.merge` points at the **integration branch**, not at a branch of the same name. This is stock git, not a tool bug: with `branch.autoSetupMerge` at its default, a branch whose start point is a *remote-tracking* ref gets that ref as its upstream.

`git branch -vv` does say so, in the one place nobody reads:

```bash
cd clone-a && git branch -vv --list "demo/*"
```

```output
  demo/sync-trap-dev     94ec539 [origin/demo/sync-trap-dev] demo: baseline on the shared branch
* demo/sync-trap-feature 94ec539 [origin/demo/sync-trap-dev] demo: baseline on the shared branch
```

## Some work on the feature branch

```bash
cd clone-a &&
echo "work in progress" > feature.txt &&
git add feature.txt &&
git commit -q -m "feature: work in progress" &&
git log --oneline -2
```

```output
854406b feature: work in progress
94ec539 demo: baseline on the shared branch
```

Meanwhile a teammate, in a separate clone, advances the shared branch. This matters: it means the upcoming pull cannot fast-forward and has to build a real merge commit.

```bash
cd clone-b &&
git fetch -q origin &&
git switch -q -c demo/sync-trap-dev origin/demo/sync-trap-dev &&
echo "someone else work" > teammate.txt &&
git add teammate.txt &&
git commit -q -m "someone else: unrelated change on the shared branch" &&
git push -q origin demo/sync-trap-dev &&
git log --oneline -1
```

```output
8836002 someone else: unrelated change on the shared branch
```

## Sync, first half: the pull

Back on the feature branch, click Sync. Its first half is a pull from the upstream — and the upstream is the integration branch, so this merges the shared branch *into* the feature branch. With `pull.rebase=false` that produces a merge commit.

```bash
cd clone-a &&
git switch -q demo/sync-trap-feature &&
echo "pull.rebase = $(git config --get pull.rebase)" &&
echo &&
git pull --no-rebase --no-edit origin
```

```output
pull.rebase = false

From https://github.com/realgpasternak/copilot-playground
   94ec539..8836002  demo/sync-trap-dev -> origin/demo/sync-trap-dev
Merge made by the 'ort' strategy.
 teammate.txt | 1 +
 1 file changed, 1 insertion(+)
 create mode 100644 teammate.txt
```

```bash
cd clone-a &&
git log --oneline --graph -4 &&
echo &&
echo "--- the merge commit and its parents ---" &&
git log -1 --format="%h  %s%nparents: %p" HEAD
```

```output
*   2c927d0 Merge branch 'demo/sync-trap-dev' of https://github.com/realgpasternak/copilot-playground into demo/sync-trap-feature
|\  
| * 8836002 someone else: unrelated change on the shared branch
* | 854406b feature: work in progress
|/  
* 94ec539 demo: baseline on the shared branch

--- the merge commit and its parents ---
2c927d0  Merge branch 'demo/sync-trap-dev' of https://github.com/realgpasternak/copilot-playground into demo/sync-trap-feature
parents: 854406b 8836002
```

Same shape as the real incident: a merge commit whose first parent is the feature work and whose second parent is the then-tip of the shared branch.

And that second parent is exactly why nothing downstream objects. The remote tip is now an **ancestor** of the local HEAD, which makes any push of HEAD onto that branch a plain fast-forward:

```bash
cd clone-a &&
if git merge-base --is-ancestor origin/demo/sync-trap-dev HEAD; then
  echo "origin/demo/sync-trap-dev IS an ancestor of HEAD -> a push would fast-forward"
else
  echo "not an ancestor -> a push would be rejected"
fi
```

```output
origin/demo/sync-trap-dev IS an ancestor of HEAD -> a push would fast-forward
```

## Sync, second half: the push

Here the two tools part ways, and the difference is one config value: `push.default`.

The git CLI defaults to `simple`, which notices that the upstream has a different *name* from the current branch and refuses:

```bash
cd clone-a && git -c push.default=simple push; echo "exit code: $?"
```

```output
fatal: The upstream branch of your current branch does not match
the name of your current branch.  To push to the upstream branch
on the remote, use

    git push origin HEAD:demo/sync-trap-dev

To push to the branch of the same name on the remote, use

    git push origin HEAD

To avoid automatically configuring an upstream branch when its name
won't match the local branch, see option 'simple' of branch.autoSetupMerge
in 'git help config'.

exit code: 128
```

Note git's own hint: it recommends `branch.autoSetupMerge=simple` to stop the upstream being mis-set in the first place.

But `simple` is only one of the available semantics. `push.default=upstream` pushes to the configured upstream ref regardless of its name — and that is the behaviour a "Sync" button implements, because pushing to the thing you just pulled from is the entire premise of a combined pull+push. Same command, same repo, one config value different:

```bash
cd clone-a && git -c push.default=upstream push; echo "exit code: $?"
```

```output
To https://github.com/realgpasternak/copilot-playground.git
   8836002..2c927d0  demo/sync-trap-feature -> demo/sync-trap-dev
exit code: 0
```

That is the incident, in one line of output: the local branch on the left, `demo/sync-trap-dev` on the right. The `..` (rather than `+`) marks it as a fast-forward — no force, no rejection, no branch protection to trip, nothing in the output that reads as alarming.

## The damage

The feature work is now on the shared branch, with no pull request and no review:

```bash
cd clone-a &&
git fetch -q origin &&
echo "--- commits added to the shared branch ---" &&
git log --oneline 8836002..origin/demo/sync-trap-dev &&
echo &&
echo "--- files now on the shared branch ---" &&
git ls-tree --name-only origin/demo/sync-trap-dev
```

```output
--- commits added to the shared branch ---
2c927d0 Merge branch 'demo/sync-trap-dev' of https://github.com/realgpasternak/copilot-playground into demo/sync-trap-feature
854406b feature: work in progress

--- files now on the shared branch ---
.github
feature.txt
shared.txt
teammate.txt
```

```bash
cd clone-a &&
echo "--- what actually exists on the remote ---" &&
git ls-remote --heads origin "refs/heads/demo/*"
```

```output
--- what actually exists on the remote ---
2c927d0fd28e9e1d644e3d5ac7e0600197e33142	refs/heads/demo/sync-trap-dev
```

Note what is *not* there: `demo/sync-trap-feature` was never created on the remote. Only the shared branch moved. From the outside it looks like someone pushed straight to the integration branch.

## The fix

Set `branch.autoSetupMerge` to `simple`. It sets up tracking only when the new branch has the **same name** as the remote branch it started from — so branching off an integration branch leaves no upstream at all, and the first push has to be explicit:

```bash
cd clone-a &&
git config branch.autoSetupMerge simple &&
git switch -c demo/sync-trap-feature2 origin/demo/sync-trap-dev --quiet &&
echo "--- upstream config for a branch created the same way, with the fix in place ---" &&
git config --get-regexp "^branch\.demo/sync-trap-feature2\." || echo "(none - no upstream was configured)"
```

```output
--- upstream config for a branch created the same way, with the fix in place ---
(none - no upstream was configured)
```

No upstream, so Sync has nowhere to push and has to ask. The same command that silently rewrote the shared branch a moment ago now refuses to guess:

```bash
cd clone-a && git -c push.default=upstream push; echo "exit code: $?"
```

```output
fatal: The current branch demo/sync-trap-feature2 has no upstream branch.
To push the current branch and set the remote as upstream, use

    git push --set-upstream origin demo/sync-trap-feature2

To have this happen automatically for branches without a tracking
upstream, see 'push.autoSetupRemote' in 'git help config'.

exit code: 128
```

Same `push.default=upstream` semantics, same repo, same command — and now it refuses to guess. The trap is closed at the source.

```bash
git config --global branch.autoSetupMerge simple
```

To repair a branch that already has the wrong upstream, give it the right one:

```bash
git push -u origin <branch-name>
```

## Summary

Three ingredients, none of which looks like a problem on its own:

| Ingredient | Value in the incident |
|---|---|
| `branch.<feature>.merge` | `refs/heads/dev` — set automatically at branch creation |
| `pull.rebase` | `false` — so the pull builds a merge commit with the old `dev` tip as a parent |
| push semantics | pushes to the upstream ref regardless of name |

The merge commit is what makes it silent. Because it carries the old integration tip as its second parent, the push is a legitimate fast-forward. Every safety net in git and on the server is looking for a non-fast-forward, and this is not one.

Worth noting separately: an unprotected integration branch is what turns a local config mistake into a rewritten shared branch. Branch protection on `dev` would have rejected this push regardless of the config.

## On reproducibility

Showboat documents are normally re-runnable with `showboat verify`, which re-executes every block and diffs the output. **This document is not**, deliberately. It was run live against a real remote, so its blocks create and mutate remote branches; a second run would hit branches that already exist, and re-running the push block against a shared repo is not something a verifier should do by surprise. The transcript above is a record of a single real execution, not a replayable script.

A variant using a local bare repository as `origin` would be fully verifiable — the trap lives entirely in local config and refspec resolution, and the remote contributes nothing to it.

The demo branches created here were deleted after this run.
