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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            grh1 = CreateGraphics();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            p1 = 1;
            p2 = 1;
            
        }

        public int q, height, width, n, i, ex;
        int p1, p2;
        int cur1, cur2, rad1, rad2;

        Panel[] Panelz = new Panel[20];
        Graphics[] Grh = new Graphics[20];
        ToolTip[] Tp = new ToolTip[20];
        public Int32[] Data = new Int32[20];
        Int32[] Path1 = new Int32[40];
        Int32[] Path2 = new Int32[40];
        Int32[] order = new Int32[]{0, 4, 6, 3, 2, 1, 5 };

        //Graphics grh;

     
        int src(Panel pk)
        {
            int c = 0;
            foreach (Panel ar in Panelz)
            {
                c += 1;
                if (pk == ar)
                    break;
            }
            return c;
        }

        void dsp(Panel pk)
        {
            if (src(pk) - 1 == cur1)
            {
                if (Path1[1] > 0)
                {
                    int a = 1;
                    grh1.DrawLine(Pens.Black, pk.Location.X + pk.Width / 2, pk.Location.Y + pk.Height / 2, Path1[a], Path1[a + 1]);

                    while (a < p1)
                    {
                        grh1.DrawLine(Pens.Black, Path1[a], Path1[a + 1], Path1[a + 2], Path1[a + 3]);
                        a += 2;
                    }
                    grh1.DrawLine(Pens.Black, Path1[a - 2], Path1[a - 1], pk.Location.X + pk.Width / 2, pk.Location.Y + pk.Height / 2);
                    for (i = 1; i < p1; i++)
                    {
                        Path1[i] = 0;
                    }
                    p1 = 1;
                }

                //grh1 = CreateGraphics(); 
            }
            else
                if (src(pk) - 1 == cur2)
                {
                    if (Path2[1] > 0)
                    {
                        int a = 1;
                        grh1.DrawLine(Pens.Black, pk.Location.X + pk.Width / 2, pk.Location.Y + pk.Height / 2, Path2[a], Path2[a + 1]);

                        while (a < p2)
                        {
                            grh1.DrawLine(Pens.Black, Path2[a], Path2[a + 1], Path2[a + 2], Path2[a + 3]);
                            a += 2;
                        }
                        grh1.DrawLine(Pens.Black, Path2[a - 2], Path2[a - 1], pk.Location.X + pk.Width / 2, pk.Location.Y + pk.Height / 2);
                        for (i = 1; i < p2; i++)
                        {
                            Path2[i] = 0;
                        }
                        p2 = 1;

                    }
                    //grh2 = CreateGraphics();
                } 
        }

        public int cnt = 0;

        private void fixedBLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            q = 0;
            Form2 f2 = new Form2();
            f2.Owner = this;
            f2.ShowDialog();

            
        }

        private void ballToolStripMenuItem_Click(object sender, EventArgs e)
        {
            q = 0;
            Form3 f3 = new Form3();
            f3.Owner = this;
            f3.ShowDialog();
        }


        int count = 0;
        public int k = 0;
        public int dsb = 0, dsb1 = 0, dsb2 = 0;
        int d = 0;
        
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (q == 1)
            {
                Panelz[cnt] = new Panel();
                Panelz[cnt].BringToFront();
                Panelz[cnt].Size = new Size(width, height);
                Panelz[cnt].Width = width;
                Panelz[cnt].Height = height;
                Panelz[cnt].Location = new Point(e.X, e.Y);
                Panelz[cnt].Visible = true;
                Panelz[cnt].BackColor = Color.Black;
                Panelz[cnt].MouseDoubleClick += Pnz_DClick;
                Panelz[cnt].MouseClick += Pnz_Click;
                Panelz[cnt].Name = Convert.ToString(cnt);

                Tp[cnt] = new ToolTip();
                if (Data[cnt] != 5)
                    Tp[cnt].SetToolTip(Panelz[cnt], "RClick to move, DRClick to dispose, DLClick to change size.");
                else
                    Tp[cnt].SetToolTip(Panelz[cnt], "RClick to move, DRClick to dispose, DLClick to change size, LClick to set trajectory");
                Tp[cnt].IsBalloon = true;
                
                Panelz[cnt].Paint += onPaint;

                if (Data[cnt] == 5)
                    if (cur1 == 0)
                    {
                        cur1 = cnt;
                        rad1 = ex;
                        //MessageBox.Show("cur1 ", Convert.ToString(cur1));
                    }
                    else
                        if (cur2 == 0)
                        {
                            cur2 = cnt;
                            rad2 = ex;
                            //MessageBox.Show("cur2 ", Convert.ToString(cur2));
                        }

                this.Controls.Add(Panelz[cnt]);

                ex = 0;
                q = 0;
            }

            if (count > 0)
                if (e.Button == MouseButtons.Right)
                    if (Panelz[count - 1] != null)
                    {
                        Panelz[count - 1].Location = new Point(e.X, e.Y);
                        count = 0;
                    }

            if (kk == true)
            {
                if (cur1 == src(m) - 1)
                {
                    Path1[p1] = e.X;
                    Path1[p1 + 1] = e.Y;
                    p1 += 2;
                    z = true;
                    if (p1 == 3)
                    {
                        grh1.DrawLine(Pens.Azure, m.Location.X + m.Width / 2, m.Location.Y + m.Height / 2, Path1[p1 - 2], Path1[p1 - 1]);
                    }
                    else
                        if (p1 > 3)
                        {
                            grh1.DrawLine(Pens.Azure, Path1[p1 - 4], Path1[p1 - 3], Path1[p1 - 2], Path1[p1 - 1]);
                        }

                }
                else
                {
                    if (cur2 == src(m) - 1)
                    {
                        Path2[p2] = e.X;
                        Path2[p2 + 1] = e.Y;
                        p2 += 2;
                        z = true;

                        if (p2 == 3)
                        {
                            grh1.DrawLine(Pens.Azure, m.Location.X + m.Width / 2, m.Location.Y + m.Height / 2, Path2[p2 - 2], Path2[p2 - 1]);
                        }
                        else
                            if (p2 > 3)
                            {
                                grh1.DrawLine(Pens.Azure, Path2[p2 - 4], Path2[p2 - 3], Path2[p2 - 2], Path2[p2 - 1]);
                            }
                    }
                }

            }
            
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (count > 0)
                //if (e.Button == MouseButtons.Right)
                    if (Panelz[count - 1] != null)
                    {
                        Panelz[count - 1].Location = new Point(e.X, e.Y);
                        
                    }
        }
        
        private void Pnz_DClick(object sender, MouseEventArgs e)
        {
            Panel pnl = sender as Panel;
            if (e.Button == MouseButtons.Right)
            {
                pnl.Dispose();

                switch(Data[src(pnl) - 1])
                {
                    case 3:
                        dsb = 0;
                        break;
                    case 4:
                        dsb1 = 0;
                        break;
                    case 5:
                        dsb2 -= 1;
                        break;
                }

                dsp(pnl);
                if (Data[src(pnl) - 1] == 5)
                    if (cur1 == src(pnl) - 1)
                        cur1 = 0;
                    else
                        if (cur2 == src(pnl) - 1)
                            cur2 = 0;

                
                Data[src(pnl) - 1] = 0;
            }
            if ((e.Button == MouseButtons.Left) & (Data[src(pnl) - 1] <= 3))
            {
                
                Form4 f4 = new Form4();
                f4.Owner = this;
                f4.ShowDialog();

                if ((height > 0) & (width > 0))
                {
                    pnl.Width = width;
                    pnl.Height = height;
                    //d = src(pnl) - 1;
                    pnl.Refresh();
                }
            }

        }

        Panel pn, m;
        bool flag, kk, z = true;

        Graphics grh1;
        private void Pnz_Click(object sender, MouseEventArgs e)
        {

            if (kk == false)
            {
                flag = false;
                if (e.Button == MouseButtons.Right)
                {
                    if (count > 1)
                    {

                        Panelz[count - 1].Location = new Point(Panelz[count - 1].Location.X + e.X, Panelz[count - 1].Location.Y + e.Y);
                        count = 0;
                        flag = true;
                    }
                }

                if (flag == false)
                    if (e.Button == MouseButtons.Right)
                    {

                        pn = sender as Panel;
                        
                        count = 0;
                        foreach (Panel px in Panelz)
                        {
                            count += 1;
                            if (px == pn)
                            {

                                //label1.Text = Convert.ToString(count - 1);
                                break;
                            }

                        }
                        if (Data[count - 1] == 5)
                            dsp(pn);
                    }
            }

            int b = 0;
            if ((kk == true) && (z == true))
            {
                kk = false;
                
                if (cur1 == src(sender as Panel) - 1)
                {
                    string l = "P1 ";
                    for (i = 1; i < p1; i++)
                    {
                        l = l + Convert.ToString(Path1[i]) + " ";
                    }
                    if (e.Button == MouseButtons.Left)
                    {
                        Path1[0] = -1;
                        grh1.DrawLine(Pens.Azure, Path1[p1 - 2], Path1[p1 - 1], m.Location.X + m.Width / 2, m.Location.Y + m.Height / 2);
                    }
                    else
                        if (e.Button == MouseButtons.Right)
                            Path1[0] = -2;
                    MessageBox.Show(l);
                } else
                    if (cur2 == src(sender as Panel) - 1)
                    {
                        
                        string l = "P2 ";
                        for (i = 1; i < p2; i++)
                        {
                            l = l + Convert.ToString(Path2[i]) + " ";
                        }
                     
                        if (e.Button == MouseButtons.Left)
                        {
                            Path2[0] = -1;
                            grh1.DrawLine(Pens.Azure, Path2[p2 - 2], Path2[p2 - 1], m.Location.X + m.Width / 2, m.Location.Y + m.Height / 2);
                        }
                        else
                            if (e.Button == MouseButtons.Right)
                                Path2[0] = -2;
                        MessageBox.Show(l);
                    }
            } else
            if ((Data[src(sender as Panel) - 1] == 5) & (e.Button == MouseButtons.Left))
            {
                m = sender as Panel;
                
                dsp(m);
                kk = true;

                z = false;
                //if (b == cur1)
                    //MessageBox.Show("Accepted" + " " + Convert.ToString(src(m) - 1) + " " + Convert.ToString(b) + " " + "cur1");
                //else
                //if (b == cur2)
                    //MessageBox.Show("Accepted" + " " + Convert.ToString(src(m) - 1) + " " + Convert.ToString(b) + " " + "cur2");
            }

           
        }



        private void onPaint(object sender, PaintEventArgs e)
        {
            string s = (sender as Panel).Name;
            //MessageBox.Show(s);
            Rectangle r = new Rectangle(1, 1, Panelz[Convert.ToInt32(s)].Width - 5, Panelz[Convert.ToInt32(s)].Height - 5);
            Color nx = Color.White;
            k = Data[Convert.ToInt32(s)];
            switch (k)
            {
                case 1:
                    nx = Color.Red;
                    break;
                case 2:
                    nx = Color.Yellow;
                    break;
                case 3:
                    nx = Color.DarkOliveGreen;
                    break;
                case 4:
                    nx = Color.Blue;
                    break;
                case 5:
                    nx = Color.Violet;
                    break;
                case 6:
                    nx = Color.LimeGreen;
                    break;
            }
            Pen pajn = new Pen(nx);
            if (k <= 3)
                e.Graphics.DrawRectangle(pajn, r);
            else
                e.Graphics.DrawEllipse(pajn, r);
        }
        public string filename = "";
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();
            f5.Owner = this;
            f5.ShowDialog();
            if (filename != string.Empty)
            {
                System.IO.StreamWriter fs = new System.IO.StreamWriter(filename);
                string pt;
                for (int a = 1; a <= 6; a++)
                {
                    //pt = Convert.ToString(order[a]) + " ";
                    pt = "";
                    for (int v = 1; v <= cnt; v++)
                    {
                        if (Data[v] == order[a])
                        {
                            if ((order[a] == 4) || (order[a] == 6))
                                pt = pt + Convert.ToString(Panelz[v].Location.X) + " " + Convert.ToString(Panelz[v].Location.Y - menuStrip1.Height) + " ";
                            else
                                if (order[a] != 5)
                                    pt = pt + Convert.ToString(Panelz[v].Location.X) + " " + Convert.ToString(Panelz[v].Location.Y - menuStrip1.Height) + " " + Convert.ToString(Panelz[v].Width) + " " + Convert.ToString(Panelz[v].Height) + " ";
                                else
                                {
                                    if (v == cur1)
                                    {
                                        if (Path1[0] == -1)
                                            pt = pt + Convert.ToString(Panelz[v].Location.X) + " " + Convert.ToString(Panelz[v].Location.Y - menuStrip1.Height) + " " + Convert.ToString((p1 - 1) / 2 + 2) + " ";
                                        else
                                            if (Path1[0] == -2)
                                            {
                                                pt = pt + Convert.ToString(Panelz[v].Location.X) + " " + Convert.ToString(Panelz[v].Location.Y - menuStrip1.Height) + " " + Convert.ToString(p1) + " ";
                                            }
                                        pt = pt + Convert.ToString(rad1) + " ";
                                    }
                                    if (v == cur2)
                                    {
                                        if (Path2[0] == -1)
                                            pt = pt + Convert.ToString(Panelz[v].Location.X) + " " + Convert.ToString(Panelz[v].Location.Y - menuStrip1.Height) + " " + Convert.ToString((p2 - 1) / 2 + 2) + " ";
                                        else
                                            pt = pt + Convert.ToString(Panelz[v].Location.X) + " " + Convert.ToString(Panelz[v].Location.Y - menuStrip1.Height) + " " + Convert.ToString(p2) + " ";
                                        pt = pt + Convert.ToString(rad2) + " ";
                                    }

                                }
                            //MessageBox.Show(Convert.ToString(v) + " " +  Convert.ToString(Data[v]) + " " + Convert.ToString(a));
                        }
                    }
                    if (order[a] == 2)
                    {
                        pt = pt + Convert.ToString(0) + " " + Convert.ToString(0) + " " + Convert.ToString(1) + " " + Convert.ToString(this.Height - menuStrip1.Height) + " ";
                        pt = pt + Convert.ToString(1) + " " + Convert.ToString(0) + " " + Convert.ToString(this.Width - 1) + " " + Convert.ToString(1) + " ";
                        pt = pt + Convert.ToString(this.Width - 1) + " " + Convert.ToString(1) + " " + Convert.ToString(1) + " " + Convert.ToString(this.Height - menuStrip1.Height - 1) + " ";
                        pt = pt + Convert.ToString(1) + " " + Convert.ToString(this.Height - menuStrip1.Height - 2) + " " + Convert.ToString(this.Width - 2) + " " + Convert.ToString(1) + " ";
                    }
                    //MessageBox.Show(pt);
                    fs.WriteLine(pt);
                }
                pt = "";
                if (cur1 != 0)
                    pt = pt + Convert.ToString(Panelz[cur1].Location.X) + " " + Convert.ToString(Panelz[cur1].Location.Y - menuStrip1.Height) + " ";
                for (int a = 1; a < p1; a++)
                {
                    if (a % 2 == 0)
                        pt = pt + Convert.ToString(Path1[a] - menuStrip1.Height) + " ";
                    else
                        pt = pt + Convert.ToString(Path1[a]) + " ";
                }
                if (Path1[0] == -1)
                {
                    pt = pt + Convert.ToString(Panelz[cur1].Location.X) + " " + Convert.ToString(Panelz[cur1].Location.Y - menuStrip1.Height) + " ";
                }
                else
                    if (Path1[0] == -2)
                    {
                        int a = p1 - 3;
                        while (a > 0)
                        {
                            pt = pt + Convert.ToString(Path1[a - 1]) + " " + Convert.ToString(Path1[a] - menuStrip1.Height) + " ";
                            a -= 2;
                        }
                        pt = pt + Convert.ToString(Panelz[cur1].Location.X) + " " + Convert.ToString(Panelz[cur1].Location.Y - menuStrip1.Height) + " ";
                    }

                if (pt != "0 ")
                    fs.WriteLine(pt);
                else
                    fs.WriteLine();

                pt = "";
                if (cur2 != 0)
                    pt = pt + Convert.ToString(Panelz[cur2].Location.X) + " " + Convert.ToString(Panelz[cur2].Location.Y - menuStrip1.Height) + " ";
                for (int a = 1; a < p2; a++)
                {
                    if (a % 2 == 0)
                        pt = pt + Convert.ToString(Path2[a] - menuStrip1.Height) + " ";
                    else
                        pt = pt + Convert.ToString(Path2[a]) + " ";
                }

                if (Path2[0] == -1)
                {
                    pt = pt + Convert.ToString(Panelz[cur2].Location.X) + " " + Convert.ToString(Panelz[cur2].Location.Y - menuStrip1.Height) + " ";
                }
                else
                    if (Path2[0] == -2)
                    {
                        int a = p2 - 3;
                        while (a > 0)
                        {
                            pt = pt + Convert.ToString(Path2[a - 1]) + " " + Convert.ToString(Path2[a] - menuStrip1.Height) + " ";
                            a -= 2;
                        }
                        pt = pt + Convert.ToString(Panelz[cur2].Location.X) + " " + Convert.ToString(Panelz[cur2].Location.Y - menuStrip1.Height) + " ";
                    }

                if (pt != "0 ")
                    fs.WriteLine(pt);
                else
                    fs.WriteLine();

                MessageBox.Show("Success!");
                fs.Close();
            }
            else
                MessageBox.Show("No file - no map.");

            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        

        

        
    }
}
