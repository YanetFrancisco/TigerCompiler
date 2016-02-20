using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public abstract class Statement_Node : Expression_Node
    {
        #region Constrcutor
        public Statement_Node() { }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
