using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Type_Node : Statement_Node
    {
        public Scope scp { get; set; }

        public Tiger_Type Basic_Type
        {
            get
            {
                if (Text == "string")
                  return Tiger_Type.String;
                else if (Text == "int")
                    return Tiger_Type.Int;
                else
                    throw new Exception();
            }
        }        

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            throw new NotImplementedException();
        }

        public override void Generate_Code(IL_Generator cg)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
