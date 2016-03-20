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
        Dictionary<CursorKind, string> cursorDictionary = new Dictionary<CursorKind, string>();
        Dictionary<string, string> typeDictionary = new Dictionary<string, string>();

        /*
            Construct using specified language and path to folder of template files.
        */
        public Translator(string languageCode, string languageTemplatePath)
        {
            loadDictionary(languageTemplatePath + languageCode + ".lang");
        }

        /*
            Translate from a Cursor, will call specific methods depends on the type of cursor.
        */
        public string translate(Cursor unknownCursor, TranslationUnit tu)
        {
            switch (unknownCursor.Kind)
            {
                case CursorKind.FunctionDecl:
                    return translateFunctionDecl(unknownCursor, tu);
                case CursorKind.IntegerLiteral:
                    return translateIntegerLiteral(unknownCursor, tu);
                case CursorKind.UnexposedExpr:
                    return translateUnexposedExpr(unknownCursor, tu);
                default:
                    return "";
            }
        }

        internal string translateUnexposedExpr(Cursor unexposedExpr, TranslationUnit tu)
        {

            if (unexposedExpr.Children.Count > 0 && unexposedExpr.Children[0].Kind == CursorKind.DeclRefExpr)
            {
                return unexposedExpr.Children[0].Spelling;
            }
            return "";
        }

        internal string translateFunctionDecl(Cursor functionDecl, TranslationUnit tu)
        {
            string functionName = functionDecl.Spelling;
            string functionReturnType = typeDictionary[functionDecl.ResultType.Spelling];

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

            string functionArguments = translateParamList(paramList, tu);
            string functionBody = translateCompoundStmt(functionDecl, tu);
            return String.Format(cursorDictionary[functionDecl.Kind], functionName, functionReturnType, functionArguments, functionBody);
        }

        internal string translateCompoundStmt(Cursor compoundStmt, TranslationUnit tu)
        {
            string result = "";
            int childCount = compoundStmt.Children.Count;

            //for (int i = 0; i < childCount; i++)
            //{
            //    result += translate(compoundStmt.Children[i], tu);
            //    if (i < childCount - 1)
            //    {
            //        result += "|";
            //    }
            //}

            return result;
        }

        internal string translateParamList(Cursor[] paramList, TranslationUnit tu)
        {
            string translation = "";

            int listCount = paramList.Length;
            for (int i = 0; i < listCount; i++)
            {
                translation += translateParamDecl(paramList[i], tu);
                if (i < listCount - 1)
                {
                    translation += "|";
                }
            }

            return translation;
        }

        internal string translateParamDecl(Cursor paramDecl, TranslationUnit tu)
        {
            try
            {
                string paramName = paramDecl.Spelling;
                string paramType = typeDictionary[paramDecl.Type.Spelling];
                return String.Format(cursorDictionary[paramDecl.Kind], paramName, paramType);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("KeyNotFoundException:" + paramDecl.Type.Spelling);
            }
        }

        internal string translateVarDecl(Cursor varDecl, TranslationUnit tu)
        {
            string varName = varDecl.Spelling;
            string varType = typeDictionary[varDecl.Type.Spelling];
            string varValue = "";

            if (varDecl.Children.Count > 0)
            {
                varValue = translate(varDecl.Children[0], tu);
            }
            
            return String.Format(cursorDictionary[varDecl.Kind], varName, varType, varValue);
        }

        internal string translateIntegerLiteral(Cursor integerLit, TranslationUnit tu)
        {
            return tu.GetText(integerLit.Extent);
        }

        /*
            Load dictionary template file from specified path.
        */
        private void loadDictionary(string pathToTemplate)
        {
            string[] lines = System.IO.File.ReadAllLines(pathToTemplate);

            string[] splitter = new string[] { "::" };
            foreach (string line in lines)
            {
                string[] parts = line.Split(splitter, StringSplitOptions.None);
                CursorKind kind = stringToCursorKind(parts[0]);
                if (kind != 0)
                {
                    cursorDictionary.Add(kind, parts[1]);
                }
                else
                {
                    typeDictionary.Add(parts[0], parts[1]);
                }
            }
        }

        /*
            Converts string to CursorKind.
        */
        private CursorKind stringToCursorKind(string str)
        {
            switch (str)
            {
                case "FunctionDecl":
                    return CursorKind.FunctionDecl;
                case "ParmDecl":
                    return CursorKind.ParmDecl;
                case "VarDecl":
                    return CursorKind.VarDecl;
            }
            return 0;
        }
    }
}
