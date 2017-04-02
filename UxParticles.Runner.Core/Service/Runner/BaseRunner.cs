using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service.Runner
{
    using System.Security.Cryptography.X509Certificates;

    using UxParticles.Runner.Core.Service.Dependency;

    public enum RunnerStatus
    {
        Ready,
        Awaiting,
        Running,
    }

    public enum RunningMode
    {
        DoNotForce,
        ForceRunnerOnly,
        ForceDependendentsAndRunner,
        InvalidateOnly
    }

    public class RunningPeriod
    {
        
    }

    public class RunnerCompletedEvent { }

    public class DependencyBrokerResponse
    {
        bool IsSuccessful { get; set; }


    }

    public interface IHandle<in T> 
    {
        void Handle(T @event);
    }

    public interface IDependencyBroker : IHandle<RunnerCompletedEvent>
    {
        bool QueueDependencyRequests(RunningRequest request, IEnumerable<object> dependencies);

        bool QueueRequest(RunningRequest request);
    }

    public class RunningRequest
    {
        public object Args { get; set; }

        public RunningPeriod Period { get; set; }

        public RunningMode Mode { get; set; } = RunningMode.DoNotForce;
    }

    
    public class BaseRunner<TArgs> where TArgs : class
    {
        private readonly List<IStaticJobDependencyMapper<TArgs>> staticMappers;

        private readonly IDependencyBroker dependencyBroker;

        private readonly IRunnerDataAccess runnerDataAccess;

        public RunnerStatus Status { get; private set; }


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
        public  void Run(RunningRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Args == null)
            {
                throw new ArgumentNullException(nameof(request.Args));
            }

            this.UpdateRunnerStatus(request);

            if (!this.RequestDependencies(request))
            {
                return;
            }

            Exception exception = null;
            try
            {
                this.OnExecuteRun(request);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            this.UpdateFinalRunnerStatus(request, exception);
        }

        private void UpdateFinalRunnerStatus(RunningRequest request, Exception exception)
        {
             
        }

        private void OnExecuteRun(RunningRequest request)
        {
          
        }

        private void UpdateRunnerStatus(RunningRequest request)
        {
        }

        private bool RequestDependencies(RunningRequest request)
        {
            var requestArgs = request.Args;
            var dependencies = this.staticMappers.SelectMany(x => x.MapFrom(requestArgs)).ToList();
            if (dependencies.Count == 0)
            {
                return true;
            }

           return this.dependencyBroker.QueueDependencyRequests(request, dependencies);
        }
 
    }
}
