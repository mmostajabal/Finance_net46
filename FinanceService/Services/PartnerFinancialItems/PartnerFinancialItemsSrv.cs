using FinanceService.Contracts.FinancialItam;
using FinanceService.Contracts.Partner;
using FinanceService.Contracts.PartnerFinancialItem;
using FinanceService.Services.FinanceItem;
using FinanceService.Services.Partner;
using FinanceShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceService.Services.PartnerFinancialItems
{
    public class PartnerFinancialItemsSrv : IPartnerFinancialItems
    {
        private IPartner _partnerSrv;
        private IFinancialItem _financialItamSrv;
        public PartnerFinancialItemsSrv(IPartner partnerSrv, IFinancialItem financialItamSrv)
        {
            _partnerSrv = partnerSrv;
            _financialItamSrv = financialItamSrv;
        }
        /// <summary>
        /// GetPartnerFinancialItem
        /// </summary>
        /// <returns></returns>
        public List<PartnerFinancialItemDTO> GetPartnerFinancialItem()
        {
            List<PartnerFinancialItemDTO> partnerFinancialItems = new List<PartnerFinancialItemDTO>();


            partnerFinancialItems = _partnerSrv.GetPartnerMember().Join(_financialItamSrv.GetAll(), arg => arg.Partner_Id, arg => arg.PARTNER_ID
           , (partner, financeitems) => new PartnerFinancialItemDTO()
           {
               Fee_Percent = partner.Fee_Percent,
               Amount = financeitems.Amount,
               Partner_Id = partner.Partner_Id,
               Parent_Partner_Id = partner.Parent_Partner_Id,
               Date = financeitems.Date
           }).ToList();

            return partnerFinancialItems;
        }

        /// <summary>
        /// SumMemberTeam
        /// </summary>
        /// <returns></returns>
        public List<PartnerFinancialItemDTO> SumMemberTeam()
        {
            List<PartnerFinancialItemDTO> partnerFinancialItems = new List<PartnerFinancialItemDTO>();


            partnerFinancialItems = _partnerSrv.GetPartnerMember().Join(_financialItamSrv.GetAll(), arg => arg.Partner_Id, arg => arg.PARTNER_ID
           , (partner, financeitems) => new PartnerFinancialItemDTO()
           {
               Amount = financeitems.Amount,
               Parent_Partner_Id = partner.Parent_Partner_Id,
           }).GroupBy(g => g.Parent_Partner_Id).Select(g => new PartnerFinancialItemDTO() { Parent_Partner_Id = g.Key, Amount = g.Sum(s => s.Amount) }).ToList();

            return partnerFinancialItems;
        }
        /// <summary>
        /// SumParentMemberTeam
        /// </summary>
        /// <returns></returns>
        public List<PartnerFinancialItemDTO> SumParentMemberTeam()
        {
            List<PartnerFinancialItemDTO> partnerFinancialItems = new List<PartnerFinancialItemDTO>();


            partnerFinancialItems = _partnerSrv.GetPartnerMember().Join(_financialItamSrv.GetAll(), arg => arg.Partner_Id, arg => arg.PARTNER_ID
           , (partner, financeitems) => new PartnerFinancialItemDTO()
           {
               Amount = financeitems.Amount,
               Parent_Partner_Id = partner.Parent_Partner_Id,
           }).Union(_partnerSrv.GetPartnerParents().Join(_financialItamSrv.GetAll(), arg => arg.Partner_Id, arg => arg.PARTNER_ID
           , (partner, financeitems) => new PartnerFinancialItemDTO()
           {
               Amount = financeitems.Amount,
               Parent_Partner_Id = partner.Partner_Id,
           }))
           .GroupBy(g => g.Parent_Partner_Id).Select(g => new PartnerFinancialItemDTO() { Parent_Partner_Id = g.Key, Amount = g.Sum(s => s.Amount) }).ToList();

            return partnerFinancialItems;
        }

        /// <summary>
        /// TotalBenfitMemberTeam
        /// </summary>
        /// <returns></returns>
        public List<PartnerFinancialItemDTO> TotalBenfitMemberTeam()
        {
            List<PartnerFinancialItemDTO> partnerFinancialItems = new List<PartnerFinancialItemDTO>();


            partnerFinancialItems = _partnerSrv.GetPartnerMember().Join(_financialItamSrv.GetAll(), arg => arg.Partner_Id, arg => arg.PARTNER_ID
           , (partner, financeitems) => new PartnerFinancialItemDTO()
           {
               Amount = financeitems.Amount * partner.Fee_Percent,
               Parent_Partner_Id = partner.Parent_Partner_Id,
           }).GroupBy(g => g.Parent_Partner_Id).Select(g => new PartnerFinancialItemDTO() { Parent_Partner_Id = g.Key, Amount = g.Sum(s => s.Amount / 100) }).ToList();

            return partnerFinancialItems;
        }

        /// <summary>
        /// RemoveFinancialItemHasWrongPatnerId
        /// </summary>
        /// <param name="partnerFinancialItems"></param>
        /// <returns>List<FinancialItemsDTO></returns>
        public List<FinancialItemsDTO> RemoveFinancialItemHasWrongPatnerId(List<FinancialItemsDTO> partnerFinancialItems)
        {
            return _partnerSrv.GetAllPartner().Join(partnerFinancialItems, arg => arg.Partner_Id, arg => arg.PARTNER_ID
                , (partner, financeitems) => new FinancialItemsDTO()
                {
                    Amount = financeitems.Amount,
                    PARTNER_ID = partner.Partner_Id,
                    Date = financeitems.Date
                }).Where(c => c.Date <= DateTime.Now).ToList();

        }

        /// <summary>
        /// CalculateMembers
        /// </summary>
        /// <returns></returns>
        public List<CalculateMembersDTO> CalculateMembers()
        {

            return (from parnet in _partnerSrv.GetPartnerParents()
                    join child in _partnerSrv.GetPartnerMember() on parnet.Partner_Id equals child.Parent_Partner_Id
                    join financialItem in (_financialItamSrv.GetAll().GroupBy(g => g.PARTNER_ID).Select(s => new { Partner_Id = s.Key, TotalAmount = s.Sum(o => o.Amount) }))
                    on child.Partner_Id equals financialItem.Partner_Id
                    join financialItemParent in _financialItamSrv.GetAll().GroupBy(g => g.PARTNER_ID).Select(s => new { Partner_Id = s.Key, ParentTotalAmount = s.Sum(o => o.Amount) })
                    on parnet.Partner_Id equals financialItemParent.Partner_Id
                     
                    group new { parnet, child, financialItem.TotalAmount, financialItemParent.ParentTotalAmount } by new { parnet.Partner_Id } into parnetGroup
                    select new CalculateMembersDTO()
                    {
                        Partner_Id = parnetGroup.Key.Partner_Id,
                        Result = parnetGroup.Sum(o => Math.Round(((o.parnet.Fee_Percent - o.child.Fee_Percent) * o.TotalAmount) / 100,2)),
                        Own = parnetGroup.Select(o => Math.Round((o.ParentTotalAmount * o.parnet.Fee_Percent) / 100, 2)).FirstOrDefault(),
                        TotalAmount = parnetGroup.Select(o => o.ParentTotalAmount).FirstOrDefault(),
                        Fee_Percent = parnetGroup.Select(o => o.parnet.Fee_Percent).FirstOrDefault(),


                    }).Union(from child in _partnerSrv.GetPartnerMember()
                             join financialItem in _financialItamSrv.GetAll().GroupBy(g => g.PARTNER_ID).Select(s => new { Partner_Id = s.Key, TotalAmount = s.Sum(o => o.Amount) })
                             on child.Partner_Id equals financialItem.Partner_Id

                             group new { child, financialItem.TotalAmount } by new { child.Partner_Id } into parnetGroup
                             select new CalculateMembersDTO()
                             {
                                 Partner_Id = parnetGroup.Key.Partner_Id,
                                 Result = 0,
                                 Own = parnetGroup.Sum(o => Math.Round((o.TotalAmount * o.child.Fee_Percent) / 100, 2)),
                                 TotalAmount = parnetGroup.Select(o => o.TotalAmount).FirstOrDefault(),
                                 Fee_Percent = parnetGroup.Select(o => o.child.Fee_Percent).FirstOrDefault(),

                             })

                    .ToList();
        }
    }
}
