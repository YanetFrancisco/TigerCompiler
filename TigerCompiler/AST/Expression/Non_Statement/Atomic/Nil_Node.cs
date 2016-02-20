using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Nil_Node : Atomic_Node
    {
        public Scope scp { get; set; }
        #region Constructor
        public Nil_Node() { }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;
            Type_Info = new Nil_Info();
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            g.Tiger_Emit(OpCodes.Ldnull);
        }
        #endregion
    }
   
}
