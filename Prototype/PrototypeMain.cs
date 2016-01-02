
using System;

namespace ExplanationGenerator.Prototype
{
    class PrototypeMain
    {
        /// <summary>
        /// Main Method for the prototype
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Run(string[] args)
        {
            ClangWrapper cw = new ClangWrapper("src/main.c");
            ClangWrapper.DumpAST(cw.getAST(), 0);

            Translator t = new Translator(cw);
            ExplanationTree.DumpTree(t.Translate());

            cw.Dispose();
            Console.ReadLine();
        }
    }
}
