using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Black_N_White
{
    public partial class MainForm : Form
    {
        private static MainForm mForm;
        public MainForm()
        {
            InitializeComponent();
            Plate p = new Plate();
            p.Location = new Point(50, 50);
            p.Visible = true;
            this.Controls.Add(p);
            mForm = this;
            setLabel("White:" + 2 + "      Black:" + 2);

        }

        public static MainForm getInstance()
        {
            return mForm;
        }

        public void setLabel(String s)
        {
            this.BlackNWhiteLabel.Text = s;
        }
    }
}
