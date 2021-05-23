using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;

namespace Chater
{
    public partial class SendPic : Form
    {
        public SendPic()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = "C:\\";
            op.Multiselect = false;
            op.Title = "请选择图片:";
            op.Filter = "图片文件|*.jpg;*.jpeg;*gif;*png";
            if(op.ShowDialog()==DialogResult.OK)
                textBox1.Text = Path.GetFullPath(op.FileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length ==0)
                MessageBox.Show("请选择图片");
            else
            {
                try
                {
                    string msg = "[" + Gamble.ChaterMain.nickname + "发送了一张图片]";
                    byte[] buff = Encoding.UTF8.GetBytes(msg);
                    List<byte> bf = new List<byte> { };
                    bf.Add(0);
                    bf.AddRange(buff);
                    byte[] sen = bf.ToArray();

                    FileInfo fi = new FileInfo(@textBox1.Text);
                    byte[] length = new byte[fi.Length];
                    byte[] buffer = new byte[10485761];//最高发送10M图片
                    FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read);
                    fs.Read(buffer, 0, length.Length);
                    fs.Flush();
                    fs.Close();
                    List<byte> buf = new List<byte> { };//获取字节码
                    buf.Add(1);
                    buf.AddRange(buffer);
                    byte[] send = buf.ToArray();

                    Gamble.ChaterMain.cnt.Send(sen);
                    Gamble.ChaterMain.cnt.Send(send, send.Length, SocketFlags.None);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("不能发送大于10M图片!");
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    try
                    {
                        Gamble.ChaterMain.cnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Gamble.ChaterMain.cnt.Connect(Gamble.ChaterMain.host);
                    }
                    catch { }
                }
                catch { }
                textBox1.Text = "";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.KeyCode == Keys.Control)
            {
                textBox1.Text += Environment.NewLine;
            }
        }
    }
}
