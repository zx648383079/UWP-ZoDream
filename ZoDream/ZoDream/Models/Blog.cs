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

        public string Description { get; set; }
        
        public string Content { get; set; }

        public int Type { get; set; } = 0;

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        public int Recommend { get; set; }

        [JsonProperty("click_count")]
        public int ClickCount { get; set; }
        
    }
}
