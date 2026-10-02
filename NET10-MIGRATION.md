# .NET 10 migration notes — Pre-Release

Base: the tested compact .NET 10 reference. The project operator confirmed a successful first encoding test and green Apps. Beta 2 keeps this runtime and compact structure.

## Changes

- SDK-style Visual Basic project targeting net10.0-windows, Windows Forms, x64.
- Self-contained publishing includes Microsoft.NETCore.App and Microsoft.WindowsDesktop.App 10.0.12. No desktop runtime download is required by the main application.
- Embedded PowerShell upgraded from Windows PowerShell 5 reference assemblies to Microsoft.PowerShell.SDK 7.6.6.
- Original DirectN and ManagedCuda dependency versions retained to avoid unrelated GPU changes.
- Unused System.Web/WPF imports and obsolete Code Access Security demand removed.
- Updated ICustomTypeDescriptor interface implementations, explicit namespace handling and Windows ANSI code page support.
- Hardware encoder parameter layout falls back to the Windows message-box font height before MainForm exists. The Windows smoke test requires defaults for NVEnc, QSVEnc and VCEEnc with no main window.
- Original assembly/product branding and interface retained. No redesign in this build.
- Compact portable layout: the native launcher is in the main folder; the self-contained managed application, runtime DLLs and localization directories are in Runtime. Apps, Fonts and Icons retain their reference locations.
- Packaging uses the corrected Beta reference and preserves all Apps file hashes and timestamps.

## Storage compatibility

The existing project uses BinaryFormatter for settings, projects, jobs, profiles and cloning. This first migration build retains the old format through Microsoft's unsupported System.Runtime.Serialization.Formatters compatibility package and its runtime switch. A centralized culture surrogate stores culture names and custom language labels without serializing runtime internals. It also imports legacy Framework culture names and identifiers. Framework auxiliary TextInfo, number/date formats and calendars are mapped to serialization records, then reconstructed through public APIs. This avoids the unsupported .NET 10 TextInfo deserialization callback. CompareInfo retains its existing runtime serialization support. All legacy formatter creation uses this adapter. This preserves the existing format but retains BinaryFormatter's known risks. Do not treat this development build as a completed storage migration or release approval. A supported replacement and controlled import of old data remain separate migration work.

WinForms designer serialization warning WFO1000 is suppressed for existing custom controls; this build does not use designer serialization to persist user data. Legacy dependency fallback warnings remain a compatibility concern requiring Windows tests.

## Verification

The source compiles and publishes for win-x64 on .NET 10 without compiler errors in the development environment. That does not establish Windows runtime behavior.

Build-NG.ps1 builds the native FrameServer and compact launcher on Windows and publishes the application into Runtime. Windows PowerShell creates an actual .NET Framework culture serialization fixture. The root launcher then runs --net10-smoke-test, checking the runtime, embedded icon, culture and custom language roundtrips, Framework culture import, initialized full settings cloning, settings file save/reload, filter profiles, root path resolution and embedded PowerShell. Results are stored in net10-smoke-test.txt. Failed checks stop the workflow.

Local adapter tests pass for German, invariant and custom cultures, Turkish casing and custom list separators, number/date formats, Gregorian calendars, nested payloads with Framework field layouts and read-only TextInfo. These legacy-shaped local payloads do not replace the actual Framework-generated fixture in the Windows workflow. Native launcher execution, the Framework-generated fixture and full WinForms settings tests require the new Windows workflow output; they have not been established by the local cross-build.

Before replacing the reference: verify clean startup, Apps versions, VS/AVS preview and filters, QSV/NV/VCE encoding on available hardware, audio/muxing, taskbar icon, fresh settings, copied old settings, projects and queued jobs, profile saving/reloading and script events. Keep original settings and the reference program untouched during tests.

## Windows checks observed

The compact reference passed the Windows runtime/settings smoke test, including legacy Framework globalization import and hardware encoder defaults. The project operator confirmed the first encoding test completed without errors and all Apps were green. Beta 2 requires its own Windows build and visual check of the restored Accept / Next button size.

## Beta 4 update

Beta 4 is based on Beta 2 and retains its interface and dependency versions. Only the encoder error validation and release information are changed. Windows runtime and VC-1 encoding validation of this update are pending.

## Pre-Release documentation release

Based on the tested Beta 4 correction. The interface, encoding behavior and bundled .NET 10.0.12 runtime are retained. Release notes, README and application information now describe the existing hardware encoder and VPP workflow.
