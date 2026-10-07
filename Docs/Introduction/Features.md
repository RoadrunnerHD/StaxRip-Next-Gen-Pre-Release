# [Documentation](../README.md) / [Introduction](README.md) / Features

## General
- Support for a wide variety of formats and tools.
- Batch Processing, can not only be used for encoding but for any task like remuxing,
  demuxing or AviSynth+/VapourSynth script generation.
- Cut/Trim feature, for MKV output it works even in Copy/Mux mode, no re-encoding required.
- Minimal user interaction required due to rich configuration and automation features.
- Macro system to be used in scripts and command lines, this enables a design that allows
  to change any setting at any time which is much more flexible and powerful than a design
  that requires to perform one step after another.
- The job list can be processed with multiple StaxRip instances in parallel for maximum performance using modern CPUs.
- Video and multiple audio tracks can be processed in parallel for maximum performance with modern hardware.
- Support for an unlimited amount of audio and subtitle tracks.
- Automatic and manual update check.
- Powerful media info using the media info frontend MediaInfo.NET
  which includes a folder view to show media info for a folder in a grid view.
- Powerful media preview using the media player mpv.net.
- Support via Discord and Issue Tracker.
- Easy to use and feature rich crop dialog with hardware acceleration.
- Powerful resize and aspect ratio related features with auto-detection.
- Thumbnail generation tools.
- Full access and control to underlying tools with full support for
  command line and script editing to leverage the full potential of the tools.
- Container and hard-coded subtitles.
- Clean and easy to read log file and dedicated log file viewer to easily navigate the processing steps.
- Video comparison tool to compare video frames in a tab view.
- The processing window can be minimized to the system tray.
- For all essential console tools such as QSVEncC and mkvmerge there is a built-in command line preview
  either directly integrated in the dialog or via menu accessible.


## Tool Management
- Powerful tool management dialog.
- Bundled encoders, media utilities, frame servers and filters.
- Tools are searchable.
- Custom paths can be defined or removed.
- Custom paths can be defined using the powerful search and index tool voidtools Everything.
- Tool overview either as CSV file opened in (MS) Office or using PowerShell Out-GridView
  which supports a powerful search and filter feature.
- Feature to open the folder of a tool or start the tool including console tools showing the help.


## Help
- For every tool the help file or help page can be shown in the
  Apps management dialog and also in the main menu under Help.
- The Apps management dialog provides easy access to tool websites and download sites.
- Tooltips by mouse hover, right-click and Ctrl+right-click.
- Hardware encoder help is accessible from the encoder dialogs and the Apps management dialog.
- Customizable assistant messages.


## Preprocessing
- Preprocessing steps such as demuxing is fully customizable with custom command lines.
- Demuxers: mkvextract, MP4Box, ffmpeg, eac3to.
- Demuxing GUI for mkvextract, MP4Box, ffmpeg and eac3to to define which tracks should be demuxed.
- Automatic demuxing mode to automatically demux everything.
- Automatic indexing with DGIndex, DGIndexNV, ffms2 and L-Smash-Works.
- Support for opening ISO images natively.


## Projects, Templates, Profiles
- AviSynth+ and VapourSynth video filter plugins that are not
  included can be used by adding custom filter profiles.
- Customizable video filter profiles that can be enabled via context menu.
- Project system that automatically saves encoding settings for a particular source file.
- Previously encoded projects can be found in a Recent menu to encode them again
  with different settings, encoded audio and video of previous runs can be reused or overwritten.
- Project templates for different types of sources, a startup template
  can be defined to customize the startup settings.
- Profile system for video filter profiles, video encoder profiles, audio encoding profiles,
  muxing profiles, filter setup (full script) profiles.
- Project options used by projects and project templates and also separate global settings.


## Video Processing
- Video editing using classic AviSynth+ and VapourSynth, both equally well-supported
  and everything like QTGMC just works out of the box effortlessly.
- Large amount of included AviSynth+ and VapourSynth plugins, overall around 200 tools included.
- Code editor to enable full control over AviSynth+ and VapourSynth,
  everything that is possible with AviSynth+ and VapourSynth is also possible with StaxRip.
- Code preview to preview the code generated for AviSynth+ and VapourSynth.
- Built-in hardware accelerated video preview and preview via mpv.net.
- Built-in script info to show script parameters like frame rate and
  advanced script preview using various external tools like AVSMeter.
- External AviSynth+ (.avs) and VapourSynth (.vpy) scripts can be opened/imported.
- Auto crop.
- Full high bit depth and HDR support.
- Automatic import of (VUI) color metadata.
- Automatic extraction and import of HDR10+ and Dolby Vision metadata.
- Compressibility check.
- It can be customized which source filters should be used for different formats.
- Unicode support for AviSynth+ on Windows 10.


## Encoding
- Hardware encoding for AMD, Intel and NVIDIA.
- Hardware video encoders: QSVEncC, NVEncC and VCEEncC. Software video encoder integrations have been removed.
- Audio encoders: eac3to, qaac, fdkaac, opusenc, DeeZy/DEE, ffmpeg.
- Searchable encoder dialogs expose hardware encoder options and a command-line preview.
- Video encoder dialogs are searchable to quickly find options in a drop-down and search field.
- Video encoder dialogs have a command line preview, this preview has a context
  menu that allows to quickly find GUI options for command line switches.
- Custom command lines and scripts are available for supported processing tasks.
- Parallel audio/video encoding.


## Muxing
- MKV output using mkvmerge.
- MP4 output using MP4Box.
- ffmpeg supporting various container formats.
- Copy/Mux modes for audio and video plain remuxing without encoding.
- Cut/Trim feature for MKV output that works even in Copy/Mux mode, no re-encoding required.


## Power User
- Event Command feature that allows to assign custom commands to various events
  for instance to execute custom command lines on given events.
- Scriptable via PowerShell.
- Customizable main menu and context menus.
- Long path support, if enabled in Windows 10 file paths can be longer than 260 characters.
- A PowerShell terminal can be shown with support for the new Windows Terminal,
  this terminal has a special environment for easy access of included console
  tools, all macros are available as environment variables in this
  terminal and in all features that allow to define custom command lines.


## Installation
- Portable package with bundled .NET 10, AviSynth+, VapourSynth and Python. Some tools may require Visual C++ runtime libraries.
- Installed AviSynth+ or VapourSynth can optionally be used instead of the included.


## User Interface
- High DPI scaling.
- Dark Mode and themes; new settings use Dark | Blue.

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
