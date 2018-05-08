Imports MySql.Data.MySqlClient
Imports System.Data.SqlClient

Public Class MySqlHelper

    '数据库连接字符串
    Dim connStr As String = "database=currenthedge;server=10.30.100.82;Uid=root;Pwd=root;"

    '创建 SqlConnection 连接
    Dim conn As New MySqlConnection(connStr)

    ' 执行查询的操作,    
    ' <param name="cmdText">需要执行语句,一般是Sql语句,也有存储过程</param>    
    ' <param name="cmdType">判断Sql语句的类型,一般都不是存储过程</param>    
    ' <returns>dataTable,查询到的表格</returns>
    Public Function ExecSelectNo(ByVal cmdText As String, ByVal cmdType As CommandType) As DataTable

        Dim sqlAdapter As New MySqlDataAdapter(cmdText, conn)
        Dim ds As New DataSet

        Try
            sqlAdapter.Fill(ds)           '用adapter将dataSet填充     
            Return ds.Tables(0)             'datatable为dataSet的第一个表   

        Catch ex As Exception
            Return Nothing
        Finally
            conn.Close()
        End Try
    End Function


    ' 执行增删改三个操作,
    ' <param name="cmdText">需要执行语句,一般是Sql语句,也有存储过程</param>    
    ' <param name="cmdType">判断Sql语句的类型,一般都不是存储过程</param>    
    Public Function ExecAddDelUpdate(ByVal cmdText As String, ByVal cmdType As CommandType) As Integer

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