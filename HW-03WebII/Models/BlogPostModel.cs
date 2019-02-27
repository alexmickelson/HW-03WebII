using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HW_03WebII.Models
{
    public class BlogPostModel
    {
        [Required, Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public DateTime Posted { get; set; }
        
        [NotMapped]
        public string Tags { get; set; }

        public List<BlogTags> BlogTags { get; set; }

    }
}
