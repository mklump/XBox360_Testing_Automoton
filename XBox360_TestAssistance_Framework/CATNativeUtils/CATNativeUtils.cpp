// -----------------------------------------------------------------------
// <copyright file="CATNativeUtils.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


#include "stdafx.h"
#include <string>
#include <atlbase.h>
#include <xdevkit.h>
#include <dia2.h>
#include <xbdm.h>
#include <dbghelp.h>

// Content largely copied from SymbolHelper.h/.cpp in CallStackDisplay sample

// We link with dbghelp.lib to make it convenient to call functions in dbghelp.dll,
// however we specify delay loading of dbghelp.dll so that we can load a specific
// version in LoadDbgHelp. This is crucial since otherwise we may get the version in
// the system directory, and then dbghelp.dll will not load symsrv.dll.
#pragma comment(lib, "dbghelp.lib")

// return NULL on failure
extern "C" __declspec(dllexport) void* __stdcall LookupSymbol(const wchar_t* symbolName, const wchar_t* symbolFile, const void* baseAddress, const DM_PDB_SIGNATURE* signature)
{
	void* result = NULL;

	// Create a DIA2 data source
    CComPtr <IDiaDataSource> pSource;
    HRESULT hr = CoCreateInstance( __uuidof( DiaSource ),
                                   NULL,
                                   CLSCTX_INPROC_SERVER,
                                   __uuidof( IDiaDataSource ),
                                   ( void** )&pSource );
	
    if (FAILED(hr))
        return result;

    // See if there is a PDB file at the specified location, and if so,
    // load it, checking the GUID and age to make sure that it is the correct
    // PDB file. Note that the timeStamp is not used anymore.
    hr = pSource->loadAndValidateDataFromPdb( symbolFile, const_cast<GUID*>( &signature->Guid ), 0, signature->Age );
    if (FAILED(hr))
        return result;

    // Create a session for the just loaded PDB file.
    CComPtr <IDiaSession> pSession;
    if (FAILED(pSource->openSession(&pSession)))
		return result;
   
	pSession->put_loadAddress((ULONG_PTR)baseAddress);

	CComPtr<IDiaSymbol> diaSymbol;
	CComPtr<IDiaEnumSymbols> diaEnumSymbols;

	hr = pSession->get_globalScope((IDiaSymbol**)&diaSymbol);
    if (FAILED(hr))
		return result;

	hr = pSession->findChildren(diaSymbol, SymTagFunction, symbolName, nsfCaseSensitive, (IDiaEnumSymbols**)&diaEnumSymbols);
    if (FAILED(hr))
		return result;

	LONG l;
	hr = diaEnumSymbols->get_Count(&l);
    if (FAILED(hr))
		return result;

	for (LONG cur = 0; cur < l; cur++)
	{
		CComPtr<IDiaSymbol> diaSymbol2;
		hr = diaEnumSymbols->Item(cur, (IDiaSymbol**)&diaSymbol2);
		if (FAILED(hr))
			return result;

		DWORD rva;
		hr = diaSymbol2->get_relativeVirtualAddress(&rva);
		if (hr == S_OK)
		{
			result = (char*)baseAddress + rva;	
			break;
		}
	}

	return result;
}