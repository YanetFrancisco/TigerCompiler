using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Var_Node : Varblock_Node
    {
        public Id_Node Id { get { return GetChild(0) as Id_Node; } }

        public Scope scp { get; set; }

        public Statement_Node Var_Type
        {
            get { return ChildCount == 3 ? GetChild(1) as Statement_Node : null; }
        }

        public NonStatement_Node Expression
        {
            get { return ChildCount == 3 ? GetChild(2) as NonStatement_Node : GetChild(1) as NonStatement_Node; }
        }

        #region Constructor
        public Var_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            //if (Scope.IsStandardFunction(Id.Text))
            //{
            //    report.AddError(Id.Line, Id.CharPositionInLine, Id.Text + " is a function of the Standard Library.");
            //    Is_Valid = false;
            //    return;
            //}

            if (scope.Contain_Info(Id.Text, false))//si contiene una funcion o variable con el mismo nombre declarada en el mismo scope
            {
                report.AddError(Id.Line, Id.CharPositionInLine, "The ID " + Id.Text + " already exist in the current context.");
                Is_Valid = false;
                return;
            }

            //Tiger_Info var_info = scope.Find_Info(Id.Text);
            //if (var_info != null && var_info.Depth == scope.Depth)
            //{
            //    report.AddError(Id.Line, Id.CharPositionInLine, "The ID " + Id.Text + " is already in use.");
            //    Is_Valid = false;
            //    return;
            //}
            Type_Info var_type = null;
            if (Var_Type != null)
            {
                var_type = scope.Find_Type_Info(Var_Type.Text, true);
                if (var_type == null)
                {
                    report.AddError(Var_Type.Line, Var_Type.CharPositionInLine, "The type " + Var_Type.Text + " does not exist in the current context");
                    Is_Valid = false;
                    return;
                }
            }
            if (Expression == null)
            {
                report.AddError(Expression.Line, Expression.CharPositionInLine, "The initialization expression of a variable must return some value");
                Is_Valid = false;
                return;
            }
            Expression.Check_Semantics(scope, report);

            if (Expression.Is_Valid)
            {
                if (var_type != null)
                {
                    bool bothrecord = var_type.Basic_Type == Tiger_Type.Record && Expression.Type_Info.Basic_Type == Tiger_Type.Record;
                    bool botharray = var_type.Basic_Type == Tiger_Type.Array && Expression.Type_Info.Basic_Type == Tiger_Type.Array;

                    if (!var_type.Equals(Expression.Type_Info) || ((bothrecord || botharray)
                                                                   && (var_type.ID != Expression.Type_Info.ID)))
                    {
                        report.AddError(Expression.Line, Expression.CharPositionInLine, "The initialization expression type must match the variable type");
                        Is_Valid = false;
                        return;
                    }

                    else if (Expression.Type_Info is Nil_Info
                        && var_type.Basic_Type != Tiger_Type.Record
                        && var_type.Basic_Type != Tiger_Type.Array
                        && var_type.Basic_Type != Tiger_Type.String)
                    {
                        report.AddError(Expression.Line, Expression.CharPositionInLine, "The variable type is not nulleable");
                        Is_Valid = false;
                        return;
                    }

                }
                else if ((Expression.Type_Info.Basic_Type == Tiger_Type.Void
                    || Expression.Type_Info.Basic_Type == Tiger_Type.Error
                    || Expression.Type_Info.Basic_Type == Tiger_Type.Nil))
                {
                    report.AddError(Expression.Line, Expression.CharPositionInLine, "The initialization expression type " + Expression.Type_Info.Basic_Type + " is not valid");
                    Is_Valid = false;
                    return;
                }
                if (Expression.Type_Info.Depth > scope.Depth)
                {
                    report.AddError(Expression.Line, Expression.CharPositionInLine, "The initiaization expression type is not visible in the variable scope.");
                    Is_Valid = false;
                    return;
                }
                if (var_type == null)
                    var_type = Expression.Type_Info;

                Variable_Info var = new Variable_Info(var_type, false);
                var.ID = Id.Text;
                var.Depth = scope.Depth;
                scope.Add_Info(var.ID, var);
            }
            else
                Is_Valid = false;
            scp = scope;
        }
            
        public override void Generate_Code(IL_Generator g)
        {
            Type type;
            switch (Expression.Type_Info.Basic_Type)
            {
                case Tiger_Type.Int:
                    type = typeof(int);
                    break;
                case Tiger_Type.String:
                    type = typeof(string);
                    break;
                case Tiger_Type.Array:
                    type = typeof(Array);
                    break;
                case Tiger_Type.Record:
                    type = typeof(object);
                    break;
                case Tiger_Type.Nil:
                    type = typeof(object);
                    break;
                default:
                    throw new Exception();
            }
            g.Define_Variable(type, Id.Text, scp);
            Expression.Generate_Code(g);
            FieldBuilder var = scp.Find_Var(Id.Text);
            g.Tiger_Emit(OpCodes.Stsfld, var);
        }
        #endregion

    }
}
