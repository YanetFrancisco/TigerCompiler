using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public abstract class NonStatement_Node : Expression_Node
    {
        public Type_Info Type_Info { get; set; }

        public Scope scp { get; set; }

        #region Constructor
        public NonStatement_Node() 
        {
            //Antes era Error
            Type_Info = new Type_Info(Tiger_Type.Error);
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
