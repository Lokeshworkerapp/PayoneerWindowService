using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
  public  class PayeeFormateRequest
    {
        public string country { get; set; }
        public string currency { get; set; }
        public string payee_type { get; set; }
        public string program_id { get; set; }
    }
}
