using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessItemStateHistory
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("entitystate")]
        public TargetProcessEntityPropertyIdName EntityState { get; set; }
        [JsonProperty("userstory")]
        public TargetProcessEntityPropertyIdName UserStory { get; set; }
        [JsonProperty("teamiteration")]
        public TargetProcessEntityPropertyIdName TeamIteration { get; set; }
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }
    }
}
