using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace Server
{
    class Program
    { 
        public static Socket cnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static List<Socket> soc = new List<Socket>();
        public static Socket bcnt;//检测是否连接还有负责接受
        public static byte[] sd = new byte[2048]; //发送字符
        public static IPEndPoint host;
        public static IPEndPoint connecthost;
        public static string rec;//输出
        public static string time;
        //线程管理
        public static Thread check = new Thread(new ThreadStart(chk)) { IsBackground = false }; //检测是否有客户端连接
        public static Thread sendmessage = new Thread(new ThreadStart(sendmsg)) { IsBackground = false };
        public static Socket i;
        public static int people=0;
        public static void checkpeople()
        {
            while (true)
            {
                if (soc.Count > 0)
                {
                    for (int i = soc.Count - 1; i >= 0; i--)//从头上开始，一直到尾部
                    {
                        if (soc[i].Poll(10, SelectMode.SelectRead))             //SelectMode.SelectRead表示，如果已调用 并且有挂起的连接，true。- 或 - 如果有数据可供读取，则为 true。- 或 - 如果连接已关闭、重置或终止，则返回 true（此种情况就表示若客户端断开连接了，则此方法就返回true）； 否则，返回 false。
                        {
                            people--;
                            time = DateTime.Now.ToString();
                            IPEndPoint quit = (IPEndPoint)soc[i].RemoteEndPoint;
                            Console.WriteLine(time + " 有客户端退出,IP是" + quit.Address.ToString() + "端口是" + quit.Port.ToString());
                            soc[i].Close();//关闭socket
                            soc.RemoveAt(i);//从列表中删除断开的socket
                        }
                    }
                }
                sendmsg();
                Thread.Sleep(100);
            }
        }
        public static void chk()
        {
            while(true)
            {
                bcnt = cnt.Accept();
                soc.Add(bcnt);
                time = DateTime.Now.ToLongTimeString().ToString();
                connecthost = (IPEndPoint)bcnt.RemoteEndPoint;
                Console.WriteLine(time + " 有客户端连接,IP地址是:" + connecthost.Address.ToString() + ",端口是:" + connecthost.Port.ToString());
                people++;
            }
        }
        public static void sendmsg()
        {
            try
            {
                for (int i = soc.Count - 1; i >= 0; i--)//从头上开始，一直到尾部
                {
                    if (soc[i].Poll(10, SelectMode.SelectRead))             //SelectMode.SelectRead表示，如果已调用 并且有挂起的连接，true。- 或 - 如果有数据可供读取，则为 true。- 或 - 如果连接已关闭、重置或终止，则返回 true（此种情况就表示若客户端断开连接了，则此方法就返回true）； 否则，返回 false。
                    {
                        people--;
                        time = DateTime.Now.ToString();
                        IPEndPoint quit = (IPEndPoint)soc[i].RemoteEndPoint;
                        Console.WriteLine(time + " 有客户端退出,IP是" + quit.Address.ToString() + "端口是" + quit.Port.ToString());
                        soc[i].Close();//关闭socket
                        soc.RemoveAt(i);//从列表中删除断开的socket
                    }
                    byte[] send = Encoding.UTF8.GetBytes(Convert.ToString(people));
                    soc[i].Send(send);
                    Thread.Sleep(100);//挂起100毫秒休息
                }
            }
            catch (Exception)
            {

            }
        }
        static void Main(string[] args)
        {
            Console.Title = "Sky Chat Test";
            Console.WriteLine("             =====People Manage Test Version:1.2=====");
            try
            {
                Console.WriteLine("连接中...");
                time = DateTime.Now.ToLongTimeString().ToString();
                host = new IPEndPoint(IPAddress.Parse("172.17.0.15"),4004);
                cnt.Bind(host);
                cnt.Listen(100);//最多限制100人
                Console.WriteLine(time+" 成功!");
                check.Start();
                Thread pe = new Thread(checkpeople);
                pe.Start();
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
