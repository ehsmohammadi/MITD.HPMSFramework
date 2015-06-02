using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    [Serializable]
    public class RuleType : Enumeration, IValueObject<RuleType>
    {
        public static readonly RuleType PreCalculation = new RuleType("1", "PreCalculation");
        public static readonly RuleType PerCalculation = new RuleType("2", "PerCalculation");
        public static readonly RuleType PostCalculation = new RuleType("3", "PostCalculation");

        public RuleType(string value, string name)
            : base(value, name)
        { }


        public bool SameValueAs(RuleType other)
        {
            return Equals(other);
        }

        public static bool operator ==(RuleType left, RuleType right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(RuleType left, RuleType right)
        {
            return !(left == right);
        }
    }
}
