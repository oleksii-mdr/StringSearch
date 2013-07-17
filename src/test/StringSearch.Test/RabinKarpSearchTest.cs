using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace StringSearch.Test
{
    [TestFixture]
    public class RabinKarpSearchTest
    {
        //****************
        // Expected usage
        //*****************
        [Test]
        public void FindAllTest()
        {
            //Arrange
            string input = @"[32] Sed ut perspiciatis, unde omnis iste natus er
                sit voluptatem bar accusantium doloremque laudantium, foo
                totam rem aperiam eaque ipsa, quae ab illo verita";

            var expected = new[]
                {
                    new Tuple<int, string>(83, "bar"), 
                    new Tuple<int, string>(122, "foo")
                };

            IMultiPatternSearch uut = new RabinKarpSearch();
            uut.Init(new List<string> { "foo", "bar" });

            // Act
            var actual = uut.FindAll(input).ToArray();

            // Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void FindAll_CanUseForEachLoop()
        {
            //Arrange
            string input = "test 123string456";
            var expected = new Tuple<int, string>(8, "string");
            IMultiPatternSearch uut = new RabinKarpSearch();
            uut.Init(new List<string> { "string" });

            // Act
            foreach (var tuple in uut.FindAll(input))
            {
                // Assert
                Assert.That(expected, Is.EqualTo(tuple));
            }
        }

        [Test]
        public void FindAll_CanGetMatchedPosition()
        {
            //Arrange
            string input = "test 123string456";
            IMultiPatternSearch uut = new RabinKarpSearch();
            uut.Init(new List<string> { "string" });

            // Act
            var actual = uut.FindAll(input).ToArray().First();

            // Assert
            Assert.AreEqual(8, actual.Item1);
        }

        [Test]
        public void FindAll_CanGetMatchedPattern()
        {
            //Arrange
            string input = "test 123string456";
            IMultiPatternSearch uut = new RabinKarpSearch();
            uut.Init(new List<string> { "string" });

            // Act
            var actual = uut.FindAll(input).ToArray().First();

            // Assert
            Assert.AreEqual("string", actual.Item2);
        }

        [Test]
        public void FindAll_CanGetMatchedPosittionAndPattern()
        {
            //Arrange
            string input = "test 123string456";
            IMultiPatternSearch uut = new RabinKarpSearch();
            uut.Init(new List<string> { "string" });

            // Act
            var actual = uut.FindAll(input).ToArray().First();

            // Assert
            Assert.AreEqual(8, actual.Item1);
            Assert.AreEqual("string", actual.Item2);
        }

        //****************
        // Corner cases
        //*****************
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Init_NullPatterns_ThrowsException()
        {
            var uut = new RabinKarpSearch();
            uut.Init(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Init_EmptyListPatterns_ThrowsException()
        {
            var uut = new RabinKarpSearch();
            uut.Init(new List<string>());
        }

        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindAll_InvalidInputString_ThrowsException(string input)
        {
            IMultiPatternSearch uut = new RabinKarpSearch();
            uut.Init(new List<string> { "foo" });
            uut.FindAll(input).ToList();
        }


    }
}