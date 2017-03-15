using System.Collections;

namespace UxParticles.Runner.Core.Service
{
    public interface IDependingJob
    {
        /// <summary>
        /// List of ARGS that need to be ready
        /// </summary>
        /// <returns>The prerequisites.</returns>
        IEnumerable GetPrerequisites();
    }
}