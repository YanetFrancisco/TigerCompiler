using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Recordtype_Node : Declaration_Node
    {
        public Typefields_Node Type_Fields { get { return (ChildCount > 0) ? GetChild(0) as Typefields_Node : null; } }

        public Scope scp { get; set; }

        public Id_Node Id { get; set; }

        #region Constructor
        public Recordtype_Node()
        {
            Id = new Id_Node();
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;
            if (Type_Fields != null)
            {
                Type_Fields.Check_Semantics(scope, report);
                if (!Type_Fields.Is_Valid)
                    Is_Valid = false;
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            List<Type> fields = new List<Type>();
            List<FieldBuilder> field_builders = new List<FieldBuilder>();
            TypeBuilder aux_type = g.Define_Type(Id.Text);
            Record_Info info = scp.Find_Type_Info(Id.Text, true) as Record_Info;

            foreach (var field in info.Fields)
            {
                  switch (field.Value.Basic_Type)
                {
                    case Tiger_Type.Int:
                        field_builders.Add(aux_type.DefineField(field.Key, typeof(int), FieldAttributes.Public));
                        fields.Add(typeof(int));
                        break;
                    case Tiger_Type.String:
                        field_builders.Add(aux_type.DefineField(field.Key, typeof(string), FieldAttributes.Public));
                        fields.Add(typeof(string));
                        break;
                    case Tiger_Type.Array:
                        field_builders.Add(aux_type.DefineField(field.Key, typeof(Array), FieldAttributes.Public));
                        fields.Add(typeof(Array));
                        break;
                    case Tiger_Type.Record :
                        field_builders.Add(aux_type.DefineField(field.Key, typeof(object), FieldAttributes.Public));
                        fields.Add(typeof(object));
                        break;
                    case Tiger_Type.Nil:
                        field_builders.Add(aux_type.DefineField(field.Key, typeof(object), FieldAttributes.Public));
                        fields.Add(typeof(object));
                        break;
                    default:
                        throw new Exception();
                }
                
            }

            ConstructorBuilder ctor = aux_type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, fields.ToArray());
            var il_generator = ctor.GetILGenerator();
            //
            il_generator.Emit(OpCodes.Ldarg_0);
            il_generator.Emit(OpCodes.Call, typeof(object).GetConstructor(System.Type.EmptyTypes));
            //
            for (int i = 0; i < field_builders.Count; i++)
            {
                il_generator.Emit(OpCodes.Ldarg_0);
                il_generator.Emit(OpCodes.Ldarg_S, i+1);
                il_generator.Emit(OpCodes.Stfld, field_builders[i]);
            }
            il_generator.Emit(OpCodes.Ret);
            scp.Add_Type(Id.Text, typeof(object));
            aux_type.CreateType();
            scp.Add_Field(Id.Text, ctor, field_builders);

        }
        #endregion
    }
}
