using NUnit.Framework;
using System;
using System.IO;

namespace ExplanationGenerator.UnitTests
{
    class ClangWrapperTest
    {
        [Test]
        public void testConstructor()
        {
            ClangWrapper wrapper = new ClangWrapper();
        }

        [Test]
        public void testFileNameGetter()
        {
            ClangWrapper wrapper = new ClangWrapper();
            wrapper.loadFile("func.c");
            Assert.That(wrapper.getFileName(), Is.EqualTo("func.c"));
        }

        [Test]
        public void testFileNameGetterNull()
        {
            ClangWrapper wrapper = new ClangWrapper();
            Assert.That(wrapper.getFileName(), Is.EqualTo(null));
        }

        [Test]
        public void testDumpAST()
        {
            // Create test file
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".c";
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.Write("Hello");
            }

            ClangWrapper wrapper = new ClangWrapper();
            wrapper.loadFile(fileName);
        }
    }
}
