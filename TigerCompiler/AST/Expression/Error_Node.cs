using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;


namespace TigerCompiler
{
    public class Error_Node : Expression_Node
    {
         public string Message { get; set; }

         public Error_Node() { }

         public override void Check_Semantics(Scope scope, Report report)
         {
             throw new NotImplementedException();
         }

         public override void Generate_Code(IL_Generator g)
         {
             throw new NotImplementedException();
         }
    }
}
