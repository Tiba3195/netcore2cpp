#include "NetCoreLoader.h"
#include <string>
#include <iostream>
#include <memory>
#include <stdexcept>
#include <iomanip> 
#include "coreclr_delegates.h"
#include <limits>
#include <assert.h>
#include <dlfcn.h>

FNetCoreLoader* FNetCoreLoader::m_Self = nullptr;

using string_t = std::basic_string<char_t>;

// Function pointer types for the managed call and callback
typedef int(*report_callback_ptr)(NativeMessageArgs progress);
typedef int(*report_commandlist)(void* commandlist);
int ReportNativeMessageCallback(NativeMessageArgs progress);
int ReportCommandlistCallback(void* commandlist);

namespace
{
	// Globals to hold hostfxr exports
	hostfxr_initialize_for_runtime_config_fn init_fptr;
	hostfxr_get_runtime_delegate_fn get_delegate_fptr;
	hostfxr_close_fn close_fptr;

	// Forward declarations
	bool load_hostfxr();
	load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(std::string assembly);
}

load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;


typedef void (CORECLR_DELEGATE_CALLTYPE* call_managed_function_fn)(managed_function_args args);
call_managed_function_fn callManagedFunction = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* custom_entry_point_fn)(lib_args args);
custom_entry_point_fn sendMessageToManaged = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* set_callback_fn)(report_callback_ptr callbackFunction);
set_callback_fn setCallbackfunc = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* set_commandlist_callback_fn)(report_commandlist callbackFunction);
set_commandlist_callback_fn setCommandlistCallbackfunc = nullptr;

std::string dotnetlib_path;






void* FNetCoreLoader::GetAssemblyLoaderFunction()
{
	return (void*)load_assembly_and_get_function_pointer;
}


const char* FNetCoreLoader::GetPath()
{
	return  dotnetlib_path.c_str();
}

FNetCoreLoader::FNetCoreLoader()
{
	m_Self = this;
}

