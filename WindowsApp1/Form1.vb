Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports System.Net
Imports System.Text
Public Class Form1
    Private Mysqlclient = New MySqlHelper
    '配置json串
    Dim empData As String
    '配置array
    Dim ar As Array
    Private page = 0 '页码
    Private time = Format(Now, "yyyyMMdd")
     Private ifpei = 0
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        '读取配置文件,gittest110
        FileOpen(3, "./conf/url.conf", OpenMode.Input)
        Me.empData = LineInput(3)
        FileClose(3)
        'json实例字符串
        'Dim empData As String = "{'employees': [" & " { 'firstName':'Bill' , 'lastName':'Gates' }," & " { 'firstName':'George' , 'lastName':'Bush' }," & " { 'firstName':'Thomas' , 'lastName':'Carter' }" & "]}"


        '解析json
        Dim p As JObject = CType(JsonConvert.DeserializeObject(Me.empData), JObject)
        '检测exe版本
        Dim exeVesionStr As String = p("exe_vesion")

        '本地acc版本
        Dim vesionStr As String = p("acc_vesion")

        '线上版本
        Dim vesionTable As DataTable
        vesionTable = Me.Mysqlclient.ExecSelectNo("SELECT * FROM `jd_vesion` ")
        Try
            '判断exe版本
            Dim exeVesionTable As DataTable
            exeVesionTable = Me.Mysqlclient.ExecSelectNo("SELECT * FROM `jd_vesion` ")
            If exeVesionStr <> exeVesionTable.Rows(0)("exe_vesion").ToString Then
                InputBox("该程序的最新版本已经发布,手动复制并打开浏览器进行下载?", "下载最新版", exeVesionTable.Rows(0)("exe_url").ToString)
                Me.Close()
            End If
            '判断acc版本
            If vesionStr = vesionTable.Rows(0)("acc_vesion").ToString Then
                '版本相同
                'MessageBox.Show("版本相同")
                '循环时间
                Dim time As String = p("time").ToString()
                Me.TextBox2.Text = time
                Me.TextBox1.ReadOnly = True
                Me.TextBox3.Hide()
                Me.TextBox2.Focus()
                Me.Text = "京东导单程序_" & exeVesionStr
                '转换数组
                Me.ar = p("acc").ToArray()
                '遍历数组
                Dim i As Integer
                For i = LBound(ar) To UBound(ar)
                    '账号名称,下拉选项
                    Me.ComboBox1.Items.Insert(i, ar(i)("name").ToString())
                Next

                '下拉菜单设置
                Me.ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
                Me.ComboBox1.Text = Me.ar(0)("name").ToString()
            Else
                '"版本不同"

                Dim accTable As DataTable
                '尝试连接数据库(实例代码已经调通)
                accTable = Me.Mysqlclient.ExecSelectNo("SELECT * FROM  `jd_acc` WHERE 1")

                Dim j As Integer
                '写进配置文件

                Dim pp As JObject = CType(JsonConvert.DeserializeObject(Me.empData), JObject)
                '赋值


                Do While j < accTable.Rows.Count
                    pp("acc")(j)("name") = accTable.Rows(j)("name").ToString
                    pp("acc")(j)("url") = accTable.Rows(j)("url").ToString
                    j = j + 1
                Loop
                pp("vesion") = vesionTable.Rows(0)("vesion").ToString
                '转换string后去掉换行
                Dim jsonStr As String = Replace(Replace(pp.ToString(), vbCr, ""), vbLf, "")

                '保存配置
                FileOpen(2, "./conf/url.conf", OpenMode.Output)
                PrintLine(2, jsonStr)
                FileClose(2)

                MessageBox.Show("最新账号已经自动获取,请重新打开程序!!")

                Me.Close()


            End If
        Catch ex As Exception
            MessageBox.Show("配置服务器链接失败,请联系技术部处理")
            Me.Close()
        End Try



    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim i As Integer = Me.ComboBox1.SelectedIndex
        Me.TextBox1.ForeColor = Color.Red
        Me.TextBox1.Text = Me.ar(i)("url").ToString
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        time = Format(DateTimePicker1.Value, "yyyyMMdd")
        If Timer1.Enabled = False Then
            If time > Format(DateTimePicker2.Value, "yyyyMMdd") Then
                MsgBox("时间参数有误", vbCritical, "参数有误")
                Return
            End If
            If ifpei = 1 Then
                page = 0
            End If
            '判断是否输入参数
            If TextBox1.Text = "" Or TextBox2.Text = "" Then
                MsgBox("参数为空", vbCritical, "输入错误")
                If TextBox1.Text = "" Then
                    TextBox1.Focus()
                    Return
                End If
                If TextBox2.Text = "" Then
                    TextBox2.Focus()
                    Return
                End If

            Else

                TextBox3.Visible = True
                TextBox3.Text = "开始执行..."
                GroupBox1.Visible = False
                Timer1.Enabled = True
                Timer1.Interval = TextBox2.Text * 1000
                Button2.Text = "停止执行"

                '写入配置
                '取出对象
                Dim p As JObject = CType(JsonConvert.DeserializeObject(Me.empData), JObject)
                '赋值
                p("time") = TextBox2.Text
                '转换string后去掉换行
                Dim jsonStr As String = Replace(Replace(p.ToString(), vbCr, ""), vbLf, "")
                '保存配置
                FileOpen(2, "./conf/url.conf", OpenMode.Output)
                PrintLine(2, jsonStr)
                FileClose(2)
            End If
        Else
            Timer1.Enabled = False
            Button2.Text = "重新开始"
            Button2.Enabled = True
        End If


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        '是否加换行
        Dim str As String = TextBox1.Text & time & "&pageIndex=" & page + 1 'url赋值
        page = page + 1
        Dim responseStr As String = PostRequest("xml内容", str)

        '解析json数据



        Try
            Dim p As Linq.JObject = CType(JsonConvert.DeserializeObject(responseStr), Linq.JObject)    '用Newtonsoft.Json反序列json字符串
            If p("end").ToString() = 1 Then
                '最后一页
                If time < Format(DateTimePicker2.Value, "yyyyMMdd") Then
                    time = time + 1
                    page = 0
                Else
                    Timer1.Enabled = False
                    TextBox3.Text = "执行完成" & vbCrLf & TextBox3.Text
                    Button2.Text = "重新执行"
                    Return
                End If

            End If
            Dim result As String = p("data").ToString()   '提取Json字符串中的“data”值

            TextBox3.Text = result & vbCrLf & TextBox3.Text
            FileOpen(1, "log.txt", OpenMode.Append)
            PrintLine(1, result)
            FileClose(1)
        Catch ex As Exception
            'json解析异常的处理
            '记录错误日志
            Dim data_id, type, content, level, auth_code As String
            data_id = "jdimport_exe" 'QQ号码
            type = "京东导单程序异常" '加密后的QQ密码
            content = responseStr '验证码
            level = "1" '验证码
            auth_code = "xfz178com" '验证码
            MessageBox.Show(responseStr)
            Dim postData As String = "data_id=" & data_id & "&type=" & type & "&content=" & content & "&level=" & level & "&auth_code=" & auth_code
            Dim url As String = "http://120.79.133.35/mgdb/post_receive.php"
            Selfpost(url, postData, Me, Timer1)
        End Try




    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Timer1.Enabled = True Then
            MsgBox("请先停止执行", vbCritical, "操作错误")
            Return
        End If
        TextBox2.ReadOnly = False
        If TextBox3.Visible = False And Timer1.Enabled = False Then
        Else
            TextBox3.Visible = False
            GroupBox1.Visible = True
            ifpei = 1
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Timer1.Enabled = True Then
            MsgBox("请先停止执行", vbCritical, "操作错误")
        Else
            End
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)


        If Timer1.Enabled = True Then
            MsgBox("请先停止执行", vbCritical, "操作错误")
        Else



        End If


    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) 

    End Sub
