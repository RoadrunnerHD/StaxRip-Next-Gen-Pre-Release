param(
    [Parameter(Mandatory = $true)][string]$ReferenceBeta,
    [Parameter(Mandatory = $true)][string]$BuildOutput,
    [Parameter(Mandatory = $true)][string]$Output
)
$ErrorActionPreference = 'Stop'
$reference = (Resolve-Path -LiteralPath $ReferenceBeta).Path
$build = (Resolve-Path -LiteralPath $BuildOutput).Path
$destination = [IO.Path]::GetFullPath($Output)
if (Test-Path -LiteralPath $destination) { throw "Output already exists: $destination" }
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $reference 'StaxRipNG.exe')).FileVersion
if ($version -ne '0.2.0.0' -or -not (Test-Path (Join-Path $reference 'Runtime\StaxRipNG.dll'))) { throw 'Use the tested compact .NET 10 reference.' }
foreach ($name in @('StaxRipNG.exe', 'FrameServer.dll', 'Runtime\StaxRipNG.dll', 'Runtime\StaxRipNG.runtimeconfig.json', 'Runtime\coreclr.dll', 'Runtime\System.Windows.Forms.dll')) {
    if (-not (Test-Path -LiteralPath (Join-Path $build $name))) { throw "Missing self-contained build file: $name" }
}
New-Item -ItemType Directory -Path $destination | Out-Null
foreach ($file in Get-ChildItem -LiteralPath $reference -Recurse -File -Force) {
    $relative = [IO.Path]::GetRelativePath($reference, $file.FullName)
    if ($relative -notmatch '(\\|/)' -and $relative -match '(?i)^(StaxRipNG\.(exe|exe\.config|dll|pdb|deps\.json|runtimeconfig\.json)|DirectN\.dll|ManagedCuda\.dll|Microsoft\.Management\.Infrastructure\.dll|System\.Management\.Automation\.dll)$') { continue }
    if ($relative -match '^(Settings|Temp|Runtime)(\\|/)') { continue }
    $target = Join-Path $destination $relative
    New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force | Out-Null
    Copy-Item -LiteralPath $file.FullName -Destination $target
    [IO.File]::SetLastWriteTimeUtc($target, $file.LastWriteTimeUtc)
    if ((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash -ne (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash -or
        [IO.File]::GetLastWriteTimeUtc($target) -ne $file.LastWriteTimeUtc) { throw "Reference copy differs: $relative" }
}
foreach ($file in Get-ChildItem -LiteralPath $build -Recurse -File -Force) {
    $relative = [IO.Path]::GetRelativePath($build, $file.FullName)
    if ($relative -match '^Apps(\\|/)') { throw 'Build output must not replace reference Apps.' }
    $target = Join-Path $destination $relative
    New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force | Out-Null
    Copy-Item -LiteralPath $file.FullName -Destination $target -Force
    [IO.File]::SetLastWriteTimeUtc($target, $file.LastWriteTimeUtc)
    if ((Get-FileHash -LiteralPath $target).Hash -ne (Get-FileHash -LiteralPath $file.FullName).Hash) { throw "Build copy differs: $relative" }
}
Copy-Item (Join-Path $PSScriptRoot 'README.md') (Join-Path $destination 'README.md') -Force
Write-Host "Prepared Pre-Release package with original Apps and file dates: $destination"
