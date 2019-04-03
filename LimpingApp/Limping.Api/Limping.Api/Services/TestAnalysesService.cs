using System;
using System.Threading.Tasks;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Services
{
    public class TestAnalysesService: ITestAnalysesService
    {
        private readonly LimpingDbContext _context;

        public TestAnalysesService(LimpingDbContext context)
        {
            _context = context;
        }

        public async Task ReplaceTestAnalysis(Guid testId, TestAnalysis newTest)
        {
            var test = await _context.LimpingTests.Include(x => x.TestAnalysis).SingleAsync(x => x.Id == testId);

            if (test.TestAnalysis != null)
            {
                _context.TestAnalyses.Remove(test.TestAnalysis);
            }

            test.TestAnalysis = newTest;
            _context.Entry(test).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task EditTestAnalysis(TestAnalysis editedTest)
        {
            var found = await _context.TestAnalyses.FindAsync(editedTest.Id);
            _context.Entry(found).CurrentValues.SetValues(editedTest);
            _context.Entry(found).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }
    }
}
