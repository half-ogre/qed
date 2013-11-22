using Newtonsoft.Json;

namespace qed
{
    public class Repository
    {
        [JsonProperty(PropertyName = "html_url")]
        public string HtmlUrl { get; set; }
        
        public string Name { get; set; }
        
        public Owner Owner { get; set; }
        
        public string Url { get; set; }
    }
}