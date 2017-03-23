namespace UxParticles.Runner.Core.Service.Runner
{
    using System;
    using System.Collections.Generic;

    public abstract class StaticJobDependencyMapperBase<TIn, TOut> : IStaticJobDependencyMapper<TIn, TOut>
        where TIn : class, IDependingJob
        where TOut : class, IDependingJob
    {
        public IEnumerable<IDependingJob> MapFrom(TIn parentJob)
        {
            return this.OnMapFromJob(parentJob);
        }

        protected abstract IEnumerable<TOut> OnMapFromJob(TIn parentJob);

        public IEnumerable<IDependingJob> MapFrom(IDependingJob parentJob)
        {
            return this.OnMapFromJob(parentJob as TIn);
        }

        IEnumerable<TOut> IStaticJobDependencyMapper<TIn, TOut>.MapFrom(TIn parentJob)
        {
            return this.OnMapFromJob(parentJob);
        }

        public Type Destination => typeof(TOut);

        public Type Source => typeof(TIn);
    }
}