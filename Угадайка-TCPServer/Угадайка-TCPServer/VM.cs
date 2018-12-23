using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;

namespace Угадайка_TCPServer
{
    public class VM : BindableBase
    {
        private DelegateCommand _RequestAddressCommand;
        private DelegateCommand _ConnectCommand;
        private DelegateCommand _NewDigitCommand;
        private DelegateCommand _IsDigit;

        private Socket handle;
        private IPEndPoint ipend;

        public string IdAddress { get; set; }
        public string Area { get; set; }
        public string Name { get; set; }
        public int Digit { get; set; }
        
        public DelegateCommand IsDigit
        {
            get => _IsDigit ?? (_IsDigit = new DelegateCommand(async ()=>
            {
                await SendCommand("isdigit" + Digit);
            }));
        }
        public DelegateCommand NewDigitCommand
        {
            get => _NewDigitCommand ?? (_NewDigitCommand = new DelegateCommand(async () =>
            {
                await SendCommand("digit"+Digit);
            }));
        }
        public DelegateCommand ConnectCommand
        {
            get => _ConnectCommand ?? (_ConnectCommand = new DelegateCommand(async () =>
            {
                await Task.Run(async () =>
                {
                    handle.Connect(ipend);
                    await SendCommand("new"+Name);
                });
            }));
        }

        public async Task SendCommand(string com)
        {
            await Task.Run(() =>
            {
                byte[] buf = Encoding.UTF8.GetBytes(com);
                handle.Send(buf);
            });
        }

        public VM()
        {
            ipend = new IPEndPoint(IPAddress.Parse("127.0.0.1"),2020);
            handle = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        }
    }
}
