namespace UxParticles.Runner.Core.Service.Runner
{
    using System;
    using System.Threading.Tasks;

    public class RunnerExec : IRunnerExec
    {
        IRunnerDataAccess dataAccess;

        IDependencyCrawler dependencyCrawler;

        public RunnerExec(IRunnerDataAccess dataAccess, IDependencyCrawler dependencyCrawler)
        {
            this.dataAccess = dataAccess;
            this.dependencyCrawler = dependencyCrawler;
        }

        public async Task<RunnerState> RunAsync(IDependingJob jobToRun, bool forceRun = false)
        {
            if (jobToRun == null)
            {
                throw new ArgumentNullException(nameof(jobToRun));
            }


            var jobToRunStatus = this.dataAccess.GetRunnerState(jobToRun);

            if (jobToRunStatus.Status == RunnerExecutionStatus.Success && !forceRun)
            {
                // nothing to do, done
                return jobToRunStatus;
            }

            if (jobToRunStatus.Status == RunnerExecutionStatus.Running)
            {
                if (!forceRun)
                {
                    // get an awaiter and wait for the execution
                    return await this.AcquireRunningTask(jobToRun);
                }
            }

            return await this.BeginRunningAsync(jobToRun, forceRun);
        }

        async Task<RunnerState> BeginRunningAsync(IDependingJob jobToRun, bool forceRun)
        {
            // pushes all dependencies on the queue
            await this.dependencyCrawler.PrepareAsync(jobToRun);

            return await this.AcquireRunningTask(jobToRun);
        }

        async Task<RunnerState> AcquireRunningTask(IDependingJob jobToRun)
        {
            var tcs = new TaskCompletionSource<RunnerState>(TaskCreationOptions.LongRunning);
            // todo: create an awaiter that can be cancelled
            return await tcs.Task;
        }
    }
}