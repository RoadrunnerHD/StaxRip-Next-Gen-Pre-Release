# StaxRip Next Gen — Current release status

Updated: 2026-10-02. Release: **v0.5.0-pre.2**.

## Encoding and processing

| Area | Current NG behavior |
| --- | --- |
| Intel | QSVEncC hardware encoding through Intel Quick Sync |
| NVIDIA | NVEncC hardware encoding |
| AMD | VCEEncC hardware encoding |
| Video processing | Encoder-side crop, resize and VPP filters where supported |
| Source processing and preview | AviSynth+ and VapourSynth |
| Software video encoders | Removed |
| Deployment | Portable Windows package with bundled .NET 10 runtime |

## Corrections in this release

- Portable startup and legacy settings import corrected.
- AviSynth scripts written as UTF-8 without BOM; non-ASCII source paths handled.
- Resize menu entries restored.
- Encoder command display corrected to show actual arguments and paths.
- Bundled tool timestamps and version detection corrected; 7-Zip updated to 26.03.
- Settings loading restricted to supported data types with resource limits.
- Downloads require HTTPS and validated redirects; interrupted downloads preserve working files.
- Tool updates and project loading hardened.
- File safety and serialization regression checks included in the build; Windows TLS test certificate handling corrected.
- Build actions pinned to full commit hashes with read-only workflow permissions.

## Verification and scope

Application startup, AviSynth and VapourSynth source loading, resize menu entries and tool status were checked during the release preparation. These checks do not cover every GPU, driver, input format or bundled third-party binary.

The former 0.28/0.29 archive comparison has been replaced by this current release overview. Its archive sizes and file counts do not describe v0.5.0-pre.2.

See [NGCHANGELOG.md](NGCHANGELOG.md) for the NG development history and [SECURITY.md](SECURITY.md) for vulnerability reporting.
