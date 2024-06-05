using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace FinanceShared
{
    public class CalculateMembersDTO
    {
        public decimal Partner_Id { get; set; }
        public decimal Result { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Own { get; set; }

        public decimal Fee_Percent { get; set; }
        public decimal Total()
        {
            return Result + Own;
        }
    }
}
