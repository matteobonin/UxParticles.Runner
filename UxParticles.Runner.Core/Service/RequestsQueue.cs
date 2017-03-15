using System;
using System.Collections.Concurrent;
using System.Threading;

namespace UxParticles.Runner.Core.Service
{
    public class RequestsQueue
    {
        private ConcurrentDictionary<Guid, RunRequest> requests = new ConcurrentDictionary<Guid, RunRequest>();

        private ConcurrentDictionary<IDependingJob, Entry> reverseRequests
            = new ConcurrentDictionary<IDependingJob, Entry>();

        internal class Entry
        {
            public Entry(RunRequest request)
            {
                this.Job = request.JobToExecute;
                this.RelatedRequests.TryAdd(request.RequestGuid, request);
            }

            public IDependingJob Job;

            public bool Queued;

            public bool Done;

            public ConcurrentDictionary<Guid, RunRequest> RelatedRequests = new ConcurrentDictionary<Guid, RunRequest>();
        }

        public enum RequestQueueResult																																								public enum RequestQueueResult
        {
            AlreadyInQueue = 0,
            AddedToQueue,
            Failed = 99
        }

        public RequestQueueResult AddRequest(RunRequest request)
        {
            if (!requests.TryAdd(request.RequestGuid, request))
            {
                // request existed, done!
                return RequestQueueResult.AlreadyInQueue;
            }


            var addedEntry =
                reverseRequests.AddOrUpdate(request.JobToExecute,
                    (job) => new Entry(request),
                    (job, existingEntry) =>
                    {
                        existingEntry.RelatedRequests.AddOrUpdate(request.RequestGuid, (x) => request, (x, y) => request);
                        return existingEntry;
                    });

            //now we push the request in a queue for analysis
            this.TryPushEntryToAnalysisQueue(addedEntry);

            // successfully added
            return RequestQueueResult.AddedToQueue;
        }

        private void TryPushEntryToAnalysisQueue(Entry entry)
        {
            if (entry.Queued)
            {
                return;
            }

            Interlocked.CompareExchange(ref entry.Queued,
        }

        public int RequestCount { get { return requests.Count; } }

        public int JobCount { get { return reverseRequests.Count; } }
    }
}