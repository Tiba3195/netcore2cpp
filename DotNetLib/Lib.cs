using System;
using System.Runtime.InteropServices;
#if RUNTIMEMODE
using System.Runtime.Loader;
#endif

using System.Reflection;
using System.Collections.Generic;



namespace DotNetLib
{
    //================================================================================================================================================
    /*      
     *       
     *       
     *       
     *       
     *       
    */
    //================================================================================================================================================

    public static class Lib
    {


        public enum NativeDataType : int
        {
            NONE = -1,
            STRING = 0,
            INT,
            FLOAT,
            VECTOR2,
            VECTOR3,
            VECTOR4,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LibArgs
        {
            public IntPtr Message;
            public int Number;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ManagedFunctionArgs
        {
            public IntPtr FunctionName;
            public IntPtr Message;
            public int Number;
            public int paramcount;           
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessageArgs
        {
            public IntPtr Message;
            public int Type;
            public NativeDataType DataType;

            public NativeMessageArgs(IntPtr message, int type, NativeDataType num)
            {
                Message = message;
                Type = type;
                DataType = num;
            }
        }

        private static IntPtr NullPointer = (IntPtr)0;     
        private static FPiTop ReflectionObject = new FPiTop();
        private static RuntimeReflector Reflector;

        public delegate int ReportProgressFunction(NativeMessageArgs progress);
        public delegate int ReportCommandlistFunction(IntPtr commandlist);

        //I dont want to have to do this but i have to store managed function pointer in a list as i cant just set them on a member
        private static List<ReportProgressFunction> ReportProgressFunc = new List<ReportProgressFunction>();

        //I dont want to have to do this but i have to store managed function pointer in a list as i cant just set them on a member
        private static List<ReportCommandlistFunction> ReportCommandlistFunc = new List<ReportCommandlistFunction>();

        public static void Startup()
        {
            Console.WriteLine("Start Managed Lib");
            Console.WriteLine("Start Reflection Task");
            LoadBaseDLLs();
            Reflector = new RuntimeReflector(ReflectionObject);            
        }


        //================================================================================================================================================
        //================================================================================================================================================
        public static int FireCallback(int messageId, string message, NativeDataType dataType)
        {
            IntPtr temppointer = Marshal.StringToHGlobalUni(message);

            NativeMessageArgs Data = new NativeMessageArgs(temppointer, messageId, dataType);

            int ReturnValue = ReportProgressFunc[0](Data);
            Console.WriteLine("Native Host Returned: {0} For NativeMessageCallback()", ReturnValue);

            Marshal.FreeHGlobal(temppointer);
            return ReturnValue;
        }

        //================================================================================================================================================
        //================================================================================================================================================
        public static int FireCommandlistCallback(IntPtr commandlist)
        {
            int ReturnValue = ReportCommandlistFunc[0](commandlist);
            Console.WriteLine("Native Host Returned: {0} For CommandlistCallback()", ReturnValue);

            return ReturnValue;
        }

        //================================================================================================================================================
        //================================================================================================================================================
        public delegate void CallManagedFunctionDelegate(ManagedFunctionArgs libArgs);
        public static void CallManagedFunction(ManagedFunctionArgs libArgs)
        {
            string message = Marshal.PtrToStringAnsi(libArgs.Message);
            string functionName = Marshal.PtrToStringAnsi(libArgs.FunctionName);


            Console.WriteLine("Call Function: {0}", functionName);
        }


        //================================================================================================================================================
        //================================================================================================================================================
        public delegate void CustomEntryPointDelegate(LibArgs libArgs);
        public static void CustomEntryPoint(LibArgs libArgs)
        {
            string message = Marshal.PtrToStringUni(libArgs.Message);
            switch (libArgs.Number)
            {
                case 0:
                    Startup();
                    break;
                case 1:


                    break;
                case 2:


                    break;
                case 3:


                    break;
                case 4:


                    break;
                case 5:


                    break;
                case -1:
                    Console.WriteLine("Unknown operation");
                    break;
            }
        }

        //================================================================================================================================================
        //================================================================================================================================================
        public delegate void SetupCallBackDelegate(ReportProgressFunction reportProgressFunction);
        public static void SetupCallBack(ReportProgressFunction reportProgressFunction)
        {
            Console.WriteLine("Setup native Callback");

            try
            {
                //Add our native call back to a list so we can use it later, because this is really a native pointer we cant just set our managed member to this value, we have to add it to a list and then call it from there
                ReportProgressFunc.Add(reportProgressFunction);

                //Fire a test back to native code                
                Console.WriteLine("ReportProgressFunc Return: {0} For Test Fire", FireCallback(4, "test", NativeDataType.STRING));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

        }

        //================================================================================================================================================
        //================================================================================================================================================
        public delegate void SetupCommandlistCallBackDelegate(ReportCommandlistFunction reportCommandlistFunction);
        public static void SetupCommandlistCallBack(ReportCommandlistFunction reportCommandlistFunction)
        {
            Console.WriteLine("Setup native commandlist Callback");

            try
            {
                //Add our native call back to a list so we can use it later, because this is really a native pointer we cant just set our managed member to this value, we have to add it to a list and then call it from there
                ReportCommandlistFunc.Add(reportCommandlistFunction);

                //Fire a test back to native code                
                Console.WriteLine("ReportCommandlistFunc Return: {0} For Test Fire", ReportCommandlistFunc[0](NullPointer));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

        }


        //================================================================================================================================================
        //================================================================================================================================================
        private static void LoadBaseDLLs()
        {
            Console.WriteLine("Global Assembly Path: {0}", "/home/pi/projects/netcore2cpp/PiTop");
            string message = "/home/pi/projects/netcore2cpp/PiTop";

#if RUNTIMEMODE

            //Load base assembles, this must happen before we do to much crazy stuff with types as we may be missing some needed libs
            string Path = message + @"/AsyncIO.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/PiTop.Abstractions.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly2 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/PiTop.Algorithms.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly3 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/PiTop.Camera.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly4 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/PiTop.Display.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly5 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/PiTop.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly6 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/Serilog.Sinks.RollingFileAlternate.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly7 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/SixLabors.Fonts.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly8 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/SixLabors.ImageSharp.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly9 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/SixLabors.ImageSharp.Drawing.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly10 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/System.Device.Gpio.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly11 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);


            Path = message + @"/System.Reactive.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly12 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);


            Path = message + @"/System.ServiceModel.Primitives.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly13 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/NaCl.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly14 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);


            Path = message + @"/NetMQ.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly15 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);


            Path = message + @"/Serilog.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly16 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);

            Path = message + @"/SharpDX.Mathematics.dll";
            Console.WriteLine("Load File: {0}", Path);
            var myAssembly17 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path);
           
#endif

            //We should be safe to start doing some more complex stuff now
            Console.WriteLine("Setup Assembly Map");
         
        }

    }

}
