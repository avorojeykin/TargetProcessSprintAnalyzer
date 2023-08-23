using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CommitAllTeamList
    {
        [JsonProperty("items")]
        public List<CommitTargetProcessItem> _teams { get; set; }
    }
}
