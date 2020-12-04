
Imports DotNetLib


Public Class UI

    Private Reflector As FReflector
    Private ReflectionObject As New DotNetLib.FPiTop
    Private ClassBuilder As FClassBuilder

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim args As String() = Environment.GetCommandLineArgs()
        TextBox1.AppendText("<Command Line Args>")
        TextBox1.AppendText(vbNewLine)
        TextBox1.AppendText(String.Join(vbNewLine, args))
        TextBox1.AppendText(vbNewLine)
        TextBox1.AppendText("<Command Line Args>")
        TextBox1.AppendText(vbNewLine)
        TextBox1.AppendText(vbNewLine)
        TextBox1.AppendText(vbNewLine)


        TextBox1.AppendText("Start Object Reflection")
        ClassBuilder = New FClassBuilder(TextBox1, args)

        Reflector = New FReflector(ReflectionObject, TextBox1)
        ProgressBar1.Value += 33

        ClassBuilder.ParseReflectionData(Reflector)
        ProgressBar1.Value += 33

        ClassBuilder.GenerateCode()
        ProgressBar1.Value += 20

        ClassBuilder.ExportCode()
        ProgressBar1.Value += 100 - ProgressBar1.Value

        Close()
    End Sub
End Class
