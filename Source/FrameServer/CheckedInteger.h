#pragma once

#include <cstddef>
#include <cstdint>
#include <limits>
#include <stdexcept>

inline bool FitsFrameServerInt(std::int64_t value)
{
    return value >= (std::numeric_limits<int>::min)() &&
           value <= (std::numeric_limits<int>::max)();
}

inline int CheckedFrameServerInt(std::int64_t value, const char* description)
{
    if (!FitsFrameServerInt(value))
        throw std::overflow_error(description);
    return static_cast<int>(value);
}

inline int CheckedFrameServerLength(std::size_t value)
{
    if (value > static_cast<std::size_t>((std::numeric_limits<int>::max)()))
        throw std::length_error("Text or buffer length exceeds the Windows API limit.");
    return static_cast<int>(value);
}
