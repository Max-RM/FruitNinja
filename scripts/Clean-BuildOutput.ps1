# Clean-BuildOutput.ps1
#
# Cleans compile caches (obj folders) and build/release outputs.
# The <root>\Fruit_Ninja_MainDLL\bin and <root>\Publish folders are kept
# (emptied, not deleted) so paths used by other scripts stay valid.

$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$ProjectDir = Join-Path $Root "Fruit_Ninja_MainDLL"

function Clear-DirContents {
    param(
        [string]$Path,
        [string]$Label
    )
    if (-not (Test-Path -LiteralPath $Path)) {
        Write-Host "$Label not found, nothing to clean: $Path"
        return
    }
    Get-ChildItem -LiteralPath $Path -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Cleaned ${Label}: $Path"
}

Write-Host "Removing compile caches (obj)..."
Get-ChildItem -LiteralPath $ProjectDir -Directory -Recurse -Filter "obj" -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "Done."

Clear-DirContents (Join-Path $ProjectDir "bin") "build output (bin)"
Clear-DirContents (Join-Path $Root "Publish") "publish output"

Write-Host ""
Write-Host "Clean complete."
Write-Host "  kept (now empty): $ProjectDir\bin"
Write-Host "  kept (now empty): $Root\Publish"
Write-Host "  removed:         obj caches under $ProjectDir"
