# Project descriptor and scaffold files

## `.claude/game-project.json`

Lives at the project root, outside `Assets/`, so Unity never imports it. Written by game-bootstrap, read by playtest, ship and asset-forge. Keep it flat and JSON-strict.

    {
      "name": "My Game",
      "company": "My Studio",
      "namespace": "MyGame",
      "bundleId": "com.mystudio.mygame",
      "platforms": ["ios", "android"],
      "orientation": "portrait",
      "referenceResolution": [1080, 1920],
      "canvasScalerMode": "Expand",
      "targetFps": 60,
      "budgets": { "frameMs": 16.7, "p95FrameMs": 20, "gcCollectionsPerSession": 0, "maxDrawCalls": 150 },
      "save": { "scheme": "local", "schemaVersion": 1 },
      "iap": false,
      "ads": "none",
      "analytics": { "consentGated": true, "sdks": [] },
      "font": { "asciiOnly": true },
      "sortOrder": { "hud": 6, "popup": 20, "toast": 30 },
      "build": { "helper": "request-file", "outputRoot": "Builds" },
      "lastShipped": { "version": "0.0.0", "buildNumber": 0, "androidVersionCode": 0 },
      "playtest": { "deviceSizes": [[1170, 2532], [1290, 2796], [2048, 2732]], "autoPlayHook": "", "tapHook": "" },
      "notes": ""
    }

`playtest.autoPlayHook` is `Type.Property` for a bot switch the game exposes, `tapHook` is `Type.Method` that taps a named button; both optional.

## Scaffold files

| File | Section | Purpose |
|---|---|---|
| `Assets/_Game/Scripts/Core/GameConstants.cs` | 2, 2c, 7 | Every player-visible number, the sort-order ladder, a safe exponential price curve |
| `Assets/_Game/Scripts/Core/SaveSystem.cs` | 5, 13 | JSON save with schema stamp in the constructor, atomic write, corrupt-save quarantine, migration hook, documented reset |
| `Assets/_Game/Scripts/Core/AnalyticsEvents.cs` | 10 | The declared event schema as constants plus the `All` list the wrapper validates against |
| `Assets/_Game/Scripts/Core/Analytics.cs` | 10 | One wrapper, SDK sinks behind an interface, consent gate, batch-by-reason with flush |
| `Assets/_Game/Scripts/Core/Consent.cs` | 10 | Persisted consent, install stamp outside the save, install age |
| `Assets/_Game/Scripts/Core/ObjectPool.cs` | 14 | Prefab-keyed pool with safe fallback and kill switch |
| `Assets/_Game/Scripts/UI/SafeAreaFitter.cs` | 11 | Anchors a RectTransform to the safe area, reapplies on rotation and size change |
| `Assets/Editor/GameTools/BuildRequestPoller.cs` | build | Request-file build helper with Tools/Ship menu items and a command-line entry |
| `CLAUDE.md` | all | Project rules for Claude sessions, with placeholders filled |
| `.claude/game-project.json` | all | The descriptor above |
| `.gitignore` | build | Standard Unity ignores plus Builds and Playtest output |

Placeholders substituted by `scripts/scaffold.py`: `__GAME__`, `__COMPANY__`, `__NAMESPACE__`, `__BUNDLE__`.
