using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("teamiteration")]
        public TargetProcessEntityPropertyName TeamIteration { get; set; }
        [JsonProperty("entitystate")]
        public TargetProcessEntityPropertyIdName State { get; set; }
    }
}
