using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW_03WebII.Models
{
    public class BlogPostModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public DateTime Posted { get; set; }

    }
}
