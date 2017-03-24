namespace UxParticles.Runner.Core.Service.Dependency
{
    using System;
    using System.Collections.Generic;

    public interface IStaticJobDependencyMapper
    {
        Type Destination { get; }

        Type Source { get; }

        IEnumerable<object> MapFrom(object parentJob);
    }

    public interface IStaticJobDependencyMapper<in TJobIn> : IStaticJobDependencyMapper
        where TJobIn : class
    {
        IEnumerable<object> MapFrom(TJobIn parentJob);
    }

    public interface IStaticJobDependencyMapper<in TJobIn, out TJobOut> : IStaticJobDependencyMapper<TJobIn>
        where TJobIn : class where TJobOut : class
    {
        IEnumerable<TJobOut> MapFrom(TJobIn parentJob);
    }
}