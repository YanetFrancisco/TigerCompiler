using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;


namespace TigerCompiler
{
    public class If_Node : Atomic_Node
    {
        public NonStatement_Node Condition { get { return GetChild(0) as NonStatement_Node; } }

        public Expression_Node Then_Expression { get { return GetChild(1) as Expression_Node; } }

        public Expression_Node Else_Expression { get { return (ChildCount == 3) ? GetChild(2) as Expression_Node : null; } }

        public Scope scp { get; set; }

        #region Constructor
        public If_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Condition == null)
            {
                report.AddError(Line,CharPositionInLine, "The first expression of the if must return an integer value.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Condition.Check_Semantics(scope, report);
                       
            if (Condition.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if (Condition.Is_Valid)
                    report.AddError(Condition.Line, Condition.CharPositionInLine, "The first expression of the if must return an integer value.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            if (Children.Count == 2)
            {
                Then_Expression.Check_Semantics(scope, report);
                if (Then_Expression is Value_Node && (Then_Expression as Value_Node).Is_Assign)
                {
                    if (!Then_Expression.Is_Valid)
                        Is_Valid = false;
                    Type_Info = new Type_Info(Tiger_Type.Void);
                    scp = scope;
                    return;
                }
                if (Then_Expression.Is_Valid)
                {
                    NonStatement_Node then_exp = Then_Expression as NonStatement_Node;

                    if (then_exp != null && then_exp.Type_Info.Basic_Type!= Tiger_Type.Void)
                    {
                        report.AddError(then_exp.Line, then_exp.CharPositionInLine, "The if then expression must not return a value.");
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        return;
                    }
                    Type_Info = new Type_Info(Tiger_Type.Void);
                }
                else
                {
                    Is_Valid = false;
                    Type_Info = new Type_Info(Tiger_Type.Error);
                }
            }
            else
            {
                Then_Expression.Check_Semantics(scope, report);
                if (Then_Expression.Is_Valid)
                {
                    Else_Expression.Check_Semantics(scope, report);
                    if (!Else_Expression.Is_Valid)
                        Is_Valid = false;
                }
                else
                    Is_Valid = false;

                if (Is_Valid)
                {
                    Type_Info info1 = new Type_Info(Tiger_Type.Void);
                    Type_Info info2 = new Type_Info(Tiger_Type.Void);

                    if (Then_Expression is NonStatement_Node)
                        info1 = (Then_Expression as NonStatement_Node).Type_Info;

                    if (Else_Expression is NonStatement_Node)
                        info2 = (Else_Expression as NonStatement_Node).Type_Info;

                    if (!info1.Equals(info2))
                    {
                        report.AddError(Line, CharPositionInLine, "The 'then' expression type must match the 'else' expression type.");
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        return;
                    }
                    if (info1.Depth > scope.Depth)
                    {
                        report.AddError(Line, CharPositionInLine, "The type returned by the if expression is out of scope.");
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        return;
                    }
                    Type_Info = info1;
                }
                else
                    Type_Info = new Type_Info(Tiger_Type.Error);
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            Label jump = g.Define_Label();
            Label end = g.Define_Label();
            Condition.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Brfalse, jump);
            Then_Expression.Generate_Code(g);
            bool statement = ChildCount > 2 ? (Then_Expression is Statement_Node || Else_Expression is Statement_Node) : Then_Expression is Statement_Node;

            if (!statement)
            {
                if (ChildCount == 2 && (Then_Expression as NonStatement_Node).Type_Info.Basic_Type != Tiger_Type.Void)
                    g.Tiger_Emit(OpCodes.Pop);
                else if (ChildCount > 2 && (Then_Expression as NonStatement_Node).Type_Info.Basic_Type !=  (Else_Expression as NonStatement_Node).Type_Info.Basic_Type && (Then_Expression as NonStatement_Node).Type_Info.Basic_Type != Tiger_Type.Void) 
                    g.Tiger_Emit(OpCodes.Pop);
            }
            g.Tiger_Emit(OpCodes.Br, end);
            g.Mark_Label(jump);
            if (ChildCount > 2)
            {
                Else_Expression.Generate_Code(g);

                if (!statement && (Then_Expression as NonStatement_Node).Type_Info.Basic_Type != (Else_Expression as NonStatement_Node).Type_Info.Basic_Type && (Else_Expression as NonStatement_Node).Type_Info.Basic_Type != Tiger_Type.Void)
                    g.Tiger_Emit(OpCodes.Pop);

            }
            g.Mark_Label(end);



        }
        #endregion
    }
}
