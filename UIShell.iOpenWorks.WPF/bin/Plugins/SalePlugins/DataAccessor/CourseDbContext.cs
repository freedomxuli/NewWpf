using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SalePlugins.Model;

namespace SalePlugins.DataAccessor
{
    public class CourseDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public CourseDbContext()
            : base(BundleActivator.DbConnectionFactoryServiceTracker.IsServiceAvailable ? BundleActivator.DbConnectionFactoryServiceTracker.DefaultOrFirstService.Create() : null, true)
        {

        }
    }
}
