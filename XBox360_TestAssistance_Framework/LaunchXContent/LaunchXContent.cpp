// LaunchXContent.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "stdlib.h"
#include "stdio.h"

 DWORD __stdcall threadmain(LPVOID lpThreadParameter);


//-------------------------------------------------------------------------------------
// Name: main()
// Desc: The application's entry point
//-------------------------------------------------------------------------------------
void __cdecl main()
{
	DWORD result;
	LPSTR cmdLine = GetCommandLine();

	if ((cmdLine != NULL) && (*cmdLine != 0))
	{
		// skip past the exe name
		while ((*cmdLine != 0) && (*cmdLine != ' '))
			cmdLine++;
		if (*cmdLine != 0)
			cmdLine++;

		char* titleFullpath = cmdLine;
		result = XContentLaunchImageFromFile(titleFullpath, "default.xex");
	}
}






