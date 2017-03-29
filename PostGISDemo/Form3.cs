using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MyGIS;

namespace PostGISDemo
{
    public partial class Form3 : Form
    {

        List<string> tables;
        Form2 mapwindow;
        FPostGIS pg;

        public Form3(Form2 _mapwindow)
        {
            mapwindow = _mapwindow;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string conn = "SERVER=";
            conn += textBox4.Text.ToString();
            conn += (";DATABASE=" + textBox1.Text.ToString());
            conn += (";USER ID=" + textBox2.Text.ToString());
            conn += (";PASSWORD=" + textBox3.Text.ToString());
            pg = new FPostGIS(conn);
            if (pg.ConnnectOrNot())
            {
                tables = pg.GetTablesFromDB();
                foreach (string s in tables)
                    listBox1.Items.Add(s);
//                Form4 tablelist = new Form4(pg.GetTablesFromDB(), mapwindow, pg);
//                this.Close();
//                tablelist.ShowDialog();
            }
            else
            {
                MessageBox.Show("用户名或密码错误！");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show("请选择文件！");
            else
            {
                try
                {
                    mapwindow.layer = pg.GetDataLayer(listBox1.Items[listBox1.SelectedIndex].ToString());
                    mapwindow.view.MyMapExtent.CopyFrom(mapwindow.layer.Extent);
                    mapwindow.DrawMap();
                    mapwindow.BringToFront();
                    this.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }

        }
    }
}
