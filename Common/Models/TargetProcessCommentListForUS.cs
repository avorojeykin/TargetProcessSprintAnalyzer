using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessCommentListForUS
    {
        [JsonProperty("next")]
        public string ignoredField { get; set; }
        [JsonProperty("items")]
        public List<TargetProcessCommentItemsForUS> Items { get; set; }
    }
}
