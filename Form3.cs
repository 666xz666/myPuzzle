using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myPuzzle
{
    public partial class Form3 : Form
    {

        private string _filepath;
        public static System.Drawing.Bitmap TBScaleBitmap(System.Drawing.Bitmap bitmap, int w, int h, string mode)
        {
            System.Drawing.Bitmap map = new System.Drawing.Bitmap(w, h);
            System.Drawing.Graphics gra = System.Drawing.Graphics.FromImage(map);
            gra.Clear(System.Drawing.Color.Transparent);//清空画布并以透明背景色填充
            gra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //使绘图质量最高，即消除锯齿
            gra.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gra.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gra.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int towidth = w;
            int toheight = h;

            int x = 0;
            int y = 0;
            int ow = bitmap.Width;
            int oh = bitmap.Height;



            switch (mode)
            {
                case "HW":  //指定高宽缩放（可能变形）                
                    break;
                case "W":   //指定宽，高按比例                    
                    toheight = bitmap.Height * w / bitmap.Width;
                    break;
                case "H":   //指定高，宽按比例
                    towidth = bitmap.Width * h / bitmap.Height;
                    break;
                case "Cut": //指定高宽裁减（不变形）                
                    if ((double)bitmap.Width / (double)bitmap.Height > (double)towidth / (double)toheight)
                    {
                        oh = bitmap.Height;
                        ow = bitmap.Height * towidth / toheight;
                        y = 0;
                        x = (bitmap.Width - ow) / 2;
                    }
                    else
                    {
                        ow = bitmap.Width;
                        oh = bitmap.Width * h / towidth;
                        x = 0;
                        y = (bitmap.Height - oh) / 2;
                    }
                    break;
                case "MaxHW"://最大宽高比例缩放，比如原100*50->50*30，则结果是50*25
                    var rmaxhw_d1w = bitmap.Width * 1.0 / w;
                    var rmaxhw_d2h = bitmap.Height * 1.0 / h;
                    if (rmaxhw_d1w > rmaxhw_d2h)
                    {
                        if (rmaxhw_d1w <= 1)
                        {
                            towidth = bitmap.Width; h = bitmap.Height;
                            goto case "HW";
                        }
                        towidth = w;
                        goto case "W";
                    }
                    if (rmaxhw_d2h <= 1)
                    {
                        towidth = bitmap.Width; h = bitmap.Height;
                        goto case "HW";
                    }
                    toheight = h;
                    goto case "H";
                default:
                    break;
            }

            gra.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

            gra.Flush();
            gra.Dispose();
            bitmap.Dispose();
            return map;
        }
        public Form3(string filepath)
        {
            InitializeComponent();
            _filepath = filepath;
            Bitmap img = new Bitmap(filepath);
            double w = 400;
            double h = 400;
            if (img.Width > img.Height)
            {
                w = 600;
                h = 600 * img.Height / img.Width;
            }
            else
            {
                h = 579;
                w = 579 * img.Width / img.Height;
            }
            img = TBScaleBitmap(img, (int)w, (int)h, "CUT");
            this.Width= img.Width;
            this.Height= img.Height;
            // 创建一个新的PictureBox
            PictureBox pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                ImageLocation = _filepath,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            
            // 将PictureBox添加到窗口中
            this.Controls.Add(pictureBox);
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
