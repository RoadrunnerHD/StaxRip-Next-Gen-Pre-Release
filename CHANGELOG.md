# StaxRip Next Gen Pre-Release 3

## Component updates — Pre-Release 3 (2026-10-04)

- **Intel Quick Sync: QSVEncC 8.32**.
- **NVIDIA: NVEncC 9.37**.
- **AMD: VCEEncC 9.21**.
- The encoder versions above were checked against the bundled EXE version metadata. The already supplied encoder updates are retained.
- **Python 3.14.8**.
- **VapourSynth R80**, including **vspipe R80**.
- **BestSource R22** for AviSynth and VapourSynth.
- **d2vsource 1.4** for VapourSynth.
- New default startup templates use **AviSynth**; **Automatic Workflow** starts with **LWLibavVideoSource**. Existing saved templates and settings are preserved.
- Fixed **Help → Changelog** and **Help → What's new**: the NG release notes now open with Markdown headings and pre-release tags, without requiring legacy version lines.
- Corrected the Microsoft download links for Visual C++ 2010 SP1 and Visual C++ 2015–2022. Runtime DLLs are unchanged.
- **FFTW 3.3.11**: Windows x64 libraries for double, float and long-double precision. Corresponding source and build instructions are included under `Apps/Support/FFTW/Source`.
- Adapted the default BestSource profiles to R22 by removing the unsupported `hwdevice` and `extrahwframes` arguments. Previously saved custom profiles or projects containing these arguments also need them removed.

The package is based on the newly supplied Pre-Release 2 archive. The NVEncC, VCEEncC, qaac and eac3to updates already included by the project operator are retained. Subtitle Edit and all other bundled tools are unchanged from that archive.

Source indexing remains before HDR/Dolby Vision metadata processing. Automatic L5 JSON export, DV Offset and the existing encoding workflow are retained.

## Previous release — v0.5.0-pre.2 (2026-10-03)

- Completed source indexing before reading HDR/Dolby Vision metadata.
- Kept the **When finished do:** caption readable during indexing and Dolby Vision processing; job-only controls retain their existing enabled/disabled behavior.
- Updated QSVEncC to **8.32**.
- Detect tool versions and available build revisions from supported version commands or EXE/DLL metadata. Files without usable information retain their configured version.
- Use **Dark | Blue** as the default theme for new settings; existing saved themes are preserved.
- **Help → Website** opens this GitHub repository.
- Removed the remaining software video encoder classes, controls, resources and settings, including x264/x265, SVT-AV1 variants, VVenC, AOM, rav1e and the standalone FFmpeg video encoder. FFmpeg media utilities are retained.
- Retained automatic Dolby Vision Level 5 JSON export and **Tools → DV Offset**. Adjusted output uses the `_DV_Offset.hevc` filename.

## DV Offset – Adjusting Dolby Vision Level 5 metadata

**Tools → DV Offset** launches the Python tool for adjusting the **Level 5 active-area offsets in the Dolby Vision RPU**. These values describe the distances from the full image edges to the active picture, for example where black bars are present.

When a video is downscaled, these pixel values must also be scaled by the appropriate ratio. The tool reads **HDRDVmetadata_L5.json**, scales the offsets and injects the adjusted RPU into a new HEVC file. It does not resize or re-encode the video itself.

### Image dimensions and requirements

The intended image dimensions are **3840×2160, 1920×1080 and 1280×720**. All three resolutions use a **16:9** aspect ratio, allowing the width and height to be scaled uniformly.

**Cropped or trimmed image dimensions are not supported.** The full frame, including any existing black bars, must be preserved. Cropping changes the reference points of the active-area offsets.

**Currently, DV Offset only works with HEVC output files (.hevc). MKV support is planned.**

### How to use

1. For Dolby Vision material with available Level 5 metadata, StaxRip automatically creates an additional **HDRDVmetadata_L5.json** in the HEVC output folder.
2. Open **Tools → DV Offset**.
3. Drag the HEVC file into the console window and press **Enter**.
4. Check the newly created HEVC file containing the adjusted Dolby Vision RPU.

This tool is intended exclusively for **Dolby Vision material with matching Level 5 metadata**. At the final deletion prompt, only the L5 JSON file and the unmodified input HEVC file are deleted if you confirm. The newly created HEVC file with the adjusted RPU is retained. Enter **n** to keep all files.

## What the program can do

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

## Retained corrections

Corrections retained in Pre-Release 3:

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
