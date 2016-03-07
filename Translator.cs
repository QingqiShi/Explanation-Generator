using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClangSharp;

namespace ExplanationGenerator
{
    class Translator
    {
        Dictionary<CursorKind, string> dictionary = new Dictionary<CursorKind, string>();

        public Translator(string languageCode, string languageTemplatePath)
        {
            loadDictionary(languageTemplatePath + languageCode + ".lang");
        }

        internal string translateFunctionDecl(Cursor functionDecl)
        {
            return String.Format(dictionary[functionDecl.Kind], functionDecl.Spelling, "", "", "");
        }



        private void loadDictionary(string pathToTemplate)
        {
            string[] lines = System.IO.File.ReadAllLines(pathToTemplate);

            string[] splitter = new string[] { "::" };
            foreach (string line in lines)
            {
                string[] parts = line.Split(splitter, StringSplitOptions.None);
                dictionary.Add(stringToCursorKind(parts[0]), parts[1]);
            }
        }

        private CursorKind stringToCursorKind(string str)
        {
            switch (str)
            {
                case "FunctionDecl":
                    return CursorKind.FunctionDecl;
            }
            return 0;
        }
    }
}
