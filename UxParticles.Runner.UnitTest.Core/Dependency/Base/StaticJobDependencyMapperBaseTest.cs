namespace UxParticles.Runner.UnitTest.Core.Dependency.Base
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using UxParticles.Runner.Core.Service.Dependency;
    using UxParticles.Runner.Core.Service.Dependency.Base;

    [TestFixture]
    public class StaticJobDependencyMapperBaseTest
    {
        public class TestArgs1
        {
            public string Mapping { get; set; }
        }

        public class TestArgs2
        {
            public string Result { get; set; }
        }

        public class StaticDependencyMapperBaseTestClass : StaticDependencyMapperBaseTestClassBase<TestArgs1, TestArgs2>
        {

            public StaticDependencyMapperBaseTestClass(Func<IEnumerable<TestArgs2>> resultList)
                : base(resultList)
            {
            }
        }

        public class Mocks
        {
            private readonly IEnumerable<TestArgs2> expectedResults;

            public Mocks(IEnumerable<TestArgs2> expectedResults)
            {
                this.expectedResults = expectedResults;
            }

            public StaticDependencyMapperBase<TestArgs1, TestArgs2> GetStaticJobDependencyMapperBase()
            {
                return new StaticDependencyMapperBaseTestClass(() => this.expectedResults);
            }
        }

        [Test]
        public void ThatCanGetResults()
        {
            var mapper = new Mocks(new[] { new TestArgs2() }).GetStaticJobDependencyMapperBase();
            Assert.DoesNotThrow(() => mapper.MapFrom(new TestArgs1()));
        }

        [Test]
        public void ThatThrowsForNullArgument()
        {
            var mapper = new Mocks(null).GetStaticJobDependencyMapperBase();
            Assert.Throws<ArgumentNullException>(() => mapper.MapFrom(null));
        }

        [Test]
        public void ThatThrowsForNullResult()
        {
            var mapper = new Mocks(null).GetStaticJobDependencyMapperBase();
            Assert.Throws<InvalidOperationException>(() => mapper.MapFrom(new TestArgs1()));
        }
    }
}