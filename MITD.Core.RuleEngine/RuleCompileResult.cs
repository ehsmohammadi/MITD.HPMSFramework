using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine
{
    [Serializable]
    public class RuleCompileResult
    {
        public RuleCompileResult(string message, string libraryText)
        {
            Message = message;
            LibraryText = libraryText;
        }
        public string Message { get; private set; }
        public string LibraryText { get; private set; }
    }
}
