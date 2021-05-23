using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Gamble
{
    public partial class ChaterMain : Form
    {
        public static Socket cnt,peo;
        public static IPEndPoint host = new IPEndPoint(IPAddress.Parse("121.4.255.82"), 4003);//连接Socket
        public static IPEndPoint hostp = new IPEndPoint(IPAddress.Parse("121.4.255.82"), 4004);//连接Socket
        public static string account,nickname;
        public static MySqlConnection con = new MySqlConnection("server=121.4.255.82;port=3306;user id=root;password=923180;database=chater;SslMode=None");//使用数据库连接
        public static bool open = true;
        public static string msg,mesg,time;
        public static int id;
        public void sendmsg(string msg, byte num)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                List<byte> buf = new List<byte> { };
                buf.Add(num);
                buf.AddRange(buffer);
                byte[] send = buf.ToArray();
                Gamble.ChaterMain.cnt.Send(send);
            }
            catch (Exception)
            {

            }
        }
        public void recpeo(object o)
        {
            Socket re = o as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                try
                {
                    int leng = re.Receive(buffer);//出错点
                    if (leng == 0)
                    {
                        break;
                    }
                    mesg = Encoding.UTF8.GetString(buffer, 0, leng);
                    label3.Text = "在线人数:" + mesg;
                }
                catch (Exception)
                { }
                Thread.Sleep(50);
            }
        }
        public void recmsg(object o)
        {
            Socket re = o as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                try
                {
                    int leng = re.Receive(buffer,buffer.Length,SocketFlags.None);//出错点
                    if (leng == 0)
                    {
                        break;
                    }
                    else
                    {
                        switch (buffer[0])
                        {
                            case 0:
                                msg = Encoding.UTF8.GetString(buffer, 2, leng-2);
                                time = DateTime.Now.ToString();
                                richTextBox1.Text += time + " " + msg + Environment.NewLine;
                                continue;
                            case 1:
                                msg = BitConverter.ToString(buffer, 1, leng - 1);
                                richTextBox1.Text += time + " " + msg + Environment.NewLine;
                                continue;
                            case 3:
                                msg = Encoding.UTF8.GetString(buffer, 1, leng - 1);
                                richTextBox1.Text +=msg + Environment.NewLine;
                                continue;
                            case 5:
                                id = Convert.ToInt32(Encoding.UTF8.GetString(buffer, 1, leng - 1));
                                break;
                        }
                    }
                }
                catch (Exception)
                {

                }
                Thread.Sleep(50);
            }
        }
        public ChaterMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
  
        }
        private void Game_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            account = Login.user;
            Text = "欢迎回来!";
            open = false;
            panel1.Controls.Clear();
            Chater.SendMsg win = new Chater.SendMsg();
            win.MdiParent = this;
            win.Parent = panel1;
            win.Show();
            new Thread(o =>
            {
                try
                {
                    cnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    peo = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    cnt.Connect(host);
                    peo.Connect(hostp);
                }
                catch (Exception)
                {
                    MessageBox.Show("错误:主世界已经关闭或者正在维护");
                    Close();
                }
                try
                {
                    con.OpenAsync();//如果已经打开过，那么就不用再打开了
                }
                catch (Exception) { };
                try
                {
                    string cond = string.Format("select nename from chat where binary user = '{0}'", account);
                    MySqlCommand cmd = new MySqlCommand(cond, con);
                    MySqlDataReader read = cmd.ExecuteReader();
                    using (read)
                    {
                        while (read.Read())
                        {
                            nickname = read.GetString("nename");
                        }
                        label2.Text = "Hi," + nickname;
                    }
                    Thread r = new Thread(recmsg) { IsBackground = true };
                    r.Start(cnt);
                    Thread rp = new Thread(recpeo) { IsBackground = true };
                    rp.Start(peo);
                    MessageBox.Show("Your ID:" + id);
                    OutMenu.Text = @"退出登录'" + account + "'";
                    string msg = @"[欢迎'" + nickname + "'进入聊天室]";
                    sendmsg(msg,3);
                    Thread.Sleep(50);
                }
                catch (Exception)
                {

                }
            })
            { IsBackground = true }.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Game_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string cond = "update `chater`.`chat` set `online`=0 where binary `user`='" + account + "' limit 1";
                MySqlCommand cmd = new MySqlCommand(cond, con);
                cmd.ExecuteNonQuery();
                string msg = @"['" + nickname + "'退出了聊天室]";
                sendmsg(msg,3);
                //textBox2.Text = "";
                if (cnt.Connected)
                {
                    cnt.Shutdown(SocketShutdown.Both);
                    cnt.Close();
                }
                if (peo.Connected)
                {
                    peo.Shutdown(SocketShutdown.Both);
                    peo.Close();
                }
                try//尝试打开数据库
                {
                    con.Close();
                }
                catch (Exception)//如果已经打开，就不用重复开了
                {

                }
            }
            catch(Exception)
            {

            }
            Login j = new Login();
            j.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void ChaterMain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void button1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
                                                                                                              
        private void button1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void OutMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 更改呢称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeNename f = new ChangeNename();
            f.TopLevel = true;
            f.ShowDialog(this);
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try//尝试打开数据库
            {
                con.Open();
            }
            catch (Exception)//如果已经打开，就不用重复开了
            {

            }
            using (con)
            {
                string cond = string.Format("select nename from chat where binary user = '{0}'", account);
                MySqlCommand cmd = new MySqlCommand(cond, con);
                cmd.CommandTimeout = 30;
                MySqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    nickname = read.GetString("nename");
                }
                label2.Text = "Hi," + nickname;
            }
            OutMenu.Text = @"退出登录'" + nickname + "'";
        }

        private void 检查更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void panel1_Load(object sender, PaintEventArgs e)
        {

        }
        private void label4_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            Chater.SendPic win = new Chater.SendPic();
            win.MdiParent = this;
            win.Parent = panel1;
            win.Show();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            Chater.SendMsg win = new Chater.SendMsg();
            win.MdiParent = this;
            win.Parent = panel1;
            win.Show();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            Chater.SendDoc win = new Chater.SendDoc();
            win.MdiParent = this;
            win.Parent = panel1;
            win.Show();
        }

        private void 关于程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.TopLevel= true;
            a.ShowDialog(this);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void 更改密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePasswd f = new ChangePasswd();
            f.TopLevel = true;
            f.ShowDialog(this);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
