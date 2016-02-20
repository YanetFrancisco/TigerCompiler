using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using Antlr.Runtime;
using Antlr.Runtime.Tree;
using TigerCompiler.Grammar;
using TigerCompiler.AST;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    class Program
    {

        static int Main(string[] args)
        {
            Console.WriteLine("Tiger Compiler version 1.0");
            Console.WriteLine("Copyright(C) 2013-2014 Fabiola Becerra Riera & Yanet Francisco Suarez");
            Console.WriteLine();

            string file = (args.Length == 0) ? @"C:\Users\Yanesita&Machy\Desktop\TigerTester v1.2\tests\success\test_nuevo.tig" : args[0];  
            
            if (!File.Exists(file))
            {
                Console.WriteLine("The .tig file cannot be found.");
                return 1;
            }

            Tiger_Compiler_Program ast;
            Scope scope = new Scope();
            if (!Lexical_Syntatic_Analysis(file, out ast)) return 1;
            if (!Semantic_Analysis(ast, scope)) return 1;
            if (!Generation(ast, file)) return 1;
            return 0;

        }


        private static bool Generation(Tiger_Compiler_Program ast, string exeLocation)
        {
            return ast.GenerateCode(exeLocation);
        }

        private static bool Semantic_Analysis(Tiger_Compiler_Program ast,Scope scope)
        {
            Report report = ast.CheckSemantics(scope);
            foreach (var error in report.List_Errors)
                Console.WriteLine(error);
            return report.List_Errors.Count == 0;
        }

        private static bool Lexical_Syntatic_Analysis(string file, out Tiger_Compiler_Program ast)
        {
            Report report = new Report();

            var stream = new ANTLRFileStream(file);
            var lexer = new tigerLexer(stream) { Errors = report };
            var tokens = new CommonTokenStream(lexer);
            var parser = new tigerParser(tokens) { Errors = report };

            parser.TreeAdaptor = new Adaptor_AST();
            var tree = parser.program(). Tree as Expression_Node;

            if (report.List_Errors.Count > 0)
            {

                foreach (var item in report.List_Errors)
                {
                    Console.WriteLine(item);
                }
                ast = null;
                return false;
            }


            ast = new Tiger_Compiler_Program(tree);
            return true;
        }

    }
}
