using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessTeamMemberItem
    {
        [JsonProperty("user")]
        public TargetProcessEntityPropertyUser User { get; set; }

        [JsonProperty("role")]
        public TargetProcessEntityPropertyRole role { get; set; }

        [JsonProperty("team")]
        public TargetProcessEntityPropertyTeam team { get; set; }
    }
}
