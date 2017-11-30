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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.textBox1, "This value should not exceed 600px.");
            toolTip1.IsBalloon = true;
            toolTip2.SetToolTip(this.textBox2, "This value should not exceed 600px.");
            toolTip2.IsBalloon = true;

        }
        
        int f = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1 frame = (Form1)this.Owner;
            if (comboBox1.Text == "Floating Block")
            {
                f = 1;
            }
            if (comboBox1.Text == "Fixed Block")
            {
                f = 2;
            }
            if (comboBox1.Text == "Gate")
            {
                if (frame.dsb == 0)
                    f = 3;
                else
                    MessageBox.Show("Dispose the existing Gate first.");
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((this.textBox1.Text != string.Empty) && (this.textBox2.Text != string.Empty) && (f != 0) && (Convert.ToInt32(this.textBox2.Text) <= 600) && (Convert.ToInt32(this.textBox1.Text) <= 600))
            {
                Form1 frame = (Form1)this.Owner;
                frame.height = Convert.ToInt32(this.textBox1.Text);
                frame.width = Convert.ToInt32(this.textBox2.Text);
                frame.q = 1;
                frame.cnt += 1;
                frame.Data[frame.cnt] = f;
                if (f == 3)
                {
                    frame.dsb = 1;
                }
                //frame.label1.Text = Convert.ToString(f);

                this.Close();
            }
        }

        
        

        

       
    }
}
