using FinanceService.Contracts.PartnerFinancialItem;
using FinanceService.Services.PartnerFinancialItems;
using FinanceShared;
using System.Collections.Generic;
using System.Web.Mvc;


namespace FinanceAPP.Controllers
{
    public class ReportController : Controller
    {
        private IPartnerFinancialItems _partnerFinancialItemsSrv;

        public ReportController(IPartnerFinancialItems partnerFinancialItemsSrv)
        {
            _partnerFinancialItemsSrv = partnerFinancialItemsSrv;

        }
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Total Amounts Member
        /// </summary>
        /// <returns></returns>
        public ActionResult TotalMembers()
        {

            TempData["header"] = "Total Members";
            //List<PartnerFinancialItemDTO> partnerFinancial = _partnerFinancialItemsSrv.SumMemberTeam();
            var partnerFinancial = _partnerFinancialItemsSrv.CalculateMembers();
            return View(partnerFinancial);
        }
        /// <summary>
        /// Total Benfit of members
        /// </summary>
        /// <returns></returns>
        public ActionResult BenfitMembers()
        {

            TempData["header"] = "Total Benfit Members";
            List<PartnerFinancialItemDTO> partnerFinancial = _partnerFinancialItemsSrv.TotalBenfitMemberTeam();
            return View(partnerFinancial);
        }
        /// <summary>
        /// TotalParentMemberTeam
        /// </summary>
        /// <returns></returns>
        public ActionResult TotalParentMemberTeam()
        {
            TempData["header"] = "Total Members";
            List<PartnerFinancialItemDTO> partnerFinancial = _partnerFinancialItemsSrv.SumParentMemberTeam();
            return View(partnerFinancial);

        }

    }
}
