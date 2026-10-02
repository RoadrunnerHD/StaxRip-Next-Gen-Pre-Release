v0.5 (2026-10-02)
===================

StaxRip Next Gen Pre-Release

HARDWARE VIDEO ENCODING
- Intel Quick Sync through QSVEncC by Rigaya, NVIDIA through NVEncC and AMD through VCEEncC.
- Encoder-specific quality, bitrate, bit-depth and GOP controls; codec availability depends on the GPU and driver.

VIDEO PROCESSING / VPP
- Crop and resize through the hardware encoder, with padding for the required output dimensions.
- Intel Quick Sync deinterlacing, including normal, inverse-telecine and bob modes.
- Additional encoder-side filters for denoising, sharpening, debanding and color adjustments.
- Color-matrix, transfer and range conversion, plus HDR-to-SDR tone-mapping controls where supported.
- AviSynth+ and VapourSynth source processing and preview, with encoder-side VPP passed separately to the encoder.

COLOR AND HDR METADATA
- Source color-metadata import and explicit VUI settings for SDR/HDR workflows.
- HDR10, HDR10+ and Dolby Vision metadata options where supported by the selected encoder and tools.
- Keeps the tested Beta 4 fixes for empty encoder errors and QSV automatic color options with script/pipe inputs.

PORTABLE .NET 10
- Bundled .NET 10.0.12 Windows desktop runtime: no separate .NET installation is needed for StaxRipNG.
- Compact package: StaxRipNG.exe in the main folder; Apps, Fonts, Icons and Runtime beside it.
- Updated English README, application information and release notes to explain existing processing capabilities.
- Keeps the existing interface. Feature availability depends on the encoder, GPU, driver and source format.

v0.4 (2026-10-02)
===================

StaxRip Next Gen Beta 4

- Based on Beta 2 with its existing interface.
- Fixed a false Encoder Error after source indexing when the encoder returned an empty error message.
- Read the encoder error once and block only when it contains a meaningful message.
- Resolve automatic QSV color matrix, primaries, transfer and range from source metadata for AviSynth, VapourSynth and ffmpeg pipe inputs.
- Omit unsupported automatic options when source metadata is absent or unknown; keep explicit color settings.
- Omit automatic ATC-SEI for script/pipe readers; keep explicit ATC-SEI and direct avhw/avsw behavior.
- Kept real encoder error messages and validation active.
- Added a default encoder error regression check to the Windows runtime smoke test.
- Preserved the bundled tools, file dates, .NET 10 runtime and settings identity.

v0.2 (2026-10-01)
===================

StaxRip Next Gen Beta 2

- Based on the tested compact .NET 10 build.
- Restored the Beta 1 size of the Accept / Next button in the main window.
- Updated application information, file version and build names for Beta 2.
- Checked FrameServer text lengths, frame-rate values and frame strides before narrowing conversions.
- Corrected text-conversion buffer handling.
- Updated the build actions to Node.js 24.
- Added missing return values and explicit initialization in the reported VB code paths.
- Frame bitmap failures report an error instead of returning an unusable bitmap.
- Removed an unused legacy menu property and an ignored exception attribute.
- Prevented duplicate Unicode macro definitions in the launcher.
- Completed the remaining compiler return paths and variable initialization.
- Preserved original exception stacks when rethrowing errors.
- Replaced the legacy downloader with streaming HttpClient downloads, cancellation and error checks.
- Configured the existing PerMonitorV2 DPI mode through the .NET application API.
- The .NET 10 runtime remains included in Runtime.

v0.01 (2026-10-01)
=================

StaxRip Next Gen 0.01 Beta

First public beta of StaxRip Next Gen.

VIDEO
- Hardware encoding with Intel QSVEncC, NVIDIA NVEncC and AMD VCEEncC.
- Crop, resize and additional VPP functions through the hardware encoders.
- Intel Quick Sync VPP integration through QSVEncC by Rigaya.
- Video processing with AviSynth+ and VapourSynth.

AUDIO AND OUTPUT
- Audio processing and muxing with the included tools.
- Elementary video streams, MKV, MP4 and TS/M2TS, depending on the encoder and muxer.

INTERFACE AND TOOLS
- Consistent application name: StaxRip Next Gen 0.01 Beta.
- No duplicate crop and resize entries in the default VapourSynth templates.
- DeeZy is recognized with the correct version information.
- The taskbar icon is restored after encoding.
- Updated English README.
