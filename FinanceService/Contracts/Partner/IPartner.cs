using FinanceShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceService.Contracts.Partner
{
    public interface IPartner
    {
        List<PartnersDTO> CreatePartner(int numberPartner, int numberChild, int minMemberInSet, int minFeePercent, int maxFeePercent);

        List<PartnersDTO> GetPartnerMember();

        List<PartnersDTO> GetAllPartner();

        List<PartnersDTO> GetPartnerParents();

        PartnersDTO GetPartner(decimal PartnerId);
    }
}
