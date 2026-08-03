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

function New-Archive {
    param(
        [Parameter(Mandatory = $true)] [string] $SourceDir,
        [Parameter(Mandatory = $true)] [string] $ArchiveName
    )

    if (-not (Test-Path $SourceDir)) {
        Write-Host "  SKIPPED (not found): $SourceDir" -ForegroundColor Yellow
        return
    }

    $archivePath = Join-Path $ArchiveRoot "$ArchiveName.zip"
    if (Test-Path $archivePath) {
        Remove-Item -LiteralPath $archivePath -Force
    }

    Write-Host "  Packing: $SourceDir -> $archivePath"
    Compress-Archive -Path "$SourceDir\*" -DestinationPath $archivePath -CompressionLevel Optimal
}

function New-SourceArchive {
    $archiveName = "FruitNinja-source"
    $archivePath = Join-Path $ArchiveRoot "$archiveName.zip"
    if (Test-Path $archivePath) {
        Remove-Item -LiteralPath $archivePath -Force
    }

    $exclude = @(
        "Publish", "Archives", "dotnet-runtime-8.0.6-win-arm",
        "*.user", "*.suo", ".vs", "bin", "obj",
        "Fruit_Ninja_MainDLL\bin", "Fruit_Ninja_MainDLL\obj"
    )

    Write-Host "  Packing source: $ProjectRoot -> $archivePath"
    Compress-Archive -Path "$ProjectRoot\*" -DestinationPath $archivePath -CompressionLevel Optimal
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
