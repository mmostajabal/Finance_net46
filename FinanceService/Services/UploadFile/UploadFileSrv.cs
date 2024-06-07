using FinanceService.Contracts.UploadFile;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FinanceService.Services.UploadFile
{
    public class UploadFileSrv : IUploadFile
    {
        /// <summary>
        /// UploadFile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns>File Name</returns>
        
        public string UploadFile(string path, HttpPostedFileBase file)
        {
           

            String  FileExtension, FileDate, CopyFileName;
            CopyFileName = "";

            FileDate = DateTime.Now.ToString().Replace("/","").Replace(" ", "").Replace(":","");
            
            String fileName = Path.GetFileNameWithoutExtension(file.FileName);
            FileExtension = Path.GetExtension(file.FileName);
            CopyFileName = fileName + "-" + FileDate + "-" + FileExtension;
            String physicalPath = Path.Combine(path, CopyFileName);
            file.SaveAs(physicalPath);

            return CopyFileName;

        }

    }
}
