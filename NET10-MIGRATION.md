# .NET 10 — StaxRip Next Gen Pre-Release 3

## Application and runtime

- SDK-style Visual Basic project targeting `net10.0-windows`, Windows Forms and x64.
- The portable package includes Microsoft.NETCore.App and Microsoft.WindowsDesktop.App 10.0.12. A separate .NET installation is not required for StaxRipNG.
- Embedded PowerShell uses Microsoft.PowerShell.SDK 7.6.6.
- The native launcher is in the main folder. Managed application files, runtime libraries and localization directories are in `Runtime`; `Apps`, `Fonts` and `Icons` remain beside the launcher.

## Settings compatibility

The existing assembly identity is retained for settings compatibility. The compatibility reader restricts imported settings to supported data types, applies resource limits and suppresses serialized callbacks and constructors. Legacy culture and version values are imported through the compatibility adapter.

These controls do not make arbitrary projects, scripts or external executables trustworthy. See [SECURITY.md](SECURITY.md).

## Build and validation

`Build-NG.ps1` builds the native launcher and FrameServer on Windows and publishes the managed application into `Runtime`. The build includes settings serialization, file safety and runtime/settings checks. Failed required checks stop the build.

The prepared Pre-Release 2 build completed with zero compiler errors and zero warnings. Windows startup and GPU encoding behavior depend on the deployment environment; the project operator reported a successful QSVEncC 8.32 encode.
