using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Minus_Node : Arithmetic_Node
    {
        public Scope scp { get; set; }
        #region Constructor
        public Minus_Node() { }
        #endregion

        #region Methods
        public override void Generate_Code(IL_Generator g)
        {
            Left.Generate_Code(g);
            Right.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Sub);
        }
        #endregion
    }
}
