using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class User
    {
        public string name { get; set; }
        public Socket handler { get; set; }

        public User(string name, Socket handler)
        {
            this.name = name;
            this.handler = handler;
        }

        public void Send(string sender, string text)
        {
            Task.Run(() =>
            {
                string mess = "mess"+sender + "." + text;
                handler.Send(Encoding.UTF8.GetBytes(mess));
            });
        
        }
    }

    public class Server
    {   
        private Socket handler;

        //Другая домашка :)
        private int digit;

        public int Port { get; private set; }
        public List<User> Users { get; set; }
        public Server(int port)
        {
            handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Port = port;
            Users = new List<User>();
        }

        public void Start()
        {
            handler.Bind(new IPEndPoint(IPAddress.Any, 2020));
            handler.Listen(1000);

            Logger.Log("Сервер запущен");

            Task.Run(() =>
            {
                while (true)
                {
                    var socket = handler.Accept();

                    Task.Run(() =>
                    {
                        while (true)
                        {
                            byte[] buff = new byte[1024];
                            int l = socket.Receive(buff);

                            string str = Encoding.UTF8.GetString(buff, 0, l);

                            if (str.Substring(0, 3) == "new")
                            {
                                str = str.Remove(0, 3);
                                User user = new User(str, socket);
                                Logger.Log(str+" подключается к серверу");
                                Users.Add(user);
                            }
                            else if (str.Substring(0, 4) == "mess")
                            {
                                str = str.Remove(0, 4);
                                var list = str.Split('.');

                                //list[0]=имя получателя
                                //list[1]=сообщение

                                //ищем отправителя по хендлу сокета от кого пришло сообщение
                                var sender = Users.Find(x => x.handler == socket);
                                //ищем получателя по имени с сообщения отправителя
                                var receiver = Users.Find(x => x.name == list[0]);

                                if (receiver == null || sender == null)
                                    continue;

                                Logger.Log(sender.name+" шлет сообщение "+receiver.name);

                                receiver.Send(sender.name,list[1]);                                
                            }
                            else if (str.Substring(0, 5) == "digit")
                            {
                                str = str.Remove(0, 5);

                                digit = int.Parse(str);

                                var sender = Users.Find(x => x.handler == socket);

                                Console.WriteLine(sender.name + " " + "Загадывает число...");
                            }
                            else if (str.Substring(0, 7) == "isdigit")
                            {
                                str = str.Remove(0, 7);

                                int _digit = int.Parse(str);

                                var sender = Users.Find(x => x.handler == socket);

                                Console.WriteLine(sender.name + " " + "Пробует число " + _digit.ToString());
                                if (digit == _digit)
                                    Console.WriteLine(sender.name + " " + "Отгадывает число!");
                            }
                        }
                    });
                }

            });
        }




    }
}
