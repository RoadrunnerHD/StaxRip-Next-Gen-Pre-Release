# QSVEncC 8.32 compatibility — StaxRip Next Gen Pre-Release 3

Compared the official 8.31 and 8.32 option documents and the 8.32 release notes:
https://github.com/rigaya/QSVEnc/releases/tag/8.32
https://github.com/rigaya/QSVEnc/blob/8.32/QSVEncC_Options.en.md

- New command-line option: `--backend auto|qsv|vaapi`, available only in VA-API builds on Linux. The Windows application continues to use QSV; a Linux-only selector is not added to its Windows UI.
- Changed constraint: `--la-depth` is disabled with `--icq`. The generated command already excludes Lookahead Depth in ICQ mode through its visibility condition. LA, LA-HRD and LA-ICQ are separate modes and remain available. No change to the user's HEVC/VBR settings is required.
- Upstream runtime fixes: TrueHD timestamps; interlaced resize completion; RTGMC search-prefilter completion; KFM/Degrain/NNEDI optimizations and deeper encoder output queuing. These changes are in the encoder executable, rather than new Windows command switches.
- The new `backend=qsv` log line identifies the active backend.
- The user completed an encode successfully with the supplied 8.32 executable. The cause of the earlier failure remains unknown.

Automatic app version detection is shared by Apps and encoder logs. Known console tools are queried with a bounded version command; other EXE/DLL files use version resources without loading DLLs. Available build/revision information is retained. Unchanged files use a cache keyed by path, size and modification timestamp. Files without usable information keep their configured version.
