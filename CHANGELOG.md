# StaxRip Next Gen — Changelog

This file documents StaxRip Next Gen. Hardware video encoding uses Intel Quick Sync, NVIDIA and AMD. Software video encoders have been removed.

## v0.5.0-pre.2 (2026-10-02)

Corrections included in this pre-release:

- Fixed application startup in the portable package.
- Corrected legacy settings import, including version values.
- Restricted settings deserialization to supported data types, with resource limits and without invoking serialized callbacks or constructors.
- Required HTTPS for downloads and validated redirects; rejected URL credentials and redirects to HTTP.
- Added download cancellation and transactional file replacement so interrupted or incomplete downloads do not replace working files.
- Hardened tool directory updates and project loading.
- Corrected AviSynth script encoding to UTF-8 without a byte-order mark and addressed source paths containing non-ASCII characters.
- Restored the missing resize menu entries.
- Corrected displayed encoder commands so normal arguments and file paths are visible instead of internal literal placeholders.
- Corrected bundled tool timestamps and version detection.
- Updated the bundled 7-Zip components to 26.03 and corrected their version configuration.
- Added file safety and serialization regression checks to the build.
- Corrected the Windows TLS certificate handling in the file safety tests.

Repository maintenance:

- Added SECURITY.md with guidance for reporting vulnerabilities and handling untrusted projects and scripts.
- Pinned the build workflow actions to full commit hashes and kept workflow permissions read-only.
- Corrected the release tag to match the corresponding source code.

These changes do not constitute a guarantee that the application or every bundled tool is free of vulnerabilities. Encoder and filter availability continues to depend on the selected hardware, driver and tools.

The complete NG development history is maintained in [NGCHANGELOG.md](NGCHANGELOG.md).
