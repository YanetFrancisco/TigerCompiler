using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public abstract class Typecomparer_Node : Binary_Node
    {
        public Scope scp { get; set; }
        #region Constructor
        public Typecomparer_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;
            
            if (Left == null)
            {
                report.AddError(Left.Line, Left.CharPositionInLine, "The operands of identity operators must return values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Left.Check_Semantics(scope, report);

            if (Right == null)
            {
                report.AddError(Right.Line, Right.CharPositionInLine, "The operands of identity operators must return values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Right.Check_Semantics(scope, report);
            if ((Left.Type_Info is Nil_Info && Right.Type_Info.Basic_Type != Tiger_Type.Record
                                           && Right.Type_Info.Basic_Type != Tiger_Type.Array
                                           && Right.Type_Info.Basic_Type != Tiger_Type.String)
                || (Right.Type_Info is Nil_Info && Left.Type_Info.Basic_Type != Tiger_Type.Record
                                           && Left.Type_Info.Basic_Type != Tiger_Type.Array
                                           && Left.Type_Info.Basic_Type != Tiger_Type.String))
            {
                if (Left.Is_Valid && Right.Is_Valid)
                    report.AddError(Line, CharPositionInLine, "The operands of identity operators must have the same type.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            if (!Left.Type_Info.Equals(Right.Type_Info))
            {
                if(Left.Is_Valid && Right.Is_Valid)
                    report.AddError(Line, CharPositionInLine, "The operands of identity operators must have the same type.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            if (!(Left.Type_Info.Depth <= scope.Depth && Right.Type_Info.Depth <= scope.Depth))
            {
                Is_Valid = false;
                report.AddError(Line, CharPositionInLine, "There is an operand who's type is not visible in the current scope.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Type_Info = scope.Find_Type_Info("int", true);
            scp = scope;
        }
        #endregion
    }
}
