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

namespace Gamble
{
    public partial class ChangePasswd : Form
    {
        public static MySqlConnection con=new MySqlConnection("server=121.4.255.82;port=3306;user id=root;password=923180;database=chater");
        public static string psw;
        public static string user = Login.user;
        public static MySqlCommand cmd;
        public static string comd;
        public ChangePasswd()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            user = Login.user;
            try
            {
                con.Open();
            }
            catch
            {
            }
            using (con)
            {
                comd = string.Format("select psw from chat where binary user = '{0}'", user);
                cmd = new MySqlCommand(comd, con);
                MySqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    psw = read.GetString("psw");
                }
            }
            if (textBox1.Text !=psw)
            {
                MessageBox.Show("错误!原密码不对");
            }
            else if(!(textBox2.Text.Length >=6 && textBox2.Text.Length <=14))
            {
                MessageBox.Show("错误!请将密码设置为6-14字符");
            }
            else if (textBox3.Text != textBox2.Text)
            {
                MessageBox.Show("错误!两次密码不一致");
            }
            else
            {
                try
                {
                    con.Open();
                }
                catch
                {
                }
                try
                {
                    if (textBox2.Text.Contains(@"'"))
                    {
                        MessageBox.Show("不能使用引号!");
                    }
                    else
                    {
                            comd = "update `chater`.`chat` set `psw`='" + textBox2.Text + "'where binary `user`='" + user + "' limit 1";
                        cmd = new MySqlCommand(comd, con);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("更改成功!");
                        Close();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("检查网络是否正常!"+ex.Message);
                    Close();
                }
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox1.UseSystemPasswordChar = false;
                textBox2.UseSystemPasswordChar = false;
                textBox3.UseSystemPasswordChar = false;
            }
            else
            {
                textBox1.UseSystemPasswordChar = false;
                textBox2.UseSystemPasswordChar = true;
                textBox3.UseSystemPasswordChar = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChangePasswd_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
