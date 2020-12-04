
//This is a Generated file do not edit by hand


class FNetCoreLoader;
class FPiTop
{
public:
  FPiTop(FNetCoreLoader* NetCoreLoader);
  ~FPiTop();

  //Start Public Generated Functions  
  void InitBoard();
  void RefreshBatteryState();
  void Tick(float delta);
  void HideDisplay();
  void ShowDisplay();
  void TestFunctionOne(const char* message);
  void TestFunctionTwo(int message);
  void TestFunctionThree(int number,const char* message);
  void TestFunction(int number,const char* message,float single);
  void PrintData();
  void RunThing();
  
  //End Public Generated Functions  
  static FPiTop* GetSelf()
  {
    return m_Self;
  }
private:
FNetCoreLoader* m_NetCoreLoader = nullptr;
  //Start Private Generated Functions  
  
  //End Private Generated Functions  
  static FPiTop* m_Self;
};

