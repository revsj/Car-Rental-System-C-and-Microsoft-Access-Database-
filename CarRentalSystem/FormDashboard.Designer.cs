namespace CarRentalSystem
{
    partial class FormDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label6 = new Label();
            label7 = new Label();
            label9 = new Label();
            label12 = new Label();
            button1 = new Button();
            button2 = new Button();
            label5 = new Label();
            label8 = new Label();
            label10 = new Label();
            label11 = new Label();
            label13 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Brown;
            label1.Location = new Point(50, 51);
            label1.Name = "label1";
            label1.Size = new Size(230, 30);
            label1.TabIndex = 0;
            label1.Text = "📊 Business Summary";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Brown;
            label2.Location = new Point(50, 101);
            label2.Name = "label2";
            label2.Size = new Size(83, 15);
            label2.TabIndex = 1;
            label2.Text = "💰 Total Profit";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Brown;
            label3.Location = new Point(430, 101);
            label3.Name = "label3";
            label3.Size = new Size(99, 15);
            label3.TabIndex = 2;
            label3.Text = "✅ Cars Available";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(50, 122);
            label4.Name = "label4";
            label4.Size = new Size(54, 25);
            label4.TabIndex = 3;
            label4.Text = "label";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.Brown;
            label6.Location = new Point(50, 192);
            label6.Name = "label6";
            label6.Size = new Size(98, 15);
            label6.TabIndex = 5;
            label6.Text = "\U0001f9fe Total Invoices";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.Brown;
            label7.Location = new Point(430, 192);
            label7.Name = "label7";
            label7.Size = new Size(119, 15);
            label7.TabIndex = 6;
            label7.Text = "🚗 Currently Rented";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = Color.Brown;
            label9.Location = new Point(50, 266);
            label9.Name = "label9";
            label9.Size = new Size(116, 15);
            label9.TabIndex = 8;
            label9.Text = "🏆 Most Rented Car";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label12.ForeColor = Color.Brown;
            label12.Location = new Point(430, 266);
            label12.Name = "label12";
            label12.Size = new Size(128, 15);
            label12.TabIndex = 11;
            label12.Text = "🔄 Completed Rentals";
            // 
            // button1
            // 
            button1.BackColor = Color.Brown;
            button1.Cursor = Cursors.Hand;
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(50, 345);
            button1.Name = "button1";
            button1.Size = new Size(101, 32);
            button1.TabIndex = 13;
            button1.Text = "🔄 Refresh";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.RosyBrown;
            button2.Cursor = Cursors.Hand;
            button2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.White;
            button2.Location = new Point(168, 346);
            button2.Name = "button2";
            button2.Size = new Size(98, 32);
            button2.TabIndex = 14;
            button2.Text = "✖ Close";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(430, 122);
            label5.Name = "label5";
            label5.Size = new Size(54, 25);
            label5.TabIndex = 15;
            label5.Text = "label";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(50, 207);
            label8.Name = "label8";
            label8.Size = new Size(54, 25);
            label8.TabIndex = 16;
            label8.Text = "label";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.Location = new Point(430, 207);
            label10.Name = "label10";
            label10.Size = new Size(54, 25);
            label10.TabIndex = 17;
            label10.Text = "label";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.Location = new Point(50, 281);
            label11.Name = "label11";
            label11.Size = new Size(54, 25);
            label11.TabIndex = 18;
            label11.Text = "label";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label13.Location = new Point(430, 292);
            label13.Name = "label13";
            label13.Size = new Size(54, 25);
            label13.TabIndex = 19;
            label13.Text = "label";
            // 
            // FormDashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 235, 235);
            ClientSize = new Size(699, 627);
            Controls.Add(label13);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label8);
            Controls.Add(label5);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label12);
            Controls.Add(label9);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "FormDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "🔄 Completed Rentals";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label7;
        private Label label9;
        private Label label12;
        private Button button1;
        private Button button2;
        private Label label5;
        private Label label8;
        private Label label10;
        private Label label11;
        private Label label13;
    }
}