Imports System.Reflection
Imports System.Text

Public Class FClassBuilder
    Private m_Reflector As FReflector
    Private m_StringOutput As New StringBuilder
    Private m_OutputBox As TextBox
    Private m_CommandLineArgs As String()

    Private m_TempHeaderBody As String = ""
    Private m_TempCodeFileBody As String = ""

    Private m_ProjectPath As String = ""
    Private m_IntermediateFolder As String = ""
    Private m_OutputHeaderNames As String = "Generated.Managed.h"
    Private m_OutputCodeFileNames As String = "Generated.Managed.cpp"

    ' Private m_ClassName As String = "FExposedFunctions"

    Private m_BaseHeaderBody As String =
"
//This is a Generated file do not edit by hand
#BASEINCLUDE

class FNetCoreLoader;
class #CLASSNAME
{
public:
  #CLASSNAME(FNetCoreLoader* NetCoreLoader);
  ~#CLASSNAME();

  //Start Public Generated Functions  
#FUNCTIONS  
  //End Public Generated Functions  
  static #CLASSNAME* GetSelf()
  {
    return m_Self;
  }
private:
FNetCoreLoader* m_NetCoreLoader = nullptr;
  //Start Private Generated Functions  
#PRIVATEFUNCTIONS  
  //End Private Generated Functions  
  static #CLASSNAME* m_Self;
};

"

    Private m_BaseCodeFileBody As String =
 "
//This is a Generated file do not edit by hand

#BASEINCLUDE
#CLASSNAME* #CLASSNAME::m_Self = nullptr;

//Generated Static Members
#STATICMEMBERS

//Managed function pointer type defs
#TYPEDEFS

//Function to load the assembly and get function a pointer to the managed function
load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer_2 = nullptr;

//==================================================================
//==================================================================
#CLASSNAME::#CLASSNAME(FNetCoreLoader* NetCoreLoader)
{
    //Our runtime loader
    m_NetCoreLoader = NetCoreLoader;

    //Get the function pointer from the loader
    load_assembly_and_get_function_pointer_2 = (load_assembly_and_get_function_pointer_fn)m_NetCoreLoader->GetAssemblyLoaderFunction();

    int rc = 0;
    #INITDEFS
}
//==================================================================
//==================================================================
#CLASSNAME::~#CLASSNAME()
{
}

//Generated Functions
#FUNCTIONS
"




    Private m_VoidFunctionBody As String =
"
//==================================================================
//==================================================================
void #CLASSNAME::#FUNCTIONNAME()
{
   #INIT
   #SETUP
   #ManagedCall
}
"

    Private m_VoidFunctionBodyArgs As String =
