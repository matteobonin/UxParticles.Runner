namespace UxParticles.Runner.Core.Service.Runner
{
    using System.Threading.Tasks;

    public interface IRunner
    {
        Task<RunnerState> RunAsync(IDependingJob jobToRun, bool forceRun);
    }
}