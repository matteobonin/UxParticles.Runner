namespace UxParticles.Runner.Core.Service
{
    public interface IAnalysisQueue
    {
        void Add(IDependingJob jobToAdd);
    }
}