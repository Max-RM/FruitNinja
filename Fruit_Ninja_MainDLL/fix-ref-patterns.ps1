param(
    [string]$Path = $PSScriptRoot
)

$csFiles = Get-ChildItem -Path $Path -Filter *.cs -Recurse | Where-Object {
    $_.DirectoryName -notmatch '\\obj\\' -and $_.DirectoryName -notmatch '\\bin\\'
}

$stats = @{
    FilesProcessed     = 0
    FilesModified      = 0
    CtorPattern        = 0
    PropMethodPattern  = 0
    ArrayCastPattern   = 0
    SpriteSortMode     = 0
    PresentInterval    = 0
    DisposablePattern  = 0
    EnumeratorPattern  = 0
}

$totalChanges = 0

foreach ($file in $csFiles) {
    $stats.FilesProcessed++
    $original = [System.IO.File]::ReadAllText($file.FullName)
    $content = $original
    $fileChanges = 0

    # 1. _002Ector constructor ref-cast pattern
    # ((Type)(ref var))._002Ector(args) -> var = new Type(args)
    $newContent = $content -replace '(?x)
        \(\(([A-Za-z_][A-Za-z0-9_.]*)\)      # type name
        \(ref\ ([A-Za-z_][A-Za-z0-9_.]*(?:\[[^\]]*\])?)\)  # variable name (with optional indexer)
        \)\._002Ector\(
        ([^;]+)\);                            # constructor args (up to last );)
    ', '$2 = new $1($3);'
    $delta = [regex]::Matches($content, '\(\([A-Za-z_][A-Za-z0-9_.]*\)\(ref [A-Za-z_][A-Za-z0-9_.]*(?:\[[^\]]*\])?\)\)\._002Ector\(').Count
    if ($delta -gt 0) {
        $stats.CtorPattern += $delta
        $fileChanges += $delta
        $content = $newContent
    }

    # 2. Property/method access on ref-cast structs (non-_002Ector)
    # ((Type)(ref var)).Property or ((Type)(ref var)).Method(args)
    $newContent = $content -replace '(?x)
        \(\(([A-Za-z_][A-Za-z0-9_.]*)\)      # type name (can include dots like Microsoft.Xna...)
        \(ref\ ([A-Za-z_][A-Za-z0-9_.]*(?:\[[^\]]*\])?)\)  # variable name
        \)\.([A-Za-z_][A-Za-z0-9_]*)           # member name (property or method, single-char OK)
    ', '$2.$3'
    if ($newContent -ne $content) {
        $delta = [regex]::Matches($content, '\(\([A-Za-z_][A-Za-z0-9_.]*\)\(ref [A-Za-z_][A-Za-z0-9_.]*(?:\[[^\]]*\])?\)\)\.').Count
        $stats.PropMethodPattern += $delta
        $fileChanges += $delta
        $content = $newContent
    }

    # 3. Array creation with object cast
    # (Type[])(object)new Type[size] -> new Type[size]
    $newContent = $content -replace '\([A-Za-z_][A-Za-z0-9_.]*\[\]\)\(object\)new ', 'new '
    if ($newContent -ne $content) {
        $delta = [regex]::Matches($content, '\([A-Za-z_][A-Za-z0-9_.]*\[\]\)\(object\)new [A-Za-z_][A-Za-z0-9_.]*\[').Count
        $stats.ArrayCastPattern += $delta
        $fileChanges += $delta
        $content = $newContent
    }

    # 4. SpriteSortMode enum casts
    $newContent = $content -replace '\(SpriteSortMode\)1', 'SpriteSortMode.Deferred'
    if ($newContent -ne $content) {
        $delta = ($content -split '\(SpriteSortMode\)1').Count - 1
        $stats.SpriteSortMode += $delta
        $fileChanges += $delta
        $content = $newContent
    }
    $newContent2 = $content -replace '\(SpriteSortMode\)0', 'SpriteSortMode.Immediate'
    if ($newContent2 -ne $content) {
        $delta = ($content -split '\(SpriteSortMode\)0').Count - 1
        $stats.SpriteSortMode += $delta
        $fileChanges += $delta
        $content = $newContent2
    }

    # 5. PresentInterval enum casts
    $newContent = $content -replace '\(PresentInterval\)1', 'PresentInterval.One'
    if ($newContent -ne $content) {
        $delta = ($content -split '\(PresentInterval\)1').Count - 1
        $stats.PresentInterval += $delta
        $fileChanges += $delta
        $content = $newContent
    }

    # 6. IDisposable constrained prefix Dispose pattern (safe: removes entire line)
    $newContent = $content -replace '(?m)^\s*\(\(IDisposable\)\(\*\([^)]*\)\(\&[^)]*\)\)/\*cast due to constrained\. prefix\*/\.Dispose\(\);?\s*$', ''
    if ($newContent -ne $content) {
        $delta = [regex]::Matches($content, '\(\(IDisposable\)').Count
        $stats.DisposablePattern += $delta
        $fileChanges += $delta
        $content = $newContent
    }

    # 7. Enumerator variable declarations (e.g. for TouchCollection.GetEnumerator)
    $newContent = $content -replace '(?x)
        Enumerator\s+(\w+)\s*=\s*
        \(\([A-Za-z_][A-Za-z0-9_.]*\)      # type name
        \(ref\ ([A-Za-z_][A-Za-z0-9_.]*(?:\[[^\]]*\])?)\)
        \.\s*GetEnumerator\(\)
    ', 'var $1 = $2.GetEnumerator()'
    if ($newContent -ne $content) {
        $delta = [regex]::Matches($content, 'Enumerator\s+\w+\s*=\s*\(\([A-Za-z_][A-Za-z0-9_.]*\)\(ref\ ').Count
        $stats.EnumeratorPattern += $delta
        $fileChanges += $delta
        $content = $newContent
    }

    if ($fileChanges -gt 0) {
        $stats.FilesModified++
        $totalChanges += $fileChanges
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.UTF8Encoding]::new($false))
        Write-Host "  FIXED ($fileChanges changes): $($file.FullName)"
    }
}

Write-Host ""
Write-Host "========================================"
Write-Host "  REF-CAST PATTERN FIX SUMMARY"
Write-Host "========================================"
Write-Host "  Files scanned:         $($stats.FilesProcessed)"
Write-Host "  Files modified:        $($stats.FilesModified)"
Write-Host "  Total changes:         $totalChanges"
Write-Host "----------------------------------------"
Write-Host "  _002Ector ctors:       $($stats.CtorPattern)"
Write-Host "  Property/method refs:  $($stats.PropMethodPattern)"
Write-Host "  Array object casts:    $($stats.ArrayCastPattern)"
Write-Host "  SpriteSortMode enums:  $($stats.SpriteSortMode)"
Write-Host "  PresentInterval enums: $($stats.PresentInterval)"
Write-Host "  IDisposable patterns:  $($stats.DisposablePattern)"
Write-Host "  Enumerator patterns:   $($stats.EnumeratorPattern)"
Write-Host "========================================"
