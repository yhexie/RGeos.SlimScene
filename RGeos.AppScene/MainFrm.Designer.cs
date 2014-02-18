namespace RGeos.AppScene
{
    partial class MainFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tspOpenXModel = new System.Windows.Forms.ToolStripMenuItem();
            this.tspAddEarth = new System.Windows.Forms.ToolStripMenuItem();
            this.tspOpenTerrain = new System.Windows.Forms.ToolStripMenuItem();
            this.工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tspSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tspLoadDEM = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.工具ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(748, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspOpenXModel,
            this.tspAddEarth,
            this.tspOpenTerrain,
            this.tspLoadDEM});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // tspOpenXModel
            // 
            this.tspOpenXModel.Name = "tspOpenXModel";
            this.tspOpenXModel.Size = new System.Drawing.Size(152, 22);
            this.tspOpenXModel.Text = "加载模型(.X)";
            this.tspOpenXModel.Click += new System.EventHandler(this.tspOpenXModel_Click);
            // 
            // tspAddEarth
            // 
            this.tspAddEarth.Name = "tspAddEarth";
            this.tspAddEarth.Size = new System.Drawing.Size(152, 22);
            this.tspAddEarth.Text = "加载地球";
            this.tspAddEarth.Click += new System.EventHandler(this.tspOpenEarth_Click);
            // 
            // tspOpenTerrain
            // 
            this.tspOpenTerrain.Name = "tspOpenTerrain";
            this.tspOpenTerrain.Size = new System.Drawing.Size(152, 22);
            this.tspOpenTerrain.Text = "加载影像";
            this.tspOpenTerrain.Click += new System.EventHandler(this.tspTerrain_Click);
            // 
            // 工具ToolStripMenuItem
            // 
            this.工具ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspSelect});
            this.工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            this.工具ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.工具ToolStripMenuItem.Text = "工具";
            // 
            // tspSelect
            // 
            this.tspSelect.Name = "tspSelect";
            this.tspSelect.Size = new System.Drawing.Size(100, 22);
            this.tspSelect.Text = "选择";
            this.tspSelect.Click += new System.EventHandler(this.tspSelect_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(748, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(160, 403);
            this.treeView1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(160, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(588, 403);
            this.panel1.TabIndex = 4;
            // 
            // tspLoadDEM
            // 
            this.tspLoadDEM.Name = "tspLoadDEM";
            this.tspLoadDEM.Size = new System.Drawing.Size(152, 22);
            this.tspLoadDEM.Text = "加载DEM";
            this.tspLoadDEM.Click += new System.EventHandler(this.tspLoadDEM_Click);
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainFrm";
            this.Text = "主程序";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem tspOpenXModel;
        private System.Windows.Forms.ToolStripMenuItem tspAddEarth;
        private System.Windows.Forms.ToolStripMenuItem tspOpenTerrain;
        private System.Windows.Forms.ToolStripMenuItem 工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tspSelect;
        private System.Windows.Forms.ToolStripMenuItem tspLoadDEM;
    }
}

