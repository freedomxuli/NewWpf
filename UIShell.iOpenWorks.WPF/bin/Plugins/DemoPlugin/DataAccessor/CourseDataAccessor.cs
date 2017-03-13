using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using DemoPlugin.Model;

namespace DemoPlugin.DataAccessor
{
    public class CourseDataAccessor : DataAccessorBase<Course>
    {
        public override DbContext NewDbContext()
        {
            return new CourseDbContext();
        }
    }
}
