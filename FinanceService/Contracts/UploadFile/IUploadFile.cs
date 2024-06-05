
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FinanceService.Contracts.UploadFile
{
    public  interface IUploadFile
    {
        string UploadFile(string path, HttpPostedFileBase formFile);
    }
}
