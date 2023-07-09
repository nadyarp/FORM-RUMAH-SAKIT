Imports System.IO
Imports ClosedXML.Excel
Imports MySqlConnector
Public Class Form2
    Private con As MySqlConnection

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim builder As New MySqlConnectionStringBuilder()
        builder.Server = "localhost"
        builder.UserID = "root"
        builder.Password = ""
        builder.Database = "hospital"

        con = New MySqlConnection(builder.ConnectionString)

        If con IsNot Nothing Then
            Try
                con.Open()

                Dim query As String = "SELECT * FROM patients;"
                Dim command As New MySqlCommand(query, con)
                Dim reader As MySqlDataReader = command.ExecuteReader()

            

                con.Close()

                Loadpatients()
            Catch ex As Exception
                MessageBox.Show("Error while loading patients types: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Failed to initialize database connection.")
        End If
    End Sub



    Private Sub Loadpatients()
        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As New MySqlCommand("SELECT * FROM patients", con)
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()
                Dim dataTable As New DataTable()
                dataTable.Load(dataReader)
                DataGridView1.DataSource = dataTable
            Catch ex As Exception
                MessageBox.Show("Error while loading patients: " & ex.Message)
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

                Dim dataCommand As New MySqlCommand("SELECT * FROM patients WHERE `fullname` LIKE @keyword", con)
                dataCommand.Parameters.AddWithValue("@keyword", "%" & keyword & "%")
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()
                Dim dataTable As New DataTable()
                dataTable.Load(dataReader)
                DataGridView1.DataSource = dataTable
            Catch ex As Exception
                MessageBox.Show("Error while searching patients: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Database connection is not initialized.")
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim nama_lengkap As String = TextBox1.Text
        Dim tgl_lahir As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")
        Dim tmp_lahir As String = TextBox2.Text
        Dim occupation As String = TextBox3.Text
        Dim gender As String = If(RadioButton1.Checked, "M", "F")


        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As MySqlCommand

                If TextBox6.Text = "" Then
                    dataCommand = New MySqlCommand("INSERT INTO patients (fullname, birth_date, birth_place, sex, occupation) VALUES (@fullname, @birth_date, @birth_place, @sex, @occupation)", Me.con)
                Else
                    dataCommand = New MySqlCommand("UPDATE patients SET fullname = @fullname, birth_date = @birth_date, birth_place = @birth_place, occupation = @occupation, sex = @sex WHERE id = @patient_id", Me.con)
                    dataCommand.Parameters.AddWithValue("@patient_id", TextBox6.Text)
                End If

                dataCommand.Parameters.AddWithValue("@fullname", nama_lengkap)
                dataCommand.Parameters.AddWithValue("@birth_date", tgl_lahir)
                dataCommand.Parameters.AddWithValue("@birth_place", tmp_lahir)
                dataCommand.Parameters.AddWithValue("@sex", gender)
                dataCommand.Parameters.AddWithValue("@occupation", occupation)

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
        Loadpatients()
    End Sub

    Private Sub LoadSinglePatients(patient_id As Integer)
        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As New MySqlCommand("SELECT * FROM patients WHERE id = @patient_id", con)
                dataCommand.Parameters.AddWithValue("@patient_id", patient_id)
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()

                If dataReader.HasRows Then
                    dataReader.Read()
                    TextBox1.Text = dataReader.GetString(dataReader.GetOrdinal("fullname"))
                    Dim fulldate As String = dataReader.GetDateTime(dataReader.GetOrdinal("birth_date")).ToString("yyyy-MM-dd")
                    DateTimePicker1.Value = DateTime.Parse(fulldate)
                    TextBox2.Text = dataReader.GetString(dataReader.GetOrdinal("birth_place"))
                    TextBox3.Text = dataReader.GetString(dataReader.GetOrdinal("occupation"))

                    If dataReader.GetString(dataReader.GetOrdinal("sex")) = "M" Then
                        RadioButton1.Checked = True
                        RadioButton2.Checked = False
                    Else
                        RadioButton1.Checked = False
                        RadioButton2.Checked = True
                    End If
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
        TextBox6.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        DateTimePicker1.ResetText()
        RadioButton1.Checked = True
        RadioButton2.Checked = False

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim rowIndex As Integer = DataGridView1.CurrentCell.RowIndex
        Dim patient_id As String = DataGridView1.Rows(rowIndex).Cells(0).Value.ToString()
        TextBox6.Text = patient_id
        LoadSinglePatients(Convert.ToInt16(patient_id))
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        TextBox6.Text = row.Cells("id").Value.ToString()

        LoadSinglePatients(Integer.Parse(row.Cells("id").Value.ToString()))
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox6.Text <> "" Then
            If con IsNot Nothing Then
                Try
                    con.Open()

                    Dim dataCommand As New MySqlCommand("DELETE FROM patients WHERE id = @patient_id", con)
                    dataCommand.Parameters.AddWithValue("@patient_id", TextBox6.Text)
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
            Loadpatients()
        Else
            MessageBox.Show("Pilih pasien yang akan dihapus", "Perhatian")
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
                saveFileDialog.FileName = "Rekap Data Pasien.xlsx"

                ' melakukan cek apakah user menekan tombol simpan
                If saveFileDialog.ShowDialog() = DialogResult.OK Then
                    ' menyimpan file dengan nama dan lokasi yang dipilih oleh user
                    File.WriteAllBytes(saveFileDialog.FileName, stream.ToArray())
                    MessageBox.Show("File saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        End Using
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged

    End Sub
End Class
