using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
    class Authorization_Response
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public int consented_on { get; set; }
        public string scope { get; set; }
        public object refresh_token { get; set; }
        public int refresh_token_expires_in { get; set; }
        public object id_token { get; set; }
        public object error { get; set; }
        public object error_description { get; set; }
    }
}
