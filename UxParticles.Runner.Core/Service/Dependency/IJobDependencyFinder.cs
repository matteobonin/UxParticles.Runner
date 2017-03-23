namespace UxParticles.Runner.Core.Service.Dependency
{
    using System;
    using System.Collections.Generic;

    using UxParticles.Runner.Core.Service.Runner;

    public interface IJobDependencyFinder
    {
        /// <summary>
        /// the <see cref="IDependencyJob"/> this finder is related to
        /// </summary>
        IDependingJob RelatedTo { get; }

        /// <summary>
        /// List of ARGS that need to be ready
        /// </summary>
        /// <returns>The prerequisites.</returns>
        IEnumerable<IDependingJob> Prerequisites { get; }

        /// <summary>
        /// List of potential jobs that could be requested, but 
        /// information is provided at runtime
        /// </summary>
        /// <returns>A list of factories</returns>
        IEnumerable<Func<IDependingJob>> GetDynamicPrerequisites();
    }


}