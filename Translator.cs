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
            Translate from a Cursor, will call specific methods depending on the type of cursor.
        */
        public string translate(Cursor unknownCursor, TranslationUnit tu)
        {
            if (unknownCursor.IsExpression)
            {
                return translateExpr(unknownCursor, tu);
            }
            else if (unknownCursor.IsStatement)
            {
                return translateStmt(unknownCursor, tu);
            }
            else if (unknownCursor.IsDeclaration)
            {
                return translateDecl(unknownCursor, tu);
            }

            return "";
        }

        /*
            Translate a Cursor, assuming it is an expression.
        */
        internal string translateExpr(Cursor expr, TranslationUnit tu)
        {
            switch (expr.Kind)
            {
                case CursorKind.UnexposedExpr:
                    return translateUnexposedExpr(expr, tu);

                case CursorKind.DeclRefExpr:
                    return expr.Spelling;

                case CursorKind.IntegerLiteral:
                case CursorKind.FloatingLiteral:
                case CursorKind.CharacterLiteral:
                    return translateLiteral(expr, tu);

                default:
                    return "";
            }
        }

        /*
            Translate a Cursor, assuming it is a statement.
        */
        internal string translateStmt(Cursor stmt, TranslationUnit tu)
        {
            switch (stmt.Kind)
            {
                case CursorKind.DeclStmt:
                    return translateDeclStmt(stmt, tu);

                default:
                    return "";
            }
        }

        /*
            Translate a Cursor, assuming it is a declaration.
        */
        internal string translateDecl(Cursor decl, TranslationUnit tu)
        {
            switch (decl.Kind)
            {
                case CursorKind.FunctionDecl:
                    return translateFunctionDecl(decl, tu);
                case CursorKind.VarDecl:
                    return translateVarDecl(decl, tu);

                default:
                    return "";
            }
        }

        internal string translateDeclStmt(Cursor declStmt, TranslationUnit tu)
        {
            if (declStmt.Children.Count > 0)
            {
                if (declStmt.Children[0].IsDeclaration)
                {
                    return translateDecl(declStmt.Children[0], tu);
                }
            }
            return "";
        }

        internal string translateUnexposedExpr(Cursor unexposedExpr, TranslationUnit tu)
        {
            string result = "";
            for (int i = 0; i < unexposedExpr.Children.Count; i++)
            {
                if (unexposedExpr.Children[i].IsExpression)
                {
                    result += translateExpr(unexposedExpr.Children[i], tu);
                    if (i < unexposedExpr.Children.Count - 1)
                    {
                        result += "|";
                    }
                }
            }
            return result;
        }

        internal string translateDeclRefExpr(Cursor DeclRefExpr, TranslationUnit tu)
        {
            return DeclRefExpr.Spelling;
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

            int index;
            for (index = 0; index < functionDecl.Children.Count && functionDecl.Children[index].Kind != CursorKind.CompoundStmt; index++)
            {
                paramList[index] = functionDecl.Children[index];
            }

            string functionArguments = translateParamList(paramList, tu);
            string functionBody = translateCompoundStmt(functionDecl.Children[index], tu);
            return String.Format(cursorDictionary[functionDecl.Kind], functionName, functionReturnType, functionArguments, functionBody);
        }

        internal string translateCompoundStmt(Cursor compoundStmt, TranslationUnit tu)
        {
            string result = "";
            int childCount = compoundStmt.Children.Count;

            for (int i = 0; i < childCount; i++)
            {
                result += translate(compoundStmt.Children[i], tu);
                if (i < childCount - 1)
                {
                    result += "|";
                }
            }

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

        internal string translateLiteral(Cursor literal, TranslationUnit tu)
        {
            return tu.GetText(literal.Extent);
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
