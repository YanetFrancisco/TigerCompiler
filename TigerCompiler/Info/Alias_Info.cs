using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Alias_Info : Type_Info
    {
        public Type_Info Aliased_Type { get; protected set; }

        public Id_Node Alias { get; protected set; }

        public Alias_Info(Id_Node id, Type_Info type): base(type.Basic_Type, type.Nested_Type)
        {
            Alias = id;
            Aliased_Type = type;
        }

        public override void Update(Type_Info type)
        {
            base.Update(type);
            Alias_Info a = type as Alias_Info;
            if (a == null)
                throw new Exception();
            Aliased_Type = a.Aliased_Type;
            Alias = a.Alias;
        }

        public override bool Equals(object obj)
        {
            return Aliased_Type.Equals(obj);
        }
    }
}
