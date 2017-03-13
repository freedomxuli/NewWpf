using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SalePlugins.Model
{
    public class Course
    {
        [Key]
        public Guid ModelId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
