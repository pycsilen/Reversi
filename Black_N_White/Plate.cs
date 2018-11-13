using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Black_N_White;

namespace Black_N_White
{

    public partial class Plate : UserControl
    {
        public readonly Color m_White;
        public readonly Color m_Black;
        public static int tag;//1:white -1:black
        public Plate_Struct.UInt64Plate UInt64Plate;


        public Plate()
        {
            InitializeComponent();
            m_Black = Color.Black;
            m_White = Color.White;
            tag = -1;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            Color c = this.BackColor;

            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics gc = Graphics.FromImage(bmp);
            Pen p = new Pen(new SolidBrush(Color.Black), 1);
            for (int i = 0; i <= 10; i++)
            {
                gc.DrawLine(p, new Point(1 + i * 30, 1), new Point(1 + i * 30, 240));
                gc.DrawLine(p, new Point(1, 1 + i * 30), new Point(240, 1 + i * 30));
            }
            p.Width = 2;
            gc.DrawLine(p, new Point(0, 0), new Point(0, 240));
            gc.DrawLine(p, new Point(0, 0), new Point(240, 0));
            gc.DrawLine(p, new Point(240, 0), new Point(240, 240));
            gc.DrawLine(p, new Point(0, 240), new Point(240, 240));

            UInt64Plate = Plate_Struct.getPlate();
            drawPlate(gc, UInt64Plate);


            gc.Dispose();
            this.BackgroundImage = (Image)bmp.Clone();
        }

        private void drawPlate(Graphics gc, Plate_Struct.UInt64Plate Plate)
        {
            for (int y = 1; y <= 8; y++)
            {
                for (int x = 1; x <= 8; x++)
                {
                    if ((Plate.Mask[(y - 1) * 8 + (x - 1)] & Plate.plate) == 0)
                    {
                        continue;
                    }
                    if ((Plate.Mask[(y - 1) * 8 + (x - 1)] & Plate.white) != 0)
                    {
                        drawChess(gc, m_White, x, y);
                    }

                    if ((Plate.Mask[(y - 1) * 8 + (x - 1)] & Plate.black) != 0)
                    {
                        drawChess(gc, m_Black, x, y);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Plate"></param>
        /// <param name="currentColor"></param>
        /// <param name="x">行数1-8</param>
        /// <param name="y">列数1-8</param>
        /// <returns></returns>
        private Boolean setChess(ref Plate_Struct.UInt64Plate Plate, int currentColor, int x0, int y0, out List<int> setid)//-1:Black 1 White
        {
            int id0 = y0 * 8 + x0 - 9;//id
            UInt64 set;
            if (x0 <= 0 || y0 <= 0 || id0 < 0 || id0 > 63)
            {
                setid = new List<int>();
                //return false;
            }
            setid = Plate_Struct.getsetid(x0, y0, currentColor, Plate, out set);
            if (set == Plate.Mask[id0])
                return false;
            else
            {
                Plate = Plate_Struct.setPlate(Plate, currentColor, setid, set);
            }


            MainForm.getInstance().setLabel("White:" + Plate.whitecount + "      Black:" + Plate.blackcount);

            return true;
        }






        private void drawChess(Graphics gc, Color c, int x, int y)
        {
            gc.SmoothingMode = SmoothingMode.AntiAlias;
            gc.FillEllipse(new SolidBrush(c), new Rectangle((x - 1) * 30 + 1 + 3, (y - 1) * 30 + 1 + 3, 24, 24));
        }
        private void Plate_MouseUp(object sender, MouseEventArgs e)
        {
            if (tag == -1)
            {
                int x = e.X / 30 + 1;
                int y = e.Y / 30 + 1;

                List<int> setidList;

                List<int> cellList;
                if (!Plate_Struct.CheckPlate(tag, UInt64Plate, out cellList))
                {
                    tag *= -1;
                    return;
                }

                if (!setChess(ref UInt64Plate, tag, x, y, out setidList))
                {
                    return;
                }

                Refresh(tag, setidList);
                tag *= -1;
            }

            if (tag == 1)
            {
                List<int> cellList;
                if (Plate_Struct.CheckPlate(tag, UInt64Plate, out cellList))
                {
                    int idfromCaculate = Plate_Struct.CacuLate(UInt64Plate, -tag, 5);
                    int y1 = idfromCaculate / 8 + 1;
                    int x1 = idfromCaculate - y1 * 8 + 9;
                    List<int> setidList = null;
                    if (x1 <= 0 || y1 <= 0)
                    {
                        Console.WriteLine();
                    }
                    if (!setChess(ref UInt64Plate, tag, x1, y1, out setidList))
                        return;
                    Refresh(tag, setidList);
                    tag *= -1;
                }
                else
                {
                    tag *= -1;
                    return;
                }
            }

            if (UInt64Plate.count == 64)
                MessageBox.Show((UInt64Plate.whitecount > UInt64Plate.blackcount ? "Winner:White" : UInt64Plate.whitecount == UInt64Plate.blackcount ? "平局" : "Winner:Black"));

            //tag *= -1;

        }
        private void Refresh(int colorid, List<int> setidList)
        {
            Graphics gc = this.CreateGraphics();

            gc.SmoothingMode = SmoothingMode.AntiAlias;

            Color c = colorid == -1 ? Color.Black : Color.White;
            for (int i = 0; i < setidList.Count; i++)
            {
                int y = setidList[i] / 8 + 1;
                int x = setidList[i] - (y - 1) * 8 + 1;
                gc.FillEllipse(new SolidBrush(c), new Rectangle((x - 1) * 30 + 1 + 3, (y - 1) * 30 + 1 + 3, 24, 24));
            }
        }
    }
}
