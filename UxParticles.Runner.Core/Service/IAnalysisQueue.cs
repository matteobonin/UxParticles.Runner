namespace UxParticles.Runner.Core.Service
{
    using UxParticles.Runner.Core.Service.Runner;

    public interface IAnalysisQueue
    {
        void Add(IDependingJob jobToAdd);
    }
}