Imports System.ComponentModel
Imports System.IO
Imports System.Reflection

Public Class FReflector
    Private optionsObject As Object

    Public requiredOptions As New Queue(Of FieldInfo)()
    Public optionalOptions As New Dictionary(Of String, FieldInfo)()

    Public Methods As New Dictionary(Of String, MethodInfo)()

    Public requiredOptions2 As New Queue(Of PropertyInfo)()
    Public optionalOptions2 As New Dictionary(Of String, PropertyInfo)()

    Public requiredUsageHelp As New List(Of String)()
    Public optionalUsageHelp As New List(Of String)()
    Private m_OutputBox As TextBox
    Public ObjectName As String = ""

    ' Constructor.
    Public Sub New(optionsObject As Object, OutputBox As TextBox)
        Me.optionsObject = optionsObject

        ObjectName = optionsObject.[GetType]().Name
        m_OutputBox = OutputBox
        Dim Assemblys As AssemblyName() = optionsObject.[GetType]().Assembly.GetReferencedAssemblies()

        'Dim Assemblys As Stream = optionsObject.[GetType]().Assembly.
        For Each Assembly As AssemblyName In Assemblys
            m_OutputBox.AppendText(vbNewLine)
            m_OutputBox.AppendText("Assembly: ")
            m_OutputBox.AppendText(Assembly.Name)
        Next

        For Each method As MethodInfo In optionsObject.[GetType]().GetMethods()
            If DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.NativeExpose)(method) IsNot Nothing Then
                Dim methodname As String = GetMethodName(method)
                If Not Methods.Keys.Contains(methodname) Then
                    Methods.Add(methodname.ToLowerInvariant(), method)
                End If
            End If
        Next

        ' Reflect to find what commandline options are available.
        For Each field As FieldInfo In optionsObject.[GetType]().GetFields()
            Dim fieldName As String = GetOptionName(field)
            If DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.NativeExpose)(field) IsNot Nothing Then
                If DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.RequiredAttribute)(field) IsNot Nothing Then
                    ' Record a required option.
                    requiredOptions.Enqueue(field)

                    requiredUsageHelp.Add(String.Format("<{0}>", fieldName))
                Else

                    ' Record an optional option.
                    optionalOptions.Add(fieldName.ToLowerInvariant(), field)

                    If field.FieldType = GetType(Boolean) Then
                        optionalUsageHelp.Add(String.Format("/{0}", fieldName))
                    Else
                        optionalUsageHelp.Add(String.Format("/{0}:value", fieldName))
                    End If
                End If
            End If
        Next

        For Each field As PropertyInfo In optionsObject.[GetType]().GetProperties()
            Dim fieldName As String = GetOptionProperty(field)
            If DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.NativeExpose)(field) IsNot Nothing Then
                If DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.RequiredAttribute)(field) IsNot Nothing Then
                    ' Record a required option.
                    requiredOptions2.Enqueue(field)

                    requiredUsageHelp.Add(String.Format("<{0}>", fieldName))
                Else
                    ' Record an optional option.
                    optionalOptions2.Add(fieldName.ToLowerInvariant(), field)

                    If field.PropertyType = GetType(Boolean) Then
                        optionalUsageHelp.Add(String.Format("/{0}", fieldName))
                    Else
                        optionalUsageHelp.Add(String.Format("/{0}:value", fieldName))
                    End If
                End If
            End If

        Next
    End Sub

    Function ChangeType(value As String, type As Type) As Object
        Dim converter As TypeConverter = TypeDescriptor.GetConverter(type)

        Return converter.ConvertFromInvariantString(value)
    End Function


    Function IsList(field As FieldInfo) As Boolean
        Return GetType(IList).IsAssignableFrom(field.FieldType)
    End Function


    Function GetList(field As FieldInfo) As IList
        Return CType(field.GetValue(optionsObject), IList)
    End Function
    Function IsList(field As PropertyInfo) As Boolean
        Return GetType(IList).IsAssignableFrom(field.PropertyType)
    End Function


    Function GetList(field As PropertyInfo) As IList
        Return CType(field.GetValue(optionsObject), IList)
    End Function

    Function ListElementType(field As FieldInfo) As Type
        Dim interfaces As Object = From i In field.FieldType.GetInterfaces() Where i.IsGenericType AndAlso i.GetGenericTypeDefinition() = GetType(IEnumerable(Of ))

        Return interfaces.First().GetGenericArguments()(0)
    End Function
    Function ListElementType(field As PropertyInfo) As Type
        Dim interfaces As Object = From i In field.PropertyType.GetInterfaces() Where i.IsGenericType AndAlso i.GetGenericTypeDefinition() = GetType(IEnumerable(Of ))

        Return interfaces.First().GetGenericArguments()(0)
    End Function

    Function GetOptionName(field As FieldInfo) As String
        Dim nameAttribute As Object = DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.NameAttribute)(field)

        If nameAttribute IsNot Nothing Then
            Return nameAttribute.Name
        Else
            Return field.Name
        End If
    End Function
    Function GetMethodName(method As MethodInfo) As String
        Dim nameAttribute As Object = DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.NameAttribute)(method)

        If nameAttribute IsNot Nothing Then
            Return nameAttribute.Name
        Else
            Return method.Name
        End If
    End Function
    Function GetOptionName(field As PropertyInfo) As String
        Dim nameAttribute As Object = DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.NameAttribute)(field)

        If nameAttribute IsNot Nothing Then
            Return nameAttribute.Name
        Else
            Return field.Name
        End If
    End Function
    Function GetOptionProperty(field As PropertyInfo) As String
        Dim nameAttribute As Object = DotNetLib.RuntimeReflector.GetAttribute(Of DotNetLib.NameAttribute)(field)

        If nameAttribute IsNot Nothing Then
            Return nameAttribute.Name
        Else
            Return field.Name
        End If
    End Function


    Sub ShowError(message As String, ParamArray args As Object())
        Dim name As String = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName)

        Console.[Error].WriteLine(message, args)
        Console.[Error].WriteLine()
        Console.[Error].WriteLine("Usage: {0} {1}", name, String.Join(" ", requiredUsageHelp))

        If optionalUsageHelp.Count > 0 Then
            Console.[Error].WriteLine()
            Console.[Error].WriteLine("Options:")

            For Each [optional] As String In optionalUsageHelp
                Console.[Error].WriteLine("    {0}", [optional])
            Next
        End If
    End Sub


    ' Function GetAttribute(Of T As Attribute)(provider As ICustomAttributeProvider) As T
    'Return provider.GetCustomAttributes(GetType(T), False).OfType(Of T)().FirstOrDefault()
    '  End Function
End Class