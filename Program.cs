using System;
using ClangSharp;

namespace ExplanationGenerator
{
    class Program
    {
        /// <summary>
        /// Main Method
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            /* Create Index */
            var index = new Index();

            /* Clang Arguments */
            string[] arg = new string[1] { "" };

            /* Create Translation Units */
            TranslationUnit tu = index.CreateTranslationUnit("src/main.c", arg, null);

            /* Creation Success */
            if (tu != null)
            {
                /* Get Translation Unit Cursor */
                Cursor cursor = tu.Cursor;

                /* Visit Children */
                DumpAST(cursor, 0);

                /* Dispose Translation Unit */
                tu.Dispose();
            }

            /* Dispose Index */
            index.Dispose();

            Console.ReadLine();
        }

        static void DumpAST(Cursor rootCursor, int indent)
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

    }
}
