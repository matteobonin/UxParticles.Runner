using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service.Dependency
{
    using System.Diagnostics;

    using UxParticles.Runner.Core.Service.Dependency.Exception;
    using UxParticles.Runner.Core.Service.Runner;

    public interface IStaticDependencyFinderService
    {
        IEnumerable<ValidationResult> ValidateStaticDependencyMapper(IStaticJobDependencyMapper mapper);

        void AddStaticDependencyMapper(IStaticJobDependencyMapper mapper);

    }

    public class ValidationResult
    {
        public bool? IsValidated { get; set; }

        public string Message { get; set; }

        public System.Exception Exception { get; set; }
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

        private void EnsurePrerequisitesAreDiscovered(IStaticJobDependencyMapper mapper)
        {
            // remember: mapper.source ===<maps to>===> mapper.destination
            // therefore mapper.destination must be reversed to mapper.source
            var destinationType = mapper.Destination;

            ICollection<IStaticJobDependencyMapper> dependencies;

            if (!this.mappersByType.TryGetValue(destinationType, out dependencies))            
            {
                // never heard of destination type, we park it here
                this.unknownDependencies.Add(new UnknownDependency { Requested = destinationType, Requestor = mapper.Source });
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
                this.reverseDependencies.Add(destinationType, mappers);
            }

            mappers.Add(mapper);
        }

        private ValidationResult FindCircularDependencies(IStaticJobDependencyMapper mapper)
        {
            // to find a dependency that is circular, we need to explore the graph
            // a mapper can be structured this way:
            //   A-+->B
            //     +->C--+->D
            //           +->E
            // we can start by any dependency
            // 1)                       
            return this.FindCircularDependenciesRec(mapper, new HashSet<Type>());
        }

        private ValidationResult FindCircularDependenciesRec(IStaticJobDependencyMapper mapper, ISet<Type> typesExplored)
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
            else
            {
                Console.WriteLine($"   Mapper > {mapper.Destination.Name}=>no dependencies?");
            }

            typesExplored.Remove(typeToLookFor);
            return new ValidationResult { IsValidated = true };
        }
    }
}
