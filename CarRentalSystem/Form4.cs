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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            StyleGrid();
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView1.CellClick += dataGridView1_CellClick;
            this.AutoScroll = true;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // ===== Contact Number - PH format validation =====
            textBox2.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;

                if (char.IsDigit(e.KeyChar) && textBox2.Text.Length >= 11)
                    e.Handled = true;

                if (textBox2.Text.Length == 0 && e.KeyChar != '0')
                    e.Handled = true;

                if (textBox2.Text.Length == 1 && textBox2.Text[0] == '0' && e.KeyChar != '9')
                    e.Handled = true;
            };
            textBox2.TextChanged += (s, e) =>
            {
                textBox2.BackColor = !string.IsNullOrWhiteSpace(textBox2.Text) &&
                    !System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, @"^\d+$")
                    ? Color.MistyRose : Color.White;
            };

            // ===== Agency No - numbers only =====
            textBox6.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
            textBox6.TextChanged += (s, e) =>
            {
                textBox6.BackColor = !string.IsNullOrWhiteSpace(textBox6.Text) &&
                    !System.Text.RegularExpressions.Regex.IsMatch(textBox6.Text, @"^\d+$")
                    ? Color.MistyRose : Color.White;
            };

            // ===== Full Name - letters only =====
            textBox1.TextChanged += (s, e) =>
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    textBox1.Text, @"^[a-zA-Z\s]*$"))
                {
                    int pos = textBox1.SelectionStart - 1;
                    textBox1.Text = System.Text.RegularExpressions.Regex.Replace(
                        textBox1.Text, @"[^a-zA-Z\s]", "");
                    textBox1.BackColor = Color.MistyRose;
                    textBox1.SelectionStart = Math.Max(0, pos);
                }
                else
                {
                    textBox1.BackColor = Color.White;
                }
            };

            // ===== Email - validate on leave only =====
            textBox3.Leave += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(textBox3.Text) &&
                    !textBox3.Text.Contains("@"))
                {
                    MessageBox.Show("Please enter a valid email address.");
                    textBox3.BackColor = Color.MistyRose;
                    textBox3.Focus();
                }
                else
                {
                    textBox3.BackColor = Color.White;
                }
            };
        }

        private OleDbConnection con = new OleDbConnection(
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\revoj\source\repos\CarRentalSystem\CarRentalSystem\CarRental.accdb");

        private string selectedCarID = "";
        private string selectedCarModel = "";
        private string selectedCarBrand = "";
        private double selectedCarPrice = 0;

        // ===== LOAD DATA =====
        public void LoadData()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM Cars";
                OleDbDataAdapter da = new OleDbDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                HideImageColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void dataGridView1_DataBindingComplete(object? sender,
            DataGridViewBindingCompleteEventArgs e)
        { 
            StyleGrid();
            HideImageColumns();
        }

        // ===== FORM LOAD =====
        private void Form4_Load(object? sender, EventArgs e)
        {
            LoadData();

            dateTimePicker1.MinDate = DateTime.Today;
            dateTimePicker2.MinDate = DateTime.Today.AddDays(1);
            dateTimePicker1.ValueChanged += (s, ev) =>
            {
                dateTimePicker2.MinDate = dateTimePicker1.Value.AddDays(1);
                if (dateTimePicker2.Value <= dateTimePicker1.Value)
                    dateTimePicker2.Value = dateTimePicker1.Value.AddDays(1);
            };

            try
            {
                label15.Text = string.Empty;
                label16.Text = string.Empty;
                label22.Text = string.Empty;
                label17.Text = string.Empty;
                label21.Text = string.Empty;
                if (textBox13 != null) textBox13.Text = string.Empty;
            }
            catch { }
        }

        // ===== CONFIRM RENTAL BUTTON =====
        private void button5_Click(object? sender, EventArgs e)
        {
            // ===== EMPTY FIELD CHECK =====
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

            // ===== EMAIL FORMAT CHECK =====
            if (!textBox3.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.");
                textBox3.Focus();
                return;
            }

            // ===== CAR SELECTED CHECK =====
            if (string.IsNullOrWhiteSpace(selectedCarID))
            {
                MessageBox.Show("Please select a car from the list first!");
                return;
            }

            // ===== DOUBLE BOOKING CHECK =====
            try
            {
                using (OleDbConnection connCheck = new OleDbConnection(con.ConnectionString))
                {
                    connCheck.Open();
                    using (OleDbCommand checkCmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM Rentals WHERE [CarRentalID]=? " +
                        "AND [ReturnDate] >= ? " +
                        "AND [Status]='Paid Successfully'", connCheck))
                    {
                        checkCmd.Parameters.AddWithValue("?", textBox13.Text);
                        checkCmd.Parameters.AddWithValue("?", DateTime.Today);

                        int existing = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (existing > 0)
                        {
                            MessageBox.Show(
                                "This car is already rented and not yet returned!\n" +
                                "Please choose another car.",
                                "Car Unavailable",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking availability: " + ex.Message);
                return;
            }

            // ===== BILLING CALCULATION =====
            double rentalPricePerDay = 0;
            double.TryParse(
                label17.Text.Replace("₱", "").Replace(",", ""),
                out rentalPricePerDay);

            int days = (dateTimePicker2.Value.Date - dateTimePicker1.Value.Date).Days;
            if (days <= 0) days = 1;

            double basePrice = rentalPricePerDay * days;
            double vat = basePrice * 0.12;

            string selectedDiscount = comboBox1.SelectedItem?.ToString()?.Trim() ?? "None";
            double discount = 0;
            string discountType = "None";

            if (selectedDiscount == "Senior Citizen Discount" ||
                selectedDiscount == "PWD Discount")
            {
                discount = basePrice * 0.20;
                discountType = selectedDiscount;
            }

            double total = basePrice + vat - discount;

            // ===== SHOW INVOICE =====
            ShowReceipt(
                textBox1.Text,
                selectedCarBrand,
                selectedCarModel,
                textBox13.Text,
                days,
                basePrice,
                vat,
                discount,
                total,
                discountType
            );

            // ===== CONFIRMATION =====
            DialogResult confirm = MessageBox.Show(
                "Confirm this rental?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;

            // ===== INSERT TO DATABASE =====
            try
            {
                using (OleDbConnection conn = new OleDbConnection(con.ConnectionString))
                {
                    conn.Open();

                    // Insert Customer
                    try
                    {
                        using (OleDbCommand cmdCustomer = new OleDbCommand(
                            "INSERT INTO Customers ([FullName], [Contact Number], " +
                            "[Email], [Address], [DriverLicenseNumber], [AgencyNo], " +
                            "[Discount]) VALUES (?, ?, ?, ?, ?, ?, ?)", conn))
                        {
                            cmdCustomer.Parameters.AddWithValue("?", textBox1.Text);
                            cmdCustomer.Parameters.AddWithValue("?", textBox2.Text);
                            cmdCustomer.Parameters.AddWithValue("?", textBox3.Text);
                            cmdCustomer.Parameters.AddWithValue("?", textBox4.Text);
                            cmdCustomer.Parameters.AddWithValue("?", textBox5.Text);
                            cmdCustomer.Parameters.AddWithValue("?", textBox6.Text);
                            cmdCustomer.Parameters.AddWithValue("?", selectedDiscount);
                            cmdCustomer.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex1)
                    {
                        MessageBox.Show("Customer Error: " + ex1.Message);
                        return;
                    }

                    // Get new Customer ID
                    int newCustomerID;
                    using (OleDbCommand cmdGetID = new OleDbCommand(
                        "SELECT MAX(CustomerID) FROM Customers", conn))
                    {
                        newCustomerID = Convert.ToInt32(cmdGetID.ExecuteScalar());
                    }

                    // Insert Rental
                    try
                    {
                        using (OleDbCommand cmdRental = new OleDbCommand(
                            "INSERT INTO Rentals ([CarRentalID], [DateRented], " +
                            "[ReturnDate], [Status], [CustomerID], [TotalAmount]) " +
                            "VALUES (?, ?, ?, ?, ?, ?)", conn))
                        {
                            cmdRental.Parameters.AddWithValue("?", textBox13.Text);
                            cmdRental.Parameters.AddWithValue("?", dateTimePicker1.Value.Date);
                            cmdRental.Parameters.AddWithValue("?", dateTimePicker2.Value.Date);
                            cmdRental.Parameters.AddWithValue("?", "Paid Successfully");
                            cmdRental.Parameters.AddWithValue("?", newCustomerID);
                            cmdRental.Parameters.AddWithValue("?", total);
                            cmdRental.ExecuteNonQuery();
                        }

                        // ===== AUTO-UPDATE CAR STATUS TO "Rented" =====
                        using (OleDbCommand cmdUpdateStatus = new OleDbCommand(
                            "UPDATE Cars SET [Status] = ? WHERE [CarRentalID] = ?", conn))
                        {
                            cmdUpdateStatus.Parameters.AddWithValue("?", "Rented");
                            cmdUpdateStatus.Parameters.AddWithValue("?", textBox13.Text);
                            cmdUpdateStatus.ExecuteNonQuery();
                        }

                        // ===== AUTO-UPDATE CAR STATUS TO "Available" =====
                        using (OleDbCommand cmdUpdateStatus = new OleDbCommand(
                            "UPDATE Cars SET [Status] = ? WHERE [CarRentalID] = ?", conn))
                        {
                            cmdUpdateStatus.Parameters.AddWithValue("?", "Rented");
                            cmdUpdateStatus.Parameters.AddWithValue("?", textBox13.Text);
                            cmdUpdateStatus.ExecuteNonQuery();
                        }


                        // ===== SUCCESS MESSAGE =====
                        MessageBox.Show(
                            "Rental Confirmed Successfully!\n\n" +
                            "Thank you for choosing our service,\n" +
                            "have a wonderful day! 😊",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Clear all fields after success
                        button3_Click(sender, e);

                        // Refresh grid to show updated status
                        LoadData();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("Rental Error: " + ex2.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Main Error: " + ex.Message);
            }
        }

        // ===== INVOICE =====
        private void ShowReceipt(string customerName, string carBrand, string carModel,
            string carID, int days, double basePrice, double vat,
            double discount, double total, string discountType)
        {
            string receipt = "";
            receipt += "========================================\n";
            receipt += "           CAR RENTAL INVOICE           \n";
            receipt += "========================================\n";
            receipt += $"  Customer Name : {customerName}\n";
            receipt += $"  Car           : {carBrand} {carModel}\n";
            receipt += $"  Car ID        : {carID}\n";
            receipt += "----------------------------------------\n";
            receipt += $"  Date Rented   : {dateTimePicker1.Value:MMM dd, yyyy}\n";
            receipt += $"  Return Date   : {dateTimePicker2.Value:MMM dd, yyyy}\n";
            receipt += $"  No. of Days   : {days} day(s)\n";
            receipt += "----------------------------------------\n";
            receipt += $"  Base Price    : ₱{basePrice:F2}\n";
            receipt += $"  VAT (12%)     : ₱{vat:F2}\n";
            receipt += $"  Discount      : -{discountType}\n";
            receipt += $"  Discount Amt  : -₱{discount:F2}\n";
            receipt += "========================================\n";
            receipt += $"  TOTAL AMOUNT  : ₱{total:F2}\n";
            receipt += "========================================\n";
            receipt += "========================================\n";

            MessageBox.Show(receipt, "Car Rental Invoice",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== GRID ROW CLICK =====
        private void dataGridView1_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedCarBrand = row.Cells["Brand"].Value?.ToString() ?? "";
                selectedCarModel = row.Cells["Model"].Value?.ToString() ?? "";
                selectedCarPrice = Convert.ToDouble(row.Cells["Rental Price"].Value ?? 0);
                selectedCarID = row.Cells["CarRentalID"].Value?.ToString() ?? "";

                string? img1 = row.Cells[8].Value?.ToString();
                string? img2 = row.Cells[9].Value?.ToString();
                string? img3 = row.Cells[10].Value?.ToString();

                LoadImageToPictureBox(pictureBox2, img1);
                LoadImageToPictureBox(pictureBox3, img2);
                LoadImageToPictureBox(pictureBox5, img3);

                label15.Text = selectedCarModel;
                label16.Text = selectedCarBrand;
                label22.Text = selectedCarID;
                label17.Text = "₱" + selectedCarPrice.ToString("F2");
                label21.Text = row.Cells["Status"].Value?.ToString() ?? "";

                try
                {
                    string? CarRentalID = null;
                    string[] candidates = { "CarRentalID", "CarID", "Car Rental ID", "ID" };
                    foreach (var name in candidates)
                    {
                        if (dataGridView1.Columns.Contains(name))
                        {
                            CarRentalID = row.Cells[name].Value?.ToString();
                            break;
                        }
                    }

                    if (CarRentalID == null && row.Cells.Count > 0)
                        CarRentalID = row.Cells[0].Value?.ToString();

                    if (textBox13 != null)
                        textBox13.Text = CarRentalID ?? string.Empty;
                }
                catch { }
            }
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

        // ===== HIDE IMAGE COLUMNS =====
        private void HideImageColumns()
        {
            try
            {
                if (dataGridView1.Columns == null) return;
                string[] cols = { "image1", "image2", "image3" };
                foreach (var name in cols)
                {
                    if (dataGridView1.Columns.Contains(name))
                        dataGridView1.Columns[name].Visible = false;
                }
            }
            catch { }
        }

        // ===== STYLE GRID =====
        private void StyleGrid()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.GridColor = Color.LightGray;

            // Header — stays Brown even when clicked
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Brown;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Brown;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Normal rows — IndianRed when selected, no blue
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            // Alternating rows — also IndianRed when selected
            //dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.MistyRose;
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.IndianRed;
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Padding = new Padding(2);
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.ColumnHeaderMouseClick -= DataGridView_ColumnHeaderMouseClick;
            dataGridView1.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
        }

        private void DataGridView_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.ClearSelection();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                    row.Cells[e.ColumnIndex].Selected = true;
            }
        }

        // ===== BUTTONS =====
        private void button1_Click(object? sender, EventArgs e) { LoadData(); }

        private void button2_Click(object sender, EventArgs e)
        {
            Form form1 = new Form1();
            form1.Show();
            this.Hide();
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

            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;
            textBox3.BackColor = Color.White;
            textBox6.BackColor = Color.White;

            dateTimePicker1.MinDate = DateTime.Today;
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.MinDate = DateTime.Today.AddDays(1);
            dateTimePicker2.Value = DateTime.Today.AddDays(1);

            selectedCarID = "";
            selectedCarModel = "";
            selectedCarBrand = "";
            selectedCarPrice = 0;

            // Clear car detail labels
            label15.Text = string.Empty;
            label16.Text = string.Empty;
            label17.Text = string.Empty;
            label21.Text = string.Empty;
            label22.Text = string.Empty;

            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox5.Image = null;
        }

        // Empty handlers
        private void button7_Click(object? sender, EventArgs e) { }
        private void splitContainer1_Panel1_Paint(object? sender, PaintEventArgs e) { }
        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e) { }
        private void dataGridView1_CellContentClick(object? sender, DataGridViewCellEventArgs e) { }
        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
    }
}