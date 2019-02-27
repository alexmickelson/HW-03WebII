using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW_03WebII.Models
{
    public class BlogTags
    {
        public string BlogId { get; set; }
        public string TagId { get; set; }

        public BlogPostModel Blog { get; set; }
        public TagModel Tag { get; set; }
    }
}
