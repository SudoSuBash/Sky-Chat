using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace Server
{
    /// <summary>
    /// buffer[0]的参数:
    /// 0:表示消息
    /// 1:表示图片
    /// 2.表示文件
    /// 3.表示进入退出消息
    /// 4.表示系统消息
    /// 5.表示获取id消息
    /// 6.表示发送名字信息
    /// 7.表示踢出人员信息(管理专用)
    /// </summary>
    /// <param>buffer[0]</param>
    class Program
    {
        public static List<string> name = new List<string>();
        public static Socket cnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static List<Socket> soc = new List<Socket>();
        public static Socket bcnt;//检测是否连接还有负责接受
        public static IPEndPoint host, connecthost;
        public static string rec;//输出
        private static int leng;
        public static string time;
        public static Thread check = new Thread(new ThreadStart(chk)) { IsBackground = false }; //检测是否有客户端连接
        public static string[] s;
        public static void kickp(object soc,object man)//man:管理者的socket
        {
            Socket sc = soc as Socket;
            byte[] caogao = Encoding.UTF8.GetBytes("You are kicked!");
            List<byte> l = new List<byte>();
            l.Add(4);
            l.AddRange(caogao);
            byte[] send = l.ToArray();
        }
        public static void chk()
        {
            while(true)
            {
                bcnt = cnt.Accept();
                soc.Add(bcnt);
                Thread receive = new Thread(recmsg) { IsBackground = false };
                receive.Start(bcnt);
                Thread.Sleep(100);
            }
        }
        public static void recmsg(object o)
        {
            Socket send = o as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2]; //定义缓冲区
                leng = send.Receive(buffer);
                try
                {
                    switch (buffer[0])
                    {
                        case 7:
                            rec = Encoding.UTF8.GetString(buffer, 1, leng - 1);
                            s = rec.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                            if (s[0]=="/kick")
                            {
                                int id = Convert.ToInt32(s[1]);
                                try
                                {
                                    new Thread(()=>kickp(soc[id - 1],cnt)
                                       )
                                    { IsBackground = true }.Start();
                                }
                                catch(IndexOutOfRangeException)
                                {

                                }
                            }
                            break;
                        default:
                            if (leng == 0)
                            {
                                break;
                            }
                            string receive= 0+name[buffer[1]]+":"+Encoding.UTF8.GetString(buffer, 0, leng); 
                            if (soc.Count > 0)
                            {
                                Thread sec = new Thread(sendmsg) { IsBackground = true };
                                sec.Start();
                            }
                            break;
                    }
                }
                catch (Exception)
                {

                }
                Thread.Sleep(100);
            }
        }
        public static void sendmsg()
        {
            for (int i = soc.Count - 1; i >= 0; i--)//从头上开始，一直到尾部
            {
                if (soc[i].Poll(10, SelectMode.SelectRead))             //SelectMode.SelectRead表示，如果已调用 并且有挂起的连接，true。- 或 - 如果有数据可供读取，则为 true。- 或 - 如果连接已关闭、重置或终止，则返回 true（此种情况就表示若客户端断开连接了，则此方法就返回true）； 否则，返回 false。
                {
                    soc[i].Close();//关闭socket
                    soc.Remove(soc[i]);//从列表中删除断开的socket
                    name.Remove(name[i]);//删除名字
                    continue;
                }
                byte[] send = Encoding.UTF8.GetBytes(rec);
                soc[i].Send(send);
            }
        }
        static void Main(string[] args)
        {
            Console.Title = "Sky Chat管理消息端";
            Console.WriteLine("             =====Test---Version:1.2=====");
            try
            {
                Console.WriteLine("连接中...");
                time = DateTime.Now.ToLongTimeString().ToString();
                host = new IPEndPoint(IPAddress.Parse("172.17.0.15"),4003);
                cnt.Bind(host);
                cnt.Listen(100);//最多限制100人
                Console.WriteLine(time+" 成功!");
                check.Start();
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                time = DateTime.Now.ToLongTimeString().ToString();
                Console.WriteLine(time+" 错误:" + ex.Message);
                Console.WriteLine("按enter退出");
                Console.ReadKey();
                return;
            }
        }
    }
}