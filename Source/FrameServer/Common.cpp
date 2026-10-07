#include "Common.h"
#include "CheckedInteger.h"

#include <system_error>

namespace
{
    std::string WideToMultiByte(const std::wstring& input, UINT codePage)
    {
        if (input.empty()) return {};
        const int length = CheckedFrameServerLength(input.size());
        const int count = WideCharToMultiByte(codePage, 0, input.data(), length, nullptr, 0, nullptr, nullptr);
        if (count == 0)
            throw std::system_error(static_cast<int>(GetLastError()), std::system_category(), "WideCharToMultiByte");
        std::string output(static_cast<std::size_t>(count), '\0');
        if (WideCharToMultiByte(codePage, 0, input.data(), length, output.data(), count, nullptr, nullptr) == 0)
            throw std::system_error(static_cast<int>(GetLastError()), std::system_category(), "WideCharToMultiByte");
        return output;
    }

    std::wstring MultiByteToWide(const std::string& input, UINT codePage)
    {
        if (input.empty()) return {};
        const int length = CheckedFrameServerLength(input.size());
        const int count = MultiByteToWideChar(codePage, 0, input.data(), length, nullptr, 0);
        if (count == 0)
            throw std::system_error(static_cast<int>(GetLastError()), std::system_category(), "MultiByteToWideChar");
        std::wstring output(static_cast<std::size_t>(count), L'\0');
        if (MultiByteToWideChar(codePage, 0, input.data(), length, output.data(), count) == 0)
            throw std::system_error(static_cast<int>(GetLastError()), std::system_category(), "MultiByteToWideChar");
        return output;
    }
}

std::string ConvertWideToANSI(const std::wstring& wstr)
{
    return WideToMultiByte(wstr, CP_ACP);
}

std::wstring ConvertAnsiToWide(const std::string& str)
{
    return MultiByteToWide(str, CP_ACP);
}

std::string ConvertWideToUtf8(const std::wstring& wstr)
{
    return WideToMultiByte(wstr, CP_UTF8);
}

std::wstring ConvertUtf8ToWide(const std::string& str)
{
    return MultiByteToWide(str, CP_UTF8);
}

std::string GetWinErrorMessage(int id)
{
    std::string ret(2048, '\0');
    const DWORD bufferLength = static_cast<DWORD>(CheckedFrameServerLength(ret.size()));
    const DWORD count = FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        nullptr, static_cast<DWORD>(id), MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        ret.data(), bufferLength, nullptr);
    ret.resize(static_cast<std::size_t>(count));
    return ret;
}

bool FileExists(LPCWSTR szPath)
{
    DWORD dwAttrib = GetFileAttributes(szPath);
    return (dwAttrib != INVALID_FILE_ATTRIBUTES && !(dwAttrib & FILE_ATTRIBUTE_DIRECTORY));
}
