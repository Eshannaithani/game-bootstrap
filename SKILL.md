---
name: game-bootstrap
description: >-
  Set up a new Unity mobile game project, or bring an existing one up to the build-and-ship playbook, with the conventions that prevent the expensive silent bugs: one constants class for every player-visible number, a versioned atomic save system, consent-gated analytics with a declared event schema, a safe-area fitter and one canvas-scaler rule, a sort-order ladder, prefab-keyed pooling, a request-file build helper, a project descriptor and a project CLAUDE.md. Use it whenever the user starts a new game or Unity project, says "new game", "set up the project", "scaffold", "bootstrap", "apply the SOP", or wants an existing project adopted into the playbook; also when they ask where economy numbers, saves, analytics or safe-area code should live. Not for generating art, building players or playtesting.
---

# game-bootstrap

Turns the Mobile Game SOP into files on day one, when it is cheap, instead of a retrofit at month six. Every scaffold file carries a comment saying what broke when the rule was skipped, so the next person keeps it.

## Modes

- **New project.** The path does not exist yet. If the user names a Unity version, create the project with the editor's command line (`scripts/compile_check.sh --create`), otherwise ask them to create it in Unity Hub first. Then scaffold.
- **Adopt.** The project exists. Install only what is missing, never overwrite, and report what already differs from the scaffold so the user can merge by hand.
- **Plan only** (`--plan` or "plan only"): interview and rundown, then print the file list and settings that would be written. No files touched.

## Stage 1: Interview and rundown (one confirmation)

Ask for anything the brief did not give, in one message, with sensible defaults shown:

1. Name, company, bundle id, C# namespace (default: the name in PascalCase).
2. Platforms (default iOS and Android), orientation (default portrait), reference resolution (default 1080x1920) and canvas-scaler mode (default Expand; the SOP's tablet table is in `references/sop-rules.md` section 11).
3. Target frame rate (default 60).
4. The save-architecture questions from SOP section 13. IAP? Long progression? Multi-device? Competitive? Cross-platform? Purchasable currency? The answers pick local, platform cloud or backend, and the rundown says which and why.
5. Analytics SDKs planned (none is fine; the wrapper still gates on consent).

Print the rundown: descriptor values, save scheme, file list from `references/descriptor.md`, settings that will change in PlayerSettings. Wait for "go". In plan-only mode, stop here.

## Stage 2: Write

1. `python3 scripts/scaffold.py <project> --name ... --company ... --namespace ... --bundle-id ...` copies `assets/scaffold/` into the project with placeholders substituted. Existing files are never overwritten: the script writes `<file>.bootstrap-new` beside them and lists the differences.
2. Write `.claude/game-project.json` (schema in `references/descriptor.md`) with the interview answers. Every other skill (playtest, ship, asset-forge) reads this file.
3. Write the project `CLAUDE.md` from the template with the game's values, or append the "Playbook rules" section if a CLAUDE.md exists.
4. Player settings. If the Unity editor is open on this project (confirm with `Application.dataPath` through `Unity_RunCommand`), set company, product, bundle ids, default orientation and target frame rate through a RunCommand. If not, leave them for the next editor session and say so; editing ProjectSettings.asset by hand while the editor is closed is allowed but easy to get wrong.

## Stage 3: Verify

- Editor open: refresh assets, then read the console for compile errors before reporting. A scaffold that does not compile is worse than none.
- Editor closed: `scripts/compile_check.sh <project>` runs a batch-mode import and greps the log for `error CS`. It takes a few minutes; run it in the background and read the result.
- Print what was created, what was skipped because it existed, and the three next steps: author the first screen inside the SafeArea object, register the first analytics events in both the constants file and the SDK settings asset, run `/playtest` once the first scene plays.

## Rules

- Never overwrite. Adopt mode is additive.
- Everything player-visible reads from `GameConstants`; the scaffold's example values are placeholders and the rundown says so.
- Plain ASCII in every string the scaffold writes. The SOP's font rule bites at build time, not at edit time.
- Keep the "why" comments. They are the SOP in the place where the next edit happens.
- The build helper writes a request file and builds on the next editor update. Calling BuildPipeline from an MCP call fails with "user interactions not supported".

## Reference map

- `references/sop-rules.md`: the playbook condensed by section number. Read for the interview and whenever a rule needs its reason.
- `references/descriptor.md`: `game-project.json` schema, defaults, and the scaffold file list with what each file is for.
- `assets/scaffold/`: the files. `scripts/scaffold.py` copies them; `scripts/compile_check.sh` compiles a closed project.
