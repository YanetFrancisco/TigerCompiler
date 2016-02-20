using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Id_Node : Statement_Node
    {
        public Scope scp { get; set; }
        #region Constructor
        public Id_Node() { }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            throw new NotImplementedException();
        }


        #endregion

        public override void Generate_Code(IL_Generator g)
        {
        }
    }
}
