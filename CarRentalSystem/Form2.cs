using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CarRentalSystem
{
    public partial class Form2 : Form
    {
        // Global variables
        int failedAttempts = 0;
        DateTime lockoutTime;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Check if locked for 1 minute
            if (DateTime.Now < lockoutTime)
            {
                MessageBox.Show("Please try again later or wait 1 minute to try again.");
                return;
            }

            // Username and Password
            string username = textBox1.Text;
            string pass = textBox2.Text;

            // Correct Login
            if (username == "admin123" && pass == "triple3")
            {
                failedAttempts = 0;

                Form3 form3 = new Form3();
                form3.Show();
                this.Hide();
            }
            else
            {
                failedAttempts++;

                MessageBox.Show("Invalid username or password. Please try again.");
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;

                // Lock after 3 failed attempts
                if (failedAttempts >= 3)
                {
                    lockoutTime = DateTime.Now.AddMinutes(1);

                    MessageBox.Show("Please try again later or wait 1 minute to try again.");

                    failedAttempts = 0; 
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}