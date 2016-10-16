// CppConsoleApplication.cpp : Defines the entry point for the console application.
//

#include <tchar.h>
#include "../CppConsoleApplicationDll/CppConsoleApplicationDll.h"

int _tmain(int argc, _TCHAR* argv[])
{										// COVERED
	if (false)							// COVERED
		Hello();						// UNCOVERED
	else								// UNCOVERED
		Hello();						// COVERED
	return 0;							// COVERED
}										// COVERED

