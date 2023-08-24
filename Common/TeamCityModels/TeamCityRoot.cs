using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.TeamCityModels
{
    public class TeamCityRoot
    {
        [JsonProperty("build")]
        public List<TeamCityBuilds> Builds { get; set; }
    }
}
