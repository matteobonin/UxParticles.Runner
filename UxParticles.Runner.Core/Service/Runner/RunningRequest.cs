namespace UxParticles.Runner.Core.Service.Runner
{
    using UxParticles.Runner.Core.Service.Runner.Enum;

    public class RunningRequest
    {
        public object Args { get; set; }

        public RunningPeriod Period { get; set; } 

        public RunningMode Mode { get; set; } = RunningMode.DoNotForce;
    }
}