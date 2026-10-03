# StaxRip Next Gen — Release information

## v0.5.0-pre.3 (2026-10-03)

- When a Dolby Vision source is loaded and its Level 5 JSON is available, an additional `HDRDVmetadata_L5.json` copy is created one directory above the project temp folder.
- This additional copy is created only for Dolby Vision sources with available Level 5 metadata. SDR and HDR sources without Dolby Vision do not create a new JSON file.
- The original JSON and all existing processing inside the temp folder remain unchanged.
- The additional copy is replaced only after a complete copy succeeds. Copy failures are reported in the log and do not stop source loading.

The current NG pre-release combines hardware video encoding with video processing, audio processing and muxing in a portable Windows package.

## Capabilities at a glance

| Area | What is available |
| --- | --- |
| Intel | Quick Sync hardware encoding through QSVEncC |
| NVIDIA | Hardware encoding through NVEncC |
| AMD | Hardware encoding through VCEEncC |
| Encoding controls | Encoder-specific quality, bitrate, bit depth and GOP settings |
| Dimensions | Crop, resize and padding through supported encoder VPP |
| Interlaced video | Intel Quick Sync normal, inverse-telecine and bob deinterlacing |
| Image processing | Supported denoise, sharpen, deband and color adjustment filters |
| Color and HDR | Source metadata import, VUI controls, color conversion, supported HDR-to-SDR tone mapping and HDR10/HDR10+/Dolby Vision metadata options |
| Sources and preview | AviSynth+ and VapourSynth; direct QSV readers and ffmpeg pipe input options |
| Audio and containers | Audio processing and muxing; elementary streams, MKV, MP4 and TS/M2TS where supported |
| Workflow | Configurable projects and an encoding job list |
| Deployment | Portable 64-bit Windows package with bundled .NET 10 runtime |

Software video encoders have been removed. Available codecs and filters depend on the selected encoder, GPU, driver and input format. Encoder-side VPP and script preview can use different processing paths.

## Previous release: v0.5.0-pre.2 (2026-10-02)

The previous release includes corrections for portable startup, legacy settings import, AviSynth UTF-8 script encoding and Unicode paths, missing resize menu entries, encoder command display, bundled tool timestamps and version detection, and 7-Zip 26.03 configuration.

Settings loading, HTTPS downloads, redirect handling, interrupted downloads, tool updates and project loading were hardened. File safety and serialization regression checks are included in the build, and the Windows TLS test certificate handling was corrected.

For the detailed changes, see [CHANGELOG.md](CHANGELOG.md). The NG development history is in [NGCHANGELOG.md](NGCHANGELOG.md).

This file describes the current NG release and does not announce a separate supporter edition.
