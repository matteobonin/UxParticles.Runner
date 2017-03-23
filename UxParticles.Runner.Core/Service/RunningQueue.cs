namespace UxParticles.Runner.Core.Service
{
    using UxParticles.Runner.Core.Service.Runner;

    /// <summary>
    /// We have entries that have no dependencies
    /// they always go on top of queue
    /// then there are entries that can run once some dependencies are done which means they depend on the execution of something
    /// once that somthin is done they can execute
    /// 
    /// In a normal case we do get an execution root with priority, invalidation, foce etc. then there is an engine that 
    /// decides what goes on the stack and once on the stack, associated with the root request they run in the original 
    /// order.
    /// 
    /// if we encounter a scenario where an invalidation occurrs and the entry is affected, then the engine must be able
    /// to push more on the execution stack
    /// 
    /// 
    /// </summary>
    public class RunningQueue
    {
        public void Enqueue(IDependingJob jobToExecute, bool forceExecution)
        {
            // here we send stuff in the queue to be executed

        }

        public void DequeueNext()
        {
			
        }
    }
}