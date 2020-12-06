# netcore2cpp
netcore3.1 hosting and native code generator 


Tool to build c++ classes from netcore libs using reflection, tested on linux and the raspberry pi4.

DotNetLib.Framework = Dummy lib so we can use normal .net reflection in the pre build step of the c++ app. Anythign not support by the normal framework is wrpped in RUNTIMEMODE.

DotNetLib = The realy netcore3.1 lib. Includes a pi-top[4] wrapper, shows how to use the mini lcd and button events.

ReflectionDataBuilder = Pre build step, uses reflection to build c++ header/code files, really just a wrapper around function points to managed functions.

netcore2cpp = Sample app showing how to setup netcore hosting.

FClassBuilder.vb = native code generator.

The current pre build step just passes in the current project folder and calls the generator. This is the output folder where the generated files will end up.
$(ProjectDir)ReflectionDataBuilder\bin\Debug\ReflectionDataBuilder.exe $(ProjectDir) 

# Some hard coded pathers are around the place:


public static string Path = "/home/pi/pi-top-4-.NET-SDK/";


Console.WriteLine("Global Assembly Path: {0}", "/home/pi/projects/netcore2cpp/PiTop");


string message = "/home/pi/projects/netcore2cpp/PiTop";


string_t root_path = "/home/pi/projects/netcore2cpp/PiTop/netcoreapp3.1/";


Project file is a bit of a pain so things may get a little messed up, thats why i have included all the .dlls and build artifacts for now.

# Mangaged function setup
If you want the code gen to export a managed method it needs to be static and provide a delegate with the samename#delegate and params, the method must also be decorated with [NativeExpose] so that we dont end up with un-wanted methods getting exported, return types are a bit of a pain but can be done and most primitvie types are supported.


 public delegate void TickDelegate(float delta);
 
 [NativeExpose]
 
 public static void Tick(float delta);
        
