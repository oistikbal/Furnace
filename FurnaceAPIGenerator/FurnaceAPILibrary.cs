using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;

namespace FurnaceAPIGenerator
{
    internal class FurnaceAPILibrary : ILibrary
    {
        private readonly string _apiPath;
        private readonly string _outputPath;
        private readonly string _binPath;
        private readonly string _outputNamespace = "FurnaceEngine";

        public FurnaceAPILibrary(string apiPath, string outputPath, string binPath)
        {
            _apiPath = apiPath;
            _outputPath = outputPath;
            _binPath = binPath;

            Directory.CreateDirectory(_outputPath);
            Directory.CreateDirectory(_binPath);
            Directory.CreateDirectory(_outputPath);
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {

        }

        public void Setup(Driver driver)
        {
            var options = driver.Options;
            driver.ParserOptions.LanguageVersion = CppSharp.Parser.LanguageVersion.CPP20;
            driver.ParserOptions.Verbose = true;


            options.GeneratorKind = GeneratorKind.CSharp;
            options.OutputDir = _outputPath;

            var module = options.AddModule("Entity");
            module.SharedLibraryName = "FurnaceEngine";
            module.OutputNamespace = _outputNamespace;
            module.IncludeDirs.Add(_apiPath);
            module.Headers.Add("entity_api.h");        
        }

        public void SetupPasses(Driver driver)
        {
            driver.Context.TranslationUnitPasses.RenameWithPattern("_", string.Empty, RenameTargets.Class);
        }
    }
}
