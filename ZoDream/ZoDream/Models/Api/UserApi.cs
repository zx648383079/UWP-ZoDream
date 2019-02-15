using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Models.Api
{
    class UserApi: BaseApi
    {
        public async Task<Tuple<string, User>> Login(string email, string password)
        {
            var args = new JObject();
            args["email"] = email;
            args["password"] = password;
            var data = await GetAsync<User>($"auth/login", args);
            if (data == null)
            {
                return null;
            }
            return Tuple.Create<string, User>(data.Token.ToString(), data);
        }

        public async Task<User> CheckEmailAsync(string email)
        {
            return await GetAsync<User>($"auth/check?email={email}");
        }
    }
}
