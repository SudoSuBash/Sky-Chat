using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;

/*
 * 说明:
 * chater.chat中的online状态:(等同于state变量)
 * 0.不在线
 * 1.在线,但是只有一台设备在线
 * 2.在线,但是有多台设备同时登陆
 */
namespace Gamble
{
    public partial class Login : Form
    { 
        public static MySqlConnection con = new MySqlConnection("server=121.4.255.82;port=3306;user id=root;password=923180;database=chater");
        public static string user, psw;
        bool open;
        public Login()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            new Thread(o =>
            {
                open = ChaterMain.open;
                if (open == true)
                {
                    try//尝试打开数据库
                    {
                        con.Open();
                        label1.ForeColor = SystemColors.MenuHighlight;
                        label1.Text = "连接成功!";
                    }
                    catch (Exception)//如果已经打开，就不用重复开了
                    {
                        label1.ForeColor = Color.Red;
                        label1.Text = "无法连接!";
                    }
                }
                else
                {
                    try//尝试打开数据库
                    {
                        con.Open();
                        label1.ForeColor = SystemColors.MenuHighlight;
                        label1.Text = "连接成功!";
                    }
                    catch (Exception)//如果已经打开，就不用重复开了
                    {
                        label1.ForeColor = SystemColors.MenuHighlight;
                        label1.Text = "连接成功!";
                    }
                }
            })
            { IsBackground = true }.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                System.Environment.Exit(0);
            }
            catch
            {
                ;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(label1.Text=="连接成功!")
            {
                textBox1.Text = "";
                textBox2.Text = "";
                Create a = new Create();
                a.Show();
                Thread.Sleep(100);
                HelpCreate f = new HelpCreate();
                f.Show();
            }
            else
            {
                MessageBox.Show("错误:请检查网络是否正常!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlCommand cmd;
            string cond;
            user = this.textBox1.Text;
            psw = textBox2.Text;
            if(user.Length==0)
                MessageBox.Show("账号不能为空!");
            else if(psw.Length==0)
                MessageBox.Show("密码不能为空!");
            else if (textBox1.Text.Contains(@"'"))
                MessageBox.Show("不能使用引号，使用其他字符!");
            else
            {
                try
                {
                    try//尝试打开数据库
                    {
                        con.Open();
                    }
                    catch (Exception) { };//如果已经打开，就不用重复开了
                    cond = string.Format("select count(*) from chater.chat where binary `user`='{0}' and `psw`='{1}'", user, psw);
                    cmd = new MySqlCommand(cond, con);
                    label1.ForeColor = SystemColors.MenuHighlight;
                    label1.Text = "登陆中.....";
                    int state=0;
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
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
                            cond = string.Format("select online from chat where binary user = '{0}'", user);
                            cmd = new MySqlCommand(cond, con);
                            MySqlDataReader read = cmd.ExecuteReader();
                            while (read.Read())
                            {
                                state = read.GetInt32("online");
                            }
                        }
                        if (state == 1)
                        {
                            try//尝试打开数据库
                            {
                                con.Open();
                            }
                            catch (Exception)//如果已经打开，就不用重复开了
                            {

                            }
                            user = textBox1.Text;
                            psw = textBox2.Text;
                            ChaterMain f = new ChaterMain();
                            f.Show();
                            this.Hide();
                            label1.ForeColor = SystemColors.MenuHighlight;
                            label1.Text = "连接成功!";
                        }
                        else
                        {
                            try//尝试打开数据库
                            {
                                con.Open();
                            }
                            catch (Exception)//如果已经打开，就不用重复开了
                            {

                            }
                            cmd = new MySqlCommand(cond, con);
                            cmd.ExecuteNonQuery();
                            user = textBox1.Text;
                            psw = textBox2.Text;
                            ChaterMain f = new ChaterMain();
                            f.Show();
                            this.Hide();
                            label1.ForeColor = SystemColors.MenuHighlight;
                            label1.Text = "连接成功!";
                        }
                    }
                    else
                    {
                        MessageBox.Show("错误，请查看你的账户或者密码有哪个不正确");
                        textBox2.Text = "";
                        label1.ForeColor = SystemColors.MenuHighlight;
                        label1.Text = "连接成功!";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("不能登录，请查看你自己的网络是否正确!"+ex.Message);
                    label1.ForeColor = Color.Red;
                    label1.Text = "无法连接!";
                }
            }
        }
    }
}
