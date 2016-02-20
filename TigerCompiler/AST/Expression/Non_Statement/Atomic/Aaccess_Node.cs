using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Aaccess_Node : Atomic_Node
    {
        public NonStatement_Node Index { get { return GetChild(0) as NonStatement_Node; } }
        public Scope scp { get; set; }

        #region Constructor
        public Aaccess_Node()
        {
            
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Index == null)
            {
                report.AddError(Line, CharPositionInLine, "The expression of the array access must return an integer value.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Index.Check_Semantics(scope, report);
            if (Index.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if(Index.Is_Valid)
                    report.AddError(Index.Line, Index.CharPositionInLine, "The expression of the array access must return an integer value.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            switch (Type_Info.Basic_Type)
            {
                case Tiger_Type.Int:
                    Index.Generate_Code(g);
                    g.Tiger_Emit(OpCodes.Ldelem, typeof(int));
                    break;
                case Tiger_Type.String:
                    Index.Generate_Code(g);
                    g.Tiger_Emit(OpCodes.Ldelem, typeof(string));
                    
                    break;
                case Tiger_Type.Array:
                    Index.Generate_Code(g);
                    g.Tiger_Emit(OpCodes.Ldelem, typeof(Array));
                    break;
                case Tiger_Type.Record :
                    Index.Generate_Code(g);
                    g.Tiger_Emit(OpCodes.Ldelem, typeof(object));
                    break;
                case Tiger_Type.Nil:
                    Index.Generate_Code(g);
                    g.Tiger_Emit(OpCodes.Ldelem, typeof(object));
                    break;
            }
        }
        #endregion
    }
}
