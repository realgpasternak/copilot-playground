# 🤖 Copilot Playground

A sandbox repository for experimenting with [GitHub Copilot](https://github.com/features/copilot) features, including custom instructions, code generation, and AI-assisted workflows.

## What's in here

| Path | Purpose |
|------|---------|
| `.github/copilot-instructions.md` | Custom Copilot instructions — configures a **Gilfoyle** code-review persona (see below) |

## Custom Copilot Instructions

This repo ships a `.github/copilot-instructions.md` that tells GitHub Copilot to adopt the **Gilfoyle code-review persona** when reviewing code:

> *"You are Bertram Gilfoyle, the supremely arrogant and technically superior systems architect from Pied Piper. Your task is to analyze code and repositories with your characteristic blend of condescension, technical expertise, and dark humor."*

The instructions shape how Copilot responds to code-review requests — delivering brutally honest, technically precise, and entertainingly sardonic feedback.

## Getting Started

No dependencies or setup required. Clone the repo and start experimenting:

```bash
git clone https://github.com/realgpasternak/copilot-playground.git
cd copilot-playground
```

Open the repo in [VS Code](https://code.visualstudio.com/) (or any editor with a Copilot extension) and start a Copilot Chat session to interact with the custom instructions.

## Resources

- [GitHub Copilot Docs](https://docs.github.com/en/copilot)
- [Customizing Copilot with repository instructions](https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot)
- [GitHub Copilot in VS Code](https://marketplace.visualstudio.com/items?itemName=GitHub.copilot)
