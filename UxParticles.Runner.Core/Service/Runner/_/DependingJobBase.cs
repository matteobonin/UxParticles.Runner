namespace UxParticles.Runner.Core.Service.Runner._
{
    using System;
    using System.Collections.Generic;

    public abstract class DependingJobBase<T> : IDependingJob
    {
        protected DependingJobBase(T args)
        {
            this.Args = args;
        }

        public T Args { get; }

        object IDependingJob.Args
        {
            get
            {
                return this.Args;
            }
        }

        public abstract IEnumerable<IDependingJob> GetPrerequisites();

        bool IEquatable<IDependingJob>.Equals(IDependingJob other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.GetType() != this.GetType())
            {
                return false;
            }

            return true;            
        }

        IEnumerable<Func<IDependingJob>> IDependingJob.GetDynamicPrerequisites()
        {
            throw new NotImplementedException();
        }

        protected abstract bool OnEqualProperties(DependingJobBase<T> dependingJobBase);
    }
}