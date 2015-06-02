using MITD.Domain.Model;

namespace MITD.Core.RuleEngine.Model
{
    public class RuleEngineConfigurationItemId :ObjectWithDbId<long>, IValueObject<RuleEngineConfigurationItemId>
    {
        private readonly string name;
        #region Properties

        public string Name
        {
            get { return name; }
        }
        #endregion

        #region Constructors
        // for Or mapper
        protected RuleEngineConfigurationItemId() { }

        public RuleEngineConfigurationItemId(string name)
        {
            this.name = name;
        }
        #endregion

        #region IValueObject Member
        public bool SameValueAs(RuleEngineConfigurationItemId other)
        {
            return name.Equals(other.name);
        }
        #endregion

        #region Object Override
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (RuleEngineConfigurationItemId)obj;
            return SameValueAs(other);
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override string ToString()
        {
            return name.ToString();
        }

        public static bool operator ==(RuleEngineConfigurationItemId left, RuleEngineConfigurationItemId right)
        {
            return object.Equals(left,right);
        }
        
        public static bool operator !=(RuleEngineConfigurationItemId left, RuleEngineConfigurationItemId right)
        {
            return !(left == right);
        }


        #endregion
    }
}
