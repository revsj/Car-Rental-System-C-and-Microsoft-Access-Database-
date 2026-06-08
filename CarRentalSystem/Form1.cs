namespace CarRentalSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form button2 = new Form2();
            button2.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form form4 = new Form4();
            form4.Show();
            this.Hide();
        }
    }
}
