using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessCommentsDetails
    {
        [JsonProperty("owner")]
        public TargetProcessCommentOwnerDetails commentOwnerDetails { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
