using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Utils
{
    public class Mail
    {
        public static void SendMail(string MailBody, string MailTo, string MailCc, string fromname, string subject)
        {
            SendMailService.SendMailServiceSoapClient EMS1 = new SendMailService.SendMailServiceSoapClient(SendMailService.SendMailServiceSoapClient.EndpointConfiguration.SendMailServiceSoap);
            string strTeamPwd = "975A8056C8DF50786FB680A96E0CCCBF";
            string strFromAddress = "QSBN@quantacn.com";
            string strMsg = "";
            string strrecipient = MailTo;
            string strccrecipient = MailCc;
            var result = EMS1.SendMailAsync(strTeamPwd, true, strFromAddress, strrecipient, strccrecipient, "", subject, MailBody, "").Result;

        }
    }
}
