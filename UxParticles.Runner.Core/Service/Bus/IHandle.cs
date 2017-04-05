namespace UxParticles.Runner.Core.Service.Runner
{
    public interface IHandle<in T> 
    {
        void Handle(T @event);
    }
}