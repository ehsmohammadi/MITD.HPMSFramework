using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Core.RuleEngine.Model
{
    public interface IREConfigeRepository : IRepository
    {
        IList<RuleEngineConfigurationItem> GetAll();
        void Add(RuleEngineConfigurationItem ruleEngineConfigurationItem);
    }
}
