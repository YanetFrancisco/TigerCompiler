using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Or_Node : Logical_Node
    {
        public Scope scp { get; set; }
        #region Constructor
        public Or_Node() { }
        #endregion

        #region Methods
        public override void Generate_Code(IL_Generator g)
        {
   
            Label is_true = g.Define_Label();
            Label end = g.Define_Label();
            Left.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Brtrue, is_true);
            Right.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Brtrue, is_true);
            g.Tiger_Emit(OpCodes.Ldc_I4_0);
            g.Tiger_Emit(OpCodes.Br, end);
            g.Mark_Label(is_true);
            g.Tiger_Emit(OpCodes.Ldc_I4_1);
            g.Mark_Label(end);
        }
        #endregion
    }
}
