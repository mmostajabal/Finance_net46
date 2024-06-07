using FinanceService.Contracts.Partner;
using FinanceService.Contracts.TransferExcel2List;
using FinanceService.Contracts.UploadFile;
using FinanceService.Services.TransferExcel2List;
using FinanceService.Services.UploadFile;
using FinanceShared;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceAPP.Controllers
{
    public class PartnersController : Controller
    {
        private IPartner _partnerSrv;
        private IUploadFile _uploadFileSrv;
        private ITransferExcel2List<PartnersDTO> _transferExcel2ListSrv;

        public PartnersController(IPartner partnerSrv, IUploadFile uploadFileSrv, ITransferExcel2List<PartnersDTO> transferExcel2ListSrv)
        {
            _partnerSrv = partnerSrv;
            _uploadFileSrv = uploadFileSrv;
            _transferExcel2ListSrv = transferExcel2ListSrv;
        }
        // GET: Partners
        /// <summary>
        /// Generate Partner
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                TempData["header"] = "Generate Partner";
                if (_partnerSrv.GetAllPartner().Count == 0)
                {
                    _partnerSrv.CreatePartner(10, 40, 3, 1, 20);
                    Log.Information($"Create {_partnerSrv.GetAllPartner().Count} Partner ");
                }
                return View(_partnerSrv.GetAllPartner().Select(o => new PartnersDTO
                {
                    Fee_Percent = o.Fee_Percent,
                    Partner_Id = o.Partner_Id,
                    Partner_Name = o.Partner_Name,
                    Parent_Partner_Id = o.Parent_Partner_Id,
                    Order = (o.Parent_Partner_Id == 0 ? o.Partner_Id : o.Parent_Partner_Id)
                }).OrderBy(o => o.Order).ToList());
            }
            catch (Exception ex)
            {
                Log.Error($" Details Financial item {ex.ToString()}");
            }

            return View(new PartnersDTO());
        }
        /// <summary>
        ///  Upload 
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            return View();
        }
        /// <summary>
        /// Upload Partner
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string uploadFolder = Server.MapPath("~/Temp/Partners");
                String message = "";
                int numberRecords = 0;
                List<PartnersDTO> partnersItems;
                try
                {
                    string fileName = _uploadFileSrv.UploadFile(uploadFolder, file);
                    partnersItems = _transferExcel2ListSrv.TransferExcel2List(Path.Combine(uploadFolder, fileName));
                    numberRecords = partnersItems.Count;
                    message = "Number records : " + numberRecords.ToString();

                    Log.Information(message);

                    for (int i = 0; i < numberRecords; i++)
                    {
                        partnersItems[i].Order = partnersItems[i].Parent_Partner_Id == 0 ? partnersItems[i].Partner_Id : partnersItems[i].Parent_Partner_Id;
                    }

                    GlobalVariables.PARTNER_LIST = partnersItems;

                    TempData["success"] = message;

                    return View(partnersItems);
                }
                catch (Exception ex)
                {
                    Log.Error($" Upload Financial item {ex.ToString()}");
                }
            }
            return View(new PartnersDTO());
        }
    }
}