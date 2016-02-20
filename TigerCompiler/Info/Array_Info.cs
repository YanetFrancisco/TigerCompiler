using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Array_Info : Type_Info
    {
        public Array_Info(Type_Info nested_Type) : base(Tiger_Type.Array, nested_Type)
        {

        }

        public override bool Equals(object obj)
        {
            return  obj is Nil_Info || base.Equals(obj);
        }
    }
}
