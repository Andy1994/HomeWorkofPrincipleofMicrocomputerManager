using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HomeWorkofPrincipleofMicrocomputerManager
{
    public partial class MainForm : Form
    {
        int i = 1;
        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen;//窗口初始位置为屏幕中间
            InitializeComponent();
        }

        //点击关闭退出程序
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //openFileDialog1.Filter = "图片文件|*.jpg|所有文件|*.*";
            openFileDialog1.Filter = "jpg文件(*.jpg)|*.jpg|png文件(*.png)|*.png|bmp文件(*.bmp)|*.bmp|ico文件(*.ico)|*.ico";
            openFileDialog1.Title = "打开图片";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Clipboard.SetDataObject(Image.FromFile(openFileDialog1.FileName), false);
                richTextBox1.Paste();
            }
        }

        private void PracticeButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button buttonX = new Button();

            buttonX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            buttonX.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            buttonX.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(242)))), ((int)(((byte)(253)))));
            buttonX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            buttonX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            buttonX.Size = new System.Drawing.Size(splitContainer1.SplitterDistance-10, 50);
            buttonX.Location = new System.Drawing.Point(5,55*i+5);
            buttonX.Text = "动态添加";
            splitContainer1.Panel1.Controls.Add(buttonX);
            i++;
        }

        //分界线移动，左边按钮宽度改变
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            foreach (Control con in splitContainer1.Panel1.Controls)
            {
                con.Size = new System.Drawing.Size(splitContainer1.SplitterDistance - 10, 50);
            }
        }
        
        //鼠标经过或者离开按钮边框颜色改变
        private void ButtonPractice_MouseEnter(object sender, EventArgs e)
        {
            ButtonPractice.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
        }

        private void ButtonPractice_MouseLeave(object sender, EventArgs e)
        {
            ButtonPractice.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }

        private void ButtonLevel80_MouseEnter(object sender, EventArgs e)
        {
            ButtonLevel80.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
        }

        private void ButtonLevel80_MouseLeave(object sender, EventArgs e)
        {
            ButtonLevel80.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }

        private void ButtonLevel95_MouseEnter(object sender, EventArgs e)
        {
            ButtonLevel95.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
        }

        private void ButtonLevel95_MouseLeave(object sender, EventArgs e)
        {
            ButtonLevel95.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }

        private void ButtonLevel100_MouseEnter(object sender, EventArgs e)
        {
            ButtonLevel100.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
        }

        private void ButtonLevel100_MouseLeave(object sender, EventArgs e)
        {
            ButtonLevel100.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }

        private void ButtonDocument_MouseEnter(object sender, EventArgs e)
        {
            ButtonDocument.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
        }

        private void ButtonDocument_MouseLeave(object sender, EventArgs e)
        {
            ButtonDocument.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }
    }
}
