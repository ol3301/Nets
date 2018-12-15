using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private static StreetsEntities streets = new StreetsEntities();

        private static void Init()
        {
            streets.IND.Add(new IND
            {
                Index = 0
            });

            streets.STREETS.Add(new STREETS
            {
                Address = "Шевченка 6",
                IdIndex = 0
            });
            streets.STREETS.Add(new STREETS
            {
                Address = "Шевченка 7",
                IdIndex = 0
            });
            streets.STREETS.Add(new STREETS
            {
                Address = "Шевченка 8",
                IdIndex = 0
            });
            streets.STREETS.Add(new STREETS
            {
                Address = "Шевченка 9",
                IdIndex = 0
            });

            streets.SaveChanges();
        }

        static void Main(string[] args)
        { 


            Socket server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2020);

            server.Bind(ipPoint);
            server.Listen(100);
            Console.WriteLine("Сервер запущен");
            while (true)
            {
                Socket handle = server.Accept();

                Console.WriteLine("Подключился клиент");

                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байтов
                byte[] data = new byte[256];

                do
                {
                    bytes = handle.Receive(data);
                    builder.Append(Encoding.UTF8.GetString(data,0,bytes));
                } while (handle.Available > 0);

                Console.WriteLine("Запрашивает адреса по индексу: {0}",builder.ToString());

                int index = int.Parse(builder.ToString());

                var strs = from c in streets.STREETS
                    join h in streets.IND on c.IdIndex equals h.Id
                    where h.Index == index
                    select new
                    {
                        c.Address
                    };

                string buf = string.Empty;

                foreach (var item in strs)
                    buf += item + "\n";

                handle.Send(Encoding.UTF8.GetBytes(buf));

                Console.WriteLine("Отправили {0} адресов",strs.Count());

                handle.Shutdown(SocketShutdown.Both);
                handle.Close();
            }

            streets.Dispose();          
        }
    }
}
