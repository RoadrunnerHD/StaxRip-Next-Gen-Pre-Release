# Build StaxRip Next Gen Pre-Release 3

This archive contains the complete application source, native launcher and FrameServer source, resources, DV Offset Python script, regression tests, build scripts, release documentation and the release tool-configuration snapshot. Bundled third-party executables and the .NET runtime are supplied in the portable program archive; they are not application source files.

## Windows build

Install the .NET 10 SDK and Visual Studio 2022 Build Tools with the v143 C++ toolset and Windows SDK. Use a developer PowerShell where `MSBuild.exe` is available.

```powershell
pwsh -NoProfile -ExecutionPolicy Bypass -File .\Build-NG.ps1
```

The output is `Source\publish-net10`. All required tests must pass.

To assemble the full portable package, use the complete Pre-Release 3 portable folder as the reference containing the matching Apps, Fonts and Icons:

```powershell
.\Package-NG.ps1 -ReferenceBeta "C:\StaxRip-Next-Gen-Pre-Release-3" -BuildOutput ".\Source\publish-net10" -Output "C:\StaxRip-Pre3-Rebuilt"
```

The legacy `ReferenceBeta` parameter names the reference portable folder. It does not select an older beta. The configuration snapshot in `Package\Apps\Conf` is copied into the resulting package. Third-party tool binaries are retained from the reference folder.

## Release build used for this package

The managed application was compiled from this source with .NET SDK 10.0.401 for Windows x64. The native launcher was cross-compiled using `Tools/Build-Launcher-MinGW.sh` with MinGW-w64 GCC 13.2.0. The original FrameServer DLL is retained; its source was not changed. `Build-NG.ps1` provides the Windows/MSVC build route.

FFTW 3.3.11 source and precision-specific configure instructions are included under `Tools/FFTW`. The portable folder also contains these under `Apps/Support/FFTW/Source`.

See `VALIDATION-Pre-Release-3.txt` for completed checks and Windows runtime checks still pending.