"
//==================================================================
//==================================================================
void #CLASSNAME::#FUNCTIONNAME(...)
{
   #INIT
   #SETUP
   #ManagedCall
}
"

    Private m_VoidFunctionHeader As String = "  void #FUNCTIONNAME();"
    Private m_VoidFunctionHeaderArgs As String = "  void #FUNCTIONNAME(...);"
    Private m_StringArgs As String = "const char* #String"
    Private m_IntArgs As String = "int #Int"
    Private m_FloatArgs As String = "float #Float"
    Private m_BoolArgs As String = "bool #Bool"

    Private m_FunctionTypeDef As String = "typedef void (CORECLR_DELEGATE_CALLTYPE* call_#FUNCTIONNAME_fn)(...);"
    Private m_FunctionPointer As String = "call_#FUNCTIONNAME_fn #FUNCTIONNAME_ptr = nullptr;"

    Private m_FunctionDef As String = "
    rc = load_assembly_and_get_function_pointer_2(
		m_NetCoreLoader->GetPath(),
		#DOTNETTYPESTRING,
		STR(#STRFUNCTIONNAME) /*method_name*/,
		#TYPESTRING,
		nullptr,
		(void**)&#FUNCTIONNAME_ptr);"

    Private m_FunctionTypeDefString As String = "DotNetLib.#OBJECTNAME+#FUNCTIONNAMEDelegate, DotNetLib"

    Private m_Dependancys As New List(Of String)

    Public Sub New(OutputBox As TextBox, args As String())
        m_OutputBox = OutputBox
        m_CommandLineArgs = args

        m_ProjectPath = If(args.Count > 1, m_CommandLineArgs(1), "C:\Users\JustinB\source\repos\ConsoleApplication1\")
    End Sub
    Public Sub ParseReflectionData(Reflector As FReflector)
        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Parse Reflection Data")

        m_Reflector = Reflector

        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Done Parse Reflection Data")
    End Sub


    Private Sub GenerateHeader()

        m_TempHeaderBody = m_BaseHeaderBody.Replace("#CLASSNAME", m_Reflector.ObjectName)
        m_TempHeaderBody = m_TempHeaderBody.Replace("#BASEINCLUDE", m_StringOutput.ToString())

        m_StringOutput.Clear()

        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Function count: ")
        m_OutputBox.AppendText(m_Reflector.Methods.Values.Count)

        'Public functions
        For Each item In m_Reflector.Methods.Values
            Dim TempFunction As String = ""
            Dim ParamCount As Int32 = item.GetParameters().Count()
            Dim CurrentCount = 0

            m_OutputBox.AppendText(vbNewLine)
            m_OutputBox.AppendText("Function: ")
            m_OutputBox.AppendText(item.Name)

            If ParamCount > 0 Then
                TempFunction = m_VoidFunctionHeaderArgs.Replace("#FUNCTIONNAME", item.Name)
                Dim Params As String = ""
                For Each param In item.GetParameters()

                    Dim ParamString As String = ""

                    If param.ParameterType = GetType(String) Then
                        ParamString = m_StringArgs.Replace("#String", param.Name)
                    End If

                    If param.ParameterType = GetType(Int32) Then
                        ParamString = m_IntArgs.Replace("#Int", param.Name)
                    End If

                    If param.ParameterType = GetType(Single) Then
                        ParamString = m_FloatArgs.Replace("#Float", param.Name)
                    End If

                    If param.ParameterType = GetType(Boolean) Then
                        ParamString = m_BoolArgs.Replace("#Bool", param.Name)
                    End If

                    CurrentCount += 1

                    If ParamCount > 1 Then

                        If CurrentCount >= ParamCount Then
                            Params += ParamString
                        Else
                            Params += ParamString + ","
                        End If

                    Else
                        Params = ParamString
                    End If
                Next
                TempFunction = TempFunction.Replace("...", Params)

                m_OutputBox.AppendText(" <Params Type/Name: ")
                m_OutputBox.AppendText(Params)
                m_OutputBox.AppendText(">")
            Else
                TempFunction = m_VoidFunctionHeader.Replace("#FUNCTIONNAME", item.Name)
            End If

            m_StringOutput.AppendLine(TempFunction)
        Next

        m_TempHeaderBody = m_TempHeaderBody.Replace("#FUNCTIONS", m_StringOutput.ToString())

        'Private functions
        m_StringOutput.Clear()
        m_TempHeaderBody = m_TempHeaderBody.Replace("#PRIVATEFUNCTIONS", "")
    End Sub

    Private Sub GenerateCodeFile()
        'Code File Includes
        m_TempCodeFileBody = m_BaseCodeFileBody.Replace("#CLASSNAME", m_Reflector.ObjectName)
        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Setup Includes")
        m_StringOutput.AppendLine("#include " & ControlChars.Quote & "Generated.Managed.h" & ControlChars.Quote)
        m_StringOutput.AppendLine("#include " & ControlChars.Quote & "NetCoreLoader.h" & ControlChars.Quote)
        ' m_StringOutput.AppendLine("#include " & ControlChars.Quote & "common_type.h" & ControlChars.Quote)
        m_StringOutput.AppendLine("#include " & ControlChars.Quote & "coreclr_delegates.h" & ControlChars.Quote)

        m_TempCodeFileBody = m_TempCodeFileBody.Replace("#BASEINCLUDE", m_StringOutput.ToString())

        'Functions
        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Generate .cpp")
        m_StringOutput.Clear()

        Dim SB_TypeDefs As New StringBuilder
        Dim SB_FuncPointer As New StringBuilder
        Dim SB_FuncDefs As New StringBuilder

        For Each item In m_Reflector.Methods.Values

            Dim TempFunction As String = ""
            Dim TempFunctionTypeDef As String = m_FunctionTypeDef
            Dim TempFunctionPointer As String = m_FunctionPointer
            Dim TempFunctionDef As String = m_FunctionDef
            Dim TempFunctionTypeDefString As String = m_FunctionTypeDefString

            Dim ParamCount As Int32 = item.GetParameters().Count()
            Dim CurrentCount = 0
            Dim SB_InstanceMethod As New StringBuilder

            If Not item.IsStatic Then
                SB_InstanceMethod.AppendLine("ManagedArgs.functionName = " + "STR(" + ControlChars.Quote & item.Name & "\n" & ControlChars.Quote + ")" + ";")
            Else
                m_OutputBox.AppendText(vbNewLine)
                m_OutputBox.AppendText("Generate Function Pointer Body")
                TempFunctionTypeDef = TempFunctionTypeDef.Replace("#FUNCTIONNAME", item.Name)
                TempFunctionPointer = TempFunctionPointer.Replace("#FUNCTIONNAME", item.Name)
                TempFunctionDef = TempFunctionDef.Replace("#STRFUNCTIONNAME", ControlChars.Quote & item.Name & ControlChars.Quote)
                TempFunctionDef = TempFunctionDef.Replace("#FUNCTIONNAME", item.Name)
                TempFunctionDef = TempFunctionDef.Replace("#DOTNETTYPESTRING", "STR(" + ControlChars.Quote & "DotNetLib." & m_Reflector.ObjectName & ", DotNetLib" & ControlChars.Quote + ") /*type_name*/")

                TempFunctionTypeDefString = TempFunctionTypeDefString.Replace("#FUNCTIONNAME", item.Name)
                TempFunctionTypeDefString = TempFunctionTypeDefString.Replace("#OBJECTNAME", m_Reflector.ObjectName)
                TempFunctionDef = TempFunctionDef.Replace("#TYPESTRING", "STR(" + ControlChars.Quote & TempFunctionTypeDefString & ControlChars.Quote + ") /*delegate_type_name*/")
                m_OutputBox.AppendText(vbNewLine)
                m_OutputBox.AppendText("Done Generate Function Pointer Body")
            End If

            Dim SigParams As String = ""

            If ParamCount > 0 Then
                TempFunction = m_VoidFunctionBodyArgs.Replace("#FUNCTIONNAME", item.Name)

                Dim Params As String = ""
                If Not item.IsStatic Then
                    SB_InstanceMethod.AppendLine("   ManagedArgs.paramcount = " & ParamCount & ";")
                End If

                For Each param In item.GetParameters()

                    Dim ParamString As String = ""
                    Dim FuncSigParams As String = ""

                    If param.ParameterType = GetType(String) Then
                        ParamString = m_StringArgs.Replace("#String", param.Name)
                        FuncSigParams = param.Name

                        If Not item.IsStatic Then
                            SB_InstanceMethod.AppendLine("   ManagedArgs.message = " + param.Name + ";")
                        End If

                    End If

                    If param.ParameterType = GetType(Int32) Then
                        ParamString = m_IntArgs.Replace("#Int", param.Name)
                        FuncSigParams = param.Name
                        If Not item.IsStatic Then
                            SB_InstanceMethod.AppendLine("   ManagedArgs.number = " + param.Name + ";")
                        End If

                    End If

                    If param.ParameterType = GetType(Single) Then
                        ParamString = m_FloatArgs.Replace("#Float", param.Name)
                        FuncSigParams = param.Name
                        If Not item.IsStatic Then
                            SB_InstanceMethod.AppendLine("   ManagedArgs.number = " + param.Name + ";")
                        End If

                    End If

                    If param.ParameterType = GetType(Boolean) Then
                        ParamString = m_BoolArgs.Replace("#Bool", param.Name)
                        FuncSigParams = param.Name
                        If Not item.IsStatic Then
                            SB_InstanceMethod.AppendLine("   ManagedArgs.number = " + param.Name + ";")
                        End If

                    End If

                    CurrentCount += 1

                    If ParamCount > 1 Then

                        If CurrentCount >= ParamCount Then
                            Params += ParamString
                            SigParams += FuncSigParams
                        Else
                            Params += ParamString + ","
                            SigParams += FuncSigParams + ","
                        End If

                    Else
                        Params = ParamString
                        SigParams = FuncSigParams
                    End If
                Next

                TempFunction = TempFunction.Replace("...", Params)
                If item.IsStatic Then
                    TempFunctionTypeDef = TempFunctionTypeDef.Replace("...", Params)
                End If

            Else
                TempFunction = m_VoidFunctionBody.Replace("#FUNCTIONNAME", item.Name)
                If item.IsStatic Then
                    TempFunctionTypeDef = TempFunctionTypeDef.Replace("...", "")
                End If

            End If

            If item.IsStatic Then
                SB_TypeDefs.AppendLine(TempFunctionTypeDef)
                SB_TypeDefs.AppendLine(TempFunctionPointer)
                SB_TypeDefs.AppendLine("")

                SB_FuncDefs.AppendLine(TempFunctionDef)
                SB_FuncDefs.AppendLine("")
            End If

            If item.IsStatic Then

                TempFunction = TempFunction.Replace("#ManagedCall", "")
                TempFunction = TempFunction.Replace("#SETUP", "")

                If ParamCount = 0 Then
                    TempFunction = TempFunction.Replace("#INIT",
    "//Make sure we have a valid function pointer
    if(" & item.Name & "_ptr)" & vbNewLine &
    "    {" & vbNewLine &
    "       " & item.Name & "_ptr();" & vbNewLine &
    "    }")
                Else
                    TempFunction = TempFunction.Replace("#INIT",
    "//Make sure we have a valid function pointer
    if(" & item.Name & "_ptr)" & vbNewLine &
    "    {" & vbNewLine &
    "       " & item.Name & "_ptr(" & SigParams & ");" & vbNewLine &
    "    }")
                End If

            Else
                TempFunction = TempFunction.Replace("#INIT", "managed_function_args ManagedArgs;")
                TempFunction = TempFunction.Replace("#SETUP", SB_InstanceMethod.ToString())
                TempFunction = TempFunction.Replace("#ManagedCall", "m_NetCoreLoader->CallManagedFunction(ManagedArgs);")

                SB_InstanceMethod.Clear()
            End If


            TempFunction = TempFunction.Replace("#CLASSNAME", m_Reflector.ObjectName)
            m_StringOutput.AppendLine(TempFunction)


        Next

        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Apply INITDEFS")
        m_TempCodeFileBody = m_TempCodeFileBody.Replace("#INITDEFS", SB_FuncDefs.ToString())

        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Apply TYPEDEFS")
        m_TempCodeFileBody = m_TempCodeFileBody.Replace("#TYPEDEFS", SB_TypeDefs.ToString())

        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Apply FUNCTIONS")
        m_TempCodeFileBody = m_TempCodeFileBody.Replace("#FUNCTIONS", m_StringOutput.ToString())

        m_StringOutput.Clear()

        'Static members
        m_TempCodeFileBody = m_TempCodeFileBody.Replace("#STATICMEMBERS", "")
    End Sub

    Public Sub GenerateCode()
        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Generate Code For ")
        m_OutputBox.AppendText(m_Reflector.ObjectName)


        ' m_Dependancys

        GenerateHeader()

        GenerateCodeFile()

        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Done Generate Code")
    End Sub

    Public Sub ExportCode()
        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Export Code")

        IO.File.WriteAllText(m_ProjectPath & m_OutputHeaderNames, m_TempHeaderBody)
        IO.File.WriteAllText(m_ProjectPath & m_OutputCodeFileNames, m_TempCodeFileBody)

        m_OutputBox.AppendText(vbNewLine)
        m_OutputBox.AppendText("Done Export Code")
    End Sub

End Class
