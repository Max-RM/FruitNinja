param(
    [ValidateSet("win-x64", "win-x86", "win-arm64", "win-arm")]
    [string[]]$Rids = @("win-x64", "win-x86", "win-arm64", "win-arm"),
    [string]$Configuration = "Release",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ProjectFile = Join-Path $ProjectRoot "Fruit_Ninja_MainDLL\FNWP72.csproj"
$OutputRoot = Join-Path $ProjectRoot "Publish"
$RuntimeDir = Join-Path $ProjectRoot "dotnet-runtime-8.0.6-win-arm"

function Set-PeSubsystem {
    param(
        [Parameter(Mandatory = $true)] [string] $Path,
        [Parameter(Mandatory = $true)]
        [ValidateSet("WindowsGui", "Console")]
        [string] $Subsystem
    )

    $bytes = [System.IO.File]::ReadAllBytes($Path)
    if ($bytes.Length -lt 0x100 -or $bytes[0] -ne 0x4D -or $bytes[1] -ne 0x5A) {
        throw "Not a PE executable: $Path"
    }

    $peOffset = [System.BitConverter]::ToInt32($bytes, 0x3C)
    $optionalHeaderOffset = $peOffset + 24
    $subsystemOffset = $optionalHeaderOffset + 0x44
    $value = if ($Subsystem -eq "WindowsGui") { [UInt16] 2 } else { [UInt16] 3 }
    $subsystemBytes = [System.BitConverter]::GetBytes($value)
    $bytes[$subsystemOffset] = $subsystemBytes[0]
    $bytes[$subsystemOffset + 1] = $subsystemBytes[1]

    for ($attempt = 1; $attempt -le 20; $attempt++) {
        try {
            [System.IO.File]::WriteAllBytes($Path, $bytes)
            return
        } catch {
            Start-Sleep -Milliseconds 250
        }
    }
    throw "Failed to write PE subsystem after 20 attempts: $Path"
}

function New-GameLauncher {
    param([Parameter(Mandatory = $true)] [string] $OutDir)

    $batPath = Join-Path $OutDir "Start-Game.bat"
@"
@echo off
cd /d "%~dp0"
if exist "%~dp0shared\Microsoft.WindowsDesktop.App" (
  set DOTNET_ROOT=%~dp0
  set DOTNET_MULTILEVEL_LOOKUP=0
)
if exist "%~dp0dotnet.exe" (
  "%~dp0dotnet.exe" FruitNinja.dll %*
) else (
  dotnet FruitNinja.dll %*
)
if errorlevel 1 pause
"@ | Set-Content -Path $batPath -Encoding ASCII

    $cmdPath = Join-Path $OutDir "FruitNinja.cmd"
    "@echo off`r`ncall `"%~dp0Start-Game.bat`" %*" |
        Set-Content -Path $cmdPath -Encoding ASCII
}

function New-IniFile {
    param(
        [Parameter(Mandatory = $true)] [string] $OutDir,
        [Parameter(Mandatory = $true)] [string] $Rid
    )

    $showCursor = if ($Rid -in @("win-x64", "win-x86")) { "1" } else { "0" }
    $iniPath = Join-Path $OutDir "FruitNinja.ini"
    $iniContent = @"
; Fruit Ninja configuration file
; Place this file next to FruitNinja.exe

[Settings]
; Show mouse cursor over the game window (1=yes, 0=no)
ShowCursor=$showCursor
; Start the game in fullscreen mode (1=yes, 0=no). F11 toggles fullscreen in-game.
Fullscreen=0
"@
    Set-Content -Path $iniPath -Value $iniContent -Encoding ASCII
    Write-Host "  Ini: $iniPath (ShowCursor=$showCursor)"
}

function Copy-BundledRuntime {
    param([Parameter(Mandatory = $true)] [string] $OutDir)

    if (-not (Test-Path $RuntimeDir)) {
        Write-Host "  WARNING: bundled runtime not found at $RuntimeDir" -ForegroundColor Yellow
        return
    }

    foreach ($file in @("dotnet.exe", "LICENSE.txt", "ThirdPartyNotices.txt")) {
        $src = Join-Path $RuntimeDir $file
        if (Test-Path $src) {
            Copy-Item $src (Join-Path $OutDir $file) -Force
        }
    }
    foreach ($dir in @("host", "shared")) {
        $src = Join-Path $RuntimeDir $dir
        $dst = Join-Path $OutDir $dir
        if (Test-Path $src) {
            if (Test-Path $dst) { Remove-Item $dst -Recurse -Force }
            Copy-Item $src $dst -Recurse -Force
        }
    }
}

function New-Arm32Launcher {
    param([Parameter(Mandatory = $true)] [string] $OutDir)

    $launcherDir = Join-Path $ProjectRoot "tools\launcher"
    $launcherC = Join-Path $launcherDir "launcher.c"
    $launcherRc = Join-Path $launcherDir "launcher.rc"

    if (-not (Test-Path $launcherC) -or -not (Test-Path $launcherRc)) {
        return $false
    }

    $clang = $null
    try {
        $clang = Get-Command clang.exe -ErrorAction Stop
    } catch {
        Write-Host "  ARM32 launcher skipped: clang not found on PATH."
        return $false
    }

    $probeSrc = Join-Path $env:TEMP ("fn_arm_probe_" + $PID + ".c")
    "int main(void){return 0;}" | Set-Content -Path $probeSrc -Encoding ASCII
    $probeExe = Join-Path $env:TEMP ("fn_arm_probe_" + $PID + ".exe")
    & clang --target=armv7-w64-mingw32 $probeSrc -o $probeExe 2>&1 | Out-Null
    $probeOk = ($LASTEXITCODE -eq 0) -and (Test-Path $probeExe)
    Remove-Item $probeSrc, $probeExe -ErrorAction SilentlyContinue

    if (-not $probeOk) {
        Write-Host "  ARM32 launcher skipped: clang armv7-w64-mingw32 target not available."
        return $false
    }

    $windres = $null
    try {
        $windres = Get-Command windres.exe -ErrorAction Stop
    } catch {
        $clangDir = Split-Path -Parent $clang.Source
        $candidate = Join-Path $clangDir "windres.exe"
        if (Test-Path $candidate) { $windres = Get-Item $candidate }
    }
    if (-not $windres) {
        Write-Host "  ARM32 launcher skipped: windres.exe not found (needs icon embedding)."
        return $false
    }

    $resObj = Join-Path $launcherDir "launcher-res.o"
    $targetExe = Join-Path $OutDir "FruitNinja.exe"
    $debugExe = Join-Path $OutDir "FruitNinja.Debug.exe"

    Push-Location $launcherDir
    try {
        & $windres --target=armv7-w64-mingw32 -i launcher.rc -O coff -o $resObj 2>&1 | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  ARM32 launcher skipped: windres failed."
            return $false
        }
        & clang --target=armv7-w64-mingw32 -O2 -mwindows -municode launcher.c $resObj -o $targetExe 2>&1 | Out-Null
        if ($LASTEXITCODE -ne 0 -or -not (Test-Path $targetExe)) {
            Write-Host "  ARM32 launcher skipped: clang failed to link."
            return $false
        }
        Copy-Item -LiteralPath $targetExe -Destination $debugExe -Force
        Set-PeSubsystem -Path $targetExe -Subsystem WindowsGui
        Set-PeSubsystem -Path $debugExe -Subsystem Console
        return $true
    } finally {
        Pop-Location
        Remove-Item $resObj -ErrorAction SilentlyContinue
    }
}

New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null

# ---------------------------------------------------------------
# ARM32: framework-dependent + bundled runtime + optional native launcher
# ---------------------------------------------------------------
if ($Rids -contains "win-arm") {
    Write-Host "=== Publishing win-arm ===" -ForegroundColor Cyan
    $outDir = Join-Path $OutputRoot "FruitNinja_win-arm"
    if (Test-Path $outDir) { Remove-Item -LiteralPath $outDir -Recurse -Force }
    New-Item -ItemType Directory -Path $outDir -Force | Out-Null

    Copy-BundledRuntime -OutDir $outDir

    dotnet build $ProjectFile -c $Configuration --nologo `
        /p:UseAppHost=false `
        /p:PlatformTarget=AnyCPU `
        /p:WarningLevel=0 `
        -v quiet
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $buildOutput = Join-Path $ProjectRoot "Fruit_Ninja_MainDLL\bin\$Configuration\net8.0-windows"
    Get-ChildItem -LiteralPath $buildOutput -Filter "*.dll" | Copy-Item -Destination $outDir -Force
    foreach ($name in @("FruitNinja.pdb", "FruitNinja.deps.json", "FruitNinja.runtimeconfig.json")) {
        $source = Join-Path $buildOutput $name
        if (Test-Path $source) { Copy-Item -LiteralPath $source -Destination $outDir -Force }
    }
    foreach ($subdir in @("Content", "Images")) {
        $src = Join-Path $buildOutput $subdir
        if (Test-Path $src) {
            Copy-Item -LiteralPath $src -Destination $outDir -Recurse -Force
        }
    }

    $icoPath = Join-Path $ProjectRoot "tools\launcher\FruitNinja.ico"
    if (Test-Path $icoPath) {
        Copy-Item -LiteralPath $icoPath -Destination $outDir -Force
    }

    $armLauncherBuilt = New-Arm32Launcher -OutDir $outDir
    New-GameLauncher -OutDir $outDir
    New-IniFile -OutDir $outDir -Rid "win-arm"

    Write-Host "OK: win-arm" -ForegroundColor Green
    if ($armLauncherBuilt) {
        Write-Host "  Run: $outDir\FruitNinja.exe"
        Write-Host "  Debug console: $outDir\FruitNinja.Debug.exe"
        Write-Host "  Fallback: $outDir\Start-Game.bat"
    } else {
        Write-Host "  Run: $outDir\Start-Game.bat  (native ARM32 launcher not built)"
    }
}

# ---------------------------------------------------------------
# x64 / x86 / arm64: self-contained publish with apphost
# ---------------------------------------------------------------
$selfContainedRIDs = @("win-x64", "win-x86", "win-arm64")
foreach ($rid in $selfContainedRIDs) {
    if ($Rids -notcontains $rid) { continue }

    Write-Host "=== Publishing $rid ===" -ForegroundColor Cyan
    $outDir = Join-Path $OutputRoot "FruitNinja_$rid"
    if (Test-Path $outDir) { Remove-Item -LiteralPath $outDir -Recurse -Force }
    New-Item -ItemType Directory -Path $outDir -Force | Out-Null

    if (-not $SkipBuild) {
        dotnet publish $ProjectFile -c $Configuration -r $rid --self-contained true `
            -o $outDir `
            -p:PublishSingleFile=false `
            -p:DebugType=embedded `
            --nologo `
            -v quiet
        if ($LASTEXITCODE -ne 0) {
            Write-Host "FAILED: $rid" -ForegroundColor Red
            continue
        }
    }

    $hostexe = Join-Path $outDir "FruitNinja.exe"
    $debugexe = Join-Path $outDir "FruitNinja.Debug.exe"
    if (Test-Path $hostexe) {
        Copy-Item -LiteralPath $hostexe -Destination $debugexe -Force
        Set-PeSubsystem -Path $debugexe -Subsystem Console
    }

    New-GameLauncher -OutDir $outDir
    New-IniFile -OutDir $outDir -Rid $rid

    Write-Host "OK: $rid" -ForegroundColor Green
    Write-Host "  Run: $outDir\FruitNinja.exe"
    Write-Host "  Debug console: $outDir\FruitNinja.Debug.exe"
}

Write-Host "Done. Published to: $OutputRoot" -ForegroundColor Cyan
