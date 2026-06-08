using System;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace CarRentalSystem
{
    public partial class FormDashboard : Form
    {
        private OleDbConnection con = new OleDbConnection(
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\revoj\source\repos\CarRentalSystem\CarRentalSystem\CarRental.accdb");

        public FormDashboard()
        {
            InitializeComponent();
            this.Text = "Business Dashboard";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetupLabels();
            LoadStats();
        }

        // ===== SETUP LABEL STYLES =====
        private void SetupLabels()
        {
            // ── Title 
            label1.Text = "── 📊 Business Summary ──────────────────────────────────────────────────────";
            label1.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            label1.ForeColor = Color.Brown;
            label1.AutoSize = true;


            //divider// 
            Panel divider = new Panel
            {
                BackColor = Color.Brown,
                Location = new Point(20, 55),
                Size = new Size(545, 2)
            };



            // ── Stat title labels
            StyleStatTitle(label2, "💰 Total Profit");
            StyleStatTitle(label6, "🧾 Total Invoices");
            StyleStatTitle(label9, "🏆 Most Rented Car");
            StyleStatTitle(label3, "✅ Cars Available");
            StyleStatTitle(label7, "🚗 Currently Rented");
            StyleStatTitle(label12, "🔄 Completed Rentals");

            // ── Value labels
            StyleValueLabel(label4);
            StyleValueLabel(label8);
            StyleValueLabel(label11);
            StyleValueLabel(label5);
            StyleValueLabel(label10);
            StyleValueLabel(label13);

            // ── Set loading text 
            label4.Text = "Loading...";
            label8.Text = "Loading...";
            label11.Text = "Loading...";
            label5.Text = "Loading...";
            label10.Text = "Loading...";
            label13.Text = "Loading...";
        }

        // ── Helper: stat title style
        private void StyleStatTitle(Label lbl, string text)
        {
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lbl.ForeColor = Color.Brown;
            lbl.AutoSize = true;
        }

        // ── Helper: value label style 
        private void StyleValueLabel(Label lbl)
        {
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(80, 30, 30);
            lbl.AutoSize = true;
        }

        // ===== LOAD STATS FROM DATABASE =====
        private void LoadStats()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(con.ConnectionString))
                {
                    conn.Open();

                    // ── Total Profit
                    using (var cmd = new OleDbCommand(
                        "SELECT SUM([TotalAmount]) FROM Rentals " +
                        "WHERE [Status] = 'Paid Successfully'", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        double profit = result == DBNull.Value ? 0 : Convert.ToDouble(result);
                        label4.Text = $"₱{profit:N2}";
                    }

                    // ── Total Invoices
                    using (var cmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM Rentals " +
                        "WHERE [Status] = 'Paid Successfully'", conn))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        label8.Text = $"{count} rental(s)";
                    }

                    // ── Most Rented Car
                    using (var cmd = new OleDbCommand(
                        "SELECT TOP 1 r.[CarRentalID], c.[Brand], c.[Model], " +
                        "COUNT(r.[CarRentalID]) AS RentCount " +
                        "FROM Rentals r " +
                        "INNER JOIN Cars c ON r.[CarRentalID] = c.[CarRentalID] " +
                        "WHERE r.[Status] = 'Paid Successfully' " +
                        "GROUP BY r.[CarRentalID], c.[Brand], c.[Model] " +
                        "ORDER BY COUNT(r.[CarRentalID]) DESC", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string brand = reader["Brand"]?.ToString() ?? "";
                                string model = reader["Model"]?.ToString() ?? "";
                                int count = Convert.ToInt32(reader["RentCount"]);
                                label11.Text = $"{brand} {model} ({count}x)";
                            }
                            else
                            {
                                label11.Text = "No data yet";
                            }
                        }
                    }

                    // ── Cars Available 
                    using (var cmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM Cars WHERE [Status] = 'Available'", conn))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        label5.Text = $"{count} car(s)";
                    }

                    // ── Currently Rented 
                    using (var cmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM Cars WHERE [Status] = 'Rented'", conn))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        label10.Text = $"{count} car(s)";
                    }

                    // ── Completed Rentals 
                    using (var cmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM Rentals " +
                        "WHERE [Status] = 'Paid Successfully' " +
                        "AND [ReturnDate] < ?", conn))
                    {
                        cmd.Parameters.AddWithValue("?", DateTime.Today);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        label13.Text = $"{count} rental(s)";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stats: " + ex.Message);
            }
        }

        // ===== REFRESH BUTTON =====
        private void button1_Click(object sender, EventArgs e)
        {
            LoadStats();
        }

        // ===== CLOSE BUTTON =====
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}