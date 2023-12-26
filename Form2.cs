using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myPuzzle
{
    public partial class Form2 : Form
    {
        private string _filePath;
        private int _num;
        private Form _mainform;
        private Bitmap[,] pieces;
        private PictureBox[] Pieces;
        private PictureBox[] Finished;
        private System.Windows.Forms.Timer timer1;
        private int _option;
        public Form2(string filepath,int num, Form mainform, int option)
        {
            _mainform = mainform;
            InitializeComponent();
            _filePath = filepath;
            _num = (int)Math.Sqrt(num);
            this.FormClosing += Form1_FormClosing;//关闭绑定函数
            _option = option;
            init();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mainform.Show();
        }

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
        private void init()
        {
            //清理
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is PictureBox)
                {
                    this.Controls.RemoveAt(i);
                }
            }


            //切割
            Bitmap source = new Bitmap(_filePath);
            double w = 600;
            double h=579;
            if(source.Width> source.Height)
            {
                w = 600;
                h = 600 * source.Height / source.Width;
            }
            else
            {
                h = 579;
                w = 579 * source.Width / source.Height;
            }
            source = TBScaleBitmap(source, (int)w, (int)h, "CUT");
            int pieceWidth = source.Width / _num;
            int pieceHeight = source.Height / _num;
            pieces = new Bitmap[_num, _num];
            for (int i = 0; i < _num; i++)
            {
                for (int j = 0; j < _num; j++)
                {
                    Rectangle rect = new Rectangle(j * pieceWidth, i * pieceHeight, pieceWidth, pieceHeight);
                    pieces[i, j] = source.Clone(rect, source.PixelFormat);
                }
            }
            for (int i = 0; i < _num; i++)
            {
                for (int j = 0; j < _num; j++)
                {
                    Rectangle rect = new Rectangle(j * pieceWidth, i * pieceHeight, pieceWidth, pieceHeight);
                    pieces[i, j] = source.Clone(rect, source.PixelFormat);
                }
            }
            // pieces 是已经分割好的图片

            //打乱
            int rows = pieces.GetLength(0);
            int cols = pieces.GetLength(1);

            // 将二维数组转换为一维数组
            int n = 0;
            Bitmap[] flatArray = new Bitmap[rows * cols];
            Finished = new PictureBox[_num * _num];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    flatArray[i * cols + j] = pieces[i, j];
                    Finished[n] = new PictureBox();
                    Finished[n].Image = pieces[i, j];
                    Finished[n].Size = Finished[n].Image.Size;
                    Finished[n].Location = new Point(j * Finished[n].Width, i * Finished[n].Height);
                    n++;
                }
            }

            // 使用Fisher-Yates算法打乱一维数组
            Random rand = new Random();
           
            for (int i = flatArray.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                Bitmap temp = flatArray[i];
                flatArray[i] = flatArray[j];
                flatArray[j] = temp;
            }

            // 将一维数组转回为二维数组
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    pieces[i, j] = flatArray[i * cols + j];
                }
            }

            // 对于每个拼图块，创建一个 PictureBox 并添加到窗口中
            Pieces = new PictureBox[_num*_num];
            int pos = 0;
            for (int i = 0; i < pieces.GetLength(0); i++)
            {
                for (int j = 0; j < pieces.GetLength(1); j++)
                {
                    Pieces[pos] = new PictureBox();
                    Pieces[pos].Image = pieces[i, j];
                    Pieces[pos].Size = Pieces[pos].Image.Size;
                    Pieces[pos].Location = new Point(j * Pieces[pos].Width, i * Pieces[pos].Height);
                   
                    // 将 PictureBox 添加到窗口中
                    this.Controls.Add(Pieces[pos]);
                    pos++;
                }
            }

            //为每个picturebox添加鼠标属性
            foreach (var piece in Pieces)
            {
                piece.MouseDown += Piece_MouseDown;
                piece.DragEnter += Piece_DragEnter;
                piece.DragDrop += Piece_DragDrop;
                piece.AllowDrop = true;  // 允许拖放操作
            }

            textBox1.Text = "";
            if (_option == 1) { 
            // 设定倒计时的总时间
            int time = 30 * (_num-1);
            TimeSpan totalTime = new TimeSpan(0, 0, time);
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 1000; // 设置timer的间隔为1秒
            timer1.Tick += (s, e) =>
            {
                totalTime = totalTime.Add(TimeSpan.FromSeconds(-1)); // 每秒减少1秒
                label1.Text = totalTime.ToString(@"mm\:ss"); // 更新label的显示
                if (totalTime.TotalSeconds == 0) // 如果倒计时结束
                {
                    timer1.Stop(); // 停止timer
                    if (IsFinished())
                    {
                        label1.Text = "挑战成功！";
                    }
                    else
                    {
                        label1.Text = "挑战失败！";
                    }
                }
            };
            label1.Text = "开始计时！";
            timer1.Start(); // 启动timer
            }
        }

        private void Piece_MouseDown(object sender, MouseEventArgs e)
        {
            var piece = (PictureBox)sender;
            piece.DoDragDrop(piece, DragDropEffects.Move);
        }

        private void Piece_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PictureBox)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void Piece_DragDrop(object sender, DragEventArgs e)
        {
            var source = (PictureBox)e.Data.GetData(typeof(PictureBox));
            var target = (PictureBox)sender;

            // 交换source和target的图片
            var temp = source.Image;
            source.Image = target.Image;
            target.Image = temp;
            if (IsFinished())
            {
                if(_option==1&&label1.Text!="挑战失败！")
                {
                    timer1.Stop();
                    label1.Text = "挑战成功！";
                }
                textBox1.Text = "拼图完成！";
            }
        }
        private bool IsFinished()
        {
            int cnt = 0;
            while (cnt < pieces.Length)
            {
                if (Finished[cnt].Image != Pieces[cnt].Image) return false; 
                cnt++;
            }
            return true;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
        
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_option == 1)
            {
                timer1.Stop();
            }
            // 创建OpenFileDialog的实例
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置OpenFileDialog的属性
            openFileDialog.Filter = "图片文件(*.jpg;*.png)|*.jpg;*.png";
            openFileDialog.RestoreDirectory = true;

            // 显示OpenFileDialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取选中文件的路径
                _filePath = openFileDialog.FileName;
            }
            init();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            if (_option == 1)
            {
                timer1.Stop();
            }
            
            init();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 newform = new Form3(_filePath);
            newform.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
