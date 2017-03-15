namespace UxParticles.Runner.Core.Service
{
    /// <summary>
    /// This is responsible for the persistence of each job's status
    /// </summary>
    public interface IRunnerDataAccess
    {
        RunnerState GetRunnerState(IDependingJob job);

        void UpdateRunnerState(IDependingJob job, RunnerState newState);

    }
}