using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.CSharp;

namespace REPLPlugin.MCS
{
    public class ScriptEvaluator : Evaluator
    {
        private static readonly HashSet<string> StdLib =
                new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) {"mscorlib", "System.Core", "System", "System.Xml"};

        private TextWriter logger;

        public ScriptEvaluator(TextWriter logger) : base(BuildContext(logger))
        {
            this.logger = logger;

            ImportAppdomainAssemblies(ReferenceAssembly);
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;
        }

        private void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            string name = args.LoadedAssembly.GetName().Name;
            if (StdLib.Contains(name))
                return;
            ReferenceAssembly(args.LoadedAssembly);
        }

        private static CompilerContext BuildContext(TextWriter tw)
        {
            var reporter = new StreamReportPrinter(tw);

            var settings = new CompilerSettings
            {
                    Version = LanguageVersion.Experimental,
                    GenerateDebugInfo = false,
                    StdLib = true,
                    Target = Target.Library
            };

            return new CompilerContext(settings, reporter);
        }

        private static void ImportAppdomainAssemblies(Action<Assembly> import)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = assembly.GetName().Name;
                if (StdLib.Contains(name))
                    continue;
                import(assembly);
            }
        }
    }
}