using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service
{
    using UxParticles.Runner.Core.Service.Runner;
    using UxParticles.Runner.Core.Service.Runner._;

    public interface IDependencyCrawler
	{
		void Prepare();

		/// <summary>
		/// Prepares the job to be executed.
		/// </summary>
		Task PrepareAsync(IDependingJob jobToPrepare);
	}


    // responsible for organising the execution of a particular task
}
