﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TargetProcessCommentItemsForUS
    {
        [JsonProperty("comment")]
        public List<TargetProcessCommentsDetails> commentsDetails { get; set; }
    }
}
