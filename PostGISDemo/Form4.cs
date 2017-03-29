using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MyGIS;

namespace PostGISDemo
{
    public partial class Form4 : Form
    {
        List<string> tables;
        Form2 mapwindow;
        FPostGIS pg;

        public Form4(List<string> _tables,Form2 _mapwindow,FPostGIS _pg)
        {
            tables = _tables;
            mapwindow = _mapwindow;
            pg = _pg;
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            foreach (string s in tables)
                listBox1.Items.Add(s);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
