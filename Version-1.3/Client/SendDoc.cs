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
namespace Chater
{
    public partial class SendDoc : Form
    {
        public SendDoc()
        {
            InitializeComponent();
        }
        public byte[] ReadDoc(string filename)
        {
            FileInfo fl = new FileInfo(filename);
            byte[] buffer = new byte[fl.Length];
            using (FileStream fs = fl.OpenRead())
            {
                fs.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                MessageBox.Show("请选择文件");
            else
            {
                try
                {
                    byte[] buffer = ReadDoc(textBox1.Text);
                    List<byte> buf = new List<byte> { };
                    buf.Add(2);
                    //buf.AddRange(buffer);
                    byte[] send = buf.ToArray();

                    string msg = Gamble.ChaterMain.nickname;
                    Gamble.ChaterMain.cnt.Send(send);
                }
                catch (Exception)
                {

                }
                textBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = "C:\\";
            op.Multiselect = false;
            op.Title = "请选择文件:";
            op.Filter = "所有文件|*.*";
            if (op.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = Path.GetFullPath(op.FileName);
            }
        }
    }
}
