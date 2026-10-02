# StaxRip Next Gen

**Pre-Release — portable Windows application with bundled .NET 10**

StaxRip Next Gen is an independent continuation of StaxRip for hardware-based video processing on Windows. It combines hardware encoders, encoder-side video processing, AviSynth+ and VapourSynth with audio processing and muxing in one workflow.

## Hardware video encoding

- **Intel Quick Sync:** QSVEncC by Rigaya, with encoder-specific quality, bitrate, bit-depth and GOP controls. Project: [QSVEnc](https://github.com/rigaya/QSVEnc).
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

For a split 7z archive, keep all parts in the same folder and open the first part ending in `.7z.001` to extract the package.

On first launch, choose a settings location. Use a separate settings folder for independent tests. Back up existing settings and projects before switching versions.

The source code ZIP is for building the application. It does not include the complete portable package or all bundled tools.

## Pre-Release

Pre-Release is based on the tested Beta 4 correction. It updates the English release notes, README and application information to explain the processing features already available. The existing interface and encoding behavior are retained.

The fixes for false empty Encoder Error messages and QSV automatic color options with script/pipe inputs remain included.

## Changes and feedback

Open Help → What's new or Help → Changelog for the NG release notes. For bug reports, include the application version, encoder, GPU, source format and relevant log excerpt.

## Origins and acknowledgements

Original project: [StaxRip](https://github.com/staxrip/staxrip)

StaxRip Next Gen is developed by Roadrunner. Thanks to stax76, the original developer of StaxRip, as well as Dendraspis and all previous contributors.

In memory of stax76: His work forms the foundation of this project. We remember him with gratitude and respect.

## License

The application source code is licensed under the MIT License. The full license text is included in `License.txt`. Bundled third-party tools are subject to their respective licenses.
