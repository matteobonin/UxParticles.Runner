 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using UxParticles.Runner.Core.Service.Dependency;
using UxParticles.Runner.Core.Service.Queue;

[TestFixture]
public class ConcurrentOneToManyTest
{
    public class Mocks
    {
        public Mocks()
        {

        }

        public ConcurrentOneToMany<string,string> GetConcurrentOneToMany()
        {
            return new ConcurrentOneToMany<string, string>();
        }
    }

    [Test]
    [TestCaseSource(nameof(GetTestCases))]
    public void ThatCanAddSeveralItemsPerKeySerially(string[][] keys, int keyCount, int[] countByKey)
    {
        var list = new ConcurrentOneToMany<string, string>();
        foreach (var collection in keys)
        {
            var key = collection.First();
            foreach (var item in collection.Skip(1))
            {
                Console.WriteLine($"Adding {key} => {item}");
                list.AddOrUpdate(key, item);
            }
        }

        Assert.That(list.Count, Is.EqualTo(keyCount));

        for (int i = 0; i <= countByKey.GetUpperBound(0); i++)
        {
            var key = keys[i][0];
            var count = countByKey[i];

            Console.WriteLine($"For {key} expecting {count}");
            Assert.That(list.CountByKey(key), Is.EqualTo(count));
        }
    }

    [Test]
    [TestCaseSource(nameof(GetTestCases))]

    public void ThatMultipleAdditionsDoNotChangeTheResults(string[][] keys, int keyCount, int[] countByKey)
    {
        var list = new ConcurrentOneToMany<string, string>();
        foreach (var x in Enumerable.Range(0,100))
        {
            foreach (var collection in keys)
            {
                var key = collection.First();
                foreach (var item in collection.Skip(1))
                {
                    Console.WriteLine($"Adding {key} => {item}");
                    list.AddOrUpdate(key, item);
                }
            }
        }

        Assert.That(list.Count, Is.EqualTo(keyCount));

        for (int i = 0; i <= countByKey.GetUpperBound(0); i++)
        {
            var key = keys[i][0];
            var count = countByKey[i];

            Console.WriteLine($"For {key} expecting {count}");
            Assert.That(list.CountByKey(key), Is.EqualTo(count));
        }
    }

    [Test]
    [TestCaseSource(nameof(GetTestCases))]

    public void ThatMultipleConcurrentAdditionsDoNotChangeTheResults(string[][] keys, int keyCount, int[] countByKey)
    {
        var list = new ConcurrentOneToMany<string, string>();
        Parallel.ForEach(
            Enumerable.Range(0, 100),
            x =>
                {
                    foreach (var y in Enumerable.Range(0, 100))
                    {
                        foreach (var collection in keys)
                        {
                            var key = collection.First();
                            foreach (var item in collection.Skip(1))
                            {
                                list.AddOrUpdate(key, item);
                            }
                        }
                    }
                });

        Assert.That(list.Count, Is.EqualTo(keyCount));

        for (int i = 0; i <= countByKey.GetUpperBound(0); i++)
        {
            var key = keys[i][0];
            var count = countByKey[i];

            Console.WriteLine($"For {key} expecting {count}");
            Assert.That(list.CountByKey(key), Is.EqualTo(count));
        }
    }

    public static IEnumerable GetTestCases()
    {
        yield return
            new TestCaseData(new[] { new[] { "First", "Second" }, new[] { "A", "B", "C", "D" }, }, 2, new[] { 1, 3 })
                .SetName("Case 1");

        yield return
            new TestCaseData(new[] { new[] { "A" }, new[] { "A", "B", "C", "D" }, }, 1, new[] { 3, 3 })
                .SetName("Case 2");

        yield return new TestCaseData(new[] { new[] { "A" } }, 0, new[] { 0 }).SetName("Case 3");
    } 
}