using System;

namespace UxParticles.Runner.Core.Service
{
    public class RunnerState 
    { 

        internal static RunnerState GetStarted(object request0)
        {
            throw new NotImplementedException();
        }

        public RunnerExecutionStatus Status { get; set; }

        public DateTime LastCompletedOn { get; set; }

        public StateInformation StateInformation { get; internal set; }

    }
}