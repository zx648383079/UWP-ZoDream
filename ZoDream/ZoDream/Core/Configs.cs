using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Helpers;

namespace ZoDream.Core
{
    public class Configs
    {
        const string TOKEN_KEY = "APP_TOKEN";

        internal const string HOST = "http://zodream.localhost/open/";

        internal const string APPID = "";

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
