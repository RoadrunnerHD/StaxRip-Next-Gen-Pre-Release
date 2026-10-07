#ifndef UNICODE
#define UNICODE
#endif
#ifndef _UNICODE
#define _UNICODE
#endif
#include <windows.h>
#include <string>

int WINAPI wWinMain(HINSTANCE, HINSTANCE, PWSTR, int) {
    std::wstring root(32768, L'\0');
    DWORD size = GetModuleFileNameW(nullptr, root.data(), static_cast<DWORD>(root.size()));
    if (!size || size >= root.size()) return 1;
    root.resize(size);
    const auto separator = root.find_last_of(L"\\/");
    if (separator == std::wstring::npos) return 1;
    root.resize(separator == 2 && root[1] == L':' ? 3 : separator);
    const std::wstring executable = root + L"\\Runtime\\StaxRipNG.exe";
    const wchar_t* arguments = GetCommandLineW();
    if (*arguments == L'"') {
        ++arguments;
        while (*arguments && *arguments != L'"') ++arguments;
        if (*arguments) ++arguments;
    } else {
        while (*arguments && *arguments != L' ' && *arguments != L'\t') ++arguments;
    }
    std::wstring command = L"\"" + executable + L"\"" + arguments;
    if (!SetEnvironmentVariableW(L"STAXRIP_PORTABLE_ROOT", root.c_str())) return 1;
    STARTUPINFOW startup{};
    startup.cb = sizeof(startup);
    PROCESS_INFORMATION process{};
    if (!CreateProcessW(executable.c_str(), command.data(), nullptr, nullptr, FALSE, 0, nullptr, root.c_str(), &startup, &process)) {
        MessageBoxW(nullptr, L"StaxRip could not start. Keep the complete Runtime folder beside StaxRipNG.exe.", L"StaxRip Next Gen Pre-Release 3", MB_ICONERROR);
        return 1;
    }
    CloseHandle(process.hThread);
    WaitForSingleObject(process.hProcess, INFINITE);
    DWORD exitCode = 1;
    GetExitCodeProcess(process.hProcess, &exitCode);
    CloseHandle(process.hProcess);
    return static_cast<int>(exitCode);
}
