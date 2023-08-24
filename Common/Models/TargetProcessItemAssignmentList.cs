using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessItemAssignmentList
    {
        [JsonProperty("ResourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("Assignments")]
        public TargetProcessAssignmentItemInnerList Assignments { get; set; }
    }
}
