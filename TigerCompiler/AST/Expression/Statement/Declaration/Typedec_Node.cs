using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Typedec_Node : Typeblock_Node
    {
        public Id_Node Id { get { return GetChild(0) as Id_Node; } }

        public Statement_Node _Type { get { return GetChild(1) as Statement_Node; } }

        public Scope scp { get; set; }

        #region Constructor
        public Typedec_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (_Type is Arraytype_Node)
            {
                Arraytype_Node array_node = _Type as Arraytype_Node;
                array_node.Id = Id;
                array_node.Check_Semantics(scope, report);

                if (array_node.Is_Valid)
                {
                    Type_Info info = scope.Find_Type_Info(array_node.Array_Type.Text, true);
                    Array_Info array = new Array_Info(info);
                    array.ID = Id.Text;
                    array.Depth = scope.Depth;
                    scope.Add_Type_Info(array);
                }
                else
                    Is_Valid = false;
            }

            else if (_Type is Recordtype_Node)
            {
                Recordtype_Node record_node = _Type as Recordtype_Node;
                record_node.Id = Id;
                record_node.Check_Semantics(scope, report);

                if (record_node.Is_Valid)
                {
                    Record_Info record = new Record_Info();
                    record.ID = Id.Text;
                    record.Depth = scope.Depth;

                    if (record_node.Type_Fields != null)
                    {
                        List<Typefield_Node> fields = record_node.Type_Fields.Fields;
                        foreach (Typefield_Node field in fields)
                        {
                            string name = field.Name_Field.Text;
                            Type_Info info = scope.Find_Type_Info(field.Type_Field.Text, true);
                            record.Add_Field(name, info);
                        }
                    }
                    scope.Add_Type_Info(record);
                }
                else
                    Is_Valid = false;
            }

            else //el tipo es un basic o un alias
            {
                Type_Info type = scope.Find_Type_Info(_Type.Text, true);

                if (type == null)
                {
                    report.AddError(_Type.Line, _Type.CharPositionInLine, "The type " + _Type.Text + " does not exist in the current context");
                    Is_Valid = false;
                    return;
                }

                Alias_Info alias = new Alias_Info(Id, type);
                alias.ID = Id.Text;
                alias.Depth = type.Depth;
                scope.Add_Type_Info(alias);
            }
            scp = scope;
        }

        public void Preliminar_Semantic_Check(Scope scope, Report report)
        {
            Is_Valid = true;
            if (Id.Text == "string " || Id.Text == "int")
            {
                report.AddError(Id.Line, Id.CharPositionInLine, "The type " + Id.Text + " is a basic type.");
                Is_Valid = false;
                return;
            }

            Tiger_Info type_info = scope.Find_Type_Info(Id.Text, false);
            if (type_info != null )
            {
                report.AddError(Id.Line, Id.CharPositionInLine, "The type " + Id.Text + " already exist in the current context.");
                Is_Valid = false;
                return;
            }
            if (_Type is Arraytype_Node)
            {
                Type_Info array = new Array_Info(null);
                array.ID = Id.Text;
                array.Depth = -1;
                scope.Add_Unresolved(array);
            }

            else if (_Type is Recordtype_Node)
            {
                Type_Info record = new Record_Info();
                record.ID = Id.Text;
                record.Depth = -1;
                scope.Add_Unresolved(record);
            }

            else //el tipo es un basic o un alias 
            {
                Type_Info alias = new Alias_Info(Id, new Type_Info(Tiger_Type.Error));
                alias.ID = Id.Text;
                alias.Depth = -1;
                scope.Add_Unresolved(alias);
                //scope.Add_Unresolved(alias, false, Id.Text);

            }
        }

        public override void Generate_Code(IL_Generator g)
        {
            if (_Type is Arraytype_Node)
            {
                (_Type as Arraytype_Node).Generate_Code(g);
            }

            else if (_Type is Recordtype_Node)
            {
                (_Type as Recordtype_Node).Generate_Code(g);
            }

            else //el tipo es un basic o un alias 
            {
                string alias = Id.Text;
                Type type;
                string name = _Type.Text;
                if (name == "int")
                    type = typeof(int);
                else if (name == "string")
                    type = typeof(string);
                else
                    type = scp.Find_Type(name);
                scp.Add_Type(alias, type);
            }
        }
        #endregion
    }
}
