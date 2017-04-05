namespace UxParticles.Runner.Core.Service.Runner
{
    using System;

    using UxParticles.Runner.Core.Service.Bus;
    using UxParticles.Runner.Core.Service.Runner.Enum;

    public class RunningRequest : IMessage<RunnerOutcome>
    {
        public Guid RunningRequestGuid { get; } = Guid.NewGuid();

        public object Args { get; set; }

        public RunningPeriod Period { get; set; } 

        public RunningMode Mode { get; set; } = RunningMode.DoNotForce;
    }
}