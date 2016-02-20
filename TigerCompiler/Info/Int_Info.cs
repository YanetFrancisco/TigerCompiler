using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Int_Info : Type_Info
    {
        public Int_Info(): base(Tiger_Type.Int)
        {
 
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
