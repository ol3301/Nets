using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetExamChatClient
{
    public class VM : BindableBase
    {
        private TcpClient client;
        

        public string Ip { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string TextArea { get; set; }
        public string CurentMess { get; set; }
        public string Pass { get; set; }
        public ObservableCollection<string> Users { get; set; }
        private DelegateCommand _ConnectCommand;
        private DelegateCommand _SendCommand;

        public DelegateCommand SendCommand
        {
            get => _SendCommand ?? (_SendCommand = new DelegateCommand(async()=>
            {
                StructMessage mess = new StructMessage();
                mess.title = Com.MessText;
                mess.param["sender"] = Name;
                mess.param["mess"] = CurentMess;
                AppendArea(Name+": "+CurentMess);
                CurentMess = "";
                await SendStruct(mess);
            }));
        }
        public DelegateCommand ConnectCommand
        {
            get => _ConnectCommand ?? (_ConnectCommand = new DelegateCommand(async ()=>
            {
                try
                {
                    if (!client.Connected)
                        await client.ConnectAsync(IPAddress.Parse(Ip), Port);
                    await SignIn();
                    Reading();
                }
                catch(Exception ex)
                {
                    AppendArea("Ошибка. "+ex.Message);
                }
            }));
        }

        public VM()
        {
            client = new TcpClient();
            Users = new ObservableCollection<string>();
            Ip = "127.0.0.1";
            Port = 2020;
            Name = "1";
            Pass = "2";
        }

        private async Task SignIn()
        {
            StructMessage mess = new StructMessage();
            mess.title = Com.NewServer;
            mess["login"] = Name;
            mess["passw"] = Pass;
            await SendStruct(mess);
        }

        private void AppendArea(string text)
        {
            TextArea += text + "\n";
        }

        private async Task SendStruct(StructMessage mess)
        {
            await SendText(mess.ToString());
        }

        private async Task SendText(string text)
        {
            byte[] buff = Encoding.UTF8.GetBytes(text + "@");
            await SendBuff(buff);
        }

        private async Task SendBuff(byte[] buff)
        {
            NetworkStream stream = client.GetStream();            
            await stream.WriteAsync(buff, 0, buff.Length);           
        }

        private async void Reading()
        {
            NetworkStream stream = client.GetStream();          
            await Task.Run(async () =>
            {
                while (client.Connected)
                {
                    byte[] buff = new byte[client.ReceiveBufferSize];
                    int bytes = await stream.ReadAsync(buff, 0, buff.Length);
                    await CommandWorking(new StructMessage(Encoding.UTF8.GetString(buff, 0, bytes)));
                }
            });
        }

        private async Task CommandWorking(StructMessage mess)
        {
            if(mess.title == Com.NewServer)
            {
                if (mess.type == Com.AuthOk)
                    AppendArea("Подключены!");
                else if (mess.type == Com.AuthNo)
                    AppendArea("Не правельный логин или пароль");
                else if (mess.type == Com.ServerOnline)
                    AppendArea("В данный аккаунт уже выполнен вход");
            }
            else if(mess.title == Com.ActiveUsers)
            {
                Application.Current.Dispatcher.Invoke(new Action(()=>
                {
                    Users.Clear();
                    foreach (var str in mess.param)
                    {
                        Users.Add(str.Key);
                    }
                }));
            }
            else if(mess.title == Com.MessText)
            {
                AppendArea($"{mess["sender"]}: {mess["mess"]}");
            }
        }
    }
}
