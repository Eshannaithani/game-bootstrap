# __GAME__ (Unity)

Company __COMPANY__, bundle id __BUNDLE__, namespace `__NAMESPACE__`. Project descriptor: `.claude/game-project.json` (read it first; every skill does).

## Playbook rules (from the Mobile Game SOP)

- Player-visible numbers come from `Assets/_Game/Scripts/Core/GameConstants.cs`, never from literals. Grep for hardcoded figures before a release.
- UI is authored in the editor and bound at runtime. Never build UI in code except as a fresh-scene fallback.
- Every screen-space canvas has a `SafeArea` child with `SafeAreaFitter`; HUD lives inside it. One CanvasScaler mode everywhere (see the descriptor).
- Saves go through `SaveSystem` only: schema stamp, atomic write, quarantine. New ambiguous fields bump `SaveSchemaVersion` and get a migration.
- Analytics go through `Analytics.Track` with names from `AnalyticsEvents`. Adding an event is a two-file change: the constant and the SDK allow-list asset. Nothing collects before `Consent.Granted`.
- Every string the player can see is plain ASCII.
- Sort order: HUD 6, popups 20, toast 30. Modals register with the coordinator.
- Pool anything spawned more than a few times a minute through `ObjectPool`.
- Verify by rendering: edit-mode capture and play-mode capture at device resolution before believing a UI change. Confirm outcomes, not calls.

## Tooling rules

- Only one Unity editor open at a time; MCP calls route to the wrong project otherwise. Guard RunCommands with an `Application.dataPath` check.
- Builds go through the request file (`Tools/Ship/...` menu or `Builds/build_request.json`), never `BuildPipeline` from a tool call.
- Play mode driven from tools freezes when the editor loses focus: set `Application.runInBackground = true` after entering play.
- Skills: `/playtest` for verified play sessions, `/ship` for audits and builds, `/asset-forge`, `/sprite-forge`, `/audio-forge` for generated content.
