Imports System.ComponentModel
Imports System.Configuration.Install

Public Class ProjectInstaller

    Public Sub New()
        MyBase.New()

        '组件设计器需要此调用。
        InitializeComponent()

        '调用 InitializeComponent 后添加初始化代码

    End Sub

    Private Sub ServiceProcessInstaller1_AfterInstall(sender As Object, e As InstallEventArgs) Handles ServiceProcessInstaller1.AfterInstall

    End Sub
End Class
