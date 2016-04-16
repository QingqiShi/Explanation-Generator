using ClangSharp;
using NUnit.Framework;
using System.IO;

namespace ExplanationGenerator.UnitTests
{
    class GeneratorTest
    {
        [Test]
        public void testConstructor()
        {
            Generator generator = new Generator();
        }

        [TestCase ("", "")]
        [TestCase ("[a]", @"<div class=""indent"">a</div>")]
        [TestCase("a|b", @"a<br>b")]
        public void testGeneration(string translation, string expected)
        {
            Generator generator = new Generator();

            string actual = generator.replaceTranslation(translation);

            Assert.AreEqual(expected, actual);
        }
    }
}
