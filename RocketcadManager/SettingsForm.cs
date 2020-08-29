using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    public partial class SettingsForm : Form
    {
        private Config config;

        public SettingsForm(Config config)
        {
            this.config = config;
            InitializeComponent();
            StringBuilder sb = new StringBuilder();
            foreach (string str in config.CadDirectories)
            {
                sb.Append(str);
                sb.Append('\n');
            }
            textBoxCadDirectories.Text = sb.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Set config values here
            string[] strDirs = textBoxCadDirectories.Text.Split('\n');

            List<string> newCadDirs = new List<string>();
            foreach (string str in strDirs)
            {
                string dir = str.Trim(new char[] { ' ', '\"', '\n', '\r' });
                if (dir == "")
                    continue;
                if (Directory.Exists(dir))
                {
                    newCadDirs.Add(dir);
                }
                else
                {
                    MessageBox.Show("Invalid directory!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            config.CadDirectories = newCadDirs;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                DialogResult = DialogResult.Cancel;
        }
    }
}
