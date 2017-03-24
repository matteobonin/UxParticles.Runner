﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service.Dependency
{
    using System.Diagnostics;

    using UxParticles.Runner.Core.Service.Runner;

    public interface IStaticDependencyFinderService
    {
        bool ValidateRunner(IStaticJobDependencyMapper mapper);

        void AddStaticDependencyMapper(IStaticJobDependencyMapper mapper);

    }

    public class ValidationResult
    {
        public bool? IsValidated { get; set; }

        public string Message { get; set; }
    }

    public class StaticDependencyFinderService : IStaticDependencyFinderService
    {
        public class UnknownDependency
        {
            public Type Requestor { get; set; }

            public Type Requested { get; set; }
        }

        private List<UnknownDependency> unknownDependencies = new List<UnknownDependency>();

        private List<IStaticJobDependencyMapper> knownJobs = new List<IStaticJobDependencyMapper>();

        private Dictionary<IDependingJob, ICollection<IDependingJob>> dependenciesByJob =
            new Dictionary<IDependingJob, ICollection<IDependingJob>>();

        /// <summary>
        /// this is the collection of types that are needed but we know nothing about them
        /// the key is the source type and the collection is the mappers that are needed
        /// </summary>
        private Dictionary<IDependingJob, ICollection<IDependingJob>> missingDependenciesByJob =
            new Dictionary<IDependingJob, ICollection<IDependingJob>>();

        /// <summary>
        /// This is the reverse set, ie all the types this type depend on. so the key is required by all the reverse mappers
        /// </summary>
        private Dictionary<Type, ICollection<IStaticJobDependencyMapper>> reverseDependencies =
            new Dictionary<Type, ICollection<IStaticJobDependencyMapper>>();

        /// <summary>
        /// Given a type all the mappers into a diffrent type
        /// </summary>
        private Dictionary<Type, ICollection<IStaticJobDependencyMapper>> mappersByType =
            new Dictionary<Type, ICollection<IStaticJobDependencyMapper>>();

        public bool ValidateRunner(IStaticJobDependencyMapper jobToValidate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDependingJob> GetDependencies(IDependingJob job)
        {
            throw new NotImplementedException();
        }

        public void AddStaticDependencyMapper(IStaticJobDependencyMapper mapper)
        {
            this.ThrowIfRunnerIsNotValidated(mapper);

            this.EnsureRemoveRunnerMapperFromListOfUnknowns(mapper);
            this.EnsurePrerequisitesAreDiscovered(mapper);
            this.EnsureMapperIsKnown(mapper);

            this.AppendToReverseDependencies(mapper);
        }

        public IEnumerable<Type> KnownJobTypes
        {
            get
            {
                return this.mappersByType.Keys.ToList();
            }
        }

        public IEnumerable<Type> UnknownJobTypes
        {
            get
            {
                return this.knownJobs.Select(x => x.GetType()).Distinct();
            }
        }

        private void EnsureMapperIsKnown(IStaticJobDependencyMapper mapper)
        {
            // ok, another mapper mapper.source ==> Mapper
            var mapperType = mapper.Source;

            ICollection<IStaticJobDependencyMapper> mappers;
            if (!this.mappersByType.TryGetValue(mapperType, out mappers))
            {
                mappers = new List<IStaticJobDependencyMapper>();
                this.mappersByType.Add(mapperType, mappers);
            }

            mappers.Add(mapper);
        }

        private void ThrowIfRunnerIsNotValidated(IStaticJobDependencyMapper runner)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(nameof(runner));
            }
        }

        private void EnsurePrerequisitesAreDiscovered(IStaticJobDependencyMapper mapper)
        {
            // remember: mapper.source ===<maps to>===> mapper.destination
            // therefore mapper.destination must be reversed to mapper.source
            var destinationType = mapper.Destination;

            ICollection<IStaticJobDependencyMapper> dependencies;

            if (this.reverseDependencies.TryGetValue(destinationType, out dependencies))
            {
                // dependency is now aware of this
                dependencies.Add(mapper);
            }
            else
            {
                // never heard of destination type, we park it here
                this.unknownDependencies.Add(
                    new UnknownDependency { Requested = destinationType, Requestor = mapper.Source });
            }

        }


        /// <summary>
        /// if this is new and there are dependencies, then we need to update the reverse lookups too!
        /// </summary>
        /// <param name="runner"></param>
        private void EnsureRemoveRunnerMapperFromListOfUnknowns(IStaticJobDependencyMapper mapper)
        {
            var sourceType = mapper.GetType();
            var unknownsToRemove = this.unknownDependencies.Where(x => x.Requested == sourceType).ToList();
            foreach (var unknownDependency in unknownsToRemove)
            {
                // so we have found the unknown dependency, renove from the list
                this.unknownDependencies.Remove(unknownDependency);
            }
        }

        private void AppendToReverseDependencies(IStaticJobDependencyMapper mapper)
        {
            var destinationType = mapper.Destination;
            ICollection<IStaticJobDependencyMapper> mappers;
            if (!this.reverseDependencies.TryGetValue(destinationType, out mappers))
            {
                mappers = new List<IStaticJobDependencyMapper>();
                this.mappersByType.Add(destinationType, mappers);
            }

            mappers.Add(mapper);
        }


        private IEnumerable<ValidationResult> ValidateMapper(IStaticJobDependencyMapper mapper)
        {

            yield return new ValidationResult() { Message = "Mapper cannot be null", IsValidated = mapper != null };

            foreach (var circularDependencyResult in FindCircularDependencies(mapper))
            {
                yield return circularDependencyResult;
            }


        }

        private IEnumerable<ValidationResult> FindCircularDependencies(IStaticJobDependencyMapper job)
        {
            yield break;            
        }
    }
}
