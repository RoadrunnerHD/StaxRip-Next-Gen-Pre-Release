# StaxRip Next Gen — Features and release notes

## v0.5.0-pre.3 (2026-10-03)

- When a Dolby Vision source is loaded and its Level 5 JSON is available, an additional `HDRDVmetadata_L5.json` copy is created one directory above the project temp folder.
- The original JSON and all existing processing inside the temp folder remain unchanged.
- The additional copy is replaced only after a complete copy succeeds. Copy failures are reported in the log and do not stop source loading.

## v0.5.0-pre.3 — What the program can do

StaxRip Next Gen is a portable Windows application for hardware video encoding, video processing, audio processing and muxing. It brings these steps together in one configurable workflow.

### Hardware video encoding

- **Intel Quick Sync:** QSVEncC.
- **NVIDIA:** NVEncC.
- **AMD:** VCEEncC.
- Encoder-specific quality, bitrate, bit depth and GOP settings.
- Hardware codec availability depends on the GPU, graphics driver and selected encoder. Software video encoders have been removed.

### Video processing / VPP

- Crop, resize and padding to prepare output dimensions and borders.
- Intel Quick Sync deinterlacing, including normal, inverse-telecine and bob modes.
- Denoising, sharpening, debanding, brightness, contrast, saturation and gamma adjustments through supported encoder filters.
- Color matrix, primaries, transfer and range conversion.
- HDR-to-SDR tone mapping, including supported Libplacebo options.
- Encoder-side VPP settings are passed separately from the preview script; the preview can therefore differ from the encoded result.

### Sources, preview and HDR

- AviSynth+ and VapourSynth source decoding, indexing, script processing and preview.
- Direct QSV readers and ffmpeg pipe inputs through the encoder decoder settings.
- Import of source color metadata and explicit VUI settings for SDR/HDR workflows.
- Automatic QSV color options resolved from source metadata for script and pipe inputs.
- HDR10, HDR10+ and Dolby Vision metadata controls where supported by the selected encoder and tools.

### Audio, output and jobs

- Audio processing and muxing with the included tools.
- Elementary video streams, MKV, MP4 and TS/M2TS output, depending on the selected encoder, muxer and stream formats.
- Configurable projects and a job list for preparing multiple encoding tasks.

### Portable Windows package

- Bundled .NET 10 Windows desktop runtime; no separate .NET installation is needed for StaxRipNG.
- Launch `StaxRipNG.exe` from the main folder after extracting the complete package.
- Keep `Apps`, `Fonts`, `Icons` and `Runtime` beside the launcher.
- Requires 64-bit Windows, compatible hardware and drivers, and any Visual C++ runtime libraries required by the included tools.

## v0.5.0-pre.2 (2026-10-02)

Corrections included in this pre-release:

- Fixed application startup in the portable package.
- Corrected legacy settings import, including version values.
- Restricted settings deserialization to supported data types, with resource limits and without invoking serialized callbacks or constructors.
- Required HTTPS for downloads and validated redirects; rejected URL credentials and redirects to HTTP.
- Added download cancellation and transactional file replacement so interrupted or incomplete downloads do not replace working files.
- Hardened tool directory updates and project loading.
- Corrected AviSynth script encoding to UTF-8 without a byte-order mark and addressed source paths containing non-ASCII characters.
- Restored the missing resize menu entries.
- Corrected displayed encoder commands so normal arguments and file paths are visible instead of internal literal placeholders.
- Corrected bundled tool timestamps and version detection.
- Updated the bundled 7-Zip components to 26.03 and corrected their version configuration.
- Added file safety and serialization regression checks to the build.
- Corrected the Windows TLS certificate handling in the file safety tests.

Repository maintenance:

- Added SECURITY.md with guidance for reporting vulnerabilities and handling untrusted projects and scripts.
- Pinned the build workflow actions to full commit hashes and kept workflow permissions read-only.
- Corrected the release tag to match the corresponding source code.

These changes do not constitute a guarantee that the application or every bundled tool is free of vulnerabilities. Encoder and filter availability continues to depend on the selected hardware, driver and tools.

The complete NG development history is maintained in [NGCHANGELOG.md](NGCHANGELOG.md).
