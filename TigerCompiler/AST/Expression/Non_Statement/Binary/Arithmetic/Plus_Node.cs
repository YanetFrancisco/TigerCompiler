using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Plus_Node : Arithmetic_Node
    {
        #region Constructor
        public Plus_Node() { }
        #endregion

        #region Methods
        public override void Generate_Code(IL_Generator g)
        {
            Left.Generate_Code(g);
            Right.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Add);
        }
        #endregion
    }
}
