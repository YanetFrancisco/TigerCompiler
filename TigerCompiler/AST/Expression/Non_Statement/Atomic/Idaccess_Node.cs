using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;


namespace TigerCompiler
{
    public class Idaccess_Node : Atomic_Node
    {
        public Id_Node Id { get { return GetChild(0) as Id_Node; } }

        public string Record_Id { get; set; }
        
        public Scope scp { get; set; }

        #region Constructor
        public Idaccess_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            
            KeyValuePair<ConstructorBuilder, List<FieldBuilder>> fields = scp.Return_Field(Record_Id);
            if (fields.Key != null)
            {
                FieldBuilder field = fields.Value.Find(x => x.Name == Id.Text);
                if(field!= null)
                    g.Tiger_Emit(OpCodes.Ldfld, field);
            }   


            
        }
        #endregion
    }
}
