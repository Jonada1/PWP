using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.Services.Interfaces
{
    public interface ITestAnalysesService
    {
        Task ReplaceTestAnalysis(Guid testId, TestAnalysis newTest);
        Task EditTestAnalysis(TestAnalysis editedTest);
    }
}
