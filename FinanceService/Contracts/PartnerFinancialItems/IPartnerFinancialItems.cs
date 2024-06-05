using FinanceShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceService.Contracts.PartnerFinancialItem
{
    public  interface IPartnerFinancialItems
    {
        List<PartnerFinancialItemDTO> GetPartnerFinancialItem();
        List<PartnerFinancialItemDTO> SumMemberTeam();
        List<PartnerFinancialItemDTO> SumParentMemberTeam();
        List<PartnerFinancialItemDTO> TotalBenfitMemberTeam();
        List<FinancialItemsDTO> RemoveFinancialItemHasWrongPatnerId(List<FinancialItemsDTO> partnerFinancialItems);

        List<CalculateMembersDTO> CalculateMembers();
    }
}
