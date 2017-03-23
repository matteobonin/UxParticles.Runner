using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service.Dependency
{
    using UxParticles.Runner.Core.Service.Runner;
    
    public interface IDependencyFinderService
    {
        bool ValidateRunner(IDependingJob mapper);
         
        void AddRunner(IDependingJob runner);

        IEnumerable<IDependingJob> GetDependencies(IDependingJob job);
    }

    public class ValidationResult
    {
        public bool? IsValidated { get; set; }

        public string Message { get; set; }
    }


 
    public class DependencyFinderService : IDependencyFinderService
    {
        public class UnknownDependency
        {
            public IDependingJob Requestor { get; set; }

            public IDependingJob Requested { get; set; }
        }

        private  List<UnknownDependency> unknownDependencies = new List<UnknownDependency>();

        private List<IDependingJob> knownJobs = new List<IDependingJob>();

        private Dictionary<IDependingJob, ICollection<IDependingJob>> dependenciesByJob = new Dictionary<IDependingJob, ICollection<IDependingJob>>();

        private Dictionary<IDependingJob, ICollection<IDependingJob>> missingDependenciesByJob = new Dictionary<IDependingJob, ICollection<IDependingJob>>();

        private Dictionary<IDependingJob,ICollection<IDependingJob>> reverseDependencies = new Dictionary<IDependingJob, ICollection<IDependingJob>>();

        public bool ValidateRunner(IDependingJob jobToValidate)
        {
         throw   new NotImplementedException();
        }

        public IEnumerable<IDependingJob> GetDependencies(IDependingJob job)
        {
            throw new NotImplementedException();
        }

        public void AddRunner(IDependingJob runner)
        {
            this.ThrowIfRunnerIsNotValidated(runner);

            this.EnsureRemoveRunnerFromListOfUnknowns(runner);

            this.EnsurePrerequisitesAreDiscovered(runner);



            this.TryMapDependencies(runner);

            this.knownJobs.Add(runner);
        }

        public IEnumerable<Type> KnownJobTypes
        {
            get
            {
                return this.knownJobs.Select(x => x.GetType()).Distinct();
            }
        }


        public IEnumerable<Type> UnknownJobTypes
        {
            get
            {
                return this.knownJobs.Select(x => x.GetType()).Distinct();
            }
        }
 
        private void TryMapDependencies(IDependingJob runner)
        {

        }

        private void ThrowIfRunnerIsNotValidated(IDependingJob runner)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(nameof(runner));
            }
        }

        private void EnsurePrerequisitesAreDiscovered(IDependingJob runner)
        {
            var prerequisites = runner.GetPrerequisites();
            foreach (var prerequisite in prerequisites)
            {
                ICollection<IDependingJob> dependency;

                if (this.reverseDependencies.TryGetValue(runner, out dependency))
                {
                    // dependency is now aware of this
                    dependency.Add(runner);
                }
                else 
                {
                    // never heard of dependency, we park it here
                    this.unknownDependencies.Add(new UnknownDependency { Requested = prerequisite, Requestor = runner });                    
                }
                
            }
        }

        /// <summary>
        /// if this is new and there are dependencies, then we need to update the reverse lookups too!
        /// </summary>
        /// <param name="runner"></param>
        private void EnsureRemoveRunnerFromListOfUnknowns(IDependingJob runner)
        {
            var unknownsToRemove = this.unknownDependencies.Where(x => x.Requested.Equals(runner)).ToList();
            foreach (var unknownDependency in unknownsToRemove)
            {
                this.unknownDependencies.Remove(unknownDependency);
            }            
        }



        private IEnumerable<ValidationResult> ValidateMapper(IDependingJob mapper)
        {
             
            yield return new ValidationResult() { Message = "Mapper cannot be null", IsValidated = mapper!=null };

            foreach (var circularDependencyResult in FindCircularDependencies(mapper))
            {
                yield return circularDependencyResult;
            }


        }

        private IEnumerable<ValidationResult> FindCircularDependencies(IDependingJob job)
        {
            yield break;            
        }
    }
}
