using ClangSharp;
using System;
using System.Collections.Generic;

namespace ExplanationGenerator.Prototype
{
    class Translator
    {
        ClangWrapper wrapper;
        Cursor rootCursor;

        public Translator(ClangWrapper wrapper)
        {
            rootCursor = wrapper.getAST();
            this.wrapper = wrapper;
        }

        public ExplanationTree Translate()
        {
            return traverse(rootCursor, null);
        }

        ExplanationTree traverse(Cursor current, ExplanationTree parent)
        {
            if (parent == null)
            {
                parent = new ExplanationTree("EXPLANATIONS:\n", current.Location);
            }

            string exp = useTemplate(current);
            ExplanationTree tree;
            if (exp == null)
            {
                tree = parent;
            } else
            {
                tree = new ExplanationTree(exp, current.Location);
            }

            foreach (var child in current.Children)
            {
                ExplanationTree childTree = traverse(child, tree);
                if (childTree != null && childTree != tree)
                {
                    tree.addChild(childTree);
                }
            }
            return tree;
        }

        string useTemplate(Cursor current)
        {
            switch (current.Kind)
            {
                case CursorKind.FunctionDecl:
                    return "Define a function with " + current.NumArguments + " arguments of type (" + current.GetArgument(0).Type.Spelling + ", " + current.GetArgument(1).Type.Spelling + ") called " + current.Spelling + ", which returns a value of type " + current.ResultType.Spelling + "\n";
                case CursorKind.ReturnStmt:
                    return "Function returns ";
                case CursorKind.IntegerLiteral:
                    return "an integer literal " + wrapper.getTU().GetText(current.Extent);
                    //case CursorKind.
            }

            return null;
        }
    }

    class ExplanationTree
    {
        string explanation;
        SourceLocation location;
        IList<ExplanationTree> children = new List<ExplanationTree>();

        public ExplanationTree(string explanation, SourceLocation location)
        {
            this.explanation = explanation;
            this.location = location;
        }

        public void addChild(string explanation, SourceLocation location)
        {
            children.Add(new ExplanationTree(explanation, location));
        }

        public void addChild(ExplanationTree tree)
        {
            children.Add(tree);
        }

        public IList<ExplanationTree> getChildren()
        {
            return children;
        }

        public static void DumpTree(ExplanationTree tree)
        {
            Console.Write(tree.explanation);

            foreach (var child in tree.getChildren())
            {
                DumpTree(child);
            }
        }
    }
}
