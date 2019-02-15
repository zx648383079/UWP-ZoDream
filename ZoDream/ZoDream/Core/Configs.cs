using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Helpers;
using ZoDream.Models;

namespace ZoDream.Core
{
    public class Configs
    {
        const string TOKEN_KEY = "APP_TOKEN";

        internal const string HOST = "http://zodream.localhost/open/";

        internal const string APPID = "11543906547";
        internal const string SECRET = "012e936d3d3653b40c6fc5a32e4ea685";

        private static Configs _newInstance;

        private string _token;

        public string Token
        {
            get { return _token; }
            set {
                _token = value;
                AppData.SetValue(TOKEN_KEY, _token);
            }
        }

        public User User { get; set; }

        public bool IsGuest()
        {
            return string.IsNullOrEmpty(Token);
        }

        public Configs()
        {
            _newInstance = this;
            _token = AppData.GetValue<string>(TOKEN_KEY);
        }



        public static Configs NewInstance()
        {
            return _newInstance ?? (_newInstance = new Configs());
        }
    }
}
