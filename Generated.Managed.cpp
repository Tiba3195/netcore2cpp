
//This is a Generated file do not edit by hand

#include "Generated.Managed.h"
#include "NetCoreLoader.h"
#include "coreclr_delegates.h"

FPiTop* FPiTop::m_Self = nullptr;

//Generated Static Members


//Managed function pointer type defs
typedef void (CORECLR_DELEGATE_CALLTYPE* call_InitBoard_fn)();
call_InitBoard_fn InitBoard_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_RefreshBatteryState_fn)();
call_RefreshBatteryState_fn RefreshBatteryState_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_Tick_fn)(float delta);
call_Tick_fn Tick_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_HideDisplay_fn)();
call_HideDisplay_fn HideDisplay_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_ShowDisplay_fn)();
call_ShowDisplay_fn ShowDisplay_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_TestFunctionOne_fn)(const char* message);
call_TestFunctionOne_fn TestFunctionOne_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_TestFunctionTwo_fn)(int message);
call_TestFunctionTwo_fn TestFunctionTwo_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_TestFunctionThree_fn)(int number,const char* message);
call_TestFunctionThree_fn TestFunctionThree_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_TestFunction_fn)(int number,const char* message,float single);
call_TestFunction_fn TestFunction_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_PrintData_fn)();
call_PrintData_fn PrintData_ptr = nullptr;

typedef void (CORECLR_DELEGATE_CALLTYPE* call_RunThing_fn)();
call_RunThing_fn RunThing_ptr = nullptr;



//Function to load the assembly and get function a pointer to the managed function
load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer_2 = nullptr;

//==================================================================
//==================================================================
FPiTop::FPiTop(FNetCoreLoader* NetCoreLoader)
{
    //Our runtime loader
    m_NetCoreLoader = NetCoreLoader;

    //Get the function pointer from the loader
    load_assembly_and_get_function_pointer_2 = (load_assembly_and_get_function_pointer_fn)m_NetCoreLoader->GetAssemblyLoaderFunction();

    int rc = 0;
    
    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("InitBoard") /*method_name*/,
		STR("DotNetLib.FPiTop+InitBoardDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&InitBoard_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("RefreshBatteryState") /*method_name*/,
		STR("DotNetLib.FPiTop+RefreshBatteryStateDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&RefreshBatteryState_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("Tick") /*method_name*/,
		STR("DotNetLib.FPiTop+TickDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&Tick_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("HideDisplay") /*method_name*/,
		STR("DotNetLib.FPiTop+HideDisplayDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&HideDisplay_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("ShowDisplay") /*method_name*/,
		STR("DotNetLib.FPiTop+ShowDisplayDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&ShowDisplay_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("TestFunctionOne") /*method_name*/,
		STR("DotNetLib.FPiTop+TestFunctionOneDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&TestFunctionOne_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("TestFunctionTwo") /*method_name*/,
		STR("DotNetLib.FPiTop+TestFunctionTwoDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&TestFunctionTwo_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("TestFunctionThree") /*method_name*/,
		STR("DotNetLib.FPiTop+TestFunctionThreeDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&TestFunctionThree_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("TestFunction") /*method_name*/,
		STR("DotNetLib.FPiTop+TestFunctionDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&TestFunction_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("PrintData") /*method_name*/,
		STR("DotNetLib.FPiTop+PrintDataDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&PrintData_ptr);


    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		STR("DotNetLib.FPiTop, DotNetLib") /*type_name*/,
		STR("RunThing") /*method_name*/,
		STR("DotNetLib.FPiTop+RunThingDelegate, DotNetLib") /*delegate_type_name*/,
		nullptr,
		(void**)&RunThing_ptr);


}
//==================================================================
//==================================================================
FPiTop::~FPiTop()
{
}

//Generated Functions

//==================================================================
//==================================================================
void FPiTop::InitBoard()
{
   //Make sure we have a valid function pointer
    if(InitBoard_ptr)
    {
       InitBoard_ptr();
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::RefreshBatteryState()
{
   //Make sure we have a valid function pointer
    if(RefreshBatteryState_ptr)
    {
       RefreshBatteryState_ptr();
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::Tick(float delta)
{
   //Make sure we have a valid function pointer
    if(Tick_ptr)
    {
       Tick_ptr(delta);
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::HideDisplay()
{
   //Make sure we have a valid function pointer
    if(HideDisplay_ptr)
    {
       HideDisplay_ptr();
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::ShowDisplay()
{
   //Make sure we have a valid function pointer
    if(ShowDisplay_ptr)
    {
       ShowDisplay_ptr();
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::TestFunctionOne(const char* message)
{
   //Make sure we have a valid function pointer
    if(TestFunctionOne_ptr)
    {
       TestFunctionOne_ptr(message);
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::TestFunctionTwo(int message)
{
   //Make sure we have a valid function pointer
    if(TestFunctionTwo_ptr)
    {
       TestFunctionTwo_ptr(message);
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::TestFunctionThree(int number,const char* message)
{
   //Make sure we have a valid function pointer
    if(TestFunctionThree_ptr)
    {
       TestFunctionThree_ptr(number,message);
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::TestFunction(int number,const char* message,float single)
{
   //Make sure we have a valid function pointer
    if(TestFunction_ptr)
    {
       TestFunction_ptr(number,message,single);
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::PrintData()
{
   //Make sure we have a valid function pointer
    if(PrintData_ptr)
    {
       PrintData_ptr();
    }
   
   
}


//==================================================================
//==================================================================
void FPiTop::RunThing()
{
   //Make sure we have a valid function pointer
    if(RunThing_ptr)
    {
       RunThing_ptr();
    }
   
   
}


