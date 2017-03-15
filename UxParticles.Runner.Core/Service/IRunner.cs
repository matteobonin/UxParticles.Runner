using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service
{
    public interface IRunner
    {
        Task<RunnerState> RunAsync(IDependingJob jobToRun, bool forceRun);
    }
}