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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        int f = 0;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1 frame = (Form1)this.Owner;
            if (comboBox1.Text == "Main Ball")
            {
                if (frame.dsb1 == 0)
                {
                    f = 4;
                    textBox1.Text = Convert.ToString(15);
                }
                else
                    MessageBox.Show("Dispose the existing Main Ball first.");
                label2.Visible = false;
                textBox2.Visible = false;
            }
            if (comboBox1.Text == "Guard")
            {
                if (frame.dsb2 < 2)
                {
                    f = 5;
                    textBox1.Text = Convert.ToString(15);
                    label2.Visible = true;
                    textBox2.Visible = true;
                }
                else
                    MessageBox.Show("Dispose one of the existing Guards first.");
                
            }
            if (comboBox1.Text == "Key Ball")
            {
                f = 6;
                textBox1.Text = Convert.ToString(20);
                label2.Visible = false;
                textBox2.Visible = false;
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if ((this.textBox1.Text != string.Empty) && (f != 0))
            {
                Form1 frame = (Form1)this.Owner;
                frame.height = Convert.ToInt32(this.textBox1.Text);
                frame.width = Convert.ToInt32(this.textBox1.Text);
                frame.q = 1;
                frame.cnt += 1;
                frame.Data[frame.cnt] = f;
                if (f == 4)
                    frame.dsb1 = 1;
                if (f == 5)
                    frame.dsb2 += 1;
                if (f == 5)
                    if (this.textBox2.Text != string.Empty)
                        frame.ex = Convert.ToInt32(this.textBox2.Text);
                    else
                        frame.ex = 0;
                //frame.label1.Text = Convert.ToString(f);
                
                this.Close();
            }
        }

        


    }
}
