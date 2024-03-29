﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.TeamCityModels
{
    public class TeamCityBuildType
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }
    }
}
