using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;


namespace TigerCompiler
{
    public class For_Node : Flow_Control_Node
    {
        public Id_Node Id { get { return GetChild(0) as Id_Node; } }

        public NonStatement_Node Init_Expression { get { return GetChild(1) as NonStatement_Node; } }

        public NonStatement_Node End_Expression { get { return GetChild(2) as NonStatement_Node; } }

        public Expression_Node Body { get { return GetChild(3) as Expression_Node; } }

        public Label Last_Label;

        public Scope scp { get; set; }

        #region Constructor
        public For_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Init_Expression == null)
            {
                report.AddError(Init_Expression.Line, Init_Expression.CharPositionInLine, "The lower bound expression of the for statement must return an integer value.");
                Is_Valid = false;
                return;
            }
            Init_Expression.Check_Semantics(scope, report);
            if (Init_Expression.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if (Init_Expression.Is_Valid)
                    report.AddError(Init_Expression.Line, Init_Expression.CharPositionInLine, "The lower bound expression of the for statement must return an integer value.");
                Is_Valid = false;
                return;
            }

            if (End_Expression == null)
            {
                report.AddError(End_Expression.Line, End_Expression.CharPositionInLine, "The upper bound expression of the for statement must return an integer value.");
                Is_Valid = false;
                return;
            }
            End_Expression.Check_Semantics(scope, report);
            if (End_Expression.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if (End_Expression.Is_Valid)
                    report.AddError(End_Expression.Line, End_Expression.CharPositionInLine, "The upper bound expression of the for statement must return an integer value.");
                Is_Valid = false;
                return;
            }

            Scope for_scope = scope.Create_Child_Scope();
            Variable_Info iter_var = new Variable_Info(new Int_Info(), true);
            iter_var.ID = Id.Text;
            iter_var.Depth = for_scope.Depth;
            for_scope.Add_Info(iter_var.ID,iter_var);

            Body.Check_Semantics(for_scope, report);

            if (Body is Value_Node && (Body as Value_Node).Is_Assign)
            {
                if (!Body.Is_Valid)
                    Is_Valid = false;
                scp = for_scope;
                return;
            }
            if (Body is NonStatement_Node && (Body as NonStatement_Node).Type_Info.Basic_Type != Tiger_Type.Void)
            {
                if (Body.Is_Valid)
                    report.AddError(Body.Line, Body.CharPositionInLine, "The loop expression of the for statement must not return a value.");
                Is_Valid = false;
                return;
            }
            if (!Body.Is_Valid)
                Is_Valid = false;
            scp = for_scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            Label init = g.Define_Label();
            Label end = g.Define_Label();
            Last_Label = end;
            LocalBuilder to = g.Variable_Temporal(typeof(int));

            FieldBuilder iter = g.Define_Variable(typeof(int), Id.Text,scp);
            Init_Expression.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Stsfld, iter);              //asigno el valor al iterador
            End_Expression.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Stloc, to.LocalIndex);
            g.Mark_Label(init);
            g.Tiger_Emit(OpCodes.Ldsfld, iter);
            g.Tiger_Emit(OpCodes.Ldloc, to.LocalIndex);
            g.Tiger_Emit(OpCodes.Cgt);
            g.Tiger_Emit(OpCodes.Brtrue, end);
            Body.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Ldsfld, iter);
            g.Tiger_Emit(OpCodes.Ldc_I4_1);
            g.Tiger_Emit(OpCodes.Add);
            g.Tiger_Emit(OpCodes.Stsfld, iter);
            g.Tiger_Emit(OpCodes.Br, init);
            g.Mark_Label(end);
         }
        #endregion
    }
}
