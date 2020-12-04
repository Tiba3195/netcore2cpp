#include <cstdio>
#include "NetCoreLoader.h"
#include "Generated.Managed.h"
#include "StepTimer.h"

FNetCoreLoader* m_NetCoreLoader = nullptr;
FPiTop* m_PiTop = nullptr;
DX::StepTimer m_gameTime;
bool Run = true;

void Tick()
{
	//Does the ticking
	m_gameTime.Tick([&]()
		{
			//pass delta time to the managed class
			m_PiTop->Tick(m_gameTime.GetElapsedSeconds());
		});
}

int main()
{

	m_gameTime.SetFixedTimeStep(true);
	m_gameTime.SetTargetElapsedSeconds(1.f / 60.f);

	m_NetCoreLoader->SetupNetCoreHost();
	m_PiTop = new  FPiTop(m_NetCoreLoader);
	m_PiTop->InitBoard();
	m_PiTop->RefreshBatteryState();

	m_PiTop->TestFunction(99,"blah",0.15f);	
	m_PiTop->TestFunctionTwo(99);
	m_PiTop->TestFunctionThree(99, "blah");

	//Run the update loop
	do
	{
		Tick();

	} while (Run);

    return 0;
}