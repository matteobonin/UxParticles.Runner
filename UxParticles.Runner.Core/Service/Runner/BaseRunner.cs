using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service.Runner
{
    using UxParticles.Runner.Core.Service.Dependency;
    using UxParticles.Runner.Core.Service.Runner.Enum;

    public class BaseRunner<TArgs> where TArgs : class
    {
        private readonly List<IStaticJobDependencyMapper<TArgs>> staticMappers;

        private readonly IDependencyBroker dependencyBroker;

        private readonly IRunnerDataAccess runnerDataAccess;
        
        public BaseRunner(
            IEnumerable<IStaticJobDependencyMapper<TArgs>> staticMappers,
            IDependencyBroker dependencyBroker,
            IRunnerDataAccess runnerDataAccess)
        {
            this.staticMappers = staticMappers?.ToList() ?? new List<IStaticJobDependencyMapper<TArgs>>();
            this.dependencyBroker = dependencyBroker;
            this.runnerDataAccess = runnerDataAccess;
        }

        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/></exception>
        public RunnerOutcome Run(RunningRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Args == null)
            {
                throw new ArgumentNullException(nameof(request.Args));
            }

            var outcome = this.EnsureCanBeginRunnerOperations(request);
            if (outcome.HasValue)
            {
                return outcome.Value;                
            }

            outcome = this.EnsureDependenciesAreSatisfied(request);
            if (outcome.HasValue)
            {
                return outcome.Value;
            }

            return this.ExecuteRunnerOperations(request);
        }

        private RunnerOutcome ExecuteRunnerOperations(RunningRequest request)
        {
            Exception exception = null;
            try
            {
                this.OnExecuteRun(request);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return this.EndRunnerOperations(request, exception);
        }

        private RunnerOutcome EndRunnerOperations(RunningRequest request, Exception exception)
        {
            RunnerStatus status = exception == null ? RunnerStatus.Success : RunnerStatus.Error;

            this.runnerDataAccess.UpdateRunnerState(request, status, exception);

            if (status == RunnerStatus.Error)
            {
                return RunnerOutcome.Error;
            }
            else
            {
                return RunnerOutcome.Completed;
            }
        }

        private void OnExecuteRun(RunningRequest request)
        {
          
        }

        private RunnerOutcome? EnsureCanBeginRunnerOperations(RunningRequest request)
        {
            var currentrunnerStatus = this.runnerDataAccess.GetRunnerStatus(request);
            if (currentrunnerStatus == RunnerStatus.Running)
            {
                return RunnerOutcome.AlreadyRunning;
            }

            if (request.Mode == RunningMode.InvalidateOnly)
            {
                this.runnerDataAccess.UpdateRunnerState(request, RunnerStatus.Invalidated, null);
                return RunnerOutcome.InvalidatedOnly;
            }

            if (currentrunnerStatus == RunnerStatus.Success)
            {
                if (request.Mode != RunningMode.ForceDependendentsAndRunner
                    && request.Mode != RunningMode.ForceRunnerOnly)
                {
                    return RunnerOutcome.AlreadyCompleted;
                }
            }
            
            this.runnerDataAccess.UpdateRunnerState(request, RunnerStatus.Running, null);
            return null;
        }

        private RunnerOutcome? EnsureDependenciesAreSatisfied(RunningRequest request)
        {
            var requestArgs = request.Args;
            var dependencies = this.staticMappers.SelectMany(x => x.MapFrom(requestArgs)).ToList();
            if (dependencies.Count == 0)
            {
                return null;
            }

            if (!this.dependencyBroker.QueueDependencyRequests(request, dependencies))
            {
                return null;
            }

            return RunnerOutcome.Awaiting;
        }
    }
}
