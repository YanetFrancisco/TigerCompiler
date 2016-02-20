using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Neg_Node : Unary_Node
    {
        public Scope scp { get; set; }
        #region Constructor
        public NonStatement_Node Operand { get { return GetChild(0) as NonStatement_Node; } }
        public Neg_Node()
        {
        }
        #endregion 

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;
            
            if (Operand == null)
            {
                report.AddError(Operand.Line, Operand.CharPositionInLine, "The expression of the unary minus operator must return an int value.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Operand.Check_Semantics(scope, report);
            if (Operand.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if(Operand.Is_Valid)
                    report.AddError(Operand.Line, Operand.CharPositionInLine, "The expression of the unary minus operator must return an int value.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Type_Info = scope.Find_Type_Info("int", true);
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            Operand.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Neg);

        }
        #endregion
    }
}
