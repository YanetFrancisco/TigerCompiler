using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Int_Node : Atomic_Node
    {
        public Scope scp { get; set; }
        #region Constructor
        public Int_Node() { }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            int value;
            if (!int.TryParse(Text, out value))
            {
                report.AddError(Line, CharPositionInLine, "Invalid integer value");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Type_Info = new Int_Info();
            scp = scope;

        }

        public override void Generate_Code(IL_Generator g)
        {
            var value = int.Parse(Text);
            g.Tiger_Emit(OpCodes.Ldc_I4, value);
        }
        #endregion
    }
   
}
