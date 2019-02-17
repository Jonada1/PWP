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

        public async Task ReplaceTest(Guid testId, TestAnalysis newTest)
        {
            var oldTest = await _context.TestAnalyses.FindAsync(testId);
            _context.TestAnalyses.Remove(oldTest);
            _context.TestAnalyses.Add(newTest);
            await _context.SaveChangesAsync();
        }

        public async Task EditTest(TestAnalysis editedTest)
        {
            var found = await _context.TestAnalyses.FindAsync(editedTest.Id);
            _context.Entry(found).CurrentValues.SetValues(editedTest);
            _context.Entry(found).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }
    }
}
