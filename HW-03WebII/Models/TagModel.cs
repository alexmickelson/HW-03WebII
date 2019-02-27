using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HW_03WebII.Models
{
    public class TagModel
    {
        [Key]
        public string Name { get; set; }

        public List<BlogTags> BlogTags { get; set; }
    }
}
