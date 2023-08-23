using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.TeamCityModels
{
    public class TeamCityBuilds
    {
        [JsonProperty("buildType")]
        public TeamCityBuildType buildType { get; set; }

        [JsonProperty("startDate")]
        public string startDate { get; set; }
    }
}
