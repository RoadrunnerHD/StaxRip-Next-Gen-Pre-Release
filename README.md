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

**Pre-Release 3 — portable Windows application with bundled .NET 10**

StaxRip Next Gen is an independent continuation of StaxRip for hardware-based video processing on Windows. It combines hardware encoders, encoder-side video processing, AviSynth+ and VapourSynth with audio processing and muxing in one workflow.

## Hardware video encoding

- **Intel Quick Sync:** QSVEncC, with encoder-specific quality, bitrate, bit-depth and GOP controls. Project: [QSVEnc](https://github.com/rigaya/QSVEnc).
- **NVIDIA:** NVEncC for supported NVIDIA hardware.
- **AMD:** VCEEncC for supported AMD hardware.

Available codecs and acceleration features depend on the selected encoder, GPU and driver. StaxRip Next Gen exposes the encoder settings; the hardware encoder performs video compression.

## Video processing and VPP

VPP means video post-processing. It lets the encoder process video before compression, using the facilities supported by its hardware and filter implementation.

- **Dimensions:** crop, resize and padding to prepare the required output size and borders.
- **Interlaced sources:** Intel Quick Sync deinterlacing, including normal, inverse-telecine and bob modes.
- **Image processing:** denoising, sharpening, debanding, brightness, contrast, saturation and gamma controls through the available encoder-side filters.
- **Color processing:** matrix, primaries, transfer and range conversion.
- **HDR processing:** HDR-to-SDR tone-mapping controls, including supported Libplacebo options.

Encoder Options contains the VPP settings offered by each encoder. The filter list also includes encoder-side VPP entries. These entries pass processing arguments to the encoder separately from the AviSynth/VapourSynth preview script. Preview and final encoding can therefore use different processing paths.

Filter availability and combinations vary by encoder, hardware and input format. A listed option does not guarantee that every GPU supports it.

## Sources, color information and HDR

AviSynth+ and VapourSynth provide source decoding, indexing and script-based processing. Direct QSV readers and ffmpeg pipe inputs are also available through the encoder's decoder settings.

Source color metadata can be imported, and explicit VUI settings are available for SDR and HDR workflows. With AviSynth, VapourSynth and ffmpeg pipe inputs, automatic QSV color options are resolved from the original source metadata. When a value is absent or unsupported, the unsupported automatic copy option is omitted; no replacement color value is guessed. Explicit user values remain unchanged. Direct QSV readers retain their own automatic metadata copying.

HDR10, HDR10+ and Dolby Vision metadata controls are available where supported by the selected encoder and bundled tools. Metadata handling and tone mapping are separate operations: select the settings appropriate to the intended output.

## Audio and output

The included tools provide audio processing and muxing. Output can be elementary video streams, MKV, MP4 or TS/M2TS, depending on the selected encoder, muxer and stream formats.

StaxRip Next Gen offers extensive controls and is not a one-click encoder.

## Included .NET 10 runtime

The complete portable package includes **.NET 10.0.12**, including the Windows desktop runtime used by the application. Users do not need to install .NET separately to run StaxRipNG.

The runtime and managed application files are kept in `Runtime`. Runtime updates are delivered with StaxRip Next Gen updates. Some bundled tools may have separate runtime requirements, including Visual C++ libraries.

## Requirements

- 64-bit Windows.
- A GPU compatible with the selected hardware encoder and a suitable graphics driver.
- The Visual C++ runtime libraries required by the included tools.

## Extracting and launching

Extract the complete package into its own folder and launch `StaxRipNG.exe` in the main folder. Keep `Apps`, `Fonts`, `Icons` and `Runtime` beside it. The bundled .NET 10 runtime and managed application files are contained in `Runtime`; do not move its files into the main folder.

Download `StaxRip-Next-Gen-Pre-Release-3-Program.7z` from the [release assets](https://github.com/RoadrunnerHD/StaxRip-Next-Gen-Pre-Release/releases/tag/v0.5.0-pre.3) and extract it using 7-Zip. For split archives, download every part into the same folder and open the first `.7z.001` part. The matching source archive is `StaxRip-Next-Gen-Pre-Release-3-Source.zip`.

On first launch, choose a settings location. Use a separate settings folder for independent tests. Back up existing settings and projects before switching versions.

The source code ZIP is for building the application. It does not include the complete portable package or all bundled tools.

## Pre-Release 3

- Completed source indexing before reading HDR/Dolby Vision metadata.
- Kept the **When finished do:** caption readable during indexing and Dolby Vision processing; job-only controls retain their existing enabled/disabled behavior.
- Updated QSVEncC to **8.32**.
- Detect tool versions and available build revisions from supported version commands or EXE/DLL metadata. Files without usable information retain their configured version.
- Use **Dark | Blue** as the default theme for new settings; existing saved themes are preserved.
- **Help → Website** opens this GitHub repository.
- Removed the remaining software video encoder classes, controls, resources and settings, including x264/x265, SVT-AV1 variants, VVenC, AOM, rav1e and the standalone FFmpeg video encoder. FFmpeg media utilities are retained.
- Retained automatic Dolby Vision Level 5 JSON export and **Tools → DV Offset**. Adjusted output uses the `_DV_Offset.hevc` filename.

## Dolby Vision L5 metadata and DV Offset

**What does L5 describe?**

L5 metadata describes the actual visible picture area within the complete video frame. Black bars are not part of this active area. Four offset values specify the distances from the outer frame edges on the left, right, top and bottom to the visible picture.

When the video is downscaled, these pixel distances must be adjusted accordingly. For example, a UHD video with 3840 × 2160 pixels has top and bottom offsets of 280 pixels each. Its active picture area is therefore 3840 × 1600 pixels. After downscaling to Full HD at 1920 × 1080 pixels, the offsets are 140 pixels each and the active picture area is 1920 × 800 pixels.

**Why UHD, Full HD and HD?**

This workflow is deliberately designed for **3840 × 2160, 1920 × 1080 and 1280 × 720**. The complete 16:9 frame and the relative position of the visible picture are preserved. The existing L5 distances are scaled proportionally to the output resolution:

* UHD → Full HD: halve the offsets.
* UHD → HD: divide the offsets by three.
* Full HD → HD: multiply the offsets by two-thirds.

The existing Dolby Vision mastering is reused. **DV Offset does not create a new Dolby Vision master or calculate new creative brightness or color adjustments.** The three intended modes allow the existing picture geometry and its associated metadata to be carried over appropriately when changing resolution.

**Example: changing picture formats in IMAX scenes**

In a film with changing picture formats, such as **Top Gun: Maverick** in a corresponding version, wider film scenes and taller IMAX scenes can have different active picture areas. Black bars remain outside those areas.

If the original L5 metadata contains these changes, the different offsets are adjusted for the respective sections of the film. When going from UHD to Full HD, for example, **280 pixels become 140 pixels**, while **68 pixels become 34 pixels** in another section. These numbers illustrate the principle and are not measured values from the film.

This preserves the picture-format changes described by the L5 metadata at the new resolution.

**How to use DV Offset**

For Dolby Vision material with available L5 metadata, StaxRip additionally exports the file `HDRDVmetadata_L5.json` to the HEVC output folder.

After encoding, open **Tools → DV Offset**, drag the matching `.hevc` file into the console window and press **Enter**. The tool scales the existing L5 offsets and writes the adjusted Dolby Vision RPU into a new file ending in `_DV_Offset.hevc`.

**The tool adjusts metadata. The video is resized beforehand during encoding.** HEVC output is currently supported; MKV support is planned.

At the final deletion prompt, the original input HEVC file and the L5 JSON are deleted only if you confirm. The newly created file is retained. Enter **`n`** to keep all files.

## Changes and feedback

Open Help → What's new or Help → Changelog for the NG release notes. For bug reports, include the application version, encoder, GPU, source format and relevant log excerpt.

## Origins and acknowledgements

Original project: [StaxRip](https://github.com/staxrip/staxrip)

StaxRip Next Gen is developed by Roadrunner. Thanks to stax76, the original developer of StaxRip, as well as Dendraspis and all previous contributors.

In memory of stax76: His work forms the foundation of this project. We remember him with gratitude and respect.

## License

The application source code is licensed under the MIT License. The full license text is included in `License.txt`. Bundled third-party tools are subject to their respective licenses.
