using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace HomeWorkofPrincipleofMicrocomputerManager
{
    public partial class MainForm : Form
    {
        private MySqlConnection conn;
        bool isShowQuestionShowPanel = false;
        Button ButtonTag;
        private int ButtonBackColor = 0;
        //private string serverip = LoginForm.serverip;

        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen;//窗口初始位置为屏幕中间
            InitializeComponent();
            TimeStatus.Text = String.Format("用户ID：{0} 密码：{1} 用户类型：{2} 专业：{3} 登录次数：{4} 点赞数量：{5}", LoginForm.userid, LoginForm.password, LoginForm.usertype, LoginForm.classname, LoginForm.logincount, LoginForm.zancount);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MyInfoButton.Text = LoginForm.username;//按钮Text改成用户名字

            //签到程序，一天只能签到一次
            string today = DateTime.Now.ToString("yyyy-MM-dd");//取得今天日期

            MySqlDataReader reader = null;
            MySqlCommand cmd;

            string connStr = String.Format("server={0};user id=root; password=; database=wjylsystem; pooling=false; Charset=utf8",
                LoginForm.serverip);

            try
            {
                //创建数据库链接
                conn = new MySqlConnection(connStr);
                conn.Open();
                //数据库查询代码
                cmd = new MySqlCommand("select * from loginlog where userid='" + LoginForm.userid + "' AND logindate='" + today + "'", conn);
                //查询结果放到reader中
                reader = cmd.ExecuteReader();
                //当天签过到就不再签到，第一次登陆就签到，并记录日期
                if (reader.HasRows)
                {
                    QianDaoCount.Text = String.Format("签到：{0}次", LoginForm.logincount);
                }
                else
                {
                    reader.Close();
                    LoginForm.logincount++;
                    QianDaoCount.Text = String.Format("签到：{0}次", LoginForm.logincount);
                    //个人信息签到数据更新
                    cmd = new MySqlCommand("update userinfo set logincount="+ LoginForm.logincount +" where userid='" + LoginForm.userid + "'", conn);
                    cmd.ExecuteNonQuery();
                    //签到记录更新
                    cmd = new MySqlCommand("INSERT INTO loginlog (userid, logindate) VALUES ('" + LoginForm.userid + "','" + today + "')", conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (conn != null) conn.Close();
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                answerQuestionRichTextBox.Paste();
            }
        }

        private void PracticeButton_Click(object sender, EventArgs e)
        {
            RemoveButton();
            ButtonPractice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(1);
            addQuestion(1);
        }


        private void ButtonLevel80_Click(object sender, EventArgs e)
        {
            RemoveButton();
            ButtonLevel80.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(2);
            addQuestion(2);
        }

        private void ButtonLevel95_Click(object sender, EventArgs e)
        {
            RemoveButton();
            ButtonLevel95.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(3);
            addQuestion(3);
        }

        private void ButtonLevel100_Click(object sender, EventArgs e)
        {
            RemoveButton();
            ButtonLevel100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(4);
            addQuestion(4);
        }

        private void ButtonDocument_Click(object sender, EventArgs e)
        {
            RemoveButton();
            ButtonDocument.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(5);
        }

        //连接数据库取得题目
        void addQuestion(int i)
        {
            qustionSelectLable.Visible = false;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            int LocationFlag = 1;
            string sql = null;
            string buttonName = null;
            if (i == 1) sql = "select * from questiontable";
            else if (i == 2) sql = "select * from questiontable where difficulty='80'";
            else if (i == 3) sql = "select * from questiontable where difficulty='95'";
            else if (i == 4) sql = "select * from questiontable where difficulty='100'";
            try
            {
                cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    buttonName = reader.GetString(0);
                    addQuestionButton(LocationFlag, buttonName);
                    LocationFlag++;
                }
                //有题目显示出来，判断数量，决定要不要改变SplitterDistance
                if (LocationFlag != 1)
                {
                    if (this.Width == 1000 && LocationFlag > 8)
                    {
                        splitContainer2.SplitterDistance += 15;
                    }
                    else if (this.Width > 1000 && LocationFlag > 12)
                    {
                        splitContainer2.SplitterDistance += 15;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (conn != null) conn.Close();
            }
        }

        //为题目添加按钮
        void addQuestionButton(int i, string title)
        {
            System.Windows.Forms.Button buttonX = new Button();
            buttonX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            buttonX.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            buttonX.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(242)))), ((int)(((byte)(253)))));
            buttonX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            buttonX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonX.Text = title;
            buttonX.Size = new System.Drawing.Size(splitContainer2.SplitterDistance - 10, 60);
            buttonX.Location = new System.Drawing.Point(5, 65 * (i - 1) + 5);

            buttonX.Tag = buttonX.Text;//为button添加Tag
            buttonX.Click += new EventHandler(this.questionButton_Click);//添加单击鼠标事件
            splitContainer2.Panel1.Controls.Add(buttonX);
        }

        //题目按钮单击事件
        private void questionButton_Click(object sender, EventArgs e)
        {
            //回复原来背景色
            if(ButtonTag != null)ButtonTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            //显示问题
            if (!isShowQuestionShowPanel)
            {
                isShowQuestionShowPanel = true;
                questionShow.Visible = true;
                answerQuestionPanel.Visible = true;
            }
            //获取按钮Tag
            ButtonTag = (Button)sender;
            string bTag = ButtonTag.Tag.ToString();
            TimeStatus.Text = bTag;
            //点击之后按钮背景色改变
            ButtonTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));

            MySqlCommand cmd;
            MySqlDataReader reader = null;
            byte[] blob = null;
            string sql = "select * from questiontable where title='" + bTag + "'";
            try
            {
                cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                reader.Read();
                questionShowTitleLable.Text = reader.GetString(0);//显示题目
                questionShowQuestionLeixinLable2.Text = reader.GetString(1);//显示题目类型
                questionShowQuestionNanduLable2.Text = reader.GetString(2);//显示题目难度
                //显示题目内容
                long len = reader.GetBytes(3, 0, null, 0, 0);
                blob = new byte[len];
                len = reader.GetBytes(3, 0, blob, 0, (int)len);
                MemoryStream ms = new MemoryStream(blob, false);
                questionShowRichText.LoadFile(ms, RichTextBoxStreamType.RichText);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (conn != null) conn.Close();
            }
        }

        void RecoveryButtonBackColor(int i)
        {
            if (i != ButtonBackColor)
            {
                if (ButtonBackColor == 1) ButtonPractice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
                else if (ButtonBackColor == 2) ButtonLevel80.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
                else if (ButtonBackColor == 3) ButtonLevel95.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
                else if (ButtonBackColor == 4) ButtonLevel100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
                else if (ButtonBackColor == 5) ButtonDocument.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
                ButtonBackColor = i;
            }
        }
        //移除面板2的按钮
        void RemoveButton()
        {
            splitContainer2.Panel1.Controls.Clear();
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

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlCommand cmd;
            try
            {
                //建立内存流
                MemoryStream ms = new MemoryStream();
                ms.Position = 0;
                //把当前的richtextbox内容包括图片和文本保存到流中  
                answerQuestionRichTextBox.SaveFile(ms, RichTextBoxStreamType.RichText);
                //ms.Position = 0;
                //richTextBox2.LoadFile(ms, RichTextBoxStreamType.RichText);
                ms.Position = 0;
                //将内存流储存为byte
                byte[] blob = new byte[ms.Length];
                ms.Read(blob, 0, blob.Length);
                TimeStatus.Text = ms.Length.ToString();
                //保存二进制流都是使用的参数，没有谁在代码中直接给出的。
                string title = DateTime.Now.ToString();
                string sql = "insert into questiontable(title,questype,difficulty,content,answer) values('"+title+"','问答题','90',?data,'NULL')";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add("?data", MySql.Data.MySqlClient.MySqlDbType.LongBlob, blob.Length).Value = blob;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                ms.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            byte[] blob = null;
            try
            {
                string sql = "select * from test";
                cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    long len = reader.GetBytes(0, 0, null, 0, 0);
                    blob = new byte[len];
                    len = reader.GetBytes(0, 0, blob, 0, (int)len);
                    MemoryStream ms = new MemoryStream(blob, false);
                    questionShowRichText.LoadFile(ms, RichTextBoxStreamType.RichText);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if(reader != null)reader.Close();
                if(conn != null)conn.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            questionShowRichText.Text = "";
        }
    }
}
