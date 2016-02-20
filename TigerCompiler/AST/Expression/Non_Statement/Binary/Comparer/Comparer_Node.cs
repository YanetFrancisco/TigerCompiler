using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public abstract class Comparer_Node : Binary_Node
    {
        public Scope scp { get; set; }

        #region Constructor
        public Comparer_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;
            
            if (Left == null)
            {
                report.AddError(Left.Line, Left.CharPositionInLine, "The operands of order operators must return integer or string values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Left.Check_Semantics(scope, report);
            if (Left.Type_Info.Basic_Type != Tiger_Type.Int && Left.Type_Info.Basic_Type != Tiger_Type.String)
            {
                if(Left.Is_Valid)
				    report.AddError(Left.Line, Left.CharPositionInLine, "The operands of order operators must return integer or string values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
				Is_Valid = false;
                return;
            }
            
			if (Right == null)
            {
                report.AddError(Right.Line, Right.CharPositionInLine, "The operands of order operators must return integer or string values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                Is_Valid = false;
                return;
            }
            Right.Check_Semantics(scope, report);
            if (Right.Type_Info.Basic_Type != Tiger_Type.Int && Right.Type_Info.Basic_Type != Tiger_Type.String)
            {
                if(Right.Is_Valid)
				    report.AddError(Left.Line, Left.CharPositionInLine, "The operands of order operators must return integer or string values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
				Is_Valid = false;
                return;
            }
            if (Left.Type_Info.Basic_Type != Right.Type_Info.Basic_Type )
            {
                Is_Valid = false;
                report.AddError(Line, CharPositionInLine, "The operands of order operators must return integer or string values.");
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Type_Info = scope.Find_Type_Info("int", true);
            scp = scope;

        }
        #endregion
    }
}
