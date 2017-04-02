namespace UxParticles.Runner.Core.Service.Dependency
{
    using System;
    using System.Collections.Generic;

    public interface IStaticDependencyMapperService
    {
        void AddStaticDependencyMapper(IStaticJobDependencyMapper mapper);

        IEnumerable<Type> MappedTypes { get; }

        IEnumerable<Type> UnmappedTypes { get; }        
    }
}