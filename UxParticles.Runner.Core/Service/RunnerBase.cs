using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service
{
    public class RunnerBase
    {
        IRunnerDataAccess dataAccess;

        public RunnerBase(IRunnerDataAccess dataAcess)
        {
            this.dataAccess = dataAcess;
        }



        public async Task<RunnerState> HandleAsync(RunRequest request, IEnumerable<RunnerState> dependencies)
        {
            // when it arrives here I know the request has satisfied the static dependencies
            // but it may well still require the dynamic ones or new ones might arise at any time
            try
            {
                this.dataAccess.UpdateRunnerState(request.JobToExecute, RunnerState.GetStarted(request));

            }
            catch(Exception ex)
            {
                return new RunnerState
                {
                    StateInformation = new StateInformation
                    {
                        Exception = ex,
                        DateTime = DateTime.UtcNow,

                    },
                    Status = RunnerExecutionStatus.Failed
                };
            }

            //this.dataAccess.UpdateRunnerState(request.JobToExecute, RunnerState.GetDone(request));
            return null;
        }

        protected async Task<RunnerState> GetDependencyDone()
        {
            throw new NotImplementedException();
        }

    }
}