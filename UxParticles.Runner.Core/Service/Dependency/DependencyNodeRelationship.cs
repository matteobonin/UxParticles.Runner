namespace UxParticles.Runner.Core.Service.Dependency
{
    public class DependencyNodeRelationship
    {
        public DependencyNode To { get; set; }

        public IStaticJobDependencyMapper Mapper { get; set; }
    }
}