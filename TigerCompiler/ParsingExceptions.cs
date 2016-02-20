using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace TigerCompiler
{
    class ParsingExceptions :Exception
    {
        #region Constructor
        public ParsingExceptions(string message, Exception innerException) : base(message, innerException) { }
        #endregion

        #region Properties
        public RecognitionException RecognitionError 
        {
            get
            {
                return InnerException as RecognitionException;
            }
        }
        #endregion
    }
}
