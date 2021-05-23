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

namespace Gamble
{
    public partial class Create : Form
    {
        public static MySqlConnection con = new MySqlConnection("server=121.4.255.82;port=3306;user id=root;password=923180;database=chater");
        public static string comd;
        public static MySqlCommand cmd;
        public string psw;
        public Create()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 5 && textBox1.Text.Length <= 13)
            {
                if (textBox1.Text.Contains(@"'"))
                    label6.Text = "不能使用引号，使用其他字符!";
                else
                {
                    try
                    {
                        comd = "select count(*) from chater.chat where binary `user`='" + textBox1.Text + "'";
                        cmd = new MySqlCommand(comd, con);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            label6.Text = "已经有了这个用户名，请使用其他用户名!";
                        else
                            label6.Text = "";
                    }
                    catch
                    {
                        MessageBox.Show("检查网络是否正常!");
                        Login a = new Login();
                        a.label1.Text = "不能连接!";
                        Close();
                    }
                }
            }
            else
            {
                label6.Text = "请输入一个字符数为5~13之间的用户名!";
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            label6.Text = "请输入一个字符数为5~13之间的用户名!";
            label7.Text = "输入一个长度为6~14的密码!";
            label9.Text = "请输入一个长度大于0小于12个字符的呢称!";
            try
            {
                con.Open();
            }
            catch (Exception){ }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            psw = textBox2.Text;
            if (!(textBox2.Text.Length >= 6 && textBox2.Text.Length <= 14))
            {
                label7.Text = "输入一个长度为6~14的密码!";
                label8.Text = "";
            }
            else if (textBox2.Text.Contains(@"'"))
            {
                label7.Text = "不能使用引号，使用其他字符!";
                label8.Text = "";
            }
            else if (textBox2.Text != textBox3.Text)
            {
                label8.Text = "两次密码不一致";
                label7.Text = "";
            }
            else
            {
                label8.Text = "";
                label7.Text = "";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length >= 6 && textBox2.Text.Length <= 14)
            {
                if (textBox3.Text != textBox2.Text)
                    label8.Text = "两次密码不一致";
                else
                    label8.Text = "";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
                textBox3.UseSystemPasswordChar = false;
            }
            else
            {
                textBox3.UseSystemPasswordChar = true;
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if(textBox4.Text.Length==0 || textBox4.Text.Length >=15)
                label9.Text = "请输入一个长度大于0小于15个字符的呢称!";
            else if (textBox4.Text.Contains(@"'"))
                label9.Text = "不能使用引号，使用其他字符!";
            else
            {
                try
                {
                    Thread.Sleep(50);
                    comd = "select count(*) from chater.chat where binary nename='" + textBox4.Text + "'";
                    cmd = new MySqlCommand(comd, con);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    {
                        label9.Text = "已有这个呢称，使用其他呢称";
                    }
                    else
                    {
                        label9.Text = "";
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("检查网络是否正常!"+ex.Message);
                    Login a = new Login();
                    a.label1.Text = "不能连接!";
                    Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(label6.Text.Length==0 &&(label7.Text.Length==0 && (label8.Text.Length==0 && label9.Text.Length==0)))
            {
                try
                {
                    comd = "insert into chat(`user`,`psw`,`nename`,`online`,`manage`) values('"+textBox1.Text+"','"+textBox2.Text+"','" + textBox4.Text + "',0,0)";
                    cmd=new MySqlCommand(comd,con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("注册成功!");
                    Close();
                }
                catch
                {
                    MessageBox.Show("检查网络是否正常!");
                    Login a = new Login();
                    a.label1.Text = "不能连接!";
                    Close();
                }
            }
            else
            {
                MessageBox.Show("请解决所有已经提示的问题再来注册!", "提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HelpCreate f = new HelpCreate();
            f.Show();
        }
    }
}