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
  
    public partial class Form1 : Form
    {
        private string _filePath;
        private int _num;
        private int option;
        public Form1()
        {
            InitializeComponent();
            _filePath = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            if (comboBox1.Text != "" && _filePath != ""&& comboBox2.Text!="")
            {
                textBox3.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            
            if(comboBox1.Text==""|| _filePath == ""||comboBox2.Text=="")
            {
                textBox3.Text = "请选择图片、分割数和模式！";
            }
            else{ 
            _num = int.Parse(comboBox1.Text);
            // 创建新窗口的实例，并传递文件路径
            Form2 newForm = new Form2(_filePath,_num,this,option);

            // 显示新窗口
            
            newForm.Show();
            this.Hide();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && _filePath != "" && comboBox2.Text != "")
            {
                textBox3.Text = "";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "普通模式")
            {
                option = 0;
            }
            else if(comboBox2.Text=="挑战模式")
            {
                option = 1;
            }
            if (comboBox1.Text != "" && _filePath != "" && comboBox2.Text != "")
            {
                textBox3.Text = "";
            }
        }
    }
}
