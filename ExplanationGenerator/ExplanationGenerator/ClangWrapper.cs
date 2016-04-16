using ClangSharp;
using System;
using System.IO;

namespace ExplanationGenerator
{
    class ClangWrapper
    {
        private Cursor _rootCursor;
        private Index index;
        private TranslationUnit tu;

        /*
            Use clang to load a file form disk
        */
        public void loadFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                generateAST(fileName);
            }
            else
            {
                throw new FileNotFoundException("File not found", fileName);
            }
        }

        /*
            Get root cursor of the generated AST
        */
        public Cursor getRoot()
        {
            return _rootCursor;
        }

        /*
            Get translation unit
        */
        public TranslationUnit getTranslationUnit()
        {
            return tu;
        }

        public void dispose()
        {
            /* Dispose Translation Unit */
            tu.Dispose();

            /* Dispose Index */
            index.Dispose();
        }

        /*
            Internal method to generate the Clang AST.
        */
        private void generateAST(string fileName)
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
                _rootCursor = tu.Cursor;
            }
        }
    }
}