End Class


Module Module1

    Public Function PostRequest(ByVal xmlRequest As String, ByVal postUrl As String) As String
        Dim xml As String = xmlRequest
        '实例化一个字符转码对象'  
        Dim encoding As System.Text.Encoding = System.Text.Encoding.GetEncoding("utf-8")
        '创建一个web请求对象'  
        Dim request As System.Net.WebRequest = System.Net.WebRequest.Create(postUrl)
        '设置请求方式为post'  
        request.Method = "POST"
        '定义字节数组'  
        Dim postdata() As System.Byte = encoding.GetBytes(xmlRequest)
        '设置request对象的请求字节的长度'  
        request.ContentLength = postdata.Length
        '获取request对象的数据流'  
        Try
            Dim requesstream As System.IO.Stream = request.GetRequestStream()
            '将数据内容填充到流中'  
            requesstream.Write(postdata, 0, postdata.Length)
            '关闭流'  
            requesstream.Close()
            '根据请求的request对象获取响应的response对象'  
            Try
                Dim response As System.Net.WebResponse = request.GetResponse()
                '获取response数据流对象'  
                Dim responsestream As IO.StreamReader = New IO.StreamReader(response.GetResponseStream())
                '将response流中的数据读取'  
                Dim html As String = responsestream.ReadToEnd()

                requesstream.Close()
                response.Close()
                '返回本次请求的响应数据'  
                Return html
            Catch ex As Exception
                Return ex.Message
            End Try
        Catch ex As Exception
            Return ex.Message

        End Try
    End Function

    Public Function Selfpost(url As String, postdata As String, Ob As Object, timer As Object) As Boolean
        timer.Enabled = False
        ' Create a request using a URL that can receive a post. 
        Dim request As WebRequest = WebRequest.Create(url)
        ' Set the Method property of the request to POST.
        request.Method = "POST"
        ' Create POST data and convert it to a byte array.


        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postdata)
        ' Set the ContentType property of the WebRequest.
        request.ContentType = "application/x-www-form-urlencoded"
        ' Set the ContentLength property of the WebRequest.
        request.ContentLength = byteArray.Length
        ' Get the request stream.
        Dim dataStream As Stream = request.GetRequestStream()
        ' Write the data to the request stream.
        dataStream.Write(byteArray, 0, byteArray.Length)
        ' Close the Stream object.
        dataStream.Close()
        ' Get the response.
        Dim response As WebResponse = request.GetResponse()
        ' Display the status.
        Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
        ' Get the stream containing content returned by the server.
        dataStream = response.GetResponseStream()
        ' Open the stream using a StreamReader for easy access.
        Dim reader As New StreamReader(dataStream)
        ' Read the content.
        Dim responseFromServer As String = reader.ReadToEnd()
        ' Display the content.
        Console.WriteLine(responseFromServer)
        Console.Read()
        ' Clean up the streams.

        '确认是否关闭程序

        Dim result As DialogResult = MessageBox.Show("程序运行过程出了一点小问题,已经提交到技术部,是否继续运行？", "异常", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
        If result = DialogResult.OK Then
            timer.Enabled = False
            Return True
        Else
            Ob.Close()
        End If



        reader.Close()
        dataStream.Close()
        response.Close()
    End Function

End Module


