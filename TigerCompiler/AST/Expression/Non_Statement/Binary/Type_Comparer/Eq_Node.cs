using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Eq_Node : Typecomparer_Node
    {
        #region Constructor
        public Eq_Node() { }
        #endregion

        #region Methods
       
        public override void Generate_Code(IL_Generator g)
        {
            Left.Generate_Code(g);
            Right.Generate_Code(g);
            if ((Left as NonStatement_Node).Type_Info.Basic_Type == Tiger_Type.String)
            {
                MethodInfo compare = typeof(String).GetMethod("Equals", new[] { typeof(string), typeof(string) });
                g.Tiger_Emit(OpCodes.Call, compare);
                g.Tiger_Emit(OpCodes.Ldc_I4_1);
                g.Tiger_Emit(OpCodes.Ceq);
            }
            else
            {
                g.Tiger_Emit(OpCodes.Ceq);
            }
        }
        #endregion
    }
}
