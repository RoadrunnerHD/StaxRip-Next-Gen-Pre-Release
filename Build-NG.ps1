$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'Tests\SerializationSafety.Tests.vbproj') --configuration Release
if ($LASTEXITCODE -ne 0) { throw 'Serialization safety tests failed.' }
& dotnet run --project (Join-Path $PSScriptRoot 'Tests\FileSafety.Tests.vbproj') --configuration Release
if ($LASTEXITCODE -ne 0) { throw 'File safety tests failed.' }
& dotnet run --project (Join-Path $PSScriptRoot 'Tests\QsvColorMetadata.Tests.vbproj') --configuration Release
if ($LASTEXITCODE -ne 0) { throw 'QSV color metadata regression tests failed.' }
& dotnet run --project (Join-Path $PSScriptRoot 'Tests\DolbyVisionLevel5Copy.Tests.vbproj') --configuration Release
if ($LASTEXITCODE -ne 0) { throw 'Dolby Vision Level 5 copy tests failed.' }
$source = Join-Path $PSScriptRoot 'Source'
$publish = Join-Path $source 'publish-net10'
$runtime = Join-Path $publish 'Runtime'
$msbuild = Get-Command MSBuild.exe -ErrorAction Stop
if (Test-Path -LiteralPath $publish) { Remove-Item -LiteralPath $publish -Recurse -Force }
New-Item -ItemType Directory -Path $runtime -Force | Out-Null
& $msbuild.Source (Join-Path $source 'FrameServer\FrameServer.vcxproj') /t:Rebuild /p:Configuration=Release /p:Platform=x64
if ($LASTEXITCODE -ne 0) { throw "FrameServer build failed: $LASTEXITCODE" }
& $msbuild.Source (Join-Path $source 'Launcher\Launcher.vcxproj') /t:Rebuild /p:Configuration=Release /p:Platform=x64
if ($LASTEXITCODE -ne 0) { throw "Native launcher build failed: $LASTEXITCODE" }
& dotnet publish (Join-Path $source 'StaxRip.vbproj') --configuration Release --runtime win-x64 --self-contained true --output $runtime
if ($LASTEXITCODE -ne 0) { throw ".NET 10 publish failed: $LASTEXITCODE" }
Copy-Item (Join-Path $source 'bin\FrameServer.dll') (Join-Path $runtime 'FrameServer.dll') -Force
Copy-Item (Join-Path $source 'bin\FrameServer.dll') (Join-Path $publish 'FrameServer.dll') -Force
Copy-Item (Join-Path $source 'Launcher\bin\StaxRipNG.exe') (Join-Path $publish 'StaxRipNG.exe') -Force
Copy-Item (Join-Path $PSScriptRoot 'License.txt') $publish -Force
Copy-Item (Join-Path $PSScriptRoot 'README.md') $publish -Force
$runtimeConfig = Get-Content (Join-Path $runtime 'StaxRipNG.runtimeconfig.json') -Raw | ConvertFrom-Json
if ($runtimeConfig.runtimeOptions.tfm -ne 'net10.0') { throw 'Unexpected runtime target.' }
foreach ($file in @('coreclr.dll','System.Windows.Forms.dll')) {
    if (-not (Test-Path (Join-Path $runtime $file))) { throw "Missing bundled runtime file: $file" }
}
& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot 'Create-Legacy-Fixture.ps1') -Output (Join-Path $publish 'legacy-cultures.bin')
if ($LASTEXITCODE -ne 0) { throw 'Failed to create .NET Framework import fixture.' }
$process = Start-Process -FilePath (Join-Path $publish 'StaxRipNG.exe') -ArgumentList '--net10-smoke-test' -PassThru
if (-not $process.WaitForExit(120000)) { $process.Kill(); throw 'Compact runtime/settings test timed out.' }
if ($process.ExitCode -ne 0) {
    Get-Content (Join-Path $publish 'net10-smoke-test.txt') -ErrorAction SilentlyContinue
    throw "Compact runtime/settings test failed: $($process.ExitCode)"
}
Get-Content (Join-Path $publish 'net10-smoke-test.txt')
Remove-Item (Join-Path $publish 'legacy-cultures.bin') -Force
Write-Host "Published compact self-contained .NET 10 Windows x64 build: $publish"
