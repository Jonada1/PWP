using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Services.Lifetimes;

namespace Limping.Api.DataMigrations
{
    public abstract class BaseDataMigration : ITransientService
    {
        public virtual string Id => GetType().Name;

        public abstract Task Apply();
    }
}
