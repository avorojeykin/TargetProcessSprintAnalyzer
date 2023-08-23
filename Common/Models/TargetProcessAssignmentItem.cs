using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessAssignmentItem
    {
        [JsonProperty("ResourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("Id")]
        public int Id { get; set; }    
        [JsonProperty("GeneralUser")]
        public TargetProcessAssignmentGeneralUser GeneralUser { get; set; }
        [JsonProperty("Role")]
        public TargetProcessAssignmentRole Role { get; set; }
    }
}
