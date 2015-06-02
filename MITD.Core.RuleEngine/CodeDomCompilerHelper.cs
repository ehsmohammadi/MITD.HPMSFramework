using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace MITD.Core.RuleEngine
{
    public class CodeDomCompilerHelper
    {
        public static CompileResult Compile(String textCommands, params string[] RefrencedAssemblies)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase; 
            UriBuilder uri = new UriBuilder(codeBase); 
            string path = Uri.UnescapeDataString(uri.Path); 
            var assemblyDirectory =  Path.GetDirectoryName(path); 
            
            CodeDomProvider cpd = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.CompilerOptions = "/lib:\"" + assemblyDirectory + "\"";
            if (RefrencedAssemblies != null)
            {
                for (int i = 0; i < RefrencedAssemblies.Length; i++)
                {
                    cp.ReferencedAssemblies.Add(RefrencedAssemblies[i]);
                }
            }
            cp.GenerateExecutable = false;

            // Invoke compilation.
            CompilerResults cr = cpd.CompileAssemblyFromSource(cp, textCommands);
            //cr.Errors.HasErrors
            string errorMsg="";
            var s = new StringBuilder();
            for (int i = 0; i < cr.Errors.Count;i++ )
            {
                if (cr.Errors[i].IsWarning) continue;
                if (i == 0) errorMsg = "CompileErrors : ";
                errorMsg = "error" + (i + 1).ToString() + ":Line (" + cr.Errors[i].Line + ") Column (" + cr.Errors[i].Column + ")" + cr.Errors[i].ErrorText + "  ";
                s.AppendLine(errorMsg);
            }
            errorMsg = s.ToString();
            if (cr.Errors.Count > 0)
            {
                return new CompileResult() { Assembly = null, ErrorMsg = errorMsg,HasError=true };
            }
            else
            {
                return new CompileResult() { Assembly = cr.CompiledAssembly, ErrorMsg = errorMsg,HasError=false };
            }
        }
    }
    public class CompileResult
    {
        public Assembly Assembly { get; set; }
        public string ErrorMsg { get; set; }
        public bool HasError { get; set; }
    }
}

