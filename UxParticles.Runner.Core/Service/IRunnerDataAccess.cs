namespace UxParticles.Runner.Core.Service
{
    using System;
    using UxParticles.Runner.Core.Service.Runner;
    using UxParticles.Runner.Core.Service.Runner.Enum;
    using UxParticles.Runner.Core.Service.Runner._;

    /// <summary>
    /// This is responsible for the persistence of each job's status
    /// </summary>
    public interface IRunnerDataAccess
    {
        RunnerState GetRunnerState(IDependingJob job);

        void UpdateRunnerState(IDependingJob job, RunnerState newState);
        void UpdateRunnerState(RunningRequest request);
        void UpdateRunnerState(RunningRequest request, RunnerStatus status, Exception exception);

        bool IsRunning(RunningRequest request);

        RunnerStatus GetRunnerStatus(RunningRequest request);
    }
}