using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessItemStateHistoryList
    {
        [JsonProperty("items")]
        public List<TargetProcessItemStateHistory> Items { get; set; }
    }
}
