using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetExamChatClient
{
    public class Com
    {
        public const string NewServer = "NewServer";
        public const string NewClient = "NewClient";
        public const string AuthOk = "AuthOk";
        public const string AuthNo = "AuthNo";
        public const string NotServer = "NotServer";
        public const string ServerOnline = "ServerOnline";

        public const string NewSession = "NewSession";
        public const string DestroyTransfer = "DestroyTransfer";
        public const string DestroySession = "DestroySession";
        public const string NewTransfer = "NewTransfer";
        public const string UpdateTransfer = "UpdateTransfer";
        public const string TransferReady = "TransferReady";
        public const string RemoveTransfer = "RemoveTransfer";

        //

        public const string ActiveUsers = "ActiveUsers";
        public const string MessText = "MessText";
    }

    class StructMessage
    {
        public string title { get; set; }
        public string type { get; set; } = "Null";
        public Dictionary<string, string> param { get; set; }

        public StructMessage(string title, string type)
        {
            this.title = title;
            this.type = type;

            param = new Dictionary<string, string>();
        }
        public StructMessage(string data)
        {
            title = Help.FindText(ref data, ',');
            type = Help.FindText(ref data, ',');

            if (data.Length > 0)
                param = new Dictionary<string, string>(
                    data.Split(';')
                    .Select(part => part.Split('='))
                    .Where(part => part.Length == 2)
                    .ToDictionary(sp => sp[0], sp => sp[1]));
            else
                param = new Dictionary<string, string>();
        }
        public StructMessage()
        {
            param = new Dictionary<string, string>();
        }

        public void AddParam(string key, string value)
        {
            param.Add(key, value);
        }
        public void RemoveParam(string key)
        {
            param.Remove(key);
        }
        public override string ToString()
        {
            string par = "";
            foreach (KeyValuePair<string, string> i in param)
                par += i.Key + "=" + i.Value + ";";

            return $"{title},{type},{par}";
        }

        public string this[string i]
        {
            get { return param[i]; }
            set { param[i] = value; }
        }
    }

    class Help
    {
        static Random rnd = new Random();
        public static string GenId(int len)
        {
            string res = "";
            for (int i = 0; i < len; ++i)
                res += rnd.Next(9).ToString();
            return res;
        }
        public static string FindText(ref string buf, char ch)
        {
            int pos = buf.IndexOf(ch);
            string res = "";
            if (pos > 0)
            {
                res = buf.Substring(0, pos);
                buf = buf.Remove(0, pos + 1);
            }
            return res;
        }
    }
}
