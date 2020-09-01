namespace RocketcadManager
{
    partial class WarningsListForm
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
            this.listViewWarnings = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderError = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listViewWarnings
            // 
            this.listViewWarnings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderError,
            this.columnHeaderFile});
            this.listViewWarnings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewWarnings.HideSelection = false;
            this.listViewWarnings.Location = new System.Drawing.Point(0, 0);
            this.listViewWarnings.Name = "listViewWarnings";
            this.listViewWarnings.Size = new System.Drawing.Size(800, 450);
            this.listViewWarnings.TabIndex = 0;
            this.listViewWarnings.UseCompatibleStateImageBehavior = false;
            this.listViewWarnings.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 40;
            // 
            // columnHeaderError
            // 
            this.columnHeaderError.Text = "Error";
            this.columnHeaderError.Width = 40;
            // 
            // columnHeaderFile
            // 
            this.columnHeaderFile.Text = "File";
            this.columnHeaderFile.Width = 30;
            // 
            // WarningsListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listViewWarnings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "WarningsListForm";
            this.ShowInTaskbar = false;
            this.Text = "Warnings and Errors";
            this.Load += new System.EventHandler(this.WarningsListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewWarnings;
        private System.Windows.Forms.ColumnHeader columnHeaderFile;
        private System.Windows.Forms.ColumnHeader columnHeaderError;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
    }
}
