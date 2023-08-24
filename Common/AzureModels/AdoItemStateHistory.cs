using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class AdoItemStateHistory
    {
        [JsonProperty("System.ChangedDate")]
        public DateTime Date { get; set; }
        [JsonProperty("System.State")]
        public string EntityState { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("System.Title")]
        public string Name { get; set; }
        [JsonProperty("System.IterationPath")]
        public string TeamIteration { get; set; }
        [JsonProperty("System.AreaPath")]
        public string TeamName { get; set; }
        [JsonProperty("System.WorkItemType")]
        public string ResourceType { get; set; }

        public static AdoItemStateHistory DeserializeJson(string jsonString)
        {
            AdoItemStateHistory adoItemStateHistory = new AdoItemStateHistory();
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(jsonString);
            adoItemStateHistory.Date = dynamicObject.fields["System.ChangedDate"];
            adoItemStateHistory.Id = dynamicObject.id;
            adoItemStateHistory.Name = dynamicObject.fields["System.Title"];
            adoItemStateHistory.ResourceType = dynamicObject.fields["System.WorkItemType"];
            adoItemStateHistory.TeamName = dynamicObject.fields["System.AreaPath"];
            adoItemStateHistory.TeamIteration = dynamicObject.fields["System.IterationPath"];
            adoItemStateHistory.EntityState = dynamicObject.fields["System.State"];
            return adoItemStateHistory;
        }

    }

    
}
