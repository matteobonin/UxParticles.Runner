namespace UxParticles.Runner.Core.Service.Runner._
{
    using System.Threading.Tasks;

    public interface IRunnerExec
    {
        Task<RunnerState> RunAsync(IDependingJob jobToRun, bool forceRun);
    }
}