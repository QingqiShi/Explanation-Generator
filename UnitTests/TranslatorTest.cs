using ClangSharp;
using NUnit.Framework;
using System.IO;

namespace ExplanationGenerator.UnitTests
{
    class TranslatorTest
    {

        private Cursor getAST(string code)
        {
            ClangWrapper wrapper = new ClangWrapper();

            // Create temporary test file
            string fileName = "func.c";
            using (StreamWriter sw = System.IO.File.CreateText(fileName))
            {
                sw.Write(code);
            }

            // Load file
            wrapper.loadFile("func.c");
            
            System.IO.File.Delete(fileName);

            return wrapper.getRoot();
        }

        private void createTemplateFiles()
        {
            // Create temporary test files
            using (StreamWriter sw = System.IO.File.CreateText("en.lang"))
            {
                sw.WriteLine("FunctionDecl::{0}{1}{2}{3}");
            }

            using (StreamWriter sw = System.IO.File.CreateText("zh.lang"))
            {
                sw.WriteLine("FunctionDecl::{0}{1}{2}{3}");
            }
        }

        private void deleteTemplateFiles()
        {
            System.IO.File.Delete("en.lang");
            System.IO.File.Delete("zh.lang");
        }

        [TestCase("void func() {}", CursorKind.FunctionDecl, "func", Type.Kind.Void)]
        [TestCase("int main() {}", CursorKind.FunctionDecl, "main", Type.Kind.Int)]
        public void testGetAST(string code, CursorKind fstChildKind, string fstChildSpelling, Type.Kind fstChildTypeKind)
        {
            Cursor root = getAST(code);

            Assert.AreEqual(CursorKind.TranslationUnit, root.Kind);
            Assert.AreEqual(fstChildKind, root.Children[0].Kind);
            Assert.AreEqual(fstChildSpelling, root.Children[0].Spelling);
            Assert.AreEqual(fstChildTypeKind, root.Children[0].ResultType.TypeKind);
        }

        [TestCase("en")]
        [TestCase("zh")]
        public void testConstructor(string languageCode)
        {
            createTemplateFiles();

            Translator translator = new Translator(languageCode, "");

            deleteTemplateFiles();
        }

        [TestCase("en", "void func() { }", "[Function Declaration, function name: func[]]")]
        [TestCase("zh", "void func() { }", "[函数声明，函数名：func[]]")]
        public void testTranslateFunctionDecl(string languageCode, string code, string expectedTranslation)
        {
            createTemplateFiles();

            Translator translator = new Translator(languageCode, "");
            Cursor root = getAST(code);
            Cursor functionDecl = root.Children[0];

            string translation = translator.translateFunctionDecl(functionDecl);

            Assert.AreEqual(expectedTranslation, translation);

            deleteTemplateFiles();
        }
    }
}
