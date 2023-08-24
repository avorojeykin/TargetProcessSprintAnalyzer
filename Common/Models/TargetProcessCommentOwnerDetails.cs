using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessCommentOwnerDetails
    {
        [JsonProperty("resourceType")]
        public string resourceType { get; set; }
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("firstName")]
        public string firstName { get; set; }
        [JsonProperty("lastName")]
        public string lastName { get; set; }
        [JsonProperty("login")]
        public string login { get; set; }
        [JsonProperty("fullName")]
        public string fullName { get; set; }
    }
}
