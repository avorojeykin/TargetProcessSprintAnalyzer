using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessAssignmentGeneralUser
    {
        [JsonProperty("ResourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("Kind")]
        public string Kind { get; set; }
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }
        [JsonProperty("LastName")]
        public string LastName { get; set; }
        [JsonProperty("Login")]
        public string Login { get; set; }
        [JsonProperty("FullName")]
        public string FullName { get; set; }
    }
}
