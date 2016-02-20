using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TigerCompiler
{
    public class String_Info : Type_Info
    {
        public String_Info(): base(Tiger_Type.String)
        {
 
        }

        public override bool Equals(object obj)
        {
            return obj is Nil_Info || base.Equals(obj);
        }
    }
}
