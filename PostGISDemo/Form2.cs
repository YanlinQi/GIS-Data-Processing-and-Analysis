using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyGIS;

namespace PostGISDemo
{
    public partial class Form2 : Form
    {
        public FView view = null;
        private Bitmap backwindow;
        public FLayer layer = null;
        private MOUSECOMMAND MouseCommand = MOUSECOMMAND.Pan;
        private int MouseStartX = 0;
        private int MouseStartY = 0;
        private int MouseMovingX = 0;
        private int MouseMovingY = 0;
        bool isTsbPanClick = false;
        bool isTsbSelectClick = false;
        private Form1 attributeDialog = null;

        public int MouseNumber;

        public Form2()
        {
            MouseNumber = 0;
            InitializeComponent();
            //添加皮肤
            this.skinEngine1.SkinFile = "MSN.ssk";
            view = new FView(new FExtent(new FVertex(0, 0), new FVertex(1, 1)), panel1.ClientRectangle);
            ((Control)this).MouseWheel += new MouseEventHandler(panel1_MouseWheel);
        }


        void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (layer != null && panel1.Bounds.Contains(e.Location))
                {
                    if (e.Delta > 0)
                    {
                        FVertex mouselocation = view.ToMapVertex(new Point(e.X, e.Y));
                        double ZoomInfactor = 0.85;
                        double newwidth = view.MyMapExtent.width * ZoomInfactor;
                        double newheight = view.MyMapExtent.height * ZoomInfactor;
                        double newminx = mouselocation.getX() - (mouselocation.getX() - view.MyMapExtent.minX) * ZoomInfactor;
                        double newminy = mouselocation.getY() - (mouselocation.getY() - view.MyMapExtent.minY) * ZoomInfactor;
                        view.MyMapExtent.setValue(new FVertex(newminx + newwidth, newminy + newheight), new FVertex(newminx, newminy));
                        DrawMap();
                    }
                    else if (e.Delta < 0)
                    {
                        FVertex mouselocation = view.ToMapVertex(new Point(e.X, e.Y));
                        double ZoomInfactor = 0.85;
                        double newwidth = view.MyMapExtent.width / ZoomInfactor;
                        double newheight = view.MyMapExtent.height / ZoomInfactor;
                        double newminx = mouselocation.getX() - (mouselocation.getX() - view.MyMapExtent.minX) / ZoomInfactor;
                        double newminy = mouselocation.getY() - (mouselocation.getY() - view.MyMapExtent.minY) / ZoomInfactor;
                        view.MyMapExtent.setValue(new FVertex(newminx + newwidth, newminy + newheight), new FVertex(newminx, newminy));
                        DrawMap();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this.PointToScreen(new Point(e.X, e.Y)));
            }
            else if (e.Button == MouseButtons.Left)
            {
                MouseStartX = e.X;
                MouseStartY = e.Y;
            }
        }

