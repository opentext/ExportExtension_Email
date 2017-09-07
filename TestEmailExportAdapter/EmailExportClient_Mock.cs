using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace CaptureCenter.EmailExport
{
    public class EmailExportClient_Mock : IEmailExportClient
    {
        public CultureInfo Culture { get; set; }
        public string Servername { get; set; }
        public int Portnumber { get; set; }
        public bool Secure { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void SetPassword(string password)
        {
            Password = password;
        }

        // Send email
        public delegate void SendEmail_Function(
            List<string> to, List<string> cc, List<string> bcc,
            string documentName, string documentPath);
        public static SendEmail_Function SendEmail_CallBack = null;
        public void SendEmail(
            string from, List<string> to, List<string> cc, List<string> bcc,
            string subject, string body, string documentName, string documentPath)
        {
            if (SendEmail_CallBack == null)
                throw new Exception("SendEmail not support by mocked client");
            SendEmail_CallBack(to, cc, bcc, documentName, documentPath);
        }
    }
}
