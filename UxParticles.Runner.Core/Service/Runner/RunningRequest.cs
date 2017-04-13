namespace UxParticles.Runner.Core.Service.Runner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    using UxParticles.Runner.Core.Service.Bus;
    using UxParticles.Runner.Core.Service.Runner.Enum;

    /// <summary>
    /// This comparer compares for Type, Period and Args
    /// </summary>
    public class RunningRequestComparer : IEqualityComparer<RunningRequest>
    {
        public bool Equals(RunningRequest x, RunningRequest y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            //not same reference
            if (x.RequestType != y.RequestType)
            {
                return false;
            }

            // same type
            if (x.Period != null && y.Period != null)
            {
                if (!x.Period.Equals(y.Period))
                {
                    return false;
                }
            }

            // same period
            if (x.SerialisedArgs != null && y.SerialisedArgs != null)
            {
                // use comparison on serialised bit
                return x.SerialisedArgs.Equals(y.SerialisedArgs);
            }
            else
            {
                // use equals on args
                if (x.Args != null)
                {
                    return x.Args.Equals(y.Args);
                }
            }

            return false;
           
        }

        public int GetHashCode(RunningRequest obj)
        {
            unchecked
            {
                int hash = 17;

                if (obj.RequestType != null)
                {
                    hash = hash * 23 + obj.RequestType.GetHashCode();
                }

                if (obj.Period != null)
                {
                    hash = hash * 23 + obj.Period.GetHashCode();
                }

                if (obj.SerialisedArgs != null)
                {
                    hash = hash * 23 + obj.SerialisedArgs.GetHashCode();
                }
                else if (obj.Args != null)
                {
                    hash = hash * 23 + obj.Args.GetHashCode();
                }

                return hash;
            }
        }
    }


    public class RunningRequest : IMessage<RunnerOutcome>
    {
        public RunningRequest()
        {
            
        }

        public RunningRequest(
            object args,
            RunningPeriod period, 
            RunningMode mode,
            string requestType,
            string serialisedArgs)
        {
            this.Args = args;
            this.Period = period;
            this.Mode = mode;
            this.RequestType = requestType;
            this.SerialisedArgs = serialisedArgs;
        }

        /// <summary>
        /// This is the unique identifier of the running request
        /// </summary>
        public Guid RunningRequestGuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Underlying arguments to run
        /// </summary>
        public object Args { get; set; }

        /// <summary>
        /// The <see cref="RunningPeriod"/> this request relates to
        /// </summary>
        public RunningPeriod Period { get; set; } 

        /// <summary>
        /// The type of this request (allows for Args Reuse)
        /// </summary>
        public string RequestType { get; set; }


        /// <summary>
        /// Running mode. Default is <see cref="RunningMode.DoNotForce"/>
        /// </summary>
        public RunningMode Mode { get; set; } = RunningMode.DoNotForce;

        /// <summary>
        /// This is a serialised version of the Args.
        /// </summary>
        public string SerialisedArgs { get; }
       
    }

    public interface IRunningRequestFactory
    {
        RunningRequest Create(
            object args,
            RunningPeriod period,
            RunningMode mode = RunningMode.DoNotForce,
            string serialisedVersion = null);
    }

    public class RunningRequestFactory : IRunningRequestFactory
    {
        public RunningRequest Create(
            object args,
            RunningPeriod period,
            RunningMode mode = RunningMode.DoNotForce,
            string serialisedVersion = null)
        {
            if (serialisedVersion == null)
            {
                serialisedVersion = this.Serialize(args);
            }

            return new RunningRequest(args, period, mode,args.GetType().Name, serialisedVersion);
        }

        private string Serialize(object args)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(args.GetType());
                serializer.WriteObject(ms, args);
                var jsonString = ms.ToArray();
                return Encoding.UTF8.GetString(jsonString, 0, jsonString.Length);
            }
        }
    }
}