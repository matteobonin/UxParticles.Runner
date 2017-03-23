using System;

namespace UxParticles.Runner.Core.Service
{
    using UxParticles.Runner.Core.Service.Runner;

    public class RunRequest
    {
        /// <summary>
        /// The unique key of the request
        /// </summary>
        public Guid RequestGuid { get; set; }

        public string RequestedBy { get; set; }

        public IDependingJob JobToExecute { get; set; } = NullJob.Instance;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:UxParticles.Runner.Core.RunningBatch"/> must force a run even if the Job has a status
        /// </summary>
        /// <value><c>true</c> if force run; otherwise, <c>false</c>.</value>
        public bool ForceRun { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:UxParticles.Runner.Core.RunningBatch"/> invalidates only and does not 
        /// </summary>
        /// <value><c>true</c> if invalidate only; otherwise, <c>false</c>.</value>
        public bool InvalidateOnly { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority is 0 for instant execution</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the requested date for this
        /// </summary>
        public DateTime RequestedAt { get; set; }

    }
}