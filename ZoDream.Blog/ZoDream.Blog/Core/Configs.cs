using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Blog.Helpers;

namespace ZoDream.Blog.Core
{
    public class Configs
    {
        const string TOKEN_KEY = "APP_TOKEN";

        internal const string HOST = "http://zodream.localhost/open/";

        private static Configs newInstance;

        private string token;

        public string Token
        {
            get { return token; }
            set {
                token = value;
                AppData.SetValue(TOKEN_KEY, token);
            }
        }


        public Configs()
        {
            newInstance = this;
            token = AppData.GetValue<string>(TOKEN_KEY);
        }



        public static Configs NewInstance()
        {
            if (null == newInstance)
            {
                newInstance = new Configs();
            }
            return newInstance;
        }
    }
}
