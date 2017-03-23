namespace UxParticles.Runner.Core.Service.Runner
{
    using System;
    using System.Collections.Generic;

    public interface IStaticJobDependencyMapper 
    {
        IEnumerable<IDependingJob> MapFrom(IDependingJob parentJob);

        Type Destination { get; }

        Type Source { get; }
    }

    public interface IStaticJobDependencyMapper<in TJobIn> : IStaticJobDependencyMapper where TJobIn : IDependingJob
    {
        IEnumerable<IDependingJob> MapFrom(TJobIn parentJob);
    }

    public interface IStaticJobDependencyMapper<in TJobIn, out TJobOut> : IStaticJobDependencyMapper<TJobIn>
        where TJobIn : IDependingJob
         where TJobOut : IDependingJob
    {
        IEnumerable<TJobOut> MapFrom(TJobIn parentJob);
    }
}