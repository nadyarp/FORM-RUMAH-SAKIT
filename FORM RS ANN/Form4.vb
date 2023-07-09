Imports System.IO
Imports ClosedXML.Excel
Imports MySqlConnector
Public Class Form4
    Private con As MySqlConnection

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim builder As New MySqlConnectionStringBuilder()
        builder.Server = "localhost"
        builder.UserID = "root"
        builder.Password = ""
        builder.Database = "hospital"

        con = New MySqlConnection(builder.ConnectionString)

        If con IsNot Nothing Then
            Try
                con.Open()

                Dim query As String = "SELECT * FROM drugs;"
                Dim command As New MySqlCommand(query, con)
                Dim reader As MySqlDataReader = command.ExecuteReader()



                con.Close()

                Loaddrugs()
            Catch ex As Exception
                MessageBox.Show("Error while loading drugs types: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Failed to initialize database connection.")
        End If
    End Sub



    Private Sub Loaddrugs()
        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As New MySqlCommand("SELECT * FROM drugs", con)
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()
                Dim dataTable As New DataTable()
                dataTable.Load(dataReader)
                DataGridView1.DataSource = dataTable
            Catch ex As Exception
                MessageBox.Show("Error while loading drugs: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Database connection is not initialized.")
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Form1.Show()
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        Dim keyword As String = TextBox5.Text

        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As New MySqlCommand("SELECT * FROM drugs WHERE `name` LIKE @keyword", con)
                dataCommand.Parameters.AddWithValue("@keyword", "%" & keyword & "%")
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()
                Dim dataTable As New DataTable()
                dataTable.Load(dataReader)
                DataGridView1.DataSource = dataTable
            Catch ex As Exception
                MessageBox.Show("Error while searching drugs: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Database connection is not initialized.")
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim name As String = TextBox1.Text
        Dim diagnose As String = TextBox2.Text
        Dim quantity As String = TextBox3.Text
        Dim price As String = TextBox3.Text


        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As MySqlCommand

                If TextBox6.Text = "" Then
                    dataCommand = New MySqlCommand("INSERT INTO drugs (name, diagnose, quantity, price) VALUES (@name, @diagnose, @quantity, @price)", Me.con)
                Else
                    dataCommand = New MySqlCommand("UPDATE drugs SET name = @name, diagnose = @diagnose, quantity = @quantity, price = @price WHERE id = @drugs_id", Me.con)
                    dataCommand.Parameters.AddWithValue("@drugs_id", TextBox6.Text)
                End If

                dataCommand.Parameters.AddWithValue("@name", name)
                dataCommand.Parameters.AddWithValue("@diagnose", diagnose)
                dataCommand.Parameters.AddWithValue("@quantity", quantity)
                dataCommand.Parameters.AddWithValue("@price", price)

                Dim affected_rows As Integer = dataCommand.ExecuteNonQuery()

                If affected_rows > 0 Then
                    MessageBox.Show("Berhasil menyimpan data", "Informasi")
                End If
            Catch ex As Exception
                MessageBox.Show("Error while saving data: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Database connection is not initialized.")
        End If

        ClearForm()
        Loaddrugs()
    End Sub

    Private Sub LoadSingledrugs(drugs_id As Integer)
        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As New MySqlCommand("SELECT * FROM drugs WHERE id = @drugs_id", con)
                dataCommand.Parameters.AddWithValue("@drugs_id", drugs_id)
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()

                If dataReader.HasRows Then
                    dataReader.Read()
                    TextBox1.Text = dataReader.GetString(dataReader.GetOrdinal("name"))
                    TextBox2.Text = dataReader.GetString(dataReader.GetOrdinal("diagnose"))
                    TextBox3.Text = dataReader.GetInt32(dataReader.GetOrdinal("quantity")).ToString()
                    TextBox4.Text = dataReader.GetDecimal(dataReader.GetOrdinal("price")).ToString("0.##")
                Else
                    MessageBox.Show("Data Not Found", "Attention")
                End If

                dataReader.Close()
            Catch ex As Exception
                MessageBox.Show("Error while loading data: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Database connection is not initialized.")
        End If
    End Sub

    Private Sub ClearForm()
        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""
        TextBox6.Text = ""


    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim rowIndex As Integer = DataGridView1.CurrentCell.RowIndex
        Dim drugs_id As String = DataGridView1.Rows(rowIndex).Cells(0).Value.ToString()
        TextBox6.Text = drugs_id
        LoadSingledrugs(Convert.ToInt16(drugs_id))
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        TextBox6.Text = row.Cells("id").Value.ToString()

        LoadSingledrugs(Integer.Parse(row.Cells("id").Value.ToString()))
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox6.Text <> "" Then
            If con IsNot Nothing Then
                Try
                    con.Open()

                    Dim dataCommand As New MySqlCommand("DELETE FROM drugs WHERE id = @drugs_id", con)
                    dataCommand.Parameters.AddWithValue("@drugs_id", TextBox6.Text)
                    Dim affected_rows As Integer = dataCommand.ExecuteNonQuery()

                    If affected_rows > 0 Then
                        MessageBox.Show("Berhasil menghapus data", "Informasi")
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error while deleting data: " & ex.Message)
                Finally
                    con.Close()
                End Try
            Else
                MessageBox.Show("Database connection is not initialized.")
            End If

            ClearForm()
            Loaddrugs()
        Else
            MessageBox.Show("Pilih drugs yang akan dihapus", "Perhatian")
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ClearForm()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        ' membuat komponen datatable
        ' dimulai dari baris di bawah, komponen pada dataGridView1 dipindahkan ke komponen baru yaitu DataTable pada variabel dt
        Dim dt As New DataTable()

        ' menambahkan kolom datagridview1 ke datatable
        For Each col As DataGridViewColumn In DataGridView1.Columns
            dt.Columns.Add(col.Name)
        Next

        ' menambahkan baris/record data dari datagridview1 ke datatable
        For Each row As DataGridViewRow In DataGridView1.Rows
            Dim dRow As DataRow = dt.NewRow()

            For Each cell As DataGridViewCell In row.Cells
                dRow(cell.ColumnIndex) = cell.Value
            Next

            dt.Rows.Add(dRow)
        Next

        ' membuat sebuah file excel di memory
        Using wb As New XLWorkbook()
            ' menambahkan worksheet ke file excel
            wb.Worksheets.Add(dt, "Sheet1")

            Using stream As New MemoryStream()
                wb.SaveAs(stream)

                ' membuka save file dialog untuk menentukan tempat penyimpanan file
                Dim saveFileDialog As New SaveFileDialog()
                saveFileDialog.Filter = "Excel Files|*.xlsx"
                saveFileDialog.FileName = "Rekap Data Drugs.xlsx"

                ' melakukan cek apakah user menekan tombol simpan
                If saveFileDialog.ShowDialog() = DialogResult.OK Then
                    ' menyimpan file dengan nama dan lokasi yang dipilih oleh user
                    File.WriteAllBytes(saveFileDialog.FileName, stream.ToArray())
                    MessageBox.Show("File saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        End Using
    End Sub

End Class
