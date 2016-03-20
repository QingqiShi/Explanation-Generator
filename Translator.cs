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
            string functionName = functionDecl.Spelling;
            string functionReturnType = functionDecl.ResultType.Spelling;

            int paramCount = 0;
            for (int i = 0; i < functionDecl.Children.Count && functionDecl.Children[i].Kind != CursorKind.CompoundStmt; i++)
            {
                paramCount++;
            }
            Cursor[] paramList = new Cursor[paramCount];

            for (int i = 0; i < functionDecl.Children.Count && functionDecl.Children[i].Kind != CursorKind.CompoundStmt; i++)
            {
                paramList[i] = functionDecl.Children[i];
            }

            string functionArguments = translateParamList(paramList);
            string functionBody = translateCompoundStmt(functionDecl);
            return String.Format(dictionary[functionDecl.Kind], functionName, functionReturnType, functionArguments, functionBody);
        }

        internal string translateCompoundStmt(Cursor compoundStmt)
        {
            return "";
        }

        internal string translateParamList(Cursor[] paramList)
        {
            string translation = "";
            foreach (Cursor param in paramList)
            {
                translation += translateParamDecl(param);
            }
            return translation;
        }

        internal string translateParamDecl(Cursor paramDecl)
        {
            string paramName = paramDecl.Spelling;
            string paramType = paramDecl.Type.Spelling;
            return String.Format(dictionary[paramDecl.Kind], paramName, paramType);
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
                case "ParmDecl":
                    return CursorKind.ParmDecl;
            }
            return 0;
        }
    }
}
