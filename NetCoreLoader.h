#pragma once


// Provided by the AppHost NuGet package and installed as an SDK pack
#include "nethost.h"

// Header files copied from https://github.com/dotnet/core-setup
#include "coreclr_delegates.h"
#include "hostfxr.h"

#ifdef WINDOWS
#include <Windows.h>

#define STR(s) L ## s
#define CH(c) L ## c
#define DIR_SEPARATOR L'\\'

#else
#include <dlfcn.h>
#include <limits.h>

#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'
#define MAX_PATH PATH_MAX

#endif
enum NativeDataType : int
{
	T_NONE = -1,
	T_STRING = 0,
	T_INT,
	T_FLOAT,
	T_VECTOR2,
	T_VECTOR3,
	T_VECTOR4,
};

struct lib_args
{

	const char_t* message;
	int number;
};

struct managed_function_args
{
	const char_t* functionName;
	const char_t* message;
	int number;
	int paramcount;

	managed_function_args()
	{
		functionName = nullptr;
		message = nullptr;
		number = 0;
		paramcount = 0;
	}
};

struct NativeMessageArgs
{
	const char_t* message;
	int Type;
	NativeDataType DataType;
};

class FNetCoreLoader
{

public:
	FNetCoreLoader();
	int SetupNetCoreHost();

	static FNetCoreLoader* GetSelf()
	{
		return m_Self;
	}
	
	int HandleManagedEvent(NativeMessageArgs managedEvent);
	void CallManagedFunction(managed_function_args ManagedArgs);

	void* GetAssemblyLoaderFunction();

	const	char* GetPath();


private:
	static	FNetCoreLoader* m_Self;
	
};

