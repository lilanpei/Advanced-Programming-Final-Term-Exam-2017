using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Anotation_system.CodeGenerator
{
    class CsharpCodeCompiler
    {
        public CompilerResults CompilerCsharpCode()
        {
            string sourceTemplate =
            @"using System; 
            using System.Collections.Generic;

            namespace foo { 
            @Placeholder
            }";

            string CshrpCode = File.ReadAllText(@"CsharpCode.txt");
            string sourceCode = sourceTemplate.Replace("@Placeholder", CshrpCode);
            CodeSnippetCompileUnit snippetCompileUnit = new CodeSnippetCompileUnit(sourceCode);

            using (CSharpCodeProvider provider =
                new CSharpCodeProvider(new Dictionary<String, String> { { "CompilerVersion", "v3.5" } }))
            {
                CompilerParameters parameters = new CompilerParameters();
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;
                parameters.IncludeDebugInformation = false;

                CompilerResults results = provider.CompileAssemblyFromDom(parameters, snippetCompileUnit);

                if (!results.Errors.HasErrors)
                {
                    return results;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError compilerError in results.Errors)
                        sb.AppendFormat("Error in line {0}:\n\n{1}", compilerError.Line, compilerError.ErrorText);
                    Console.WriteLine("Compiler Error :{0}", sb.ToString());
                    throw new Exception("Compiler Error");
                }
            }
        }
    }
}
