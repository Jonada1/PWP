using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Configurations
{
    public class LimpingConfiguration
    {
        public LimpingServices Services { get; set; }
    }

    public class LimpingServices
    {
        public LimpingDatabase Database { get; set; }
    }

    public class LimpingDatabase
    {
        public string ConnectionString { get; set; }
        public int StartupConnectionTestRetryCount { get; set; }
    }
}
