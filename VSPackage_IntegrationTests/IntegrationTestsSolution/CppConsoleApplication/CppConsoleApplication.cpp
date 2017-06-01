// CppConsoleApplication.cpp : Defines the entry point for the console application.
//

#include <tchar.h>
#include "../CppConsoleApplicationDll/CppConsoleApplicationDll.h"
#include <string>

int _tmain(int argc, _TCHAR* argv[])
{																	// COVERED
	if (argc == 2 && argv[1] == std::wstring(L"TestEnvVariable"))	// COVERED
	{																// UNCOVERED
		char* value = nullptr;										// UNCOVERED
		size_t size = 0;											// UNCOVERED
		_dupenv_s(&value, &size, "EXIT_CODE");						// UNCOVERED
		if (value)													// UNCOVERED
			return std::atoi(value);								// UNCOVERED
	}																// UNCOVERED
	if (false)														// COVERED
		Hello();													// UNCOVERED
	else															// UNCOVERED
		Hello();													// COVERED
	return 0;														// COVERED
}																	// COVERED

