using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AzureModels
{
    public class AdoItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("System.WorkItemType")]
        public string ResourceType { get; set; }
        [JsonProperty("System.AreaPath")]
        public string TeamName { get; set; }
        [JsonProperty("System.IterationPath")]
        public string TeamIteration { get; set; }
        [JsonProperty("System.State")]
        public string State { get; set; }

        public static AdoItem DeserializeJson(string jsonString)
        {            
            AdoItem adoItem = new AdoItem();
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(jsonString);
            adoItem.Id = dynamicObject.id;
            adoItem.ResourceType = dynamicObject.fields["System.WorkItemType"];
            adoItem.TeamName = dynamicObject.fields["System.AreaPath"];
            adoItem.TeamIteration = dynamicObject.fields["System.IterationPath"];
            adoItem.State = dynamicObject.fields["System.State"];
            return (adoItem); 
        }
    }
}
