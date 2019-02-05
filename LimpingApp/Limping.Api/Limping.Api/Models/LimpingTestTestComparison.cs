using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Models
{
    public class LimpingTestTestComparison
    {
        public Guid LimpingTestId { get; set; }
        public Guid TestComparisonId { get; set; }
        public virtual LimpingTest LimpingTest { get; set; }
        public virtual TestComparison TestComparison { get; set; }
    }
}
