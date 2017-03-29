using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using MyGIS;

namespace PostGISDemo
{
    public partial class Form1 : Form
    {
        FLayer layer;
        Form2 mapwindow;

        public Form1(FLayer _layer, Form2 _mapwindow)
        {
            InitializeComponent();
            layer = _layer;
            mapwindow = _mapwindow;
        }

        public void FillValue(FLayer layer)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("InternalID", "InternalID");
            dataGridView1.Columns[0].Visible = false;
            for (int i = 0; i < layer.Fields.Count; i++)
            {
                dataGridView1.Columns.Add(layer.Fields[i].name, layer.Fields[i].name);
            }
            for (int i = 0; i < layer.Features.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = i;
                for (int j = 0; j < layer.Fields.Count; j++)
                {
                    dataGridView1.Rows[i].Cells[j + 1].Value = layer.Features[i].getAttributeValue(j);
                }
                dataGridView1.Rows[i].Selected = layer.Features[i].selected;
            }
        }

        public void UpdataSelection()
        {
            FillValue(layer);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            FillValue(layer);
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            layer.ClearSelection();
            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                layer.Features[(int)(dataGridView1.SelectedRows[i].Cells[0].Value)].selected = true;
            mapwindow.DrawMap();
        }
    }

}