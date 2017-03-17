using System;
using System.Collections;

namespace UxParticles.Runner.Core.Service
{
    public interface IDependingJob : IEquatable<IDependingJob>
    {
        /// <summary>
        /// List of ARGS that need to be ready
        /// </summary>
        /// <returns>The prerequisites.</returns>
        IEnumerable GetPrerequisites();
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

        public bool Equals(IDependingJob other)
        {
            return other is NullJob;
        }
    }
    
}