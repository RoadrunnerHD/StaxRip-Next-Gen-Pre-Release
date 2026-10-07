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

Planned release tag: `v0.5.0-pre.3`. File version: `0.5.0.0`. Product version: `pre-release-3`. Assembly identity remains `0.1.0.0` for settings compatibility.

- Completed source indexing before reading HDR/Dolby Vision metadata.
- Kept the **When finished do:** caption readable during indexing and Dolby Vision processing; job-only controls retain their existing enabled/disabled behavior.
- Updated QSVEncC to **8.32**.
- Detect tool versions and available build revisions from supported version commands or EXE/DLL metadata. Files without usable information retain their configured version.
- Use **Dark | Blue** as the default theme for new settings; existing saved themes are preserved.
- **Help → Website** opens this GitHub repository.
- Removed the remaining software video encoder classes, controls, resources and settings, including x264/x265, SVT-AV1 variants, VVenC, AOM, rav1e and the standalone FFmpeg video encoder. FFmpeg media utilities are retained.
- Retained automatic Dolby Vision Level 5 JSON export and **Tools → DV Offset**. Adjusted output uses the `_DV_Offset.hevc` filename.

## Portable layout

Launch `StaxRipNG.exe` from the extracted main folder. Keep `Apps`, `Fonts`, `Icons` and `Runtime` beside it. The bundled .NET 10.0.12 Windows desktop runtime is in `Runtime`.

## Verification

Pre-Release 3 builds with zero compiler errors and zero warnings. Settings serialization, file safety checks available on this platform, 18 QSV color-metadata cases, Dolby Vision Level 5 copying and tool-version detection passed. Windows-only file-lock and cmd tests could not run on Linux. Windows startup, frame-server runtime and GPU encoding tests remain pending. See `VALIDATION-Pre-Release-3.txt` in the source archive for the validation scope.

See [CHANGELOG.md](CHANGELOG.md) for capabilities and DV Offset instructions.
