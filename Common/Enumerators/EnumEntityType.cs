using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enumerators
{
    public enum EnumEntityType
    {
        [Description("UserStory")]
        UserStorySingle,
        [Description("Bug")]
        BugSingle,
        [Description("Task")]
        TaskSingle,
        [Description("Request")]
        RequestSingle,
        [Description("UserStories")]
        UserStoryPlural,
        [Description("Bugs")]
        BugPlural,
        [Description("Tasks")]
        TaskPlural,
        [Description("Requests")]
        RequestPlural
    }
}
