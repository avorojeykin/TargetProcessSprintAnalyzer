using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessTaskEntityState
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("numericPriority")]
        public double NumericProperty { get; set; }
    }
}
