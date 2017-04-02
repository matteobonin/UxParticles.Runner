namespace UxParticles.Runner.Core.Service.Dependency
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UxParticles.Runner.Core.Service.Dependency.Exception;

    public interface IStaticDependencyFinderService
    {
        void AddStaticDependencyMapper(IStaticJobDependencyMapper mapper);

        IEnumerable<Type> MappedTypes { get; }

        IEnumerable<Type> UnmappedTypes { get; }

        
    }

    public class ValidationResult
    {
        public System.Exception Exception { get; set; }

        public bool? IsValidated { get; set; }

        public string Message { get; set; }
    }

    public class StaticDependencyFinderService : IStaticDependencyFinderService
    {
        private List<IStaticJobDependencyMapper> knownJobs = new List<IStaticJobDependencyMapper>();

        /// <summary>
        ///     Given a type all the mappers into a diffrent type
        /// </summary>
        private Dictionary<Type, ICollection<IStaticJobDependencyMapper>> mappersByType =
            new Dictionary<Type, ICollection<IStaticJobDependencyMapper>>();

        private Dictionary<Type, ICollection<Type>> requestorsByRequestedType =
            new Dictionary<Type, ICollection<Type>>();

        /// <summary>
        ///     This is the reverse set, ie all the types this type depend on. so the key is required by all the reverse mappers
        /// </summary>
        private Dictionary<Type, ICollection<IStaticJobDependencyMapper>> reverseDependencies =
            new Dictionary<Type, ICollection<IStaticJobDependencyMapper>>();

        public IEnumerable<Type> MappedTypes => this.mappersByType.Keys.ToList();

        public IEnumerable<Type> UnmappedTypes => this.requestorsByRequestedType.Keys.ToList();

        public void AddStaticDependencyMapper(IStaticJobDependencyMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            this.EnsureMapperIsKnown(mapper);
            this.EnsureRemoveRunnerMapperFromListOfUnknowns(mapper);

            this.EnsurePrerequisitesAreDiscovered(mapper);

            this.AppendToReverseDependencies(mapper);

            this.ThrowIfRunnerIsNotValidated(mapper);
        }

        public IEnumerable<ValidationResult> ValidateStaticDependencyMapper(IStaticJobDependencyMapper jobToValidate)
        {
            if (jobToValidate == null)
            {
                yield return
                    new ValidationResult()
                        {
                            IsValidated = false, 
                            Message = "Cannot have null mapper", 
                            Exception = new ArgumentNullException(nameof(jobToValidate))
                        };

                yield break;
            }

            var circularDependencyMapper = this.FindCircularDependencies(jobToValidate);
            yield return circularDependencyMapper;
        }

        private void AppendToReverseDependencies(IStaticJobDependencyMapper mapper)
        {
            var destinationType = mapper.Destination;
            ICollection<IStaticJobDependencyMapper> mappers;

            if (!this.reverseDependencies.TryGetValue(destinationType, out mappers))
            {
                mappers = new List<IStaticJobDependencyMapper>();
                this.reverseDependencies.Add(destinationType, mappers);
            }

            mappers.Add(mapper);
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

        private void EnsurePrerequisitesAreDiscovered(IStaticJobDependencyMapper mapper)
        {
            // remember: mapper.source ===<maps to>===> mapper.destination
            // therefore mapper.destination must be reversed to mapper.source
            var destinationType = mapper.Destination;

            ICollection<IStaticJobDependencyMapper> dependencies;

            if (!this.mappersByType.TryGetValue(destinationType, out dependencies))
            {
                // never heard of destination type, we park it here
                ICollection<Type> requestors;
                if (!this.requestorsByRequestedType.TryGetValue(destinationType, out requestors))
                {
                    requestors = new List<Type>();
                    this.requestorsByRequestedType.Add(destinationType, requestors);
                }

                requestors.Add(mapper.Source);
            }
        }

        /// <summary>
        ///     if this is new and there are dependencies, then we need to update the reverse lookups too!
        /// </summary>
        /// <param name="runner"></param>
        private void EnsureRemoveRunnerMapperFromListOfUnknowns(IStaticJobDependencyMapper mapper)
        {
            var sourceType = mapper.GetType();
            if (this.requestorsByRequestedType.ContainsKey(sourceType))
            {
                this.requestorsByRequestedType.Remove(sourceType);
            }
        }

        private ValidationResult FindCircularDependencies(IStaticJobDependencyMapper mapper)
        {
            // to find a dependency that is circular, we need to explore the graph
            // a mapper can be structured this way:
            // A-+->B
            // +->C--+->D
            // +->E
            // we can start by any dependency
            // 1)                       
            return this.FindCircularDependenciesRec(mapper, new HashSet<Type>());
        }

        private ValidationResult FindCircularDependenciesRec(
            IStaticJobDependencyMapper mapper, 
            ISet<Type> typesExplored)
        {
            var typeToLookFor = mapper.Source;
            if (typesExplored.Contains(typeToLookFor))
            {
                return new ValidationResult { IsValidated = false, Exception = new StaticCircularDependencyException() };
            }

            typesExplored.Add(typeToLookFor);

            var mapperDependency = mapper.Destination;
            ICollection<IStaticJobDependencyMapper> dependencies;
            if (this.mappersByType.TryGetValue(mapperDependency, out dependencies))
            {
                foreach (var dependentMapper in dependencies)
                {
                    var childResult = this.FindCircularDependenciesRec(dependentMapper, typesExplored);
                    if (childResult.IsValidated.HasValue && !childResult.IsValidated.Value)
                    {
                        // close recursion
                        return childResult;
                    }
                }
            }

            typesExplored.Remove(typeToLookFor);
            return new ValidationResult { IsValidated = true };
        }

        private void ThrowIfRunnerIsNotValidated(IStaticJobDependencyMapper runner)
        {
            var validations = this.ValidateStaticDependencyMapper(runner);
            foreach (var validationResult in validations)
            {
                if (validationResult.Exception != null)
                {
                    throw validationResult.Exception;
                }

                if (validationResult.IsValidated.HasValue && validationResult.IsValidated.Value == false)
                {
                    throw new InvalidOperationException(validationResult.Message);
                }
            }
        }
    }
}