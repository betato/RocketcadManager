namespace RocketcadManager
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.fileView = new System.Windows.Forms.TreeView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripOpenFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripWarnings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSettings = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listViewContainer = new System.Windows.Forms.SplitContainer();
            this.listViewDependants = new System.Windows.Forms.ListView();
            this.col1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewDependancies = new System.Windows.Forms.ListView();
            this.col3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxStock = new System.Windows.Forms.GroupBox();
            this.numericStock = new System.Windows.Forms.NumericUpDown();
            this.groupBoxRequired = new System.Windows.Forms.GroupBox();
            this.numericRequiredAssembly = new System.Windows.Forms.NumericUpDown();
            this.numericRequiredAdditional = new System.Windows.Forms.NumericUpDown();
            this.labelTotal = new System.Windows.Forms.Label();
            this.numericRequiredTotal = new System.Windows.Forms.NumericUpDown();
            this.LabelAdditional = new System.Windows.Forms.Label();
            this.labelAssemblies = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxNotes = new System.Windows.Forms.TextBox();
            this.LabelDescription = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openContainingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listViewContainer)).BeginInit();
            this.listViewContainer.Panel1.SuspendLayout();
            this.listViewContainer.Panel2.SuspendLayout();
            this.listViewContainer.SuspendLayout();
            this.groupBoxStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericStock)).BeginInit();
            this.groupBoxRequired.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRequiredAssembly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRequiredAdditional)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRequiredTotal)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileView
            // 
            this.fileView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileView.HideSelection = false;
            this.fileView.Location = new System.Drawing.Point(0, 0);
            this.fileView.Name = "fileView";
            this.fileView.Size = new System.Drawing.Size(354, 479);
            this.fileView.TabIndex = 0;
            this.fileView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileView_AfterSelect);
            this.fileView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fileView_MouseDown);
            this.fileView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fileView_MouseUp);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel1,
            this.toolStripProgressBar1,
            this.statusLabel2});
            this.statusStrip.Location = new System.Drawing.Point(0, 504);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1064, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel1
            // 
            this.statusLabel1.Name = "statusLabel1";
            this.statusLabel1.Size = new System.Drawing.Size(83, 17);
            this.statusLabel1.Text = "Loading files...";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripOpen,
            this.toolStripOpenFolder,
            this.toolStripSeparator2,
            this.toolStripRefresh,
            this.toolStripWarnings,
            this.toolStripSeparator1,
            this.toolStripSettings});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1064, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripOpen
            // 
            this.toolStripOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripOpen.Enabled = false;
            this.toolStripOpen.Image = global::RocketcadManager.Icons.OpenFile;
            this.toolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpen.Name = "toolStripOpen";
            this.toolStripOpen.Size = new System.Drawing.Size(23, 22);
            this.toolStripOpen.Text = "Open file";
            this.toolStripOpen.Click += new System.EventHandler(this.toolStripOpen_Click);
            // 
            // toolStripOpenFolder
            // 
            this.toolStripOpenFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripOpenFolder.Image = global::RocketcadManager.Icons.OpenFolder;
            this.toolStripOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpenFolder.Name = "toolStripOpenFolder";
            this.toolStripOpenFolder.Size = new System.Drawing.Size(23, 22);
            this.toolStripOpenFolder.Text = "Open Containing Folder";
            this.toolStripOpenFolder.Click += new System.EventHandler(this.toolStripOpenFolder_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripRefresh
            // 
            this.toolStripRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripRefresh.Image")));
            this.toolStripRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRefresh.Name = "toolStripRefresh";
            this.toolStripRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripRefresh.Text = "Refresh";
            this.toolStripRefresh.Click += new System.EventHandler(this.toolStripRefresh_Click);
            // 
            // toolStripWarnings
            // 
            this.toolStripWarnings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripWarnings.Image = global::RocketcadManager.Icons.Warnings;
            this.toolStripWarnings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripWarnings.Name = "toolStripWarnings";
            this.toolStripWarnings.Size = new System.Drawing.Size(23, 22);
            this.toolStripWarnings.Text = "Warnings and Errors";
            this.toolStripWarnings.Click += new System.EventHandler(this.toolStripWarnings_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSettings
            // 
            this.toolStripSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSettings.Image = global::RocketcadManager.Icons.Settings;
            this.toolStripSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSettings.Name = "toolStripSettings";
            this.toolStripSettings.Size = new System.Drawing.Size(23, 22);
            this.toolStripSettings.Text = "Settings";
            this.toolStripSettings.Click += new System.EventHandler(this.toolStripSettings_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fileView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1064, 479);
            this.splitContainer1.SplitterDistance = 354;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listViewContainer);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBoxStock);
            this.splitContainer2.Panel2.Controls.Add(this.groupBoxRequired);
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Panel2.Controls.Add(this.LabelDescription);
            this.splitContainer2.Panel2.Controls.Add(this.textBoxDescription);
            this.splitContainer2.Size = new System.Drawing.Size(706, 479);
            this.splitContainer2.SplitterDistance = 312;
            this.splitContainer2.TabIndex = 6;
            // 
            // listViewContainer
            // 
            this.listViewContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewContainer.Location = new System.Drawing.Point(0, 0);
            this.listViewContainer.Name = "listViewContainer";
            this.listViewContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // listViewContainer.Panel1
            // 
            this.listViewContainer.Panel1.Controls.Add(this.listViewDependants);
            // 
            // listViewContainer.Panel2
            // 
            this.listViewContainer.Panel2.Controls.Add(this.listViewDependancies);
            this.listViewContainer.Size = new System.Drawing.Size(312, 479);
            this.listViewContainer.SplitterDistance = 214;
            this.listViewContainer.TabIndex = 0;
            // 
            // listViewDependants
            // 
            this.listViewDependants.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col1,
            this.col2});
            this.listViewDependants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDependants.HideSelection = false;
            this.listViewDependants.Location = new System.Drawing.Point(0, 0);
            this.listViewDependants.Name = "listViewDependants";
            this.listViewDependants.Size = new System.Drawing.Size(312, 214);
            this.listViewDependants.TabIndex = 0;
            this.listViewDependants.UseCompatibleStateImageBehavior = false;
            this.listViewDependants.View = System.Windows.Forms.View.Details;
            // 
            // col1
            // 
            this.col1.Text = "Used By";
            this.col1.Width = 100;
            // 
            // col2
            // 
            this.col2.Text = "Quantity";
            this.col2.Width = 100;
            // 
            // listViewDependancies
            // 
            this.listViewDependancies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col3,
            this.col4});
            this.listViewDependancies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDependancies.HideSelection = false;
            this.listViewDependancies.Location = new System.Drawing.Point(0, 0);
            this.listViewDependancies.Name = "listViewDependancies";
            this.listViewDependancies.Size = new System.Drawing.Size(312, 261);
            this.listViewDependancies.TabIndex = 1;
            this.listViewDependancies.UseCompatibleStateImageBehavior = false;
            this.listViewDependancies.View = System.Windows.Forms.View.Details;
            // 
            // col3
            // 
            this.col3.Text = "Uses";
            this.col3.Width = 100;
            // 
            // col4
            // 
            this.col4.Text = "Quantity";
            this.col4.Width = 100;
            // 
            // groupBoxStock
            // 
            this.groupBoxStock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxStock.Controls.Add(this.numericStock);
            this.groupBoxStock.Location = new System.Drawing.Point(2, 29);
            this.groupBoxStock.Name = "groupBoxStock";
            this.groupBoxStock.Size = new System.Drawing.Size(376, 49);
            this.groupBoxStock.TabIndex = 16;
            this.groupBoxStock.TabStop = false;
            this.groupBoxStock.Text = "In Stock";
            // 
            // numericStock
            // 
            this.numericStock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericStock.Location = new System.Drawing.Point(70, 19);
            this.numericStock.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericStock.Name = "numericStock";
            this.numericStock.Size = new System.Drawing.Size(300, 20);
            this.numericStock.TabIndex = 13;
            // 
            // groupBoxRequired
            // 
            this.groupBoxRequired.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRequired.Controls.Add(this.numericRequiredAssembly);
            this.groupBoxRequired.Controls.Add(this.numericRequiredAdditional);
            this.groupBoxRequired.Controls.Add(this.labelTotal);
            this.groupBoxRequired.Controls.Add(this.numericRequiredTotal);
            this.groupBoxRequired.Controls.Add(this.LabelAdditional);
            this.groupBoxRequired.Controls.Add(this.labelAssemblies);
            this.groupBoxRequired.Location = new System.Drawing.Point(2, 84);
            this.groupBoxRequired.Name = "groupBoxRequired";
            this.groupBoxRequired.Size = new System.Drawing.Size(376, 100);
            this.groupBoxRequired.TabIndex = 15;
            this.groupBoxRequired.TabStop = false;
            this.groupBoxRequired.Text = "Required";
            // 
            // numericRequiredAssembly
            // 
            this.numericRequiredAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericRequiredAssembly.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericRequiredAssembly.Location = new System.Drawing.Point(70, 19);
            this.numericRequiredAssembly.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numericRequiredAssembly.Name = "numericRequiredAssembly";
            this.numericRequiredAssembly.ReadOnly = true;
            this.numericRequiredAssembly.Size = new System.Drawing.Size(300, 20);
            this.numericRequiredAssembly.TabIndex = 10;
            // 
            // numericRequiredAdditional
            // 
            this.numericRequiredAdditional.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericRequiredAdditional.Location = new System.Drawing.Point(70, 45);
            this.numericRequiredAdditional.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericRequiredAdditional.Name = "numericRequiredAdditional";
            this.numericRequiredAdditional.Size = new System.Drawing.Size(300, 20);
            this.numericRequiredAdditional.TabIndex = 11;
            this.numericRequiredAdditional.ValueChanged += new System.EventHandler(this.numericRequiredAdditional_ValueChanged);
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(6, 73);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(31, 13);
            this.labelTotal.TabIndex = 14;
            this.labelTotal.Text = "Total";
            // 
            // numericRequiredTotal
            // 
            this.numericRequiredTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericRequiredTotal.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericRequiredTotal.Location = new System.Drawing.Point(70, 71);
            this.numericRequiredTotal.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numericRequiredTotal.Name = "numericRequiredTotal";
            this.numericRequiredTotal.ReadOnly = true;
            this.numericRequiredTotal.Size = new System.Drawing.Size(300, 20);
            this.numericRequiredTotal.TabIndex = 12;
            // 
            // LabelAdditional
            // 
            this.LabelAdditional.AutoSize = true;
            this.LabelAdditional.Location = new System.Drawing.Point(6, 47);
            this.LabelAdditional.Name = "LabelAdditional";
            this.LabelAdditional.Size = new System.Drawing.Size(53, 13);
            this.LabelAdditional.TabIndex = 8;
            this.LabelAdditional.Text = "Additional";
            // 
            // labelAssemblies
            // 
            this.labelAssemblies.AutoSize = true;
            this.labelAssemblies.Location = new System.Drawing.Point(6, 21);
            this.labelAssemblies.Name = "labelAssemblies";
            this.labelAssemblies.Size = new System.Drawing.Size(59, 13);
            this.labelAssemblies.TabIndex = 1;
            this.labelAssemblies.Text = "Assemblies";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(2, 190);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(385, 289);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(377, 263);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Thumbnail";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(371, 257);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBoxNotes);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(377, 263);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Notes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBoxNotes
            // 
            this.textBoxNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNotes.Location = new System.Drawing.Point(3, 3);
            this.textBoxNotes.Multiline = true;
            this.textBoxNotes.Name = "textBoxNotes";
            this.textBoxNotes.Size = new System.Drawing.Size(371, 257);
            this.textBoxNotes.TabIndex = 4;
            // 
            // LabelDescription
            // 
            this.LabelDescription.AutoSize = true;
            this.LabelDescription.Location = new System.Drawing.Point(6, 6);
            this.LabelDescription.Name = "LabelDescription";
            this.LabelDescription.Size = new System.Drawing.Size(60, 13);
            this.LabelDescription.TabIndex = 6;
            this.LabelDescription.Text = "Description";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(72, 3);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.Size = new System.Drawing.Size(300, 20);
            this.textBoxDescription.TabIndex = 7;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.openContainingFolderToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(202, 48);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Image = global::RocketcadManager.Icons.OpenFile;
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openFileToolStripMenuItem.Text = "Open File";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // openContainingFolderToolStripMenuItem
            // 
            this.openContainingFolderToolStripMenuItem.Image = global::RocketcadManager.Icons.OpenFolder;
            this.openContainingFolderToolStripMenuItem.Name = "openContainingFolderToolStripMenuItem";
            this.openContainingFolderToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openContainingFolderToolStripMenuItem.Text = "Open Containing Folder";
            this.openContainingFolderToolStripMenuItem.Click += new System.EventHandler(this.openContainingFolderToolStripMenuItem_Click);
            // 
            // statusLabel2
            // 
            this.statusLabel2.Name = "statusLabel2";
            this.statusLabel2.Size = new System.Drawing.Size(171, 17);
            this.statusLabel2.Text = "Waiting for addin connection...";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 526);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "RocketcadManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.listViewContainer.Panel1.ResumeLayout(false);
            this.listViewContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listViewContainer)).EndInit();
            this.listViewContainer.ResumeLayout(false);
            this.groupBoxStock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericStock)).EndInit();
            this.groupBoxRequired.ResumeLayout(false);
            this.groupBoxRequired.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRequiredAssembly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRequiredAdditional)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRequiredTotal)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView fileView;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel1;
        private System.Windows.Forms.Label labelAssemblies;
        private System.Windows.Forms.TextBox textBoxNotes;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer listViewContainer;
        private System.Windows.Forms.ListView listViewDependants;
        private System.Windows.Forms.ColumnHeader col1;
        private System.Windows.Forms.ColumnHeader col2;
        private System.Windows.Forms.ListView listViewDependancies;
        private System.Windows.Forms.ColumnHeader col4;
        private System.Windows.Forms.ColumnHeader col3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripButton toolStripSettings;
        private System.Windows.Forms.ToolStripButton toolStripWarnings;
        private System.Windows.Forms.ToolStripButton toolStripRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripOpen;
        private System.Windows.Forms.ToolStripButton toolStripOpenFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label LabelDescription;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openContainingFolderToolStripMenuItem;
        private System.Windows.Forms.Label LabelAdditional;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown numericRequiredAssembly;
        private System.Windows.Forms.NumericUpDown numericRequiredTotal;
        private System.Windows.Forms.NumericUpDown numericRequiredAdditional;
        private System.Windows.Forms.GroupBox groupBoxStock;
        private System.Windows.Forms.NumericUpDown numericStock;
        private System.Windows.Forms.GroupBox groupBoxRequired;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel2;
    }
}

