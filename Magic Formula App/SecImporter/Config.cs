using System.Text.Json.Serialization;

namespace SecImporter
{
    public class Config
    {
        [JsonPropertyName("stop")]
        public string Stop { get; set; }
    }
}
