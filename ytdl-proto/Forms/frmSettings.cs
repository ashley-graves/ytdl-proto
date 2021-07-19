using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YTDL.Classes;

namespace YTDL.Forms {
    public partial class frmSettings : Form {
        public frmSettings() {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e) {
            propertyGrid1.SelectedObject = Globals.options;
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e) {
            SolidBrush _Brush = new SolidBrush(Color.Black);//monochrome brush
            RectangleF _TabTextArea = tabControl1.GetTabRect(e.Index);//Drawing area
            StringFormat _sf = new StringFormat();//Package text layout format information
            _sf.LineAlignment = StringAlignment.Center;
            _sf.Alignment = StringAlignment.Center;
            if(tabControl1.ImageList.Images.Count > e.Index)
                tabControl1.ImageList.Draw(e.Graphics, new Point(16, (int)Math.Floor(_TabTextArea.Top + 16)), e.Index);
            else
                e.Graphics.DrawString(tabControl1.Controls[e.Index].Text, SystemInformation.MenuFont, _Brush, _TabTextArea, _sf);
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e) {
            Globals.options.WriteConfigFile(Globals.configPath);
        }
    }
}
