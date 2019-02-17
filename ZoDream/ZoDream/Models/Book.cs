using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Cover { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }


        public static string PageTitle;
    }
}
