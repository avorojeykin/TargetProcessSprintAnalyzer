using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessTaskItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("effort")]
        public double Effort { get; set; }
        [JsonProperty("entityState")]
        public TargetProcessTaskEntityState EntityState { get; set; }
        [JsonProperty("assignedUser")]
        public TargetProcessTaskAssignedUserList AssignedUsers { get; set; }
    }
}
