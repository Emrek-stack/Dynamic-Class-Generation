using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Dynamo
{
 public class Program
    {
        private static readonly IEnumerable<string> DefaultNamespaces =
            new[]
            {
                "System", 
                "System.IO", 
                "System.Net", 
                "System.Linq", 
                "System.Text", 
                "System.Text.RegularExpressions", 
                "System.Collections.Generic"
            };

        private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\{0}.dll";

        private static readonly IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core"))
            };

        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.WindowsRuntimeApplication)
                    .WithOverflowChecks(true)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(DefaultNamespaces);

        public static SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text, Encoding.UTF8);
            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
        }

        public static void Main(string[] args)
        {
            //ReferenceFinder finder = new ReferenceFinder();
            //finder.Find("Read");

            var code = CreateClasses.CreateClass(ClassMetadata());
            //var fileToCompile = @"C:\Users\..\Documents\Visual Studio 2013\Projects\SignalR_Everything\Program.cs";
            var source = code;
            var parsedSyntaxTree = Parse(source, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7));

            var compilation
                = CSharpCompilation.Create("Test.dll", new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);
            try
            {
                //compilation = compilation.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
                compilation = compilation.WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                var result = compilation.Emit(@"c:\Publish\Test.dll");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Assembly assembly = Assembly.LoadFile(@"c:\Publish\Test.dll");
            Type type = assembly.GetType("CodeGenerationSample.Order");
            var obj = Activator.CreateInstance(type);


            var json = "{\"Title\" : \"Emre Karahan\",\"Text\" : \"Yym mncq\",\"Tags\" : \"Very Very Tags\"}";

            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);

            Console.Read();
        }


        public static  Dictionary<string, Type> ClassMetadata()
        {

            string json = @"{""Title"":""System.String"",""Text"":""System.String"",""Count"":""System.Int32""}";

            var props = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Type>>(json);


            //string className = "BlogPost";

            //var props = new Dictionary<string, Type>() {
            //    { "Title", typeof(string) },
            //    { "Text", typeof(string) },
            //    { "Tags", typeof(string) }
                
            //};

            //var values = Newtonsoft.Json.JsonConvert.SerializeObject(props);

            return props;
        }
    }

}