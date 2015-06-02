using MITD.Core.RuleEngine.Model;
using MITD.Data.NH;
using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.NH
{
    public class REConfigeRepository : NHRepository, IREConfigeRepository
    {
        private NHRepository<RuleEngineConfigurationItem> rep;
        
        private void init()
        {
            rep = new NHRepository<RuleEngineConfigurationItem>(unitOfWork);
        }
        public REConfigeRepository(NHUnitOfWork unitOfWork):base(unitOfWork)
        {
            init();
        }

        public REConfigeRepository(IUnitOfWorkScope unitOfWorkScope)
            : base(unitOfWorkScope)
        {
            init();
        }

        public IList<RuleEngineConfigurationItem> GetAll()
        {
            return rep.GetAll();
        }
        public void Add(RuleEngineConfigurationItem ruleEngineConfigurationItem)
        {
            rep.Add(ruleEngineConfigurationItem);
        }
    }
}
