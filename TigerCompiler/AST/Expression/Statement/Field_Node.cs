using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public class Field_Node : Statement_Node
    {
        public Id_Node Name_Field { get { return GetChild(0) as Id_Node; } }

        public NonStatement_Node Value_Field { get { return GetChild(1) as NonStatement_Node; } }

        public Scope scp { get; set; }

        #region Constructor
        public Field_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Value_Field == null)
            {
                report.AddError(Value_Field.Line, Value_Field.CharPositionInLine, "The specified expression must return a value.");
                Is_Valid = false;
                return;
            }
            Value_Field.Check_Semantics(scope, report);

            if (!Value_Field.Is_Valid)
                Is_Valid = false;
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}


