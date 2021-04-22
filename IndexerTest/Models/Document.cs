using System;
using Newtonsoft.Json;

namespace IndexerTest.Models
{
    public class Document
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
