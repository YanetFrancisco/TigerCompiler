using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class While_Node : Flow_Control_Node
    {
        public NonStatement_Node Condition { get { return GetChild(0) as NonStatement_Node; } }

        public Expression_Node Body { get { return GetChild(1) as Expression_Node; } }

        public Label Last_Label;

        public Scope scp { get; set; }


        #region Constructor
        public While_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Condition == null)
            {
                report.AddError(Condition.Line, Condition.CharPositionInLine, "The condition expression of the while statement must return an integer value.");
                Is_Valid = false;
                return;
            }
            Condition.Check_Semantics(scope, report);
            if (Condition.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if (Condition.Is_Valid)
                    report.AddError(Condition.Line, Condition.CharPositionInLine, "The condition expression of the while statement must return an integer value.");
                Is_Valid = false;
                return;
            }
            
            Body.Check_Semantics(scope, report);
            if (Body is Value_Node && (Body as Value_Node).Is_Assign)
            {
                if (!Body.Is_Valid)
                    Is_Valid = false;
                scp = scope;
                return;
            }
            if (Body is NonStatement_Node && (Body as NonStatement_Node).Type_Info.Basic_Type != Tiger_Type.Void)
            {
                if (Body.Is_Valid)
                    report.AddError(Body.Line, Body.CharPositionInLine, "The loop expression of the while statement must not return a value.");
                Is_Valid = false;
                return;
            }
            if (!Body.Is_Valid)
                Is_Valid = false;
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            Label start = g.Define_Label();
            Label end = g.Define_Label();
            Last_Label = end;
            g.Mark_Label(start);
            Condition.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Brfalse, end);
            Body.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Br, start);
            g.Mark_Label(end);
        }
        #endregion
    }
}
