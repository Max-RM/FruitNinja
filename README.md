# Fruit Ninja Desktop

Desktop port of the Windows Phone 7 / Silverlight Fruit Ninja game, targeting `net8.0-windows` with MonoGame 3.8.2.

## What changed

- Ported from WP7 Silverlight+XNA to MonoGame `Framework.WindowsDX` 3.8.2.1105
- Stubbed WP7-specific APIs (`IsolatedStorage`, `Guide`, `Phone` calls)
- The game renders internally at 800×480 via `RenderTarget2D`, then scales to window with letterboxing (5:3 aspect ratio preserved)
- Resizable window, touch/mouse input with coordinate scaling
- Native ARM32 launcher (`FruitNinja.exe`) for devices without `dotnet` installed; ARM32 ships with the bundled `dotnet-runtime-8.0.6-win-arm`
- `scripts/Publish-All.ps1` publishes `win-x64`, `win-x86`, `win-arm64`, and `win-arm`

## Save files

Game saves, settings, and analytics are stored in:

```
%LOCALAPPDATA%\FruitNinja\FruitNinja\
```

Typically: `C:\Users\<User>\AppData\Local\FruitNinja\FruitNinja\`

| File | Purpose |
|------|---------|
| `setting.dat` | Game settings (binary) |
| `analytics.dat` | Play statistics (binary) |
| `itemsinfo.xml` | Purchased/equipped items |
| `savegame.xml` | Highscores, achievements, full game state |

Delete these files to reset all progress and settings.

## Configuration

Place `FruitNinja.ini` next to the executable to control mouse cursor visibility:

```ini
[Settings]
ShowCursor=1
```

- `ShowCursor=1` — cursor visible over game window (default for x64, x86)
- `ShowCursor=0` — cursor hidden (default for ARM32, ARM64)

## Build

```powershell
dotnet build Fruit_Ninja_MainDLL\FNWP72.csproj -c Release
```

## Publish

All architectures:

```powershell
.\scripts\Publish-All.ps1
```

Single architecture:

```powershell
.\scripts\Publish-win-x64.ps1
.\scripts\Publish-win-arm.ps1
.\scripts\Publish-All.ps1 -Rids win-x64
```

Output goes to:

```text
Publish\FruitNinja_win-x64\
Publish\FruitNinja_win-x86\
Publish\FruitNinja_win-arm64\
Publish\FruitNinja_win-arm\
```

Run `FruitNinja.exe` or `Start-Game.bat` from the needed folder. For `win-arm`, the script copies the bundled `dotnet-runtime-8.0.6-win-arm` runtime and builds a native ARM32 launcher.

## Package

After publishing, create `.zip` archives for distribution:

```powershell
.\scripts\Package-All.ps1                              # all RIDs
.\scripts\Package-All.ps1 -Items win-x64,win-arm        # select RIDs
.\scripts\Package-All.ps1 -Items source                 # source code only
```

Archives go to `Archives\`.

## Files layout

| Path | Description |
|------|-------------|
| `Fruit Ninja v1.1 [file.windowsviet.com].xap` | Original WP7 XAP |
| `Fruit_Ninja_v1.1_APP/` | Original game assets (Content, Images) |
| `Fruit_Ninja_MainDLL/` | Ported C# source code |
| `scripts/` | Publish and package scripts |
| `tools/launcher/` | ARM32 native launcher source (C) |
| `dotnet-runtime-8.0.6-win-arm/` | Bundled .NET 8 ARM32 runtime |
| `Publish/FruitNinja_<rid>/` | Published game per architecture |
| `Publish/<rid>/FruitNinja.exe` | Native launcher (GUI subsystem) |
| `Publish/<rid>/FruitNinja.Debug.exe` | Debug launcher (console subsystem, shows crash logs) |
| `Publish/<rid>/Start-Game.bat` | Fallback launcher |
| `Publish/<rid>/FruitNinja.cmd` | Shortcut to Start-Game.bat |
| `Publish/<rid>/FruitNinja.ini` | Per-architecture config (cursor visibility) |

Ported by MaxRM, TNT ENTERTAINMENT inc organization in 2026 on July 18.
