using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace CSharpExamples
{
    [TestFixture]
    public class Parallel
    {
        [Test]
        public void PLINQ_Aggregate_CountInstances_Of_EachLetter()
        {
            const string str = "This is a string containing some text.";
            var watch = new Stopwatch();

            // Sequential
            watch.Start();
            var result = str.Aggregate(
                new int[26], // Seed is the container for the results. 26 here as 26 chars in english alphabet.
                (letterArray, c) => // create anonymous objects for character and seed.
                {
                    // Normalise to upper case char index by subracting A. 
                    var index = char.ToUpper(c) - 'A';

                    // If within the alphabet the increment the seed. 
                    if (index >= 0 && index <= 26)
                        letterArray[index]++;

                    // Return the results. 
                    return letterArray;
                });
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            Assert.IsNotEmpty(result);

            // Parallel
            watch.Reset();
            watch.Start();
            var resultParallel = str.AsParallel().Aggregate(
                new int[26], // Seed is the container for the results. 26 here as 26 chars in english alphabet.
                (letterArray, c) => // create anonymous objects for character and seed.
                {
                    // Normalise to upper case char index by subracting A. 
                    var index = char.ToUpper(c) - 'A';

                    // If within the alphabet the increment the seed. 
                    if (index >= 0 && index <= 26)
                        letterArray[index]++;

                    // Return the results. 
                    return letterArray;
                },
                (main, local) => main.Zip(local, (i, i1) => i + i1).ToArray(), // Create thread local accumulators
                final => final // Perform any final transformation
                );
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            Assert.IsNotEmpty(resultParallel);
        }
    }
}
