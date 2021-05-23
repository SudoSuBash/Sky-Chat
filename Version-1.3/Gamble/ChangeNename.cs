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
    public partial class ChangeNename : Form
    {
        public static MySqlConnection con = new MySqlConnection("server=121.4.255.82;port=3306;user id=root;password=923180;database=chater");//雷打不动
        public static string user = Login.user;
        public static string nename = ChaterMain.nickname;
        public static string comd;
        public static MySqlCommand cmd;
        public ChangeNename()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Length ==0 || textBox1.Text.Length >12)
            {
                MessageBox.Show("呢称的字符要求12个字符以内!");
            }
            else
            {
                try
                {
                    if (textBox1.Text.Contains(@"'"))
                    {
                        MessageBox.Show("不能使用引号!");
                    }
                    else
                    {
                        user = Login.user;
                        comd = "update `chater`.`chat` set `nename`='" + textBox1.Text + "'where binary `user`='" + user + "' limit 1";
                        cmd = new MySqlCommand(comd, con);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(@"更改成功!点击""刷新""来更新你的信息");
                        Close();
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("网络不好，检查你的网络");
                }
            }
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            try
            {
                con.Open();
            }
            catch
            {
            }
        }

        private void ChangeNename_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
