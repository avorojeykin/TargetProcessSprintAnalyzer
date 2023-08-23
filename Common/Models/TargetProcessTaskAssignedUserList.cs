using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessTaskAssignedUserList
    {
        [JsonProperty("items")]
        public List<TargetProcessTaskAssignedUser> Items { get; set; }      
    }
}
