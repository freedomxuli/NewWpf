using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DemoPlugin.Model
{
    public class Course
    {
        [Key]
        public Guid ModelId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
