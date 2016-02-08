using ClangSharp;
using System.IO;

namespace ExplanationGenerator
{
    class ClangWrapper
    {
        string fileName;
        Cursor cursorToAST;

        public string getFileName() { return fileName; }

        /*
            Use clang to load a file form disk
        */
        public void loadFile(string fileName)
        {
            this.fileName = fileName;
            if (System.IO.File.Exists(fileName))
            {
                generateAST();
            } else
            {
                throw new FileNotFoundException("File not found", fileName);
            }
    }

    /*
        Internal method to generate the Clang AST.
    */
    void generateAST()
    {
        /* Create Index */
        Index index = new Index();

        /* Clang Arguments */
        // string[] arg = new string[1] { "" };

        /* Create Translation Units */
        TranslationUnit tu = index.CreateTranslationUnit(fileName);

        /* Creation Success */
        if (tu != null)
        {
            /* Get Translation Unit Cursor */
            cursorToAST = tu.Cursor;
        }
    }
}
}
