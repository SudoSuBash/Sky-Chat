using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace ControlBar
{
    class Manage
    {
        public static Socket soc;
        public static MySqlConnection sql = new MySqlConnection("server=121.4.255.82;port=3306;user id=root;password=923180;database=chater;SslMode=None");
        public static MySqlCommand cmd;
        public static string acc, com, runcommand, manage = "", tp = "";
        public static int state;
        public static string[] split;
        public static System.Timers.Timer t;
        public static bool timecontrol = false;
        public static void InitTimer(int microsecond)
        {
            t = new System.Timers.Timer(microsecond);
            t.AutoReset = false;
            t.Enabled = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) => { timecontrol = true; });
        }
        public static int receivekick(object o, string name)
        {
            string c = "";
            Socket s = o as Socket;
            byte[] buf = new byte[1024 * 1024 * 3];
            DateTime n = DateTime.Now;
            InitTimer(5000);
            t.Start();
            while (buf[0] != 7 && timecontrol == false)
                c = Encoding.UTF8.GetString(buf);
            if (timecontrol == true) c = "TimeOut";
            switch (c)
            {
                /*
                 * 0:正常
                 * 1:没有这个人
                 * 2:其他异常
                 */
                case "0":
                    show("成功踢出人" + name + "\n", "Info");
                    break;
                case "1":
                    show(string.Format("错误:好像没有这个人{0}(1)\n", name), "Error");
                    break;
                case "2":
                    show("错误:未知异常(2)\n", "Error");
                    break;
                case "TimeOut":
                    show("错误:等待超时(3)\n", "Error");
                    timecontrol = false;
                    break;
            }
            return 0;
        }
        public static int execute(string cmd)
        {
            split = cmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch (split[0])
            {
                case "/kick":
                    if (split.Length != 3)
                    {
                        show("命令语法错误:需要2个参数,但是提供了" + Convert.ToString(split.Length - 1) + "个参数\n", "Error");
                        show("\n/kick命令语法:\n/kick <type> <id/name>\ntype:类型,是踢出类型为id还是name的人\n", "Warning");
                    }
                    else
                    {
                        if (split[1] != "id" && split[1] != "name")
                        {
                            show("参数的第2项应该是踢出的类型,但是你输入的不属于类型之一\n", "Error");
                            show("type:类型,是踢出类型为id还是name的人\n", "Warning");
                        }
                        else
                        {
                            try
                            {
                                if (split[1] == "id")
                                    Convert.ToInt32(split[2]);//检测是否为整数,如果不是整数跳到catch(FormatException)里面
                                Socket s = soc as Socket;
                                byte[] buffer = Encoding.UTF8.GetBytes(cmd);
                                List<byte> caogao = new List<byte>();
                                caogao.Add(6);
                                caogao.AddRange(buffer);
                                byte[] send = caogao.ToArray();
                                s.Send(send);
                                receivekick(soc, split[1]);
                            }
                            catch (FormatException)
                            {
                                show("参数的第3项应该是踢出的id(如果你选择了id的话,但是你输入的不属于id数(整数)\n", "Error");
                                show("id/name:类型,是踢出名字为id或者name的人\n", "Warning");
                            }
                            catch
                            {
                                show("错误:发送命令到服务端失败!你可能要检查你的网络连接\n", "Error");
                            }
                        }
                    }
                    return 0;
                case "/exit":
                    if (split.Length != 1)
                    {
                        show("命令语法错误:这个命令不需要参数,但是提供了" + Convert.ToString(split.Length - 1) + "个参数\n", "Error");
                        show("\n/exit命令语法:\n/exit:退出程序\n", "Warning");
                        return 0;
                    }
                    else
                    {
                        show("已成功退出程序.\n", "Info");
                        Thread.Sleep(500);
                        return 1;
                    }
                case "/about":
                    if (split.Length != 1)
                    {
                        show("命令语法错误:这个命令不需要参数,但是提供了" + Convert.ToString(split.Length - 1) + "个参数\n", "Error");
                        show("\n/about命令语法:\n/about:显示程序版本\n", "Warning");
                    }
                    else
                        show("关于程序:\n版本:1.2 Test(试行)\n这个版本终止开发日期:2021/05/23\n有问题可以反馈到邮箱:wcr070523@outlook.com\n我们测试的QQ群号:781485357,欢迎加入\n", "Info");
                    return 0;
                default:
                    show("命令错误:没有这个命令\n", "Error");
                    return 0;
            }
        }
        public static void show(string message, string type)
        {
            switch (type)
            {
                case "Error":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(DateTime.Now.ToString() + " " + "[" + type + "] " + message);
                    break;
                case "Info":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(DateTime.Now.ToString() + " " + "[" + type + "] " + message);
                    break;
                case "Warning":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(DateTime.Now.ToString() + " " + "[" + type + "] " + message);
                    break;
                case "Command":
                    Console.Write(message);
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(100);//制造一种停顿效果
            return;
        }
        static void Main()
        {
            int n = 1;
            show("欢迎来到后台管理系统!\n", "Info");
            show("现在开始尝试连接服务器的第1部分......\n", "Info");
            connect:
            {
                try
                {
                    sql.Open();
                    show("连接服务器的第1部分成功!\n", "Info");
                    n = 0;
                    upd:
                    {
                        show("请输入管理的Sky Chat账号:", "Info");
                        Console.ForegroundColor = ConsoleColor.Green;
                        acc = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        while (string.IsNullOrWhiteSpace(acc))
                        {
                            show("账号不能为空,请输入一个账号:", "Info");
                            Console.ForegroundColor = ConsoleColor.Green;
                            acc = acc = Console.ReadLine();
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        com = string.Format("select count(*) from chater.chat where binary `user`='{0}'", acc);
                        cmd = new MySqlCommand(com, sql);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        {
                            com = string.Format("select manage from chat where binary user = '{0}'", acc);
                            cmd = new MySqlCommand(com, sql);
                            MySqlDataReader read = cmd.ExecuteReader();
                            while (read.Read())
                                manage = read.GetString("manage");
                            read.Close();
                            if (manage == "0")
                            {
                                show("检测到这个账号为非管理账户,不可使用此程序!\n", "Error");
                                n++;
                                if (n == 3)
                                {
                                    show("错误输入次数太多.....按enter退出程序\n", "Error");
                                    Console.Read();
                                }
                                else
                                    goto upd;
                            }
                            else
                            {
                                show("检测到这个账号为管理账户.\n", "Warning");
                                con:
                                {
                                    try
                                    {
                                        show("现在开始尝试连接服务器的第2部分......\n", "Info");
                                        switch (manage)
                                        {
                                            case "Program":
                                                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                                soc.Connect(new IPEndPoint(IPAddress.Parse("121.4.255.82"), 4005));
                                                tp = "编程";
                                                break;
                                            case "7A":
                                                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                                soc.Connect(new IPEndPoint(IPAddress.Parse("121.4.255.82"), 4007));
                                                tp = "我们7班的管理系统";
                                                break;
                                            case "Main":
                                                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                                soc.Connect(new IPEndPoint(IPAddress.Parse("121.4.255.82"), 4000));
                                                tp = "主管理大厅";
                                                break;
                                        }
                                        show("连接服务器的第2部分成功!\n", "Info");
                                        show("欢迎进入管理后台!你管理的区服是:" + tp + "\n", "Info");
                                        while (true)
                                        {
                                            cmd: show("> ", "Command");
                                            runcommand = Console.ReadLine();
                                            if (string.IsNullOrWhiteSpace(runcommand))
                                                goto cmd;
                                            state = execute(runcommand);
                                            if (state == 1)
                                                break;
                                        }
                                    }
                                    catch
                                    {
                                        if (n <= 3)
                                        {
                                            show("错误:不能连接到服务器!尝试重连..(" + n + "/3次)\n", "Error");
                                            n++;
                                            goto con;
                                        }
                                        else
                                        {
                                            show("错误:已经3次没连入服务器!按enter退出程序...\n", "Error");
                                            Console.Read();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            show("错误:好像没有这个账户!qaq~\n", "Error");
                            n++;
                            if (n == 3)
                            {
                                show("错误输入次数太多.....按enter退出程序\n", "Error");
                                Console.Read();
                            }
                            else
                                goto upd;
                        }
                    }
                }
                catch
                {
                    if (n <= 3)
                    {
                        show("错误:不能连接到服务器!尝试重连..(" + n + "/3次)\n", "Error");
                        n++;
                        goto connect;
                    }
                    else
                    {
                        show("错误:已经3次没连入服务器!按enter退出程序...\n", "Error");
                        Console.Read();
                    }
                }
            }
        }
    }
}