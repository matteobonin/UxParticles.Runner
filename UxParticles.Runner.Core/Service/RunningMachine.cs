using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service
{
    public class RunningMachine
    {
        /// <summary>
        /// This is the Facade of the system. Add a running Request means that the process can start
        /// </summary>
        /// <returns><c>true</c>, if running request was added, <c>false</c> otherwise.</returns>
        /// <param name="request">Request.</param>
        public bool AddRunningRequest(RunRequest request)
        {
			
            return false;
        }


        /// <summary>
        /// Runs a request and awaits its result
        /// </summary>
        /// <returns>The one.</returns>
        /// <param name="request">Request.</param>
        public async Task<RunnerState> ExecuteOne(RunRequest request)
        {
            return null;
        }

    }
}