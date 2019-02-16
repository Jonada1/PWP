using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.Services.Interfaces
{
    public interface ITestAnalysesService
    {
        Task ReplaceTest(Guid testId, TestAnalysis newTest);
        Task EditTest(TestAnalysis editedTest);
    }
}
