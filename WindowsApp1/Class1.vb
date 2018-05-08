Imports MySql.Data.MySqlClient

Public Class MySqlHelper

    '数据库连接字符串
    Dim connStr As String = "Password=123456;" & "Persist Security Info=True;" & "User=jdexe;" & "Initial Catalog=jdexe;" & "Data Source=39.108.193.197"
    '"server=127.0.0.1;" & "database=todo;" & "user=root;" & "password=123456;" & "Connection Timeout=1"
    '连接字符串

    Dim db_host As String = "rm-wz9i5dzzu3sz83ko90o.mysql.rds.aliyuncs.com"  '服务器IP地址
    Dim db_user As String = "xiatian"                '登陆服务器时的“用户名”
    Dim db_pass As String = "*lrx5X&1&aV33Vr6"        '登陆服务器时的“密  码”
    Dim db_data As String = "jdexe"            '数据库的库名称sqlname（不是表名称）  
    Dim strCn As String = "SERVER=" & db_host & ";" & "DATABASE=" & db_data & ";" & "UID=" & db_user & ";PWD=" & db_pass & ";"

    '创建 SqlConnection 连接
    Dim conn As New MySqlConnection(strCn)

    ' 执行查询的操作,    
    ' <param name="cmdText">需要执行语句,一般是Sql语句,也有存储过程</param>    
    ' <param name="cmdType">判断Sql语句的类型,一般都不是存储过程</param>    
    ' <returns>dataTable,查询到的表格</returns>
    Public Function ExecSelectNo(ByVal cmdText As String) As DataTable

        Dim sqlAdapter As New MySqlDataAdapter(cmdText, conn)
        Dim ds As New DataSet
        Try
            sqlAdapter.Fill(ds)           '用adapter将dataSet填充     
            Return ds.Tables(0)          'datatable为dataSet的第一个表   
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return Nothing
        Finally
            conn.Close()
        End Try
    End Function


    ' 执行增删改三个操作,
    ' <param name="cmdText">需要执行语句,一般是Sql语句,也有存储过程</param>    
    ' <param name="cmdType">判断Sql语句的类型,一般都不是存储过程</param>    
    Public Function ExecAddDelUpdate(ByVal cmdText As String) As Integer

        Dim myCommand As MySqlCommand = New MySqlCommand(cmdText, conn)
        Try
            conn.Open()                      '打开连接    
            Return myCommand.ExecuteNonQuery()
        Catch ex As Exception
            Return 0                         '如果出错,返回0    
        Finally
            conn.Close()
        End Try
    End Function


End Class