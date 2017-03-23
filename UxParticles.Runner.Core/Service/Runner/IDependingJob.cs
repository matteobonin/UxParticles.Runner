namespace UxParticles.Runner.Core.Service.Runner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IDependingJob : IEquatable<IDependingJob>
    {
        /// <summary>
        /// List of ARGS that need to be ready
        /// </summary>
        /// <returns>The prerequisites.</returns>
        IEnumerable<IDependingJob> GetPrerequisites();

        /// <summary>
        /// List of potential jobs that could be requested, but 
        /// information is provided at runtime
        /// </summary>
        /// <returns>A list of factories</returns>
        IEnumerable<Func<IDependingJob>> GetDynamicPrerequisites(); 

        object Args { get; }
      
    } 

    public sealed class NullJob : IDependingJob
    {
        public static NullJob Instance= new NullJob();

        private  NullJob()
        {
            
        }
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable GetPrerequisites()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Func<IDependingJob>> GetDynamicPrerequisites()
        {
            throw new NotImplementedException();
        }

        public object Args { get; set; }

        public bool Equals(IDependingJob other)
        {
            return other is NullJob;
        }

        IEnumerable<IDependingJob> IDependingJob.GetPrerequisites()
        {
            throw new NotImplementedException();
        }
    }
}