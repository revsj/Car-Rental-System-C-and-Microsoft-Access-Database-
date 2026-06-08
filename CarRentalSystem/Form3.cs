using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CarRentalSystem
{
    public partial class Form3 : Form
    {
        private string currentTable = "Cars";

        public Form3()
        {
            InitializeComponent();
            this.AutoScroll = true;
            targets = new PictureBox[] { pictureBox2, pictureBox4, pictureBox5 };
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // ===== TAB BUTTONS =====
            button8.Click += (s, e) => SwitchGrid("Cars");
            button9.Click += (s, e) => SwitchGrid("Customers");
            button10.Click += (s, e) => SwitchGrid("Rentals");

            // ===== VALIDATION SUBSCRIPTIONS =====
            textBox13.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.')
                    e.Handled = true;
            };
            textBox13.TextChanged += (s, e) =>
            {
                textBox13.BackColor = !string.IsNullOrWhiteSpace(textBox13.Text) &&
                    !double.TryParse(textBox13.Text, out _)
                    ? Color.MistyRose : Color.White;
            };

            textBox6.TextChanged += (s, e) =>
            {
                textBox6.BackColor = textBox6.Text.Any(c => char.IsDigit(c))
                    ? Color.MistyRose : Color.White;
            };

            textBox4.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
            textBox4.TextChanged += (s, e) =>
            {
                textBox4.BackColor = !string.IsNullOrWhiteSpace(textBox4.Text) &&
                    !int.TryParse(textBox4.Text, out _)
                    ? Color.MistyRose : Color.White;
            };

            textBox2.TextChanged += (s, e) =>
            {
                textBox2.BackColor = textBox2.Text.Any(c => char.IsDigit(c))
                    ? Color.MistyRose : Color.White;
            };
        }

        OleDbConnection con = new OleDbConnection(
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\revoj\source\repos\CarRentalSystem\CarRentalSystem\CarRental.accdb");

        private PictureBox[] targets;
        private string?[] files = Array.Empty<string?>();

        // Columns to hide by default in Customers tab
        private readonly string[] _customerHiddenCols =
        {
            "Contact Number", "Email", "Address",
            "DriverLicenseNumber", "AgencyNo", "CustomerID", "Discount"
        };

        // ===== AUTO-UPDATE CAR STATUS BASED ON RETURN DATE =====
        private void AutoUpdateCarStatuses()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(con.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand cmdAvailable = new OleDbCommand(
                        "UPDATE Cars SET [Status] = 'Available' " +
                        "WHERE [CarRentalID] IN ( " +
                        "    SELECT [CarRentalID] FROM Rentals " +
                        "    WHERE [Status] = 'Paid Successfully' " +
                        "    AND [ReturnDate] < ? " +
                        ")", conn))
                    {
                        cmdAvailable.Parameters.AddWithValue("?", DateTime.Today);
                        cmdAvailable.ExecuteNonQuery();
                    }

                    using (OleDbCommand cmdRented = new OleDbCommand(
                        "UPDATE Cars SET [Status] = 'Rented' " +
                        "WHERE [CarRentalID] IN ( " +
                        "    SELECT [CarRentalID] FROM Rentals " +
                        "    WHERE [Status] = 'Paid Successfully' " +
                        "    AND [DateRented] <= ? " +
                        "    AND [ReturnDate] >= ? " +
                        ")", conn))
                    {
                        cmdRented.Parameters.AddWithValue("?", DateTime.Today);
                        cmdRented.Parameters.AddWithValue("?", DateTime.Today);
                        cmdRented.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Auto-update status error: " + ex.Message);
            }
        }

        // ===== SWITCH GRID =====
        private void SwitchGrid(string tableName)
        {
            currentTable = tableName;

            AutoUpdateCarStatuses();

            // Reset tab buttons
            button8.BackColor = Color.RosyBrown;
            button9.BackColor = Color.RosyBrown;
            button10.BackColor = Color.RosyBrown;
            button8.ForeColor = Color.White;
            button9.ForeColor = Color.White;
            button10.ForeColor = Color.White;

            switch (tableName)
            {
                case "Cars":
                    button8.BackColor = Color.Brown;
                    splitContainer1.Panel1Collapsed = false;
                    // Hide search bar on non-customer tabs
                    textBox14.Visible = false;
                    button11.Visible = false;
                    break;
                case "Customers":
                    button9.BackColor = Color.Brown;
                    splitContainer1.Panel1Collapsed = true;
                    // Show search bar only on Customers tab
                    textBox14.Visible = true;
                    button11.Visible = true;
                    textBox14.Text = string.Empty;
                    break;
                case "Rentals":
                    button10.BackColor = Color.Brown;
                    splitContainer1.Panel1Collapsed = true;
                    textBox14.Visible = false;
                    button11.Visible = false;
                    break;
            }

            dataGridView1.Dock = DockStyle.Fill;

            string query = tableName switch
            {
                "Customers" => "SELECT * FROM [Customers] ORDER BY [CustomerID] DESC",
                "Rentals" => "SELECT * FROM [Rentals] ORDER BY [DateRented] DESC",
                _ => "SELECT * FROM [Cars]"
            };

            try
            {
                con.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                StyleGrid(dataGridView1);

                if (tableName == "Cars")
                    HideImageColumns();

                if (tableName == "Customers")
                    HideCustomerColumns(); // hide details by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {tableName}: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
h
        // ===== HIDE IMAGE COLUMNS =====
        private void HideImageColumns()
        {
            try
            {
                string[] cols = { "image1", "image2", "image3" };
                foreach (var name in cols)
                    if (dataGridView1.Columns.Contains(name))
                        dataGridView1.Columns[name].Visible = false;
            }
            catch { }
        }

        // ===== HIDE CUSTOMER DETAIL COLUMNS (show only FullName) =====
        private void HideCustomerColumns()
        {
            try
            {
                foreach (var name in _customerHiddenCols)
                    if (dataGridView1.Columns.Contains(name))
                        dataGridView1.Columns[name].Visible = false;
            }
            catch { }
        }

        // ===== SHOW ALL CUSTOMER COLUMNS =====
        private void ShowCustomerColumns()
        {
            try
            {
                foreach (var name in _customerHiddenCols)
                    if (dataGridView1.Columns.Contains(name))
                        dataGridView1.Columns[name].Visible = true;
            }
            catch { }
        }

        // ===== LOAD DATA =====
        public void LoadData()
        {
            SwitchGrid(currentTable);
        }

        // ===== FORM LOAD =====
        private void Form3_Load(object sender, EventArgs e)
        {
            dataGridView1.Dock = DockStyle.Fill;

            // Hide search bar on startup (only shows on Customers tab)
            textBox14.Visible = false;
            button11.Visible = false;

            AutoUpdateCarStatuses();
            SwitchGrid("Cars");

            this.AutoScroll = true;
            try
            {
                splitContainer1.Panel1.AutoScroll = true;
                splitContainer1.Panel2.AutoScroll = true;
            }
            catch { }

            targets = new PictureBox[] { pictureBox2, pictureBox4, pictureBox5 };
            files = new string?[targets.Length];

            dataGridView1.DataBindingComplete += (s, ev) =>
            {
                StyleGrid(dataGridView1);
                if (currentTable == "Cars")
                    HideImageColumns();
                if (currentTable == "Customers")
                    HideCustomerColumns();
            };
        }

        // ===== GRID ROW/CELL CLICK =====
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // ── Customers tab — click name to show that customer's details ────
            if (currentTable == "Customers" && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Get clicked customer's name
                string clickedName = row.Cells["FullName"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(clickedName)) return;

                // Filter grid to show only that customer's full record
                try
                {
                    con.Open();
                    string query = "SELECT * FROM [Customers] WHERE [FullName] = ? " +
                                   "ORDER BY [CustomerID] DESC";
                    OleDbDataAdapter da = new OleDbDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("?", clickedName);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    StyleGrid(dataGridView1);

                    // Show ALL columns so the full info is visible
                    ShowCustomerColumns();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }

                return;
            }

            // ── Cars tab — populate textboxes ─────────────────────────────────
            if (currentTable != "Cars") return;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells["CarRentalID"].Value?.ToString() ?? "";
                textBox2.Text = row.Cells["Brand"].Value?.ToString() ?? "";
                textBox3.Text = row.Cells["Model"].Value?.ToString() ?? "";
                textBox4.Text = row.Cells["Year"].Value?.ToString() ?? "";
                textBox5.Text = row.Cells["PlateNumber"].Value?.ToString() ?? "";
                textBox6.Text = row.Cells["Color"].Value?.ToString() ?? "";
                textBox13.Text = row.Cells["Rental Price"].Value?.ToString() ?? "";
                comboBox1.Text = row.Cells["Status"].Value?.ToString() ?? "";

                string? img1 = row.Cells["image1"].Value?.ToString();
                string? img2 = row.Cells["image2"].Value?.ToString();
                string? img3 = row.Cells["image3"].Value?.ToString();

                LoadImageToPictureBox(pictureBox2, img1);
                LoadImageToPictureBox(pictureBox4, img2);
                LoadImageToPictureBox(pictureBox5, img3);

                files = new string?[] { img1, img2, img3 };

                textBox2.BackColor = Color.White;
                textBox4.BackColor = Color.White;
                textBox6.BackColor = Color.White;
                textBox13.BackColor = Color.White;
            }
        }

        // ===== SEARCH BAR — textBox14 =====
        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            string keyword = textBox14.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Reset — reload all customers, hide detail columns
                SwitchGrid("Customers");
                return;
            }

            SearchCustomers(keyword);
        }
        // ===== SEARCH BUTTON — button11 =====
        private void button11_Click_1(object sender, EventArgs e)
        {
            SearchCustomers(textBox14.Text.Trim());
        }

        // ===== SEARCH CUSTOMERS =====
        private void SearchCustomers(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                SwitchGrid("Customers");
                return;
            }

            try
            {
                con.Open();
                string query = "SELECT * FROM [Customers] " +
                               "WHERE [FullName] LIKE ? " +
                               "ORDER BY [CustomerID] DESC";

                OleDbDataAdapter da = new OleDbDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("?", $"%{keyword}%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                StyleGrid(dataGridView1);

                // Show all columns when searching so user sees full details
                ShowCustomerColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // ===== REFRESH BUTTON =====
        private void button7_Click(object sender, EventArgs e)
        {
            SwitchGrid(currentTable);
        }

        // ===== ADD BUTTON =====
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(textBox13.Text))
            {
                MessageBox.Show("Please fill in all fields!");
                return;
            }

            if (!double.TryParse(textBox13.Text, out _))
            {
                MessageBox.Show("Please input a valid number for Rental Price.");
                textBox13.Focus();
                return;
            }

            if (!int.TryParse(textBox4.Text, out _))
            {
                MessageBox.Show("Please input a valid number for Year.");
                textBox4.Focus();
                return;
            }

            if (textBox6.Text.Any(c => char.IsDigit(c)))
            {
                MessageBox.Show("Please input a valid text for Color. Numbers are not allowed.");
                textBox6.Focus();
                return;
            }

            if (textBox2.Text.Any(c => char.IsDigit(c)))
            {
                MessageBox.Show("Please input a valid text for Brand. Numbers are not allowed.");
                textBox2.Focus();
                return;
            }

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a Status.");
                comboBox1.Focus();
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(con.ConnectionString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(
                        "INSERT INTO CARS ([CarRentalID], [Brand], [Model], [Year], " +
                        "[PlateNumber], [Color], [Rental Price], [Status], " +
                        "[image1], [image2], [image3]) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", conn);

                    cmd.Parameters.AddWithValue("?", (object)textBox1.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object)textBox2.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object)textBox3.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object)textBox4.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object)textBox5.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object)textBox6.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object)textBox13.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?",
                        (object?)comboBox1.SelectedItem?.ToString() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object?)files[0] ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object?)files[1] ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("?", (object?)files[2] ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Car added successfully!");
                SwitchGrid("Cars");
                button3_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ===== DELETE BUTTON =====
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please select a car from the list or enter a Car ID first.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to delete Car ID: {textBox1.Text}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.No) return;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(con.ConnectionString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(
                        "DELETE FROM CARS WHERE [CarRentalID] = ?", conn);
                    cmd.Parameters.AddWithValue("?", textBox1.Text);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Car deleted successfully!");
                        SwitchGrid("Cars");
                        button3_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("No record found to delete.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ===== UPDATE BUTTON =====
        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please select a car from the list first!");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(textBox13.Text))
            {
                MessageBox.Show("Please fill in all fields before updating!");
                return;
            }

            if (!double.TryParse(textBox13.Text, out _))
            {
                MessageBox.Show("Please input a valid number for Rental Price.");
                textBox13.Focus();
                return;
            }

            if (!int.TryParse(textBox4.Text, out _))
            {
                MessageBox.Show("Please input a valid number for Year.");
                textBox4.Focus();
                return;
            }

            if (textBox6.Text.Any(c => char.IsDigit(c)))
            {
                MessageBox.Show("Please input a valid text for Color. Numbers are not allowed.");
                textBox6.Focus();
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(con.ConnectionString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(
                        "UPDATE Cars SET [Brand]=?, [Model]=?, [Year]=?, " +
                        "[PlateNumber]=?, [Color]=?, [Rental Price]=?, [Status]=? " +
                        "WHERE [CarRentalID]=?", conn);

                    cmd.Parameters.AddWithValue("?", textBox2.Text);
                    cmd.Parameters.AddWithValue("?", textBox3.Text);
                    cmd.Parameters.AddWithValue("?", textBox4.Text);
                    cmd.Parameters.AddWithValue("?", textBox5.Text);
                    cmd.Parameters.AddWithValue("?", textBox6.Text);
                    cmd.Parameters.AddWithValue("?", textBox13.Text);
                    cmd.Parameters.AddWithValue("?", comboBox1.SelectedItem?.ToString());
                    cmd.Parameters.AddWithValue("?", textBox1.Text);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        MessageBox.Show("Car updated successfully!");
                    else
                        MessageBox.Show("No record found to update.");

                    SwitchGrid("Cars");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ===== CLEAR BUTTON =====
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox6.Text = string.Empty;
            textBox13.Text = string.Empty;
            comboBox1.Text = string.Empty;

            textBox2.BackColor = Color.White;
            textBox4.BackColor = Color.White;
            textBox6.BackColor = Color.White;
            textBox13.BackColor = Color.White;

            pictureBox2.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;

            files = new string?[targets.Length];
        }

        // ===== BACK BUTTON =====
        private void button6_Click(object sender, EventArgs e)
        {
            Form form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        // ===== IMAGE LOADER =====
        private void LoadImageToPictureBox(PictureBox pb, string? path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Image?.Dispose();
                pb.Image = Image.FromFile(path);
            }
            else
            {
                pb.Image = null;
            }
        }

        // ===== IMAGE UPLOAD BUTTON =====
        private void button5_Click(object sender, EventArgs e)
        {
            if (targets.Length == 0) return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] selected = openFileDialog.FileNames;
                    int count = Math.Min(selected.Length, targets.Length);

                    for (int i = 0; i < count; i++)
                    {
                        string path = selected[i];
                        files[i] = path;
                        try
                        {
                            using (var fs = new System.IO.FileStream(
                                path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            {
                                using (var tmp = Image.FromStream(fs))
                                {
                                    targets[i].Image?.Dispose();
                                    targets[i].Image = new Bitmap(tmp);
                                }
                            }
                        }
                        catch (Exception loadEx)
                        {
                            MessageBox.Show("Error loading image: " + loadEx.Message);
                            targets[i].Image?.Dispose();
                            targets[i].Image = null;
                            files[i] = null;
                        }
                    }

                    for (int i = count; i < targets.Length; i++)
                    {
                        targets[i].Image?.Dispose();
                        targets[i].Image = null;
                        files[i] = null;
                    }
                }
            }
        }

        // ===== STYLE GRID =====
        private void StyleGrid()
        {
            StyleGrid(dataGridView1);
        }

        private void StyleGrid(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = Color.LightGray;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.Brown;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Brown;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            //dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.MistyRose;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.IndianRed;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.RowHeadersVisible = false;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.Padding = new Padding(2);
            dgv.RowTemplate.Height = 30;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.ColumnHeaderMouseClick -= DataGridView_ColumnHeaderMouseClick;
            dgv.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
            // Left align for Customers, center for everything else
            dgv.DefaultCellStyle.Alignment = currentTable == "Customers"
                ? DataGridViewContentAlignment.MiddleLeft
                : DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.Padding = currentTable == "Customers"
            ? new Padding(10, 2, 2, 2)  // extra left padding for names
            : new Padding(2);


        }

        private void DataGridView_ColumnHeaderMouseClick(object? sender,
            DataGridViewCellMouseEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            dgv.ClearSelection();
            foreach (DataGridViewRow row in dgv.Rows)
                if (!row.IsNewRow)
                    row.Cells[e.ColumnIndex].Selected = true;
        }

        // ===== ANALYTICS BUTTON =====
        private void button14_Click(object sender, EventArgs e)
        {
            FormDashboard dash = new FormDashboard();
            dash.ShowDialog();
        }

        // ===== EMPTY HANDLERS =====
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e) { }
        private void dataGridView1_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e) { }
        private void button8_Click(object sender, EventArgs e) { }
        private void button9_Click(object sender, EventArgs e) { }
        private void button10_Click(object sender, EventArgs e) { }
        private void button11_Click(object sender, EventArgs e) { }
    }
}