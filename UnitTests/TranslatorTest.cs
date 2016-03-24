using ClangSharp;
using NUnit.Framework;
using System.IO;

namespace ExplanationGenerator.UnitTests
{
    class TranslatorTest
    {

        private ClangWrapper getWrapper(string code)
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

            return wrapper;
        }

        private void createTemplateFiles()
        {
            // Create temporary test files
            using (StreamWriter sw = System.IO.File.CreateText("en.lang"))
            {
                writeTemplate(sw);
            }

            using (StreamWriter sw = System.IO.File.CreateText("zh.lang"))
            {
                writeTemplate(sw);
            }
        }

        private void writeTemplate(StreamWriter sw)
        {
            sw.WriteLine("int::Int");
            sw.WriteLine("char::Char");
            sw.WriteLine("short int::Short");
            sw.WriteLine("long int::Long");
            sw.WriteLine("double::Double");
            sw.WriteLine("float::Float");
            sw.WriteLine("void::Void");
            sw.WriteLine("FunctionDecl::{0} {1} |[{2}] |[{3}]");
            sw.WriteLine("ParmDecl::{0} {1}");
            sw.WriteLine("VarDecl::{0} {1} {2}");
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
            Cursor root = getWrapper(code).getRoot();

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

        [TestCase("en", "void func() { }", "func Void |[] |[]")]
        [TestCase("zh", "void func() { }", "func Void |[] |[]")]
        [TestCase("en", "void func(int a) { }", "func Void |[a Int] |[]")]
        [TestCase("en", "void func(char a) { }", "func Void |[a Char] |[]")]
        [TestCase("en", "void func(short a) { }", "func Void |[a Short] |[]")]
        [TestCase("en", "void func(long a) { }", "func Void |[a Long] |[]")]
        [TestCase("en", "void func(float a) { }", "func Void |[a Float] |[]")]
        [TestCase("en", "void func(double a) { }", "func Void |[a Double] |[]")]
        [TestCase("en", "void func(int a, double b) { }", "func Void |[a Int|b Double] |[]")]
        [TestCase("en", "void func(int a, double b, char c) { }", "func Void |[a Int|b Double|c Char] |[]")]
        [TestCase("zh", "void func() {int a;}", "func Void |[] |[a Int ]")]
        [TestCase("zh", "void func() {int a; double b = a;}", "func Void |[] |[a Int |b Double a]")]
        public void testTranslateFunctionDecl(string languageCode, string code, string expectedTranslation)
        {
            createTemplateFiles();

            Translator translator = new Translator(languageCode, "");
            ClangWrapper wrapper = getWrapper(code);
            Cursor root = wrapper.getRoot();
            TranslationUnit tu = wrapper.getTranslationUnit();
            Cursor functionDecl = root.Children[0];

            string translation = translator.translateFunctionDecl(functionDecl, tu);

            Assert.AreEqual(expectedTranslation, translation);

            deleteTemplateFiles();
        }

        [TestCase("en", @"int a;", "a Int ", 0)]
        [TestCase("en", @"int a = 0;", "a Int 0", 0)]
        [TestCase("en", @"int a = 12;", "a Int 12", 0)]
        [TestCase("en", @"int b; int a = b;", "a Int b", 1)]
        [TestCase("en", @"double b = 1.2;", "b Double 1.2", 0)]
        [TestCase("en", @"char c = 'a';", "c Char 'a'", 0)]
        [TestCase("en", @"float f = 2.1;", "f Float 2.1", 0)]
        [TestCase("en", @"long l = 210000;", "l Long 210000", 0)]
        public void testTranslateVarDecl(string languageCode, string code, string expectedTranslation, int child)
        {
            createTemplateFiles();

            Translator translator = new Translator(languageCode, "");
            ClangWrapper wrapper = getWrapper(code);
            Cursor root = wrapper.getRoot();
            TranslationUnit tu = wrapper.getTranslationUnit();
            Cursor varDecl = root.Children[child];

            string translation = translator.translateVarDecl(varDecl, tu);

            Assert.AreEqual(expectedTranslation, translation);

            deleteTemplateFiles();
        }
    }
}
