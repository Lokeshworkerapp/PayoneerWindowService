using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.BAL
{
    public class LogWriter
    {
        public static void Add(string MethodName_, string Request_, string resp_, string LoginUserName_, int isedit_)
        {
            var paramObj = new
            {
                MethodName = MethodName_,
                Request = Request_,
                Response = resp_,
                LoginUserName = LoginUserName_,
                //LoginUserName = "Fyorin",
                ProcedureType = isedit_
            };


            _Bal.commonIUD("SP_APLOGINFO", paramObj);

        }

    }
}
