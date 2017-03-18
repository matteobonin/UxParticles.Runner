namespace UxParticles.Runner.Core.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public class RequestsQueue
    {
        private readonly IAnalysisQueue analysisQueue;

        private readonly ConcurrentDictionary<Guid, RunRequest> requests = new ConcurrentDictionary<Guid, RunRequest>();

        private readonly ConcurrentDictionary<IDependingJob, Entry> reverseRequests =
            new ConcurrentDictionary<IDependingJob, Entry>();

        public RequestsQueue(IAnalysisQueue analysisQueue)
        {
            this.analysisQueue = analysisQueue;
        }

        public enum RequestQueueResult
        {
            AlreadyInQueue = 0, 

            AddedToQueue, 

            Failed = 99
        }

        public int JobCount => this.reverseRequests.Count;

        public int RequestCount => this.requests.Count;

        public RequestQueueResult AddRequest(RunRequest request)
        {
            ValidateRequest(request);

            if (!this.requests.TryAdd(request.RequestGuid, request))
            {
                // request existed, done!
                return RequestQueueResult.AlreadyInQueue;
            }

            var addedEntry = this.reverseRequests.AddOrUpdate(
                request.JobToExecute, 
                job => new Entry(request), 
                (job, existingEntry) =>
                    {
                        existingEntry.RelatedRequests.AddOrUpdate(request.RequestGuid, x => request, (x, y) => request);
                        return existingEntry;
                    });

            // now we push the request in a queue for analysis
            this.TryPushEntryToAnalysisQueue(addedEntry);

            // successfully added
            return RequestQueueResult.AddedToQueue;
        }

        public bool ContainsRequest(Guid requestGuid)
        {
            return this.requests.ContainsKey(requestGuid);
        }

        private static void ValidateRequest(RunRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.RequestGuid == Guid.Empty)
            {
                throw new InvalidOperationException("Request cannot have an empty GUID");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

        private void TryPushEntryToAnalysisQueue(Entry entry)
        {
            var value = Interlocked.CompareExchange(ref entry.QueuedCount, 1, 0);
            if (value != 0)
            {
                return;
            }

            this.analysisQueue.Add(entry.Job);
        }

        internal class Entry
        {
            public bool Done;

            public IDependingJob Job;

            public int QueuedCount;

            public ConcurrentDictionary<Guid, RunRequest> RelatedRequests = new ConcurrentDictionary<Guid, RunRequest>();

            public Entry(RunRequest request)
            {
                this.Job = request.JobToExecute;
                this.RelatedRequests.TryAdd(request.RequestGuid, request);
            }
        }
    }
}