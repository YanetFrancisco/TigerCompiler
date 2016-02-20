using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Type_Info : Tiger_Info
    {
        public Tiger_Type Basic_Type { get; set; }

        public Type_Info Nested_Type { get; set; }

        public Type_Info(Tiger_Type type)
        {
           Basic_Type  = type;
        }

        public Type_Info(Tiger_Type type, Type_Info nested_Type)
        {
            Basic_Type = type;
            Nested_Type = nested_Type;
        }

        public override bool Equals(object obj)
        {
            Type_Info t = obj as Type_Info;
            if (t == null)
                return false;
            if (Basic_Type != t.Basic_Type)
                return false;
            if ((Nested_Type == null && t.Nested_Type != null) || (Nested_Type != null && t.Nested_Type == null))
                return false;
            if (Nested_Type == null && t.Nested_Type == null)
                return true;
            return Nested_Type.Equals(t.Nested_Type);
        }

        public virtual void Update(Type_Info type)
        {
            Basic_Type = type.Basic_Type;
            Nested_Type = type.Nested_Type;
            Depth = type.Depth;
        }
    }
}
