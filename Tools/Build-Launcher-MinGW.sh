#!/usr/bin/env bash
set -euo pipefail
project_root="$(cd "$(dirname "$0")/.." && pwd)"
output_dir="${1:-$project_root/launcher-build}"
mkdir -p "$output_dir"
output_dir="$(cd "$output_dir" && pwd)"
cd "$project_root/Source/Launcher"
x86_64-w64-mingw32-windres --preprocessor=x86_64-w64-mingw32-gcc-win32 --preprocessor-arg=-E --preprocessor-arg=-xc --preprocessor-arg=-DRC_INVOKED Launcher.rc -O coff -o "$output_dir/Launcher.res.o"
x86_64-w64-mingw32-g++-win32 -std=c++17 -O2 -static -municode -mwindows -s Launcher.cpp "$output_dir/Launcher.res.o" -o "$output_dir/StaxRipNG.exe"
