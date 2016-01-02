using ClangSharp;
using System;

namespace ExplanationGenerator.Prototype
{
    class ClangWrapper
    {
        string fileName;
        Cursor cursorToAST;
        Index index;
        TranslationUnit tu;

        public ClangWrapper()
        {
            fileName = null;
        }

        public ClangWrapper(string fileName)
        {
            openFile(fileName);
        }

        public void openFile(string fileName)
        {
            this.fileName = fileName;
            generateAST();
        }

        public Cursor getAST()
        {
            return cursorToAST;
        }

        public TranslationUnit getTU()
        {
            return tu;
        }

        public void Dispose()
        {
            /* Dispose Translation Unit */
            tu.Dispose();

            /* Dispose Index */
            index.Dispose();
        }

        public static void DumpAST(Cursor rootCursor, int indent)
        {
            Console.WriteLine(rootCursor.Kind + " : " + rootCursor.Spelling);

            foreach (var child in rootCursor.Children)
            {
                for (int i = 1; i < indent; i++)
                {
                    Console.Write("| ");
                }
                Console.Write("|- ");
                DumpAST(child, indent + 1);
            }
        }

        void generateAST()
        {
            /* Create Index */
            index = new Index();

            /* Clang Arguments */
            string[] arg = new string[1] { "" };

            /* Create Translation Units */
            tu = index.CreateTranslationUnit(fileName, arg, null);

            /* Creation Success */
            if (tu != null)
            {
                /* Get Translation Unit Cursor */
                cursorToAST = tu.Cursor;
            }
        }
    }
}
