using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
    public class CancelPayout_Response
    {
        public Cancel_Result result { get; set; }
    }
    public class Cancel_Result
    {
        public string status_description { get; set; }
        public DateTime date_updated { get; set; }
    }
}
