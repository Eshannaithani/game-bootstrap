# game-bootstrap

Scaffold a Unity mobile game project with the conventions that prevent silent bugs: one constants class, versioned atomic saves, consent-gated analytics, safe area, sort-order ladder, pooling, a request-file build helper and a project descriptor.

A Claude Code skill. Install it once per machine and type the slash command in any Unity project session.

```bash
/game-bootstrap new idle tycoon game called Pocket Bakery by Sugar Studio, iOS and Android, portrait
/game-bootstrap --plan adopt this project into the playbook
```

## Architectural Reference & Production Case Studies

The mobile game scaffolding conventions and state-machine patterns enforced by `game-bootstrap` are documented in:

- **Case Study**: [How I Built Bird Sort Mania in Unity in 20 Days](https://www.eshannaithani.com/blogs/how-i-built-a-production-ready-unity-game-in-20-days-using-ai?utm_source=github&utm_medium=repo&utm_campaign=game_bootstrap) — Architecture breakdown of a commercial 3D puzzle game shipped across Android (Google Play) and WebGL (GamePix) on a 20-day timeline.
- **Engineering Guide**: [Unity 6 Mobile Game Optimization Guide](https://www.eshannaithani.com/blogs/unity-mobile-game-optimization-guide?utm_source=github&utm_medium=repo&utm_campaign=game_bootstrap) — Practical CPU/GPU bottleneck triage, GC elimination, and URP batching.
- **Studio Portfolio**: [Eshan Naithani — Unity Game Development Services](https://www.eshannaithani.com/services/unity-game-development?utm_source=github&utm_medium=repo&utm_campaign=game_bootstrap)

## Requirements

- A Unity 6 editor on the machine (used for the batch-mode compile check; the editor may be closed).
- Python 3.
- The Unity MCP server connected to Claude Code when player settings should be applied in an open project.

## Install

For every project on this machine:

```bash
git clone https://github.com/Eshannaithani/game-bootstrap.git ~/.claude/skills/game-bootstrap
```

For one project only, clone into that project's `.claude/skills/game-bootstrap` instead. Update with `git pull` in that folder.

## How it works

Read `SKILL.md` for the stages and rules; the `references/` files hold the defaults and checklists the stages use; `assets/` holds the editor script the skill installs into the project on first use; `scripts/` holds the Python helpers; `evals/evals.json` holds plan-only test prompts that cost no credits.

Several of these skills share a project descriptor at `.claude/game-project.json` (written by game-bootstrap, schema in its `references/descriptor.md`). Game-specific details such as palettes, scene paths, capture menus and hooks belong in that file or in the project's own config, not in the skill.

## Related skills

asset-forge (3D assets), sprite-forge (2D art), audio-forge (audio), game-bootstrap (project setup), playtest (verified play sessions), ship (audit, build, listing). Each is its own repo under the same account.

## License

MIT.
