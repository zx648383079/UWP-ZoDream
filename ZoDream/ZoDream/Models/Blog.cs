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
        public string Keywords { get; set; }
        [JsonProperty(PropertyName = "programming_language")]
        public string ProgrammingLanguage { get; set; }
        public string Language { get; set; }
        [JsonProperty(PropertyName = "parent_id")]
        public int ParentId { get; set; }
        public string Thumb { get; set; }
        [JsonProperty(PropertyName = "edit_type")]
        public int EditType { get; set; }
        public string Content { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }
        [JsonProperty(PropertyName = "term_id")]
        public int TermId { get; set; }
        public int Type { get; set; }
        [JsonProperty(PropertyName = "source_url")]
        public string SourceUrl { get; set; }
        public int Recommend { get; set; }
        [JsonProperty(PropertyName = "comment_count")]
        public int CommentCount { get; set; }
        [JsonProperty(PropertyName = "click_count")]
        public int ClickCount { get; set; }
        [JsonProperty(PropertyName = "comment_status")]
        public int CommentStatus { get; set; }
        [JsonProperty(PropertyName = "deleted_at")]
        public int DeletedAt { get; set; }
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty(PropertyName = "updated_at")]
        public string UpdatedAt { get; set; }
        public string Url { get; set; }
        public Term Term { get; set; }
        public User User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string PageTitle;
        
    }
}
