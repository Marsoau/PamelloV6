using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PamelloV6.Core.DTO
{
    public class SpeakerDTO
    {
        [JsonPropertyName("guildName")]
        public string GuildName { get; set; }

        [JsonPropertyName("vcName")]
        public string VCName { get; set; }
    }
}
