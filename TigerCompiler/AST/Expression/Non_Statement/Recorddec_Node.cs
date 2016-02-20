using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Recorddec_Node : NonStatement_Node
    {
        public Fieldlist_Node Field_List { get { return (ChildCount != 0) ? GetChild(0) as Fieldlist_Node : null; } }
        
        public Id_Node Id { get; set; }

        public Scope scp { get; set; }

        #region Constructor
        public Recorddec_Node()
        {
            Id = new Id_Node();
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Field_List != null)
            {
                Field_List.Check_Semantics(scope, report);

                if (!Field_List.Is_Valid)
                {
                    Is_Valid = false;
                    Type_Info = new Type_Info(Tiger_Type.Error);
                }
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            if (Field_List != null)
            {
                foreach (var item in Field_List.Fields)
                {
                    item.Value_Field.Generate_Code(g);
                }
            }
            if (scp.Return_Field(Id.Text).Key != null)
                g.Tiger_Emit(OpCodes.Newobj, scp.Return_Field(Id.Text).Key);
        }
        #endregion
    }
}
