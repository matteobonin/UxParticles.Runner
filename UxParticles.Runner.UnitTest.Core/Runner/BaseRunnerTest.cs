using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.UnitTest.Core.Runner
{
    using System.Collections;

    using Moq;

    using NUnit.Framework;

    using UxParticles.Runner.Core.Service;
    using UxParticles.Runner.Core.Service.Dependency;
    using UxParticles.Runner.Core.Service.Runner;
    using UxParticles.Runner.Core.Service.Runner.Enum;

    [TestFixture]
    public class BaseRunnerTest
    {
        public class Mocks
        {
            public IStaticJobDependencyMapper<TestableBaseRunnerArgs>[] mappers;

            public Mock<IDependencyBroker> dependencyBroker;

            public Mock<IRunnerDataAccess> runnerDataAccess;

            public Mocks()
            {
                this.mappers = new IStaticJobDependencyMapper<TestableBaseRunnerArgs>[] { };
                this.dependencyBroker = new Mock<IDependencyBroker>();
                this.runnerDataAccess = new Mock<IRunnerDataAccess>();
            }

            public BaseRunner<TestableBaseRunnerArgs> GetBaseRunner()
            {
                return new TestableBaseRunner<TestableBaseRunnerArgs>(
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

        [Test]
        public void ThatRequestsMappers()
        {
            var mocks = new Mocks { mappers = null };

            Assert.Throws<ArgumentNullException>(
                () =>
                    {
                        var runner = mocks.GetBaseRunner();
                        runner.Run(null);
                    });
        }


        [Test]
        public void ThatTheOutcomeOfARunnerAlreadyRunningIsAlreadyRunning()
        {
            var mocks = new Mocks { mappers = null };
            mocks.runnerDataAccess.Setup(x => x.GetRunnerStatus(It.IsAny<RunningRequest>()))
                .Returns(RunnerStatus.Running);

            var request = new RunningRequest() { Args = new TestableBaseRunnerArgs() };
            var runner = mocks.GetBaseRunner();
            var result = runner.Run(request);

            Assert.That(result, Is.EqualTo(RunnerOutcome.AlreadyRunning));
        }

        [Test]
        public void ThatTheOutcomeOfARunnerThatRunSuccessfullyIsAlreadyCompletedIfNotForced()
        {
            var mocks = new Mocks { mappers = null };
            mocks.runnerDataAccess.Setup(x => x.GetRunnerStatus(It.IsAny<RunningRequest>()))
                .Returns(RunnerStatus.Success);

            var request = new RunningRequest() { Args = new TestableBaseRunnerArgs() };
            var runner = mocks.GetBaseRunner();
            var result = runner.Run(request);

            Assert.That(result, Is.EqualTo(RunnerOutcome.AlreadyCompleted));
        }

        [Test]
        [TestCaseSource(nameof(GetOutcomeForRunners))]
        public void ThatTheOutcomeOfARunnerThatRunSuccessfullyButIsInvalidatedIsJustInvalidated(
            RunningMode currentRequest,
            RunnerStatus currentStatus,
            RunnerOutcome expectedOutcome)
        {
            var mocks = new Mocks { mappers = null };
            mocks.runnerDataAccess.Setup(x => x.GetRunnerStatus(It.IsAny<RunningRequest>())).Returns(currentStatus);



            var request = new RunningRequest() { Args = new TestableBaseRunnerArgs(), Mode = currentRequest };
            var runner = mocks.GetBaseRunner();
            var result = runner.Run(request);

            Assert.That(result, Is.EqualTo(expectedOutcome));
        }


        public static IEnumerable GetOutcomeForRunners()
        {
            // success status
            yield return
                new TestCaseData(RunningMode.DoNotForce, RunnerStatus.Success, RunnerOutcome.AlreadyCompleted).SetName(
                    "Sucess - Do Not Force");

            yield return
                new TestCaseData(RunningMode.InvalidateOnly, RunnerStatus.Success, RunnerOutcome.InvalidatedOnly)
                    .SetName("Sucess - Invalidate");

            yield return
                new TestCaseData(RunningMode.ForceRunnerOnly, RunnerStatus.Success, RunnerOutcome.Completed).SetName(
                    "Sucess - Force only runner");

            yield return
                new TestCaseData(RunningMode.ForceDependendentsAndRunner, RunnerStatus.Success, RunnerOutcome.Completed)
                    .SetName("Sucess - Force all");

            // already runnin status
            yield return
                new TestCaseData(RunningMode.DoNotForce, RunnerStatus.Running, RunnerOutcome.AlreadyRunning).SetName(
                    "AlreadyRunning - Force all");

            yield return
                new TestCaseData(RunningMode.InvalidateOnly, RunnerStatus.Running, RunnerOutcome.AlreadyRunning).SetName
                    ("AlreadyRunning - InvalidateOnly");

            yield return
                new TestCaseData(RunningMode.ForceRunnerOnly, RunnerStatus.Running, RunnerOutcome.AlreadyRunning)
                    .SetName("AlreadyRunning - ForceRunnerOnly");

            yield return
                new TestCaseData(
                    RunningMode.ForceDependendentsAndRunner,
                    RunnerStatus.Running,
                    RunnerOutcome.AlreadyRunning).SetName("AlreadyRunning - ForceDependendentsAndRunner");


        }
    }
}
 

