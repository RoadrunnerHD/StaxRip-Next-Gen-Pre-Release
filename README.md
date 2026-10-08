# StaxRip Next Gen Release

**Portable Windows application with bundled .NET 10.**

StaxRip Next Gen focuses on hardware video encoding with Intel Quick Sync (QSVEncC), NVIDIA (NVEncC) and AMD (VCEEncC), alongside HDR and Dolby Vision workflows. Software encoding can also be used where supported by the selected encoder within the application.

## Download

Download the program and the separate source archive from [Release 0.02](../../releases/tag/v0.02).

- **Program:** `StaxRip-Next-Gen-Release-0.01-Program.7z`
- **Source code:** `StaxRip-Next-Gen-Release-0.01-Source.zip`

Use the program archive to run the application. The source archive is supplied separately for development and building.

## What's new

- Next Gen UI with a consistent dark appearance, rounded controls and compact filter, resolution and encoder areas.
- Independent audio-track areas with scrolling when needed.
- Clear resolution information and an indication of whether AviSynth or VapourSynth is being used.
- Improved search fields, audio buttons and processing-output colors.
- Logs archived in the selected settings folder after opening a source and after processing a job.
- Four settings locations: program folder, AppData, ProgramData or a custom folder. A valid saved location is reused; when it is missing, the selection appears again.
- Bundled .NET 10 runtime and portable launcher.
- DV-Offset available directly from the main menu.

## Hardware encoding and VPP

- **Intel Quick Sync:** QSVEncC.
- **NVIDIA:** NVEncC.
- **AMD:** VCEEncC.

Supported codecs and acceleration features depend on the selected encoder, GPU and driver.

Encoder-side VPP settings include crop, resize, padding, deinterlacing, denoising, sharpening, debanding, color conversion and HDR tone mapping where supported. AviSynth+ and VapourSynth provide source decoding, indexing and script-based processing. Audio processing and muxing are included.

## Installation

Extract the complete program archive into its own folder and launch `StaxRipNG.exe`. Keep `Apps`, `Fonts`, `Icons` and `Runtime` beside it. The bundled .NET runtime does not need to be installed separately.

Choose a settings location on first launch. Keep the source archive separate from the program folder.

Requirements: 64-bit Windows, suitable hardware and drivers for the selected encoder, and any Visual C++ runtime libraries required by the bundled tools.

## Dolby Vision L5 metadata and DV-Offset

### What does L5 describe?

L5 metadata describes the visible picture area within the complete video frame. Black bars are outside this active area. Four offset values give the distances from the left, right, top and bottom edges of the frame to the visible picture.

```text
3840 × 2160 complete frame
┌───────────────────────┐
│█████ black bars ██████│
│                       │
│     Movie picture     │
│                       │
│█████ black bars ██████│
└───────────────────────┘
```

When the video is resized, these pixel distances must be scaled accordingly. For example, a 3840 × 2160 frame with top and bottom offsets of 280 pixels has an active area of 3840 × 1600. At 1920 × 1080, the offsets become 140 pixels and the active area becomes 1920 × 800.

### Supported resolutions

The workflow is designed for UHD (3840 × 2160), Full HD (1920 × 1080) and HD (1280 × 720). The complete 16:9 frame and the relative position of the visible picture are preserved.

The existing Dolby Vision mastering is reused. DV-Offset adjusts the L5 geometry; it does not create a new Dolby Vision master or calculate new creative brightness or color adjustments.

### Changing picture formats

Films with changing picture formats, such as a corresponding version of Top Gun: Maverick, can have different active picture areas in different scenes.

If those changes are present in the original L5 metadata, DV-Offset scales the offsets for the respective sections. At half resolution, for example, 280 pixels become 140 pixels and 68 pixels become 34 pixels. These numbers illustrate the principle and are not measured values from the film.

### How to use DV-Offset

For Dolby Vision material with available L5 metadata, StaxRip exports `HDRDVmetadata_L5.json` to the HEVC output folder.

1. Resize the video during encoding.
2. Open **DV-Offset** from the main menu.
3. Select the scaling operation, drag the matching `.hevc` file into the console and press Enter.
4. The tool adjusts the existing L5 offsets and writes the adjusted Dolby Vision RPU into a new file ending in `_DV.hevc`.

HEVC output is currently supported; MKV support is planned.

At the final deletion prompt, the original input HEVC file and the L5 JSON are deleted only if you confirm. The new output file is retained. Enter `n` to keep all files.

## Feedback

Use [Issues](../../issues) for bug reports and [Discussions](../../discussions) for questions and suggestions. For a bug report, include the application version, encoder, GPU, source format and a relevant log excerpt.

## Origins and acknowledgements

StaxRip Next Gen is developed by Roadrunner and is based on [StaxRip](https://github.com/staxrip/staxrip).

In memory of stax76, the original developer of StaxRip. His work forms the foundation of this project. Thanks to all previous contributors.

## License

The application source is licensed under the MIT License. The full license text is included in the source archive. Bundled third-party tools are subject to their respective licenses.
