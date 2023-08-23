using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessEntityPropertyUser
    {
        [JsonProperty("firstName")]
        public string firstName { get; set; }
        [JsonProperty("lastName")]
        public string lastName { get; set; }
    }
}
