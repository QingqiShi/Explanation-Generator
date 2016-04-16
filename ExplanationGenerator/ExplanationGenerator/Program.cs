
using ExplanationGenerator.Prototype;
using ExplanationGenerator.UnitTests;

namespace ExplanationGenerator
{
    public class Program
    {
        /// <summary>
        /// Main Method
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            // PrototypeMain.Run(args);

            string templatePath = args[0];
            string sourceFilePath = args[1];
            string outputFilePath = args[2];
            string languageCode = args[3];

            ClangWrapper wrapper = new ClangWrapper();
            wrapper.loadFile(sourceFilePath);

            Translator translator = new Translator(languageCode, templatePath);
            string translation = translator.translate(wrapper.getRoot().Children[0], wrapper.getTranslationUnit());

            Generator generator = new Generator();
            generator.generate(System.IO.File.ReadAllText(sourceFilePath), translation, outputFilePath);
        }
    }
}
