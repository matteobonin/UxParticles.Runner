namespace UxParticles.Runner.Core.Service
{
    using UxParticles.Runner.Core.Service.Runner;
    using UxParticles.Runner.Core.Service.Runner._;

    public interface IAnalysisQueue
    {
        void Add(IDependingJob jobToAdd);
    }
}