using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client c = new Client(2020,"127.0.0.1");
            c.Connect();
            Console.WriteLine("Подключились к серверу ");
            Console.WriteLine("Введите свой ник:");
            string name = Console.ReadLine();
            c.Send("new"+name);
            Console.WriteLine("Авторизовались");

            //читаем сообщения
            Task.Run(()=>
            {
                while (true)
                {
                    byte[] buff = new byte[1024];
                    int l = c.handler.Receive(buff);

                    string str = Encoding.UTF8.GetString(buff, 0, l);

                    var list = str.Split('.');
                    Console.WriteLine("Пришло сообщения от: {0}, \n {1}", list[0], list[1]);
                }
            });

            while (true)
            {
                Console.WriteLine("Введите имя получателя: ");
                string receivername = Console.ReadLine();

                Console.WriteLine("Введите сообжение: ");
                string mess = Console.ReadLine();

                c.Send("mess"+receivername+"."+mess);
                Console.WriteLine("Отправлено");
            }
            Console.ReadLine();
        }
    }
}
