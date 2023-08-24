using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CommitTargetProcessItem
    {
        public string DeveloperTeam { get; set; }
        public string CommitId { get; set; }
        public bool IsMerge { get; set; }
        public int? CommitUSId { get; set; }
        public GitCommitRef Commit { get; set; }
        public TargetProcessItem UserStory { get; set; }
        public TargetProcessItemStateHistory UserStoryHistory { get; set; }
        public bool IsInDevelopment { get; set; }
        public string Comments { get; set; }
    }
}
