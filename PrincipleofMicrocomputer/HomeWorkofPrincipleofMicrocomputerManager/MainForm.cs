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
        string[] questionArray;
        bool isTest = false;
        bool TeacherConfirm = false;
        string TeacherMa;
        int number70, number80, number90, number100, mintues70, mintues80, mintues90, mintues100;//考试设置，出题数量，考试时间
        int AboutTime = 0;//倒计时记录时间

        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen;//窗口初始位置为屏幕中间
            InitializeComponent();
            TimeStatus.Text = String.Format("用户ID：{0}  姓名：{3}  专业：{2}  类型：{1}", LoginForm.userid, LoginForm.usertype, LoginForm.classname, LoginForm.username);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (LoginForm.istest == 1) isTest = true;
            
            MyInfoButton.Text = LoginForm.username;//按钮Text改成用户名字
            //启动计时
            this.timer1.Interval = 60000; //设置间隔为一分钟
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

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


            //取得试卷
            try
            {
                conn.Open();
                //数据库查询代码
                cmd = new MySqlCommand("select * from teacherinfo where teachClass='"+LoginForm.classname+"'", conn);
                //查询结果放到reader中
                reader = cmd.ExecuteReader();
                reader.Read();
                string teacherName = reader.GetString(1);//取得教师姓名
                reader.Close();
                
                cmd = new MySqlCommand("select * from papertable where teacherName='"+teacherName+"' order by paperID desc", conn);
                reader = cmd.ExecuteReader();
                //取得试卷
                if (reader.HasRows)
                {
                    ButtonPaper.Enabled = true;
                    reader.Read();
                    ButtonPaper.Text = reader.GetString(2) + " " + reader.GetString(4);
                    questionArray = reader.GetString(3).Split(',');
                }
                reader.Close();

                //取得教师码
                cmd = new MySqlCommand("select * from userinfo where username='" + teacherName + "'", conn);
                reader = cmd.ExecuteReader();
                reader.Read();
                TeacherMa = reader.GetString(4);
                //TimeStatus.Text = TeacherMa;
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

            //取得试卷设置
            try
            {
                conn.Open();
                //数据库查询代码
                cmd = new MySqlCommand("select * from testsetting", conn);
                //查询结果放到reader中
                reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    string level = reader.GetString(0);
                    if (level == "70")
                    {
                        number70 = reader.GetInt32(1);
                        mintues70 = reader.GetInt32(2);
                    }
                    else if (level == "80")
                    {
                        number80 = reader.GetInt32(1);
                        mintues80 = reader.GetInt32(2);
                    }
                    else if (level == "90")
                    {
                        number90 = reader.GetInt32(1);
                        mintues90 = reader.GetInt32(2);
                    }
                    else if (level == "100")
                    {
                        number100 = reader.GetInt32(1);
                        mintues100 = reader.GetInt32(2);
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

        //计时
        private void timer1_Tick(object sender, EventArgs e)
        {
            AboutTime--;
            LeftTime.Text = AboutTime.ToString() + " 分钟";
            if (AboutTime == 0)
            {
                SubmitTest();
                MessageBox.Show("     时间到，自动交卷!     ");
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

        void ClearQuestion()
        {
            questionShowTitleLable.Text = "";
            TimeStatus.Text = "";
            questionShowQuestionLeixinLable2.Text = "";
            questionShowQuestionNanduLable2.Text = "";
            questionShowRichTextBox.Text = "";
            answerQuestionRichTextBox.Text = "";
            LeftTime.Text = "";
            buttonSubmit.Enabled = true;
            buttonSubmit.Text = "提交";
        }

        void isTestEquelToTrue()
        {
            MySqlCommand cmd;
            try
            {
                conn.Open();
                //数据库查询代码
                cmd = new MySqlCommand("update userinfo set istest=1 where userid='"+LoginForm.userid+"'", conn);
                //查询结果放到reader中
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        void isTestEquelToFalse()
        {
            MySqlCommand cmd;
            try
            {
                conn.Open();
                //数据库查询代码
                cmd = new MySqlCommand("update userinfo set istest=0 where userid='" + LoginForm.userid + "'", conn);
                //查询结果放到reader中
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        void Reminder(int i)
        {
            string level = null;
            if (i == 1) { level = "70"; AboutTime = mintues70; }
            else if (i == 2) { level = "80"; AboutTime = mintues80; }
            else if (i == 3) { level = "90"; AboutTime = mintues90; }
            else if (i == 4) { level = "100"; AboutTime = mintues100; }
            //消息框中需要显示哪些按钮，此处显示“确定”和“取消”
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定选择"+level+"分的测试吗?", "", messButton);
            if (dr == DialogResult.OK)
            {
                isTest = true;
                isTestEquelToTrue();
            }
        }

        void ConfirmTest(int i)
        {
            if (isTest && !TeacherConfirm)
            {
                MessageBox.Show("     想要抽题请联系教师!     ");
                TeacherMaLabel.Visible = true;
                TeacherMaBox.Visible = true;
                TeacherMaButton.Visible = true;
            }
            else if (!isTest && !TeacherConfirm)
            {
                Reminder(i);
                if (!isTest) return;
                RemoveButton();
                ClearQuestion();
                addQuestion(i);
                this.timer1.Start();
            }
            if (TeacherConfirm)
            {
                Reminder(i);
                RemoveButton();
                ClearQuestion();
                addQuestion(i);
                TeacherConfirm = false;
            }
        }

        private void PracticeButton_Click(object sender, EventArgs e)
        {
            if (isTest)
            {
                MessageBox.Show("     请先完成考试!     ");
                return;
            }
            RemoveButton();
            ClearQuestion();
            ButtonPractice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(1);
            addQuestion(6);
        }

        private void ButtonLevel70_Click(object sender, EventArgs e)
        {
            ButtonLevel70.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(6);
            ConfirmTest(1);
        }

        private void ButtonLevel80_Click(object sender, EventArgs e)
        {
            ButtonLevel80.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(2);
            ConfirmTest(2);
        }

        private void ButtonLevel95_Click(object sender, EventArgs e)
        {
            ButtonLevel95.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(3);
            ConfirmTest(3);
        }

        private void ButtonLevel100_Click(object sender, EventArgs e)
        {
            ButtonLevel100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(4);
            ConfirmTest(4);
        }

        private void ButtonMyAnswer_Click(object sender, EventArgs e)
        {
            if (isTest)
            {
                MessageBox.Show("     请先完成考试!     ");
                return;
            }
            RemoveButton();
            ButtonMyAnswer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            RecoveryButtonBackColor(5);
            addQuestion(5);
        }

        //连接数据库取得题目
        public void addQuestion(int i)
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
            int zongtishu = 0;
            if (i == 6) sql = "select * from questiontable order by questionID desc";
            else if (i == 2) sql = "select * from questiontable where difficulty='80' order by questionID desc";
            else if (i == 3) sql = "select * from questiontable where difficulty='90' order by questionID desc";
            else if (i == 4) sql = "select * from questiontable where difficulty='100' order by questionID desc";
            else if (i == 1) sql = "select * from questiontable where difficulty='70' order by questionID desc";
            else if (i == 5) sql = "select * from answertable where userid='" + LoginForm.userid + "' order by answerID desc";
            try
            {
                cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                if (i == 5)
                {
                    while(reader.Read())
                    {
                        answerID = reader.GetInt32(0);
                        questionID2 = reader.GetInt32(3);
                        buttonName = reader.GetString(4)+" "+reader.GetString(2);
                        correcting = reader.GetInt32(7);
                        addQuestionButton(LocationFlag, buttonName, questionID2, answerID , correcting);
                        LocationFlag++;
                    }
                }
                else if (i == 6)
                {
                    while (reader.Read())
                    {
                        questionID2 = reader.GetInt32(0);
                        buttonName = reader.GetString(1);
                        addQuestionButton(LocationFlag, buttonName, questionID2, 0, 0);
                        LocationFlag++;
                    }
                }
                else
                {
                    //取得考试需要的题目数量
                    int neednumber = 0;
                    if (i == 1) neednumber = number70;
                    else if (i == 2) neednumber = number80;
                    else if (i == 3) neednumber = number90;
                    else if (i == 4) neednumber = number100;

                    while (reader.Read())
                    {
                        zongtishu++;
                    }
                    reader.Close();
                    reader = cmd.ExecuteReader();
                    int[] queding = new int[zongtishu];
                    int k = 0;
                    for (int j = 1; j <= neednumber; j++)
                    {
                        Random rd = new Random();
                        k = rd.Next(1, 1000) % zongtishu;
                        //TimeStatus.Text = k.ToString();
                        if (queding[k] != 1)
                        {
                            queding[k] = 1;
                        }
                        else --j;
                    }
                    int h = 0;
                    while (reader.Read())
                    {
                        if (queding[h] == 1)
                        {
                            questionID2 = reader.GetInt32(0);
                            buttonName = reader.GetString(1);
                            addQuestionButton(LocationFlag, buttonName, questionID2, 0, 0);
                            LocationFlag++;
                        }
                        h++;
                    }
                }

                //有题目显示出来，判断数量，决定要不要改变SplitterDistance
                if (LocationFlag != 1)
                {
                    if (this.Width == 1000 && LocationFlag > 8)
                    {
                        if (splitContainer2.SplitterDistance < 260) 
                            splitContainer2.SplitterDistance += 15;
                    }
                    else if (this.Width > 1000 && LocationFlag > 12)
                    {
                        if (splitContainer2.SplitterDistance < 260) 
                            splitContainer2.SplitterDistance += 15;
                    }
                    if (LocationFlag < 8)
                    {
                        if (splitContainer2.SplitterDistance > 260)
                            splitContainer2.SplitterDistance -= 15;
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
            buttonX.Size = new System.Drawing.Size(240, 60);
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
            if (isTest)
            {
                LeftTime.Visible = true;
                LeftTimeLabel.Visible = true;
                AddTime.Visible = true;
                LeftTime.Text = AboutTime.ToString() + " 分钟";
            }
            else 
            {
                LeftTime.Visible = false;
                LeftTimeLabel.Visible = false;
                AddTime.Visible = false;
            }
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
                else if (ButtonBackColor == 6) ButtonLevel70.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
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

        private void ButtonLevel70_MouseEnter(object sender, EventArgs e)
        {
            ButtonLevel70.FlatAppearance.BorderColor = Color.FromArgb(195, 229, 245);
        }

        private void ButtonLevel70_MouseLeave(object sender, EventArgs e)
        {
            ButtonLevel70.FlatAppearance.BorderColor = Color.FromArgb(250, 250, 250);
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
        private void SubmitTest()
        {
            string TestTitle = null;
            if (isTest)
            {
                buttonSubmit.Enabled = false;
                buttonSubmit.Text = "禁用";
                TestTitle = "考试 " + questionShowTitleLable.Text;
                isTest = false;
                isTestEquelToFalse();
            }
            else TestTitle = questionShowTitleLable.Text;
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
                //TimeStatus.Text = ms.Length.ToString();
                //保存二进制流都是使用的参数，没有谁在代码中直接给出的。
                string time = DateTime.Now.ToString();
                //string sql = "insert into questiontable(title,questype,difficulty,content,answer) values('"+title+"','问答题','90',?data,'NULL')";
                string sql = "insert into answertable(userid,time,questionid,title,answer,score,correcting,comments,zan) values('" + LoginForm.userid + "','" + time + "','" + questionID + "','" + TestTitle + "',?data,0,0,'',0)";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add("?data", MySql.Data.MySqlClient.MySqlDbType.LongBlob, blob.Length).Value = blob;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                ms.Close();

                MessageBox.Show("     回答提交成功!     ");//弹出对话框显示提交成功
                isTest = false;//考试结束
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //问题提交
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            SubmitTest();
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

        private void ButtonPaper_Click(object sender, EventArgs e)
        {
            if (isTest)
            {
                MessageBox.Show("     请先完成考试!     ");
                return;
            }
            RemoveButton();
            int LocationFlag = 1;
            int qID = 0;
            for (int i = 1; i <= questionArray.Length; i++)
            {
                if (i % 2 == 1)
                {
                    qID = int.Parse(questionArray[i - 1]);
                }
                else 
                {
                    addQuestionButton(LocationFlag, questionArray[i-1], qID , 0, 0);
                    LocationFlag++;
                }
            }
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

        private void TeacherMaButton_Click(object sender, EventArgs e)
        {
            if (TeacherMaBox.Text == TeacherMa)
            {
                MessageBox.Show("     确认成功，点击抽题按钮可以重抽一次或者添加时间     ");
                TeacherConfirm = true;
                TeacherMaBox.Visible = false;
                TeacherMaLabel.Visible = false;
                TeacherMaButton.Visible = false;
            }
            else
            {
                MessageBox.Show("     验证码错误，请重试!     ");
            }
            TeacherMaBox.Text = "";
        }

        private void AddTime_Click(object sender, EventArgs e)
        {
            if (!TeacherConfirm)
            {
                MessageBox.Show("     想要添加时间请联系教师!     ");
                TeacherMaLabel.Visible = true;
                TeacherMaBox.Visible = true;
                TeacherMaButton.Visible = true;
            }
            else
            {
                AboutTime += 10;
                LeftTime.Text = AboutTime.ToString() + " 分钟";
                MessageBox.Show("     成功延长10分钟!     ");
                TeacherConfirm = false;
            }
        }
    }
}
