using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public abstract class Logical_Node : Binary_Node
    {
        public Scope scp { get; set; }
        #region Constructor               

        public Logical_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Left == null)
            {
                report.AddError(Left.Line, Left.CharPositionInLine, "The expressions of the logical operator must return int values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Left.Check_Semantics(scope, report);
            if (Left.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if (Left.Is_Valid)
                    report.AddError(Line, CharPositionInLine, "The expressions of the logical operator must return int values.");
                Is_Valid = false;
                Type_Info.Basic_Type = Tiger_Type.Error;
                return;
            }

            if (Right == null)
            {
                report.AddError(Right.Line, Right.CharPositionInLine, "The expressions of the logical operator must return int values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Right.Check_Semantics(scope, report);
            if (Right.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if (Right.Is_Valid)
                    report.AddError(Line, CharPositionInLine, "The expressions of the logical operator must return int values.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }

            Type_Info = scope.Find_Type_Info("int", true);
            scp = scope;


        }
        #endregion
    }
}
