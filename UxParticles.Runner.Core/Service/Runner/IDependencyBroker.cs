namespace UxParticles.Runner.Core.Service.Runner
{
    using System.Collections.Generic;

    using UxParticles.Runner.Core.Service.Runner.Events;

    public interface IDependencyBroker : IHandle<RunnerCompletedEvent>
    {
        bool QueueDependencyRequests(RunningRequest request, IEnumerable<object> dependencies);

        bool QueueRequest(RunningRequest request);
    }
}