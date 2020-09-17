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
                // Create error string
                StringBuilder errorStrBuilder = new StringBuilder();
                if (!cadComponent.NameOk())
                    errorStrBuilder.Append("Naming violation, ");
                if (!cadComponent.HasInfo)
                    errorStrBuilder.Append("Missing info file, ");
                if (cadComponent.MissingComponentError)
                    errorStrBuilder.Append("Referenced components not found, ");
                if (cadComponent.LoadingError)
                    errorStrBuilder.Append("Error loading info file, ");
                if (errorStrBuilder.Length <= 0)
                    continue; // No errors
                errorStrBuilder.Length -= 2; // Trim trailing comma and space
                string errorStr = errorStrBuilder.ToString();

                // Set error icons
                // TODO: Combine this with the similar CadComponent method
                if (!cadComponent.NameOk())
                {
                    if (cadComponent.MissingComponentError || cadComponent.LoadingError)
                        AddWarning(cadComponent, errorStr, "WarningErrorFile");
                    else if (!cadComponent.HasInfo)
                        AddWarning(cadComponent, errorStr, "WarningQuestionFile");
                    else
                        AddWarning(cadComponent, errorStr, "WarningFile");
                }
                else if (cadComponent.MissingComponentError || cadComponent.LoadingError)
                    AddWarning(cadComponent, errorStr, "ErrorFile");
                else if (!cadComponent.HasInfo)
                    AddWarning(cadComponent, errorStr, "QuestionFile");
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
