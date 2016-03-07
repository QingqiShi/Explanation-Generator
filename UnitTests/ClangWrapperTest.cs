using ClangSharp;
using NUnit.Framework;
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
        public void testLoadFile()
        {
            ClangWrapper wrapper = new ClangWrapper();

            // Create temporary test file
            string fileName = "func.c";
            using (StreamWriter sw = System.IO.File.CreateText(fileName))
            {
                sw.Write("Hello");
            }

            // Load file
            wrapper.loadFile("func.c");
            // No exception, load success

            wrapper.Dispose();
            System.IO.File.Delete(fileName);
        }

        [Test]
        public void testLoadFileFail()
        {
            ClangWrapper wrapper = new ClangWrapper();

            Assert.Throws<FileNotFoundException>(delegate { wrapper.loadFile("nonExistingFile.c"); });
        }

        [Test]
        public void testGetAstRoot()
        {
            ClangWrapper wrapper = new ClangWrapper();

            // Create temporary test file
            string fileName = "func.c";
            using (StreamWriter sw = System.IO.File.CreateText(fileName))
            {
                sw.Write("int main() { return 0; }");
            }

            // Load file
            wrapper.loadFile("func.c");

            // Assert root kind and first child kind
            Assert.AreEqual(CursorKind.TranslationUnit, wrapper.getRoot().Kind);
            Assert.AreEqual(CursorKind.FunctionDecl, wrapper.getRoot().Children[0].Kind);
            Assert.AreEqual("main", wrapper.getRoot().Children[0].Spelling);

            wrapper.Dispose();
            System.IO.File.Delete(fileName);
        }
    }
}
