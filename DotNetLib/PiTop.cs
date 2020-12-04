using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PiTop;
using PiTop.Camera;
using PiTop.OledDevice;
using PiTop.Abstractions;

using SixLabors;
using AsyncIO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;


namespace DotNetLib
{
   public class FPiTop
    {
        private static PiTop4Board board;
        private static Display display;

#if RUNTIMEMODE
        private static Font LargeFont = SystemFonts.Collection.Find("Roboto").CreateFont(28);
       // private static Font PageNumberFont = SystemFonts.Collection.Find("Roboto").CreateFont(14);
        private static Font PageHeaderText = SystemFonts.Collection.Find("Roboto").CreateFont(16);
#endif

        [NativeExpose]
        public static string Path = "/home/pi/pi-top-4-.NET-SDK/";
        private static float m_CurrentDeltaTime;
        public delegate void InitBoardDelegate();
        [NativeExpose]
        public static void InitBoard()
        {
            Console.WriteLine("InitBoard");
            SetupBoard();
        }

        public FPiTop()
        {         
            Console.WriteLine("new FPiTop");
        }
   
        private static void SetupBoard()
        {            
            Console.WriteLine("SetupBoard");
            board = PiTop4Board.Instance;
           
            display = board.Display;

            board.SelectButton.Pressed += BoardSelectButtonPressed;
            board.CancelButton.Pressed += BoardCancelButtonPressed;
            board.UpButton.Pressed += BoardUpButtonPressed;
            board.DownButton.Pressed += BoardDownButtonPressed;    
            board.BatteryStateChanged += BoardOnBatteryStateChanged;           

#if RUNTIMEMODE

            MainPage();
#endif

        }

       
        private static  void BoardOnBatteryStateChanged(object sender, BatteryState state)
        {
           // Console.WriteLine("BoardOnBatteryStateChanged");
           // PrintBatteryState(state);
        }

      
        private static void BoardSelectButtonPressed(object sender, EventArgs state)
        {
            Console.WriteLine("BoardSelectButtonPressed");
        }

       
        private static void BoardCancelButtonPressed(object sender, EventArgs state)
        {
           Console.WriteLine("BoardCancelButtonPressed");
        }

      
        private static void BoardUpButtonPressed(object sender, EventArgs state)
        {
             Console.WriteLine("BoardUpButtonPressed");           
        }

     
        private static void BoardDownButtonPressed(object sender, EventArgs state)
        {
          
        }

      

        private static void MainPage()
        {

#if RUNTIMEMODE

            display.Draw((context, cr) =>
            {
                context.Clear(Color.Black); 
                context.DrawText("Test", LargeFont, Color.White, new PointF(0, 0));
            });
#endif

        }


        private static void PrintBatteryState(BatteryState state)
        {
            Console.WriteLine(state.ChargingState);
            Console.WriteLine(state.Capacity);
            Console.WriteLine(state.TimeRemaining);
            Console.WriteLine(state.Wattage);
        }       

        public delegate void RefreshBatteryStateDelegate();
        [NativeExpose]
        public static void RefreshBatteryState()
        {

#if RUNTIMEMODE
            board.RefreshBatteryState();
#endif

        }

        public delegate void TickDelegate(float delta);
        [NativeExpose]
        public static void Tick(float delta)
        {
            m_CurrentDeltaTime += delta;

            if(m_CurrentDeltaTime >= 1.0f / 30.0f)
            {
                MainPage();
                m_CurrentDeltaTime = 0.0f;
            }

        }

        public delegate void HideDisplayDelegate();
        [NativeExpose]
        public static void HideDisplay()
        {
            display.Hide();
        }

        public delegate void ShowDisplayDelegate();
        [NativeExpose]
        public static void ShowDisplay()
        {
            display.Show();
        }

        public delegate void TestFunctionOneDelegate(string message);
        [NativeExpose]
        public static void TestFunctionOne(string message)
        {       
            Console.WriteLine("TestFunctionOne: {0}", message);
        }

        public delegate void TestFunctionTwoDelegate(int message);
        [NativeExpose]
        public static void TestFunctionTwo(int message)
        {
            Console.WriteLine("TestFunctionTwo: {0}", message);
        }

        public delegate void TestFunctionThreeDelegate(int number, string message);
        [NativeExpose]
        public static void TestFunctionThree(int number, string message)
        {
            Console.WriteLine("TestFunctionThree: {0} {1}", message, number);
        }

        public delegate void TestFunctionDelegate(int number, string message,float single);
        [NativeExpose]
        public static void TestFunction(int number, string message, float single)
        {
            Console.WriteLine("TestFunctionThree: {0} {1} {2}", message, number, single);
        }

        public delegate void PrintDataDelegate();
        [NativeExpose]
        public static void PrintData()
        {
           Console.WriteLine("PrintData");
        }

        public delegate void RunThingDelegate();
        [NativeExpose]
        public static void RunThing()
        {
            Console.WriteLine("RunThing");
        }
    }
}
