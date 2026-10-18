param(
    [ValidateSet("win-x64", "win-x86", "win-arm64", "win-arm", "source")]
    [string[]]$Items = @("win-x64", "win-x86", "win-arm64", "win-arm"),
    [string]$OutputDir = (Join-Path (Split-Path -Parent $PSScriptRoot) "Publish")
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$PublishRoot = Join-Path $ProjectRoot "Publish"
$ArchiveRoot = Join-Path $ProjectRoot "Archives"

New-Item -ItemType Directory -Path $ArchiveRoot -Force | Out-Null

# Prefer 7-Zip (fast) when available on PATH, otherwise fall back to Compress-Archive (slow).
$SevenZip = $null
foreach ($candidate in @("7z", "7za")) {
    $cmd = Get-Command $candidate -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($cmd) {
        $SevenZip = $cmd.Source
        break
    }
}

if ($SevenZip) {
    Write-Host "Using 7-Zip: $SevenZip" -ForegroundColor Green
}
else {
    Write-Host "7-Zip not found on PATH, falling back to Compress-Archive (slow)" -ForegroundColor Yellow
}

function New-Zip {
    param(
        [Parameter(Mandatory = $true)] [string] $SourceDir,
        [Parameter(Mandatory = $true)] [string] $ArchivePath,
        [string[]] $Exclude
    )

    if (Test-Path -LiteralPath $ArchivePath) {
        Remove-Item -LiteralPath $ArchivePath -Force
    }

    if ($SevenZip) {
        $sevenZipArgs = @("a", "-tzip", "-mx=9", "-y", $ArchivePath, ".\*")
        foreach ($pattern in @($Exclude)) {
            if ($pattern) { $sevenZipArgs += "-xr!$pattern" }
        }
        Push-Location -LiteralPath $SourceDir
        try {
            & $SevenZip @sevenZipArgs | Out-Null
            if ($LASTEXITCODE -ne 0) {
                throw "7-Zip failed (exit code $LASTEXITCODE) for $SourceDir"
            }
        }
        finally {
            Pop-Location
        }
    }
    else {
        Compress-Archive -Path "$SourceDir\*" -DestinationPath $ArchivePath -CompressionLevel Optimal
    }
}

function New-Archive {
    param(
        [Parameter(Mandatory = $true)] [string] $SourceDir,
        [Parameter(Mandatory = $true)] [string] $ArchiveName
    )

    if (-not (Test-Path -LiteralPath $SourceDir)) {
        Write-Host "  SKIPPED (not found): $SourceDir" -ForegroundColor Yellow
        return
    }

    $archivePath = Join-Path $ArchiveRoot "$ArchiveName.zip"

    Write-Host "  Packing: $SourceDir -> $archivePath"
    New-Zip -SourceDir $SourceDir -ArchivePath $archivePath
}

function New-SourceArchive {
    $archiveName = "FruitNinja-source"
    $archivePath = Join-Path $ArchiveRoot "$archiveName.zip"

    $exclude = @(
        "Publish", "Archives", "dotnet-runtime-8.0.6-win-arm",
        "*.user", "*.suo", ".vs", "bin", "obj",
        "Fruit_Ninja_MainDLL\bin", "Fruit_Ninja_MainDLL\obj"
    )

    Write-Host "  Packing source: $ProjectRoot -> $archivePath"
    New-Zip -SourceDir $ProjectRoot -ArchivePath $archivePath -Exclude $exclude
}

Write-Host "=== Packaging ===" -ForegroundColor Cyan

if ($Items -contains "source") {
    Write-Host "  Source archive..."
    New-SourceArchive
}

foreach ($rid in @("win-x64", "win-x86", "win-arm64", "win-arm")) {
    if ($Items -notcontains $rid) { continue }
    $srcDir = Join-Path $PublishRoot "FruitNinja_$rid"
    New-Archive -SourceDir $srcDir -ArchiveName "FruitNinja-$rid"
}

Write-Host "Done. Archives in: $ArchiveRoot" -ForegroundColor Cyan
