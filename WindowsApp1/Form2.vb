Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class 订单检测

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button3.Visible = False
        Button4.Visible = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button3.Visible = False
        Button4.Visible = False
        If Me.TextBox1.Text = "" Then
            MessageBox.Show("订单号为空")
            Return
        End If

        '清空显示
        bdddzt.Text = ""
        bdflje.Text = ""
        bdpid.Text = ""
        bdxdsj.Text = ""
        bdzhgx.Text = ""

        xsddzt.Text = ""
        xsflje.Text = ""
        xspid.Text = ""
        xsxdsj.Text = ""
        xszhgx.Text = ""


        Dim Postobj As Object = New Postclas
        Dim sign, method, order_sn, times As String

        sign = "jdimport_exe" 'QQ号码
        method = "Pdd.checkTrade" '加密后的QQ密码
        order_sn = TextBox1.Text '验证码

        Dim str As String = "http://trade.xfz178.com/index/Jd/checkTradeOnline?order_code=" & order_sn 'url赋值
        Dim responseStr As String = XMLHttpRequest("POST", str, "")

        Dim p As Linq.JObject = CType(JsonConvert.DeserializeObject(responseStr), Linq.JObject)

        '错误处理
        If p("code").ToString() = "-1" Then
            MessageBox.Show(p("data").ToString())
            Return
        End If
        '显示信息
        bdddzt.Text = p("own_data")("status").ToString()
        bdflje.Text = p("own_data")("commission").ToString() & "元"
        bdpid.Text = p("own_data")("uid").ToString()
        bdxdsj.Text = p("own_data")("order_time").ToString()
        bdzhgx.Text = p("own_data")("cos_price").ToString() & "元"

        '判断是否可以强制返利
        ' If p("if_lost").ToString() = 1 And p("if_fanli").ToString() = 1 Then
        'MessageBox.Show("线上并没有获取到订单信息,可以进行强制返利")
        'Button4.Show()
        'Return
        ' ElseIf p("if_lost").ToString() = 1 And p("if_fanli").ToString() = 0 Then
        'MessageBox.Show("线上并没有获取到订单信息")
        'Return
        'End If

        xsddzt.Text = p("online_data")("status").ToString()
        xsflje.Text = p("online_data")("commission").ToString() & "元"
        xspid.Text = p("online_data")("uid").ToString()
        xsxdsj.Text = p("online_data")("order_time").ToString()
        xszhgx.Text = p("online_data")("cos_price").ToString() & "元"

        '判断数据是否一致
        If p("if_same").ToString() = "0" Then
            MessageBox.Show("系统检测到该订单线上线下状态不一致,可以进行更新操作")
            Button3.Show()
        End If

    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim Postobj As Object = New Postclas
        Dim sign, method, order_sn, times As String

        sign = "jdimport_exe" 'QQ号码
        method = "Pdd.changeTrade" '加密后的QQ密码
        order_sn = TextBox1.Text '验证码

        Dim str As String = "http://trade.xfz178.com/index/Jd/changeTrade?order_code=" & order_sn 'url赋值
        Dim responseStr As String = XMLHttpRequest("POST", str, "")

        Dim p As Linq.JObject = CType(JsonConvert.DeserializeObject(responseStr), Linq.JObject)

        '错误处理
        If p("code").ToString() = "-1" Then
            MessageBox.Show(p("data").ToString())
            Return
        Else
            MessageBox.Show(p("data").ToString())
            Button3.Visible = False
            Return
        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim Postobj As Object = New Postclas
        Dim sign, method, order_sn, times As String

        sign = "jdimport_exe" 'QQ号码
        method = "Pdd.foceUpdateTrade" '加密后的QQ密码
        order_sn = TextBox1.Text '验证码
        Dim postData As String = "sign=" & sign & "&method=" & method & "&order_sn=" & order_sn


        Dim re As String = Postobj.Selfpost("http://trade.xfz178.com", postData)
        '解析结果
        Dim p As JObject = CType(JsonConvert.DeserializeObject(re), JObject)

        '错误处理
        If p("code").ToString() = "-1" Then
            MessageBox.Show(p("data").ToString())
            Return
        Else
            MessageBox.Show(p("data").ToString())
            Button3.Visible = False
            Return
        End If
    End Sub
    '异步请求防止程序卡死的函数
    Function XMLHttpRequest(ByVal XmlHttpMode, ByVal XmlHttpURL, ByVal XmlHttpData)
        Dim MyXmlhttp
        On Error GoTo wrong
        MyXmlhttp = CreateObject("WinHttp.WinHttpRequest.5.1")                  '创建WinHttpRequest对象
        With MyXmlhttp
            .setTimeouts(50000, 50000, 50000, 50000)                               '设置超时时间
            If XmlHttpMode = "GET" Then                                             '异步GET请求
                .Open("GET", XmlHttpURL, True)
            Else
                .Open("POST", XmlHttpURL, True)                                     '异步POST请求
                .setRequestHeader("Content-Type", "application/x-www-form-urlencoded")
            End If
            .setRequestHeader("Accept", "image/gif,image/x-xbitmap,image/jpeg,image/pjpeg,application/x-shockwave-flash,*/*")
            '.setRequestHeader("Referer", "https://passport.baidu.com/?login&tpl=mn")
            .setRequestHeader("Accept-Language", "zh-cn")
            .setRequestHeader("Accept-Encoding", "deflate")
            .setRequestHeader("User-Agent", "Mozilla/4.0(compatible;MSIE6.0;WindowsNT5.1;SV1;.NETCLR2.0.50727)")
            .send(XmlHttpData)
            .waitForResponse                                                        '异步等待

            If MyXmlhttp.Status = 200 Then                                          '成功获取页面
                XMLHttpRequest = .ResponseText
            Else
                MsgBox("Http错误代码:" & .Status, vbInformation, "提示")
            End If
        End With
        MyXmlhttp = Nothing
        Exit Function
wrong:
        MsgBox("错误原因:" & Err.Description & "", vbInformation, "提示")
        MyXmlhttp = Nothing
    End Function
End Class

