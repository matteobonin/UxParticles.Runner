using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.UnitTest.Core.Runner
{
    using Moq;

    using NUnit.Framework;

    using UxParticles.Runner.Core.Service;
    using UxParticles.Runner.Core.Service.Dependency;
    using UxParticles.Runner.Core.Service.Runner;

    public class Args1
    {
        
    }

    [TestFixture]
    public class BaseRunnerTest
    {
        public class Mocks
        {
            public IStaticJobDependencyMapper<Args1>[] mappers;

            public Mock<IDependencyBroker> dependencyBroker;

            public Mock<IRunnerDataAccess> runnerDataAccess;

            public Mocks()
            {
                this.mappers = new IStaticJobDependencyMapper<Args1>[] { };
                this.dependencyBroker = new Mock<IDependencyBroker>();
                this.runnerDataAccess = new Mock<IRunnerDataAccess>();
            }

            public BaseRunner<Args1> GetBaseRunner()
            {
                return new BaseRunner<Args1>(
                    this.mappers,
                    this.dependencyBroker.Object,
                    this.runnerDataAccess.Object);
            }
        }

        [Test]
        public void ThatCanHaveNoMappers()
        {
            var mocks = new Mocks { mappers = null };
            Assert.DoesNotThrow(() => { mocks.GetBaseRunner(); });
        }

        [Test]
        public void ThatCannotRunOnNullArgs()
        {
            var mocks = new Mocks { mappers = null };

            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    var runner = mocks.GetBaseRunner();
                    runner.Run(new RunningRequest { Args = null });
                });
        }

        [Test]
        public void ThatCannotRunOnNullRequest()
        {
            var mocks = new Mocks { mappers = null };

            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    var runner = mocks.GetBaseRunner();
                    runner.Run(null);
                });
        }
    }
 
}
