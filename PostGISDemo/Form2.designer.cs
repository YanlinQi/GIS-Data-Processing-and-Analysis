namespace PostGISDemo
{
    partial class Form2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FullExtentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine();
            this.skinEngine2 = new Sunisoft.IrisSkin.SkinEngine();
            this.AddToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.缩放ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.放大ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.缩小ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.panel1 = new PostGISDemo.MyPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_zoomin = new System.Windows.Forms.ToolStripButton();
            this.tsb_zoomout = new System.Windows.Forms.ToolStripButton();
            this.tsb_pan = new System.Windows.Forms.ToolStripButton();
            this.tsb_arrow = new System.Windows.Forms.ToolStripButton();
            this.tsb_select = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 364);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(618, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SelectToolStripMenuItem,
            this.ZoomInToolStripMenuItem,
            this.ZoomOutToolStripMenuItem,
            this.PanToolStripMenuItem,
            this.FullExtentToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 114);
            // 
            // SelectToolStripMenuItem
            // 
            this.SelectToolStripMenuItem.Name = "SelectToolStripMenuItem";
            this.SelectToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.SelectToolStripMenuItem.Text = "选择";
            this.SelectToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // ZoomInToolStripMenuItem
            // 
            this.ZoomInToolStripMenuItem.Name = "ZoomInToolStripMenuItem";
            this.ZoomInToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.ZoomInToolStripMenuItem.Text = "放大";
            this.ZoomInToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // ZoomOutToolStripMenuItem
            // 
            this.ZoomOutToolStripMenuItem.Name = "ZoomOutToolStripMenuItem";
            this.ZoomOutToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.ZoomOutToolStripMenuItem.Text = "缩小";
            this.ZoomOutToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // PanToolStripMenuItem
            // 
            this.PanToolStripMenuItem.Checked = true;
            this.PanToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PanToolStripMenuItem.Name = "PanToolStripMenuItem";
            this.PanToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.PanToolStripMenuItem.Text = "漫游";
            this.PanToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // FullExtentToolStripMenuItem
            // 
            this.FullExtentToolStripMenuItem.Name = "FullExtentToolStripMenuItem";
            this.FullExtentToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.FullExtentToolStripMenuItem.Text = "全图显示";
            this.FullExtentToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // skinEngine1
            // 
            this.skinEngine1.@__DrawButtonFocusRectangle = true;
            this.skinEngine1.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine1.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine1.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine1.SerialNumber = "";
            this.skinEngine1.SkinFile = null;
            // 
            // skinEngine2
            // 
            this.skinEngine2.@__DrawButtonFocusRectangle = true;
            this.skinEngine2.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine2.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine2.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine2.SerialNumber = "";
            this.skinEngine2.SkinFile = null;
            // 
            // AddToolStripMenuItem1
            // 
            this.AddToolStripMenuItem1.Name = "AddToolStripMenuItem1";
            this.AddToolStripMenuItem1.Size = new System.Drawing.Size(80, 21);
            this.AddToolStripMenuItem1.Text = "数据库导入";
            this.AddToolStripMenuItem1.Click += new System.EventHandler(this.AddToolStripMenuItem1_Click);
            // 
            // 缩放ToolStripMenuItem
            // 
            this.缩放ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.放大ToolStripMenuItem,
            this.缩小ToolStripMenuItem});
            this.缩放ToolStripMenuItem.Name = "缩放ToolStripMenuItem";
            this.缩放ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.缩放ToolStripMenuItem.Text = "地图操作";
            // 
            // 放大ToolStripMenuItem
            // 
            this.放大ToolStripMenuItem.Name = "放大ToolStripMenuItem";
            this.放大ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.放大ToolStripMenuItem.Text = "放大";
            this.放大ToolStripMenuItem.Click += new System.EventHandler(this.放大ToolStripMenuItem_Click);
            // 
            // 缩小ToolStripMenuItem
            // 
            this.缩小ToolStripMenuItem.Name = "缩小ToolStripMenuItem";
            this.缩小ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.缩小ToolStripMenuItem.Text = "缩小";
            this.缩小ToolStripMenuItem.Click += new System.EventHandler(this.缩小ToolStripMenuItem_Click);
            // 
            // RemoveToolStripMenuItem
            // 
            this.RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem";
            this.RemoveToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.RemoveToolStripMenuItem.Text = "移除图层";
            this.RemoveToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddToolStripMenuItem1,
            this.缩放ToolStripMenuItem,
            this.RemoveToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(618, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.AliceBlue;
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(618, 339);
            this.panel1.TabIndex = 1;
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.panel1.MouseHover += new System.EventHandler(this.panel1_MouseHover);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_zoomin,
            this.tsb_zoomout,
            this.tsb_pan,
            this.tsb_arrow,
            this.tsb_select});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(618, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_zoomin
            // 
            this.tsb_zoomin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_zoomin.Image = ((System.Drawing.Image)(resources.GetObject("tsb_zoomin.Image")));
            this.tsb_zoomin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_zoomin.Name = "tsb_zoomin";
            this.tsb_zoomin.Size = new System.Drawing.Size(23, 22);
            this.tsb_zoomin.Text = "Zoom In";
            this.tsb_zoomin.Click += new System.EventHandler(this.tsb_zoomin_Click);
            // 
            // tsb_zoomout
            // 
            this.tsb_zoomout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_zoomout.Image = ((System.Drawing.Image)(resources.GetObject("tsb_zoomout.Image")));
            this.tsb_zoomout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_zoomout.Name = "tsb_zoomout";
            this.tsb_zoomout.Size = new System.Drawing.Size(23, 22);
            this.tsb_zoomout.Text = "Zoom out";
            this.tsb_zoomout.Click += new System.EventHandler(this.tsb_zoomout_Click);
            // 
            // tsb_pan
            // 
            this.tsb_pan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_pan.Image = ((System.Drawing.Image)(resources.GetObject("tsb_pan.Image")));
            this.tsb_pan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_pan.Name = "tsb_pan";
            this.tsb_pan.Size = new System.Drawing.Size(23, 22);
            this.tsb_pan.Text = "roaming";
            this.tsb_pan.Click += new System.EventHandler(this.tsb_pan_Click);
            // 
            // tsb_arrow
            // 
            this.tsb_arrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_arrow.Image = ((System.Drawing.Image)(resources.GetObject("tsb_arrow.Image")));
            this.tsb_arrow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_arrow.Name = "tsb_arrow";
            this.tsb_arrow.Size = new System.Drawing.Size(23, 22);
            this.tsb_arrow.Click += new System.EventHandler(this.tsb_arrow_Click);
            // 
            // tsb_select
            // 
            this.tsb_select.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_select.Image = ((System.Drawing.Image)(resources.GetObject("tsb_select.Image")));
            this.tsb_select.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_select.Name = "tsb_select";
            this.tsb_select.Size = new System.Drawing.Size(23, 22);
            this.tsb_select.Text = "拖框选择";
            this.tsb_select.Click += new System.EventHandler(this.tsb_select_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 386);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form2";
            this.Text = "GIS小程序";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem SelectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FullExtentToolStripMenuItem;
        //private System.Windows.Forms.Panel panel1;
        private MyPanel panel1;
        private Sunisoft.IrisSkin.SkinEngine skinEngine1;
        private Sunisoft.IrisSkin.SkinEngine skinEngine2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_zoomout;
        private System.Windows.Forms.ToolStripButton tsb_zoomin;
        private System.Windows.Forms.ToolStripButton tsb_pan;
        private System.Windows.Forms.ToolStripButton tsb_select;
        private System.Windows.Forms.ToolStripButton tsb_arrow;
        private System.Windows.Forms.ToolStripMenuItem AddToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 缩放ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 放大ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 缩小ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }

    public class MyPanel : System.Windows.Forms.Panel
    {
        public MyPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}