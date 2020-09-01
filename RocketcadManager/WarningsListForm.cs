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

        public WarningsListForm(List<CadComponent> cadComponents)
        {
            this.cadComponents = cadComponents;
            InitializeComponent();
        }

        private void WarningsListForm_Load(object sender, EventArgs e)
        {
            foreach (CadComponent cadComponent in cadComponents)
            {
                string name = cadComponent.ComponentFileInfo.Name;
                string file = cadComponent.ComponentFileInfo.FullName;
                string[] entry = { name, null, file };
                if (!cadComponent.HasInfo)
                {
                    entry[1] = "Missing info file";
                    listViewWarnings.Items.Add(new ListViewItem(entry));
                }
                if (cadComponent.LoadingError)
                {
                    entry[1] = "Error loading info file";
                    listViewWarnings.Items.Add(new ListViewItem(entry));
                }
                if (!cadComponent.NameOk())
                {
                    entry[1] = "Naming violation";
                    listViewWarnings.Items.Add(new ListViewItem(entry));
                }
            }
            foreach (ColumnHeader column in listViewWarnings.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }
    }
}
