namespace UxParticles.Runner.Core.Service
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AnalysisQueue : IAnalysisQueue
    {
        public void Add(IDependingJob jobToAdd)
        {
            // find all the dependig jobs
        }

        public async Task<Guid> AddJobAsync(IDependingJob jobToAdd, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}