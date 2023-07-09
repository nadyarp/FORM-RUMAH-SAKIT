Imports System.Data
Imports System.IO
Imports System.Windows.Forms
Imports ClosedXML.Excel
Imports MySqlConnector

Public Class Form3
    Private con As MySqlConnection

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim builder As New MySqlConnectionStringBuilder()
        builder.Server = "localhost"
        builder.UserID = "root"
        builder.Password = ""
        builder.Database = "hospital"

        con = New MySqlConnection(builder.ConnectionString)

        If con IsNot Nothing Then
            Try
                con.Open()

                Dim query As String = "SELECT * FROM doctor_type;"
                Dim command As New MySqlCommand(query, con)
                Dim reader As MySqlDataReader = command.ExecuteReader()

                While reader.Read()
                    ComboBox1.Items.Add($"{reader("id")}. {reader("type")}")
                End While

                con.Close()

                LoadDoctors()
            Catch ex As Exception
                MessageBox.Show("Error while loading doctor types: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Failed to initialize database connection.")
        End If
    End Sub

    Private Sub LoadDoctorTypes()
        ComboBox1.Items.Clear()

        If con IsNot Nothing Then
            Try
                con.Open()

                Using command As New MySqlCommand("SELECT * FROM doctor_type;", con)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            ComboBox1.Items.Add(reader.GetString(1))
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error while loading doctor types: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Database connection is not initialized.")
        End If
    End Sub

    Private Sub LoadDoctors()
        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As New MySqlCommand("SELECT * FROM doctors", con)
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()
                Dim dataTable As New DataTable()
                dataTable.Load(dataReader)
                DataGridView1.DataSource = dataTable
            Catch ex As Exception
                MessageBox.Show("Error while loading doctors: " & ex.Message)
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

                Dim dataCommand As New MySqlCommand("SELECT * FROM doctors WHERE `fullname` LIKE @keyword", con)
                dataCommand.Parameters.AddWithValue("@keyword", "%" & keyword & "%")
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()
                Dim dataTable As New DataTable()
                dataTable.Load(dataReader)
                DataGridView1.DataSource = dataTable
            Catch ex As Exception
                MessageBox.Show("Error while searching doctors: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Database connection is not initialized.")
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim nama_lengkap As String = TextBox1.Text
        Dim nik As String = TextBox2.Text
        Dim tgl_lahir As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")
        Dim tmp_lahir As String = TextBox4.Text
        Dim gender As String = If(RadioButton1.Checked, "M", "F")
        Dim spesialisasi As String = ComboBox1.Text
        Dim spesialisasi_components As String() = spesialisasi.Split(" ")

        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As MySqlCommand

                If TextBox3.Text = "" Then
                    dataCommand = New MySqlCommand("INSERT INTO doctors (fullname, nik, birth_date, birth_place, sex, type_id) VALUES (@fullname, @nik, @birth_date, @birth_place, @sex, @type_id)", Me.con)
                Else
                    dataCommand = New MySqlCommand("UPDATE doctors SET fullname = @fullname, nik = @nik, birth_date = @birth_date, birth_place = @birth_place, sex = @sex, type_id = @type_id WHERE id = @doctor_id", Me.con)
                    dataCommand.Parameters.AddWithValue("@doctor_id", TextBox3.Text)
                End If

                dataCommand.Parameters.AddWithValue("@fullname", nama_lengkap)
                dataCommand.Parameters.AddWithValue("@nik", nik)
                dataCommand.Parameters.AddWithValue("@birth_date", tgl_lahir)
                dataCommand.Parameters.AddWithValue("@birth_place", tmp_lahir)
                dataCommand.Parameters.AddWithValue("@sex", gender)
                dataCommand.Parameters.AddWithValue("@type_id", spesialisasi_components(0))

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
        LoadDoctors()
    End Sub

    Private Sub LoadSingleDoctor(doctor_id As Integer)
        If con IsNot Nothing Then
            Try
                con.Open()

                Dim dataCommand As New MySqlCommand("SELECT * FROM doctors WHERE id = @doctor_id", con)
                dataCommand.Parameters.AddWithValue("@doctor_id", doctor_id)
                Dim dataReader As MySqlDataReader = dataCommand.ExecuteReader()

                If dataReader.HasRows Then
                    dataReader.Read()
                    TextBox1.Text = dataReader.GetString(dataReader.GetOrdinal("fullname"))
                    TextBox2.Text = dataReader.GetString(dataReader.GetOrdinal("nik"))
                    Dim fulldate As String = dataReader.GetDateTime(dataReader.GetOrdinal("birth_date")).ToString("yyyy-MM-dd")
                    DateTimePicker1.Value = DateTime.Parse(fulldate)
                    TextBox4.Text = dataReader.GetString(dataReader.GetOrdinal("birth_place"))

                    If dataReader.GetString(dataReader.GetOrdinal("sex")) = "M" Then
                        RadioButton1.Checked = True
                        RadioButton2.Checked = False
                    Else
                        RadioButton1.Checked = False
                        RadioButton2.Checked = True
                    End If

                    Dim type_id As Integer = dataReader.GetInt16(dataReader.GetOrdinal("type_id"))
                    Dim comboIndex As Integer = ComboBox1.FindString(String.Concat(type_id.ToString(), "."))
                    ComboBox1.SelectedIndex = comboIndex
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
        DateTimePicker1.ResetText()
        RadioButton1.Checked = True
        RadioButton2.Checked = False
        ComboBox1.SelectedItem = Nothing
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim rowIndex As Integer = DataGridView1.CurrentCell.RowIndex
        Dim doctor_id As String = DataGridView1.Rows(rowIndex).Cells(0).Value.ToString()
        TextBox3.Text = doctor_id
        LoadSingleDoctor(Convert.ToInt16(doctor_id))
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        TextBox3.Text = row.Cells("id").Value.ToString()

        LoadSingleDoctor(Integer.Parse(row.Cells("id").Value.ToString()))
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox3.Text <> "" Then
            If con IsNot Nothing Then
                Try
                    con.Open()

                    Dim dataCommand As New MySqlCommand("DELETE FROM doctors WHERE id = @doctor_id", con)
                    dataCommand.Parameters.AddWithValue("@doctor_id", TextBox3.Text)
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
            LoadDoctors()
        Else
            MessageBox.Show("Pilih dokter yang akan dihapus", "Perhatian")
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
                saveFileDialog.FileName = "Rekap Data Dokter.xlsx"

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
