using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    partial class WarningsListForm : Form
    {
        private List<CadComponent> cadComponents;

        public WarningsListForm(List<CadComponent> cadComponents, ImageList imageList)
        {
            this.cadComponents = cadComponents;
            InitializeComponent();
            listViewWarnings.SmallImageList = imageList;
        }

        private void WarningsListForm_Load(object sender, EventArgs e)
        {
            foreach (CadComponent cadComponent in cadComponents)
            {
                if (!cadComponent.HasInfo)
                    AddWarning(cadComponent, "Missing info file", "QuestionFile");
                if (cadComponent.LoadingError)
                    AddWarning(cadComponent, "Error loading info file", "ErrorFile");
                if (!cadComponent.NameOk())
                    AddWarning(cadComponent, "Naming violation", "WarningFile");
                if (cadComponent.MissingComponentError)
                    AddWarning(cadComponent, "Referenced components not found", "ErrorFile");
            }
            foreach (ColumnHeader column in listViewWarnings.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }
        
        private void AddWarning(CadComponent cadComponent, string message, string imageKey)
        {
            string name = cadComponent.ComponentFileInfo.Name;
            string file = cadComponent.ComponentFileInfo.FullName;
            string[] entry = { name, message, file };

            ListViewItem listItem = new ListViewItem(entry);
            listItem.ImageKey = imageKey;
            listViewWarnings.Items.Add(listItem);
        }
    }
}
