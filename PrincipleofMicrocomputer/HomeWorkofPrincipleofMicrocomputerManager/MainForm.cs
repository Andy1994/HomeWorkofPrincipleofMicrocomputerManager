using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private int questionID = 0;//当前问题ID
        //private string serverip = LoginForm.serverip;

        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen;//窗口初始位置为屏幕中间
            InitializeComponent();
            TimeStatus.Text = String.Format("用户ID：{0}  姓名：{3}  专业：{2}  类型：{1}", LoginForm.userid, LoginForm.usertype, LoginForm.classname, LoginForm.username);
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

        private void ButtonMyAnswer_Click(object sender, EventArgs e)
        {
            RemoveButton();
            ButtonMyAnswer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(5);
            addQuestion(5);
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
            int questionID2 = 0;
            int answerID = 0;
            int correcting = 0;
            if (i == 1) sql = "select * from questiontable order by questionID desc";
            else if (i == 2) sql = "select * from questiontable where difficulty='80' order by questionID desc";
            else if (i == 3) sql = "select * from questiontable where difficulty='95' order by questionID desc";
            else if (i == 4) sql = "select * from questiontable where difficulty='100' order by questionID desc";
            else if (i == 5) sql = "select * from answertable where userid='" + LoginForm.userid + "' order by answerID desc";
            try
            {
                cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    if (i == 5)
                    {
                        answerID = reader.GetInt32(0);
                        questionID2 = reader.GetInt32(3);
                        buttonName = reader.GetString(4)+" "+reader.GetString(2);
                        correcting = reader.GetInt32(7);
                        addQuestionButton(LocationFlag, buttonName, questionID2, answerID , correcting);
                        LocationFlag++;
                    }
                    else
                    {
                        questionID2 = reader.GetInt32(0);
                        buttonName = reader.GetString(1);
                        addQuestionButton(LocationFlag, buttonName, questionID2, 0, 0);
                        LocationFlag++;
                    }
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

        //为 题目/回答 添加按钮
        void addQuestionButton(int i, string title, int qID, int aID , int correcting)
        {
            System.Windows.Forms.Button buttonX = new Button();
            buttonX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(243)))));
            buttonX.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            buttonX.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(242)))), ((int)(((byte)(253)))));
            buttonX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(254)))));
            buttonX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonX.Size = new System.Drawing.Size(splitContainer2.SplitterDistance - 10, 60);
            buttonX.Location = new System.Drawing.Point(5, 65 * (i - 1) + 5);

            if (aID == 0)
            {
                buttonX.Text = title;
                buttonX.Tag = qID;//为button添加Tag
                buttonX.Click += new EventHandler(this.questionButton_Click);//添加单击鼠标事件
            }
            else
            {
                buttonX.Text = title;
                buttonX.Tag = aID;
                if (correcting == 1) buttonX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(240)))));
                buttonX.Click += new EventHandler(this.answerButton_Click);
            }
            splitContainer2.Panel1.Controls.Add(buttonX);
        }

        private void answerButton_Click(object sender, EventArgs e)
        {
            //恢复原来背景色
            if (ButtonTag != null) ButtonTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(241)))), ((int)(((byte)(242)))));

            questionShowRichTextBox.Text = "";//清空问题box
            answerQuestionRichTextBox.Text = "";//清空回答box
            buttonSubmit.Text = "禁用";
            buttonSubmit.Enabled = false;//禁止提交按钮
            answerInfoLabel.Visible = true;
            commentsLabel.Visible = true;
            heartPic.Visible = false;
            answerQuestionRichTextBox.ReadOnly = true;
            commentsLabel.Text = "";

            if (!isShowQuestionShowPanel)
            {
                isShowQuestionShowPanel = true;
                questionShow.Visible = true;
                answerQuestionPanel.Visible = true;
            }
            //获取按钮Tag
            ButtonTag = (Button)sender;
            int aID = (int)(ButtonTag.Tag);//取得answerID

            //点击之后按钮背景色改变
            ButtonTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));

            //显示回答
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            int qID = 0;
            byte[] blob2 = null;
            string sql2 = "select * from answertable where answerID=" + aID + "";
            try
            {
                cmd = new MySqlCommand(sql2, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                reader.Read();
                string time = reader.GetString(2);//回答时间
                qID = reader.GetInt32(3);
                int correcting = reader.GetInt32(7);//是否批改
                int score = reader.GetInt32(6);//分数
                string comments = reader.GetString(8);//评语
                int zan = reader.GetInt32(9);//点赞
                if (correcting == 1)
                {
                    answerInfoLabel.Text = "已批改 得分：" + score.ToString() + " 答题时间：" + time;
                    commentsLabel.Text = "教师评语：" + comments;
                    if (zan == 1) heartPic.Visible = true;
                }
                else 
                {
                    answerInfoLabel.Text = "未批改 答题时间：" + time;
                }

                //显示题目内容
                long len = reader.GetBytes(5, 0, null, 0, 0);
                blob2 = new byte[len];
                len = reader.GetBytes(5, 0, blob2, 0, (int)len);
                MemoryStream ms = new MemoryStream(blob2, false);
                answerQuestionRichTextBox.LoadFile(ms, RichTextBoxStreamType.RichText);
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

            //显示题目
            byte[] blob = null;
            string sql = "select * from questiontable where questionID=" + qID + "";
            try
            {
                cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                reader.Read();
                questionID = reader.GetInt32(0);//取得题目ID
                questionShowTitleLable.Text = reader.GetString(1);//显示题目
                TimeStatus.Text = reader.GetString(1);//显示题目
                questionShowQuestionLeixinLable2.Text = reader.GetString(2);//显示题目类型
                questionShowQuestionNanduLable2.Text = reader.GetString(3);//显示题目难度
                //显示题目内容
                long len = reader.GetBytes(4, 0, null, 0, 0);
                blob = new byte[len];
                len = reader.GetBytes(4, 0, blob, 0, (int)len);
                MemoryStream ms = new MemoryStream(blob, false);
                questionShowRichTextBox.LoadFile(ms, RichTextBoxStreamType.RichText);
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

        //题目按钮单击事件
        private void questionButton_Click(object sender, EventArgs e)
        {
            questionShowRichTextBox.Text = "";//清空问题box
            answerQuestionRichTextBox.Text = "";//清空回答box
            buttonSubmit.Text = "提交";
            buttonSubmit.Enabled = true;//开启提交按钮
            heartPic.Visible = false;
            answerInfoLabel.Visible = false;
            commentsLabel.Visible = false;

            answerQuestionRichTextBox.ReadOnly = false;
            answerQuestionRichTextBox.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);

            //恢复原来背景色
            if(ButtonTag != null)ButtonTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(243)))));
            //显示问题
            if (!isShowQuestionShowPanel)
            {
                isShowQuestionShowPanel = true;
                questionShow.Visible = true;
                answerQuestionPanel.Visible = true;
            }
            //获取按钮Tag
            ButtonTag = (Button)sender;
            int bTag = (int)(ButtonTag.Tag);
            //点击之后按钮背景色改变
            ButtonTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));

            MySqlCommand cmd;
            MySqlDataReader reader = null;
            byte[] blob = null;
            string sql = "select * from questiontable where questionID=" + bTag + "";
            try
            {
                cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                reader.Read();
                questionID = reader.GetInt32(0);//取得题目ID
                questionShowTitleLable.Text = reader.GetString(1);//显示题目
                TimeStatus.Text = reader.GetString(1);//显示题目
                questionShowQuestionLeixinLable2.Text = reader.GetString(2);//显示题目类型
                questionShowQuestionNanduLable2.Text = reader.GetString(3);//显示题目难度
                //显示题目内容
                long len = reader.GetBytes(4, 0, null, 0, 0);
                blob = new byte[len];
                len = reader.GetBytes(4, 0, blob, 0, (int)len);
                MemoryStream ms = new MemoryStream(blob, false);
                questionShowRichTextBox.LoadFile(ms, RichTextBoxStreamType.RichText);
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
                else if (ButtonBackColor == 5) ButtonMyAnswer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
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

        private void ButtonMyAnswer_MouseEnter(object sender, EventArgs e)
        {
            ButtonMyAnswer.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
        }

        private void ButtonMyAnswer_MouseLeave(object sender, EventArgs e)
        {
            ButtonMyAnswer.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }
        //问题提交
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            MySqlCommand cmd;
            try
            {
                //建立内存流
                MemoryStream ms = new MemoryStream();
                ms.Position = 0;
                //把当前的richtextbox内容包括图片和文本保存到流中  
                answerQuestionRichTextBox.SaveFile(ms, RichTextBoxStreamType.RichText);
                ms.Position = 0;
                //将内存流储存为byte
                byte[] blob = new byte[ms.Length];
                ms.Read(blob, 0, blob.Length);
                TimeStatus.Text = ms.Length.ToString();
                //保存二进制流都是使用的参数，没有谁在代码中直接给出的。
                string time = DateTime.Now.ToString();
                //string sql = "insert into questiontable(title,questype,difficulty,content,answer) values('"+title+"','问答题','90',?data,'NULL')";
                string sql = "insert into answertable(userid,time,questionid,title,answer,score,correcting,comments,zan) values('"+LoginForm.userid+"','"+time+"','"+questionID+"','"+questionShowTitleLable.Text+"',?data,0,0,'',0)";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add("?data", MySql.Data.MySqlClient.MySqlDbType.LongBlob, blob.Length).Value = blob;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                ms.Close();

                MessageBox.Show("     回答提交成功!     ");//弹出对话框显示提交成功
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //添加图片按钮
        private void buttonAddImage_Click(object sender, EventArgs e)
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

        private void buttonAddImage_MouseHover(object sender, EventArgs e)
        {
            buttonAddImage.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.SetToolTip(this.buttonAddImage, "插入图片");
        }

        private void buttonAddImage_MouseLeave(object sender, EventArgs e)
        {
            buttonAddImage.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }
        //设置粗体按钮
        private void buttonBold_MouseHover(object sender, EventArgs e)
        {
            buttonBold.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.SetToolTip(this.buttonBold, "设置粗体");
        }

        private void buttonBold_MouseLeave(object sender, EventArgs e)
        {
            buttonBold.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }

        private void buttonBold_Click(object sender, EventArgs e)
        {
            Font oldFont = this.answerQuestionRichTextBox.SelectionFont;
            Font newFont;
            if (oldFont.Bold)
                newFont = new Font(oldFont, oldFont.Style & ~FontStyle.Bold);
            else
                newFont = new Font(oldFont, oldFont.Style | FontStyle.Bold);
            this.answerQuestionRichTextBox.SelectionFont = newFont;
            this.answerQuestionRichTextBox.Focus(); 
        }
        //设置斜体按钮
        private void buttonItalic_MouseHover(object sender, EventArgs e)
        {
            buttonItalic.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.SetToolTip(this.buttonItalic, "设置粗体");
        }

        private void buttonItalic_MouseLeave(object sender, EventArgs e)
        {
            buttonItalic.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }

        private void buttonItalic_Click(object sender, EventArgs e)
        {
            Font oldFont = this.answerQuestionRichTextBox.SelectionFont;
            Font newFont;
            if (oldFont.Italic)
                newFont = new Font(oldFont, oldFont.Style & ~FontStyle.Italic);
            else
                newFont = new Font(oldFont, oldFont.Style | FontStyle.Italic);
            this.answerQuestionRichTextBox.SelectionFont = newFont;
            this.answerQuestionRichTextBox.Focus(); 
        }
        //设置下划线按钮
        private void buttonUnderline_MouseHover(object sender, EventArgs e)
        {
            buttonUnderline.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.SetToolTip(this.buttonUnderline, "设置下划线");
        }

        private void buttonUnderline_MouseLeave(object sender, EventArgs e)
        {
            buttonUnderline.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
        }

        private void buttonUnderline_Click(object sender, EventArgs e)
        {
            Font oldFont = this.answerQuestionRichTextBox.SelectionFont;
            Font newFont;
            if (oldFont.Underline)
                newFont = new Font(oldFont, oldFont.Style & ~FontStyle.Underline);
            else
                newFont = new Font(oldFont, oldFont.Style | FontStyle.Underline);
            this.answerQuestionRichTextBox.SelectionFont = newFont;
            this.answerQuestionRichTextBox.Focus(); 
        }

        private void changePasswordButton_Click(object sender, EventArgs e)
        {
            changePassword changepassword = new changePassword();
            changepassword.Show();
        }
    }
}
