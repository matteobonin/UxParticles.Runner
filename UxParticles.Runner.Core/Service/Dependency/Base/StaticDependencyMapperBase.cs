namespace UxParticles.Runner.Core.Service.Dependency.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Flags]
    public enum StaticMapperOptions
    {
        AllowAll = 0, 

        ThrowOnNullArguments = 1, 

        ThrowOnNullResult = 2
    }

    public abstract class StaticDependencyMapperBase<TIn, TOut> : IStaticJobDependencyMapper<TIn, TOut>
        where TIn : class where TOut : class
    {
        private readonly StaticMapperOptions options;

        protected StaticDependencyMapperBase()
            : this(StaticMapperOptions.AllowAll)
        {
        }

        protected StaticDependencyMapperBase(StaticMapperOptions options)
        {
            this.options = options;
        }

        public Type Destination => typeof(TOut);

        public Type Source => typeof(TIn);

        public IEnumerable<object> MapFrom(TIn parentJob)
        {
            return this.MapFromWrapper(parentJob);
        }

        public IEnumerable<object> MapFrom(object parentJob)
        {
            return this.MapFromWrapper(parentJob as TIn);
        }

        IEnumerable<TOut> IStaticJobDependencyMapper<TIn, TOut>.MapFrom(TIn parentJob)
        {
            return this.MapFromWrapper(parentJob);
        }

        protected abstract IEnumerable<TOut> OnMapFromInputArguments(TIn parentJob);

        [DebuggerStepThrough]
        private IEnumerable<TOut> MapFromWrapper(TIn parentJob)
        {
            if (parentJob == null && (StaticMapperOptions.ThrowOnNullArguments & this.options)!=0)
            {
                throw new ArgumentNullException(nameof(parentJob));
            }

            var results = this.OnMapFromInputArguments(parentJob);

            if (results == null && this.options.HasFlag(StaticMapperOptions.ThrowOnNullResult))
            {
                throw new InvalidOperationException($"Mapped result cannot be null");
            }

            return results;
        }
    }
}