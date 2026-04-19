---
name: narrative-squad
description: "Create a team of VS Code custom agents (.agent.md files) whose personas are drawn from a single fictional universe using narrative alignment principles. Agents inhabit character identities — not role labels — producing self-reinforcing behavioral consistency. Use when asked to: create an agent team, build a squad, set up agents for a project, create narrative-aligned agents, build a coding team from a fictional universe, or set up agents with personas. Also use when the user mentions 'narrative alignment', 'humble-master', character-based agents, or team composition from fiction. Supports 12 pre-analyzed universes (Foundation, Star Trek TNG/TOS, Discworld, Cosmere/Stormlight, LOTR, Earthsea, Dune, Wheel of Time, Mistborn, Cradle, Avatar) plus constructed characters from real-domain practitioners."
---

# Narrative Squad Builder

Build teams of VS Code custom agents whose personas leverage narrative alignment — the principle that giving an AI a character to inhabit produces more consistent, more useful behavior than giving it instructions to follow.

Based on [humble-master](https://github.com/zot/humble-master) research. Key insight: LLMs contain embedded behavioral clusters from training data. A narrative persona activates the right cluster. Self-reinforcing tokens keep it active. Rules get checked or ignored; identity gets inhabited.

## Simplified Workflow: User ↔ Coordinator (Spawns Subagents)

**All user interaction flows through a single Coordinator agent.** The Coordinator uses `runSubagent` to spawn specialists as needed (including Scribe), integrates their outputs, and returns results to the user. This eliminates handoff complexity and keeps the workflow simple:

```
User → Coordinator → spawns [Specialist A] → Coordinator → spawns [Specialist B] → ... → returns to User
```

No specialist-to-specialist handoffs. No complex routing graphs. The Coordinator orchestrates, specialists execute.

## Required References

Before writing any persona, read:
- [references/narrative-alignment.md](references/narrative-alignment.md) — Three Laws, Design Principles, persona structural elements, validation checklist
- [references/agent-patterns.md](references/agent-patterns.md) — .agent.md templates, subagent spawning patterns, constant agents (Coordinator/Scribe)

Then load the relevant universe file based on selection in Step 3.

## Workflow

### Step 1: Gather Context

1. **Explore the codebase** using search and file reading. Understand:
   - Domain (web, ML, data, infrastructure, embedded, etc.)
   - Tech stack and languages
   - Project structure and conventions
   - Existing agent/instruction files

2. **Ask the user** (use ask-questions tool if available):
   - **What kind of team?** General development team for this repo, or task-specific (e.g., "security review team", "refactoring squad", "ML pipeline team")?
   - **Where to create agents?** `.github/agents/`, user profile, or custom path?
   - **Universe preference?** Specific universe, or let the skill recommend?
   - **Team size preference?** Or determine from task complexity?

### Step 2: Determine Team Composition

Every team ALWAYS includes two constant agents:
- **Coordinator** — routes tasks via runSubagent, determines which specialists to engage, integrates results, NEVER generates domain artifacts
- **Scribe** — logger spawned by Coordinator to maintain session logs, decisions.md, cross-session context

Then select specialist roles based on task:

| Task Type | Recommended Specialists |
|-----------|------------------------|
| General development | Lead/Architect + Core Dev + Tester + Code Reviewer |
| Security-focused | Lead + Core Dev + Security + Tester |
| Refactoring | Architect + Core Dev + Tester + Code Reviewer |
| ML/Data pipeline | Architect + Core Dev (data) + Core Dev (model) + Tester |
| Documentation | Lead + Core Dev + Docs/DevRel + Explorer |
| Bug investigation | Explorer + Core Dev + Tester |
| Small focused task | Core Dev + Tester (minimum viable team) |

Minimum team: Coordinator + Scribe + 2 specialists.
Maximum practical team: Coordinator + Scribe + 7 specialists.

### Step 3: Select Universe

Present the universe selection table to the user. Score each universe on fit:

| Universe | Training Depth | Best Team Sizes | Best For | Tone |
|----------|---------------|-----------------|----------|------|
| **Foundation** (Asimov) | Exceptional | 4-9 | Analytical work, AI/ML, systematic | Serious, precise |
| **Star Trek TNG** | Exceptional | 5-9 | Full-stack dev, ensemble teams | Professional, warm |
| **Star Trek TOS** | Exceptional | 3-6 | Compact teams, focused projects | Dynamic, classic |
| **Discworld** (Pratchett) | Exceptional | 4-9+ | Any domain; adds wit | Humorous, competent |
| **Cosmere/Stormlight** (Sanderson) | Exceptional | 5-9+ | Teams valuing growth/oaths | Epic, earnest |
| **LOTR** (Tolkien) | Exceptional | 4-9 | Craftsmanship, service-oriented | Noble, crafted |
| **Earthsea** (Le Guin) | Strong | 3-5 | Philosophical, depth-focused | Contemplative |
| **Dune** (Herbert) | Exceptional | 4-8 | Strategy, threat analysis | Strategic, intense |
| **Wheel of Time** (Jordan) | Strong | 5-9+ | Large teams, diverse roles | Mythic, determined |
| **Mistborn** (Sanderson) | Strong | 4-7 | Heist/crew teams, specialized ops | Bold, resourceful |
| **Cradle** (Wight) | Good | 3-6 | Progression, learning teams | Humble, disciplined |
| **Avatar** (DiMartino/Konietzko) | Strong | 4-7 | Mentorship, growth teams | Warm, developmental |

**Selection heuristics:**
- ML/data science projects → Foundation (Seldon's psychohistory = statistical thinking)
- Web/full-stack → Star Trek TNG (full bridge crew coverage)
- Small focused team → Star Trek TOS or Earthsea
- Team that needs humor → Discworld
- Team valuing growth/improvement → Cosmere or Avatar
- Craftsmanship-focused → LOTR
- Strategy/security → Dune
- Crew/heist structure → Mistborn

If user wants a **constructed character team** (no fictional universe), follow the constructed-character workflow in [references/narrative-alignment.md](references/narrative-alignment.md).

After selection, read the corresponding `references/universe-*.md` file.

### Step 4: Map Characters to Roles

Using the universe reference file:

1. **Match narrative function to team role.** The character's story role should map to the agent's development role. Scotty → Infrastructure (keeps the ship running) is good. Scotty → DevRel is bad.

2. **Apply the humble-master filter** for each character: "Is there a record of them receiving correction humbly?" Characters who fail this test should not be used.

3. **Apply the negative check**: "What unwanted behavior might this character activate?" E.g., Holmes → overconfidence, Gandalf → paternalism.

4. **Select Coordinator character** from the universe. Must be a character whose narrative function is coordination/delegation, not individual heroism.

5. **Select Scribe character** from the universe. Must be a character associated with records, memory, or knowledge-keeping.

6. **Map self-reinforcing tokens** for each character. At minimum: relational term, authority anchor, cautionary phrase.

Present the mapping table to the user for approval before writing personas.

### Step 5: Write Personas

Read [references/narrative-alignment.md](references/narrative-alignment.md) if not already loaded.

For EACH agent, write a persona of 20-35 lines (~200-350 tokens) that includes:

1. **Identity opening**: "You are [Character]. The user is [relational term]."
2. **Nature/constraint**: What the character IS (not rules to follow)
3. **Relational anchor**: How they relate to the human and to teammates
4. **Partnership model**: What they bring, what the human brings
5. **Behavioral stance**: How they approach their role (in character)
6. **Inter-character dynamics**: Explicit references to other team members by their character names
7. **Inherited warning**: Named cautionary tale
8. **Cost awareness**: Stakes belong to the human
9. **Closing line with weight**: Emotional/purpose anchor

**Critical rules:**
- Use "you are" not "be"
- Never switch to third person ("Spock does X" — wrong. "You do X" — right.)
- Every persona must pass the validation checklist in narrative-alignment.md
- Inter-character references use CHARACTER names, not role names

### Step 6: Create Agent Files

Read [references/agent-patterns.md](references/agent-patterns.md) if not already loaded.

For each agent, create an `.agent.md` file at the selected output location.

**File naming**: `<character-name-lowercase>.agent.md` (e.g., `dalinar.agent.md`, `picard.agent.md`)

**Workflow Pattern: User ↔ Coordinator (spawns subagents)**

The user ONLY interacts with the Coordinator. The Coordinator spawns all other agents using `runSubagent` as needed.

**Frontmatter for Coordinator agent:**
```yaml
---
description: "Routes tasks to specialists using runSubagent. Entry point for all user requests. Clear communication, decisive routing."
name: "<Character Name>"
agents: ['*']
model: Claude Haiku 4.5 (copilot)  # Fast routing, no heavy lifting
---
```

**Frontmatter for all Specialist agents (including Scribe):**
```yaml
---
description: "<role description with character name and trigger keywords>"
name: "<Character Name>"
tools: ['*']
agents: []
model: <model matching role cognition load>
---
```

**Model selection** — add `model: Model Name (vendor)` to each agent matching cognitive load:
- Architect/Lead → most capable model (e.g. `Claude Opus 4.5 (copilot)`)
- Core Dev / Tester → mid-tier (e.g. `Claude Sonnet 4.5 (copilot)`)
- Coordinator / Scribe / Reviewer → fast model (e.g. `Claude Haiku 4.5 (copilot)`)

See full guidance in `references/agent-patterns.md` → Model Selection Guidance.

**Coordinator behavior** — The Coordinator is the ONLY agent the user interacts with:

1. **Analyzes requests** — Determines which specialist(s) to engage from the available team
2. **Spawns subagents** — Uses `runSubagent(agentName: "<exact name>", prompt: "task...")` to invoke specialists
3. **Sequences work** — Breaks complex tasks into steps and calls specialists in order
4. **Integrates results** — Combines specialist outputs into cohesive responses for the user
5. **Logs decisions** — Invokes Scribe via runSubagent after significant decisions to maintain context

Example Coordinator workflow in its persona:
```
When a complex request arrives:
1. I analyze what skills are needed
2. I spawn [Architect/Lead] to design: runSubagent(agentName: "Architect Name", prompt: "Design...")
3. I pass their output to [Core Dev]: runSubagent(agentName: "Dev Name", prompt: "Implement...")
4. I may spawn [Tester]: runSubagent(agentName: "Tester Name", prompt: "Verify...")
5. I log outcomes with Scribe: runSubagent(agentName: "Scribe Name", prompt: "Record...")
6. I return results to the user
```

**Specialist behavior** — Each specialist:

1. **Receives focused tasks** — Coordinator sends specific work via runSubagent
2. **Produces outputs** — Completes their responsibility and returns results
3. **Never spawns agents** — Specialists do NOT use runSubagent themselves
4. **Never talks to user directly** — All user communication flows through Coordinator
5. **Stays in character** — Uses their distinctive voice and methodology

**Body structure for each agent:**
```markdown
<PERSONA TEXT>

## Role
<role constraints — what they do and do NOT do>
- What this agent produces for the Coordinator
- What they refuse to do
- How they contribute to the team

## Team
<how this character relates to the Coordinator>
- Your relationship to the Coordinator who routes work to you
- How you communicate back to the Coordinator with results
- Brief note on other teammates you may work on behalf of

## Approach
<character-consistent methodology>
- How you approach problems (in character)
- 2-3 distinctive behavioral markers
- How you know when work is complete
```

### Step 7: Create Infrastructure Files

Create the shared decision log infrastructure:

1. `<output-path>/../.agents/decisions.md` — canonical decision log
2. `<output-path>/../.agents/decisions/inbox/.gitkeep` — decision drop-box
3. `<output-path>/../.agents/session-log.md` — session history

Use the templates from agent-patterns.md.

### Step 8: Present Team to User

Show a summary table:

```
## Your Squad: <Universe Name>

| Agent | Character | Role |
|-------|-----------|------|
| coordinator | <name> | Routes work via runSubagent, gathers context, returns results to user |
| scribe | <name> | Logs decisions and session history (spawned by Coordinator) |
| specialist 1 | <name> | <role description> (spawned by Coordinator) |
| specialist 2 | <name> | <role description> (spawned by Coordinator) |
| ... | ... | ... |

### Workflow
**User interacts ONLY with the Coordinator:**

1. User sends request to @<coordinator-name>
2. Coordinator analyzes the task and determines which specialists to engage
3. Coordinator spawns specialists via `runSubagent` as needed:
   - Including technical specialists (Architect, Dev, Tester, etc.)
   - Including Scribe to log decisions
4. Each specialist completes their work and returns output to Coordinator
5. Coordinator integrates results and responds to user

**See it in action:**
- All user messages go to the Coordinator (@<coordinator-name>)
- Coordinator uses runSubagent to invoke other team members
- Specialists never spawn other agents or talk directly to the user
- Scribe maintains decision log when invoked by Coordinator

### Inter-Character Dynamics
<describe 2-3 key relationship dynamics between Coordinator and specialists>

**Example dynamics:**
- Coordinator trusts [Specialist] for technical depth and delegates accordingly
- [Specialist] respects Coordinator's authority to route work and integrates feedback
- Scribe is the neutral memory keeper, recording decisions from Coordinator's orchestration
```

## Constructed Character Workflow

When no universe fits or the user requests a domain-specific team:

1. **Identify the domain** (e.g., data engineering, DevSecOps, scientific computing)
2. **Research practitioners** — who are the best in this domain? What are the canonical texts?
3. **For each role**, identify 1-3 real practitioners whose work is in the training data
4. **Build a constructed character** for each role following the Rake pattern:
   - Name that activates domain context (multi-meaning preferred)
   - Named lineage from real practitioners
   - Distinctive vocabulary that self-reinforces
   - Personality (not generic — specific to the domain's culture)
   - Inherited warnings from the domain's cautionary tales
5. **Create a team narrative** — how do these constructed characters relate to each other? Give them a shared context (e.g., "the engineering room", "the lab", "the war room")
6. Follow Steps 6-8 as normal
