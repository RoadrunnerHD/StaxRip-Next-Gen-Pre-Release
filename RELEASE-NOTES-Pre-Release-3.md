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

## Retained from Pre-Release 2

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

## Validation

Pre-Release 3 builds with zero compiler errors and zero warnings. Settings serialization, file safety checks available on this platform, 18 QSV color-metadata cases, Dolby Vision Level 5 copying and tool-version detection passed. Windows-only file-lock and cmd tests could not run on Linux. Windows startup, frame-server runtime and GPU encoding tests remain pending. See `VALIDATION-Pre-Release-3.txt` in the source archive for the validation scope.

See [CHANGELOG.md](CHANGELOG.md) for the complete feature description and retained corrections.
