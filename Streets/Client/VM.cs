using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;

namespace Client
{
    public class VM : BindableBase
    {
        private DelegateCommand _RequestAddressCommand;
        private DelegateCommand _ConnectCommand;

        private Socket handle;
        private IPEndPoint ipend;

        public string IdAddress { get; set; }
        public string Area { get; set; }
        
        public DelegateCommand ConnectCommand
        {
            get => _ConnectCommand ?? (_ConnectCommand = new DelegateCommand(async () =>
            {
                await Task.Run(() =>
                {
                    handle.Connect(ipend);
                });
            }));
        }
        public DelegateCommand RequestAddressCommand
        {
            get => _RequestAddressCommand ?? (_RequestAddressCommand = new DelegateCommand(async () =>
            {
                Area = "";
                await Task.Run(() =>
                {
                    byte[] buff = Encoding.UTF8.GetBytes(IdAddress);
                    handle.Send(buff,buff.Length,0);

                    StringBuilder builder = new StringBuilder();
                    byte[] recv = new byte[256];
                    int len;

                    do
                    {
                        len = handle.Receive(recv);
                        builder.Append(Encoding.UTF8.GetString(recv,0,len));
                    } while (handle.Available > 0);

                    Area = builder.ToString();
                });
            }));
        }

        public VM()
        {
            ipend = new IPEndPoint(IPAddress.Parse("127.0.0.1"),2020);
            handle = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        }
    }
}