int FNetCoreLoader::SetupNetCoreHost()
{
	string_t root_path = "/home/pi/projects/netcore2cpp/PiTop/netcoreapp3.1/";
	auto pos = root_path.find_last_of(DIR_SEPARATOR);
	assert(pos != string_t::npos);
	root_path = root_path.substr(0, pos + 1);

	//
	// STEP 1: Load HostFxr and get exported hosting functions
	//
	if (!load_hostfxr())
	{
		assert(false && "Failure: load_hostfxr()");
		return EXIT_FAILURE;
	}

	//
	// STEP 2: Initialize and start the .NET Core runtime
	//
	const std::string config_path = root_path + std::string("DotNetLib.runtimeconfig.json");

	load_assembly_and_get_function_pointer = get_dotnet_load_assembly(config_path);
	assert(load_assembly_and_get_function_pointer != nullptr && "Failure: get_dotnet_load_assembly()");

	//
	// STEP 3: Load managed assembly and get function pointer to a managed method
	//
	dotnetlib_path = root_path + std::string("DotNetLib.dll");
	const char_t* dotnet_type = STR("DotNetLib.Lib, DotNetLib");

	// Function pointer to managed delegate
	int rc = 0;

	std::string asString = std::to_string((uint64_t)nullptr);

	std::string asString2 = std::to_string((uint64_t)nullptr);

	int32_t MessageType = -1;

	// Function pointer to managed delegate with non-default signature
	rc = load_assembly_and_get_function_pointer(
		dotnetlib_path.c_str(),
		dotnet_type,
		STR("CustomEntryPoint") /*method_name*/,
		STR("DotNetLib.Lib+CustomEntryPointDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&sendMessageToManaged);

	assert(rc == 0 && sendMessageToManaged != nullptr && "Failure: load_assembly_and_get_function_pointer()");

	//report_callback_ptr callbackFunction
	// Function pointer to managed delegate with non-default signature
	rc = load_assembly_and_get_function_pointer(
		dotnetlib_path.c_str(),
		dotnet_type,
		STR("SetupCallBack") /*method_name*/,
		STR("DotNetLib.Lib+SetupCallBackDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&setCallbackfunc);

	assert(rc == 0 && setCallbackfunc != nullptr && "Failure: load_assembly_and_get_function_pointer()");

	//report_callback_ptr callbackFunction
	// Function pointer to managed delegate with non-default signature
	rc = load_assembly_and_get_function_pointer(
		dotnetlib_path.c_str(),
		dotnet_type,
		STR("SetupCommandlistCallBack") /*method_name*/,
		STR("DotNetLib.Lib+SetupCommandlistCallBackDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&setCommandlistCallbackfunc);

	assert(rc == 0 && setCommandlistCallbackfunc != nullptr && "Failure: load_assembly_and_get_function_pointer()");


	//report_callback_ptr callbackFunction
// Function pointer to managed delegate with non-default signature
	rc = load_assembly_and_get_function_pointer(
		dotnetlib_path.c_str(),
		dotnet_type,
		STR("CallManagedFunction") /*method_name*/,
		STR("DotNetLib.Lib+CallManagedFunctionDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&callManagedFunction);

	assert(rc == 0 && callManagedFunction != nullptr && "Failure: load_assembly_and_get_function_pointer()");

	//Startup	
	MessageType = 0;
	lib_args messageArgs
	{
		STR("Start Lib\n"),
		MessageType
	};
	sendMessageToManaged(messageArgs);


	//Load Base deps and set Global Path
	MessageType = 2;
	lib_args loadArgs
	{
		nullptr,
		MessageType
	};
	sendMessageToManaged(loadArgs);


	//Load all other deps
	MessageType = 3;
	lib_args loadNoneBaseArgs
	{
		nullptr,
		MessageType
	};
	sendMessageToManaged(loadNoneBaseArgs);


	//Setup D3D12
	MessageType = 1;
	lib_args d3dArgs
	{
		nullptr,
		MessageType
	};
	sendMessageToManaged(d3dArgs);


	//Setup Command Allocator
	MessageType = 5;
	lib_args allocatorArgs
	{
		nullptr,
		MessageType
	};
	sendMessageToManaged(allocatorArgs);


	//Setup Managed to Native communication
	setCallbackfunc(ReportNativeMessageCallback);
	setCommandlistCallbackfunc(ReportCommandlistCallback);

	//Open Managed Bridge
	MessageType = 4;
	lib_args openFormArgs
	{
		nullptr,
		MessageType
	};
	sendMessageToManaged(openFormArgs);

	return EXIT_SUCCESS;
}



/********************************************************************************************
 * Function used to load and activate .NET Core
 ********************************************************************************************/

namespace
{
	// Forward declarations
	void* load_library(std::string);
	void* get_export(void*, const char*);

#ifdef WINDOWS
	void* load_library(const char_t* path)
	{
		HMODULE h = ::LoadLibraryW(path);
		assert(h != nullptr);
		return (void*)h;
	}
	void* get_export(void* h, const char* name)
	{
		void* f = ::GetProcAddress((HMODULE)h, name);
		assert(f != nullptr);
		return f;
	}
#else
	void* load_library(const char_t* path)
	{
		void* h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
		assert(h != nullptr);
		return h;
	}
	void* get_export(void* h, const char* name)
	{
		void* f = dlsym(h, name);
		assert(f != nullptr);
		return f;
	}
#endif


	bool load_hostfxr()
	{
		// Load hostfxr and get desired exports
		void* lib = load_library("/home/pi/.dotnet/host/fxr/3.1.10/libhostfxr.so");
		init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
		get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
		close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

		return (init_fptr && get_delegate_fptr && close_fptr);
	}

	load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(std::string config_path)
	{
		// Load .NET Core
		void* load_assembly_and_get_function_pointer = nullptr;
		hostfxr_handle cxt = nullptr;
		int rc = init_fptr(config_path.c_str(), nullptr, &cxt);
		if (rc != 0 || cxt == nullptr)
		{
			std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
			close_fptr(cxt);
			return nullptr;
		}

		// Get the load assembly function pointer
		rc = get_delegate_fptr(
			cxt,
			hdt_load_assembly_and_get_function_pointer,
			&load_assembly_and_get_function_pointer);
		if (rc != 0 || load_assembly_and_get_function_pointer == nullptr)
			std::cerr << "Get delegate failed: " << std::hex << std::showbase << rc << std::endl;

		close_fptr(cxt);
		return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
	}

}

// Callback function passed to managed code to facilitate calling back into native code with status
int ReportNativeMessageCallback(NativeMessageArgs managedEvent)
{
	return FNetCoreLoader::GetSelf()->HandleManagedEvent(managedEvent);
}


// Callback function passed to managed code to facilitate calling back into native code with status
int ReportCommandlistCallback(void* commandlist)
{

	return 0;
}

int FNetCoreLoader::HandleManagedEvent(NativeMessageArgs managedEvent)
{
	switch (managedEvent.DataType)
	{
	case T_NONE:

		break;
	case T_STRING:

		break;
	case T_INT:

		break;
	case T_FLOAT:

		break;
	case T_VECTOR2:

		break;
	case T_VECTOR3:

		break;
	case T_VECTOR4:

		break;
	default:
		break;
	}

	return -managedEvent.Type;
}

void FNetCoreLoader::CallManagedFunction(managed_function_args ManagedArgs)
{
	if (callManagedFunction)
	{
		callManagedFunction(ManagedArgs);
	}	
}