        private void AddToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form3 dbconn = new Form3(this);
            dbconn.ShowDialog();
        }

        public void DrawMap()
        {
            view.SetValue(view.MyMapExtent, panel1.ClientRectangle);
            if (backwindow != null)
                backwindow.Dispose();
            backwindow = new Bitmap(panel1.ClientRectangle.Width, panel1.ClientRectangle.Height);
            Graphics g = Graphics.FromImage(backwindow);
            g.FillRectangle(new SolidBrush(Color.AliceBlue), panel1.ClientRectangle);

            FVertex v1 = view.ToMapVertex(new Point(0, view.myWindowSize.Height - 1));
            FVertex v2 = view.ToMapVertex(new Point(view.myWindowSize.Width - 1, 0));
            FExtent displayextent = new FExtent(v2, v1);

            if (layer != null)
                layer.draw(g, view, displayextent);
            Graphics graphics = panel1.CreateGraphics();
            graphics.DrawImage(backwindow, 0, 0);
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            if (layer != null)
                DrawMap();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (backwindow != null)
            {
                if (MouseButtons == MouseButtons.Left && MouseCommand != MOUSECOMMAND.Unused)
                {
                    if (MouseCommand == MOUSECOMMAND.Pan)
                        e.Graphics.DrawImage(backwindow, MouseMovingX - MouseStartX, MouseMovingY - MouseStartY);
                    else
                    {
                        e.Graphics.DrawImage(backwindow, 0, 0);
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(40, 0, 0, 0)), new Rectangle(
                            Math.Min(MouseStartX, MouseMovingX), Math.Min(MouseStartY, MouseMovingY),
                            Math.Abs(MouseStartX - MouseMovingX), Math.Abs(MouseStartY - MouseMovingY)));
                    }
                }
                else
                    e.Graphics.DrawImage(backwindow, 0, 0);
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseCommand != MOUSECOMMAND.Unused)
            {
                MouseMovingX = e.X;
                MouseMovingY = e.Y;
                panel1.Invalidate();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (MouseCommand)
                {
                    case MOUSECOMMAND.Select:
                        {
                            if (layer == null)
                                break;
                            layer.ClearSelection();
                            if (e.X == MouseStartX && e.Y == MouseStartY)//点选
                            {
                                FFeature feature = layer.SelectByClick(new Point(e.X, e.Y), view);
                                if (feature != null)
                                    feature.selected = true;
                            }
                            else//框选
                            {
                                FExtent extent = view.RectToExtent(new Rectangle(
                                    Math.Min(e.X, MouseStartX), Math.Min(e.Y, MouseStartY),
                                    Math.Abs(e.X - MouseStartX), Math.Abs(e.Y - MouseStartY)));
                                List<FFeature> features = layer.SelectByExtent(extent);
                                for (int j = 0; j < features.Count; j++)
                                    features[j].selected = true;
                            }
                            DrawMap();
                            if (attributeDialog != null)
                                attributeDialog.UpdataSelection();
                            break;
                        }
                    case MOUSECOMMAND.ZoomIn:
                        {
                            if (e.X == MouseStartX && e.Y == MouseStartY)
                            {
                                FVertex mouselocation = view.ToMapVertex(new Point(e.X, e.Y));
                                double ZoomInfactor = 0.9;
                                double newwidth = view.MyMapExtent.width * ZoomInfactor;
                                double newheight = view.MyMapExtent.height * ZoomInfactor;
                                double newminx = mouselocation.getX() - (mouselocation.getX() - view.MyMapExtent.minX) * ZoomInfactor;
                                double newminy = mouselocation.getY() - (mouselocation.getY() - view.MyMapExtent.minY) * ZoomInfactor;
                                view.MyMapExtent.setValue(new FVertex(newminx + newwidth, newminy + newheight), new FVertex(newminx, newminy));
                            }
                            else
                            {
                                view.MyMapExtent = view.RectToExtent(new Rectangle(
                                    Math.Min(e.X, MouseStartX), Math.Min(e.Y, MouseStartY),
                                    Math.Abs(e.X - MouseStartX), Math.Abs(e.Y - MouseStartY)));
                            }
                            DrawMap();
                            break;
                        }
                    case MOUSECOMMAND.ZoomOut:
                        {
                            if (e.X == MouseStartX && e.Y == MouseStartY)
                            {
                                FVertex mouselocation = view.ToMapVertex(new Point(e.X, e.Y));
                                double ZoomInfactor = 0.9;
                                double newwidth = view.MyMapExtent.width / ZoomInfactor;
                                double newheight = view.MyMapExtent.height / ZoomInfactor;
                                double newminx = mouselocation.getX() - (mouselocation.getX() - view.MyMapExtent.minX) / ZoomInfactor;
                                double newminy = mouselocation.getY() - (mouselocation.getY() - view.MyMapExtent.minY) / ZoomInfactor;
                                view.MyMapExtent.setValue(new FVertex(newminx + newwidth, newminy + newheight), new FVertex(newminx, newminy));
                            }
                            else
                            {
                                FExtent extent = view.RectToExtent(new Rectangle(
                                    Math.Min(e.X, MouseStartX), Math.Min(e.Y, MouseStartY),
                                    Math.Abs(e.X - MouseStartX), Math.Abs(e.Y - MouseStartY)));
                                //新的地图范围
                                double newwidth = view.MyMapExtent.width * view.MyMapExtent.width / extent.width;
                                double newheight = view.MyMapExtent.height * view.MyMapExtent.height / extent.height;
                                double newminx = extent.minX - (extent.minX - view.MyMapExtent.minX) * newwidth / view.MyMapExtent.width;
                                double newminy = extent.minY - (extent.minY - view.MyMapExtent.minY) * newheight / view.MyMapExtent.height;
                                view.MyMapExtent.setValue(new FVertex(newminx + newwidth, newminy + newheight), new FVertex(newminx, newminy));
                            }
                            DrawMap();
                            break;
                        }
                    case MOUSECOMMAND.Pan:
                        {
                            FVertex C1 = view.MyMapExtent.mapcenter;
                            FVertex M1 = view.ToMapVertex(new Point(MouseStartX, MouseStartY));
                            FVertex M2 = view.ToMapVertex(new Point(e.X, e.Y));
                            FVertex C2 = new FVertex(C1.getX() - (M2.getX() - M1.getX()), C1.getY() - (M2.getY() - M1.getY()));
                            view.MyMapExtent.SetMapCenter(C2);
                            DrawMap();
                            break;
                        }
                }
            }
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender.Equals(FullExtentToolStripMenuItem))
            {
                if (layer != null)
                {
                    view.MyMapExtent.CopyFrom(layer.Extent);
                    DrawMap();
                }
            }
            else
            {
                SelectToolStripMenuItem.Checked = false;
                ZoomInToolStripMenuItem.Checked = false;
                ZoomOutToolStripMenuItem.Checked = false;
                PanToolStripMenuItem.Checked = false;
                ((ToolStripMenuItem)sender).Checked = true;
                if (sender.Equals(SelectToolStripMenuItem))
                    MouseCommand = MOUSECOMMAND.Select;
                else if (sender.Equals(ZoomInToolStripMenuItem))
                    MouseCommand = MOUSECOMMAND.ZoomIn;
                else if (sender.Equals(ZoomOutToolStripMenuItem))
                    MouseCommand = MOUSECOMMAND.ZoomOut;
                else if (sender.Equals(PanToolStripMenuItem))
                    MouseCommand = MOUSECOMMAND.Pan;
            }
     
        }

        private void AttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(layer!=null)
            {
                attributeDialog = new Form1(layer, this);
                attributeDialog.Show();
            }

        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (layer != null)
            {
                layer.ClearSelection();
                DrawMap();
                if (attributeDialog != null)
                    attributeDialog.UpdataSelection();
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            layer = null;
            if (attributeDialog != null)
            {
                attributeDialog.Close();
                attributeDialog = null;
            }
            backwindow = null;
            Graphics graphics = panel1.CreateGraphics();
            graphics.Clear(Color.AliceBlue);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Shapefile文件|*.shp";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            layer = FShapefile.ReadShapeFile(openFileDialog.FileName);
            view.MyMapExtent.CopyFrom(layer.Extent);
            DrawMap();
        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.ZoomIn;
        }

        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.ZoomOut;
        }

        private void 地物选取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.Select;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void tsb_zoomout_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.ZoomOut;
        }

        private void tsb_zoomin_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.ZoomIn;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.Pan;
        }

        private void tsb_pan_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.Pan;
        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {
            if (MouseCommand ==MOUSECOMMAND.Pan)
            {
                this.Cursor = Cursors.Hand;
            }
                
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            if (MouseCommand == MOUSECOMMAND.Pan)
            {
                this.Cursor = Cursors.Arrow;
            }
        }


        private void tsb_select_Click(object sender, EventArgs e)
        {

                MouseCommand = MOUSECOMMAND.Select;
        }

        private void tsb_arrow_Click(object sender, EventArgs e)
        {
                if(MouseCommand == MOUSECOMMAND.Select)
                {
                    if (layer != null)
                    {
                        layer.ClearSelection();
                        DrawMap();
                        if (attributeDialog != null)
                            attributeDialog.UpdataSelection();
                }

            }
                this.Cursor = Cursors.Arrow;
                MouseCommand = MOUSECOMMAND.Unused;
        }


        private void stmI_select_Click(object sender, EventArgs e)
        {
            MouseCommand = MOUSECOMMAND.Select;
        }
    }
}
