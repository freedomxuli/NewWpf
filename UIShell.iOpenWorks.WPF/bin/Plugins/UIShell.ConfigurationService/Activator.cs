using System;
using System.Collections.Generic;
using System.Text;
using UIShell.OSGi;

namespace UIShell.ConfigurationService
{
    public class Activator : IBundleActivator
    {
        public void Start(IBundleContext context)
        {
            context.AddService<IConfigurationService>(new ConfigurationService());
        }

        public void Stop(IBundleContext context)
        {
            
        }
    }
}
