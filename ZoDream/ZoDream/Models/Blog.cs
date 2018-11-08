using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZoDream.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
