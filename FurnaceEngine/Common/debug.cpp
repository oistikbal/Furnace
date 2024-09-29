#pragma once
#include "debug.h"
#include <Windows.h>

enum class log_type
{
	info = 1 << 0,
	warn = 1 << 1,
	error = 1 << 2,
};
