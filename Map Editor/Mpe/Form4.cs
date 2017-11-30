using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mpe
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((this.textBox1.Text != string.Empty) && (this.textBox2.Text != string.Empty))
            {
                Form1 frame = (Form1)this.Owner;
                frame.height = Convert.ToInt32(this.textBox1.Text);
                frame.width = Convert.ToInt32(this.textBox2.Text);
                this.Close();
            }

            if ((this.textBox1.Text == string.Empty) && (this.textBox2.Text == string.Empty))
            {
                Form1 frame = (Form1)this.Owner;
                frame.width = 0;
                frame.height = 0;
                this.Close();

            }
        }
    }
}
