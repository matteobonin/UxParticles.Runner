namespace UxParticles.Runner.Core.Service.Dependency
{
    public class ValidationResult
    {
        public System.Exception Exception { get; set; }

        public bool? IsValidated { get; set; }

        public string Message { get; set; }
    }
}