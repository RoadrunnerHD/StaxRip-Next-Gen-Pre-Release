# [Documentation](../README.md) / [Introduction](README.md) / System Requirements

## Operating system

StaxRip Next Gen is a 64-bit Windows application for Windows 10 and Windows 11. Windows 7, Windows 8 and Windows 8.1 are not supported by this .NET 10 package.

## Hardware

Video encoding requires compatible Intel, NVIDIA or AMD hardware and a suitable graphics driver. Codec, bit-depth and VPP availability depend on the GPU, driver and selected QSVEncC, NVEncC or VCEEncC version. Bundled filters and utilities may have their own CPU instruction requirements.

## Runtimes

The complete portable package includes .NET 10.0.12 and the Windows desktop runtime. StaxRipNG does not require a separate .NET Framework installation.

Bundled tools and filters may require Microsoft Visual C++ runtime libraries. A missing dependency reported by a tool must be resolved for that tool.

See [Installation](Installation.md) for extraction and launch instructions.
