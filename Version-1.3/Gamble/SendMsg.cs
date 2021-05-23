using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chater
{
    public partial class SendMsg : Form
    {
        public SendMsg()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 5000)
                MessageBox.Show("不能输入超过5000个字符!");
            else if (textBox2.Text.Length == 0)
                MessageBox.Show("请输入字符");
            else
            {
                string msg = Gamble.ChaterMain.nickname + ":" + textBox2.Text;
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    List<byte> buf = new List<byte> { };
                    buf.Add(0);
                    buf.AddRange(buffer);
                    byte[] send = buf.ToArray();
                    Gamble.ChaterMain.cnt.Send(send);
                }
                catch (Exception)
                {

                }
                textBox2.Text = "";
            }
        }
    }
}
