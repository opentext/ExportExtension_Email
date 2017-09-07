using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Net.Mail;

namespace CaptureCenter.EmailExport
{
    #region Interface
    public interface IEmailExportClient
    {
        CultureInfo Culture { get; set; }
        string Servername { get; set; }
        int Portnumber { get; set; }
        bool Secure { get; set; }
        string Username { get; set; }
        string Password { get;  }
        void SetPassword(string password);
        void SendEmail(
            string from, List<string> to, List<string> cc, List<string> bcc,
            string subject, string body, string documentName, string documentPath);
    }
    #endregion

    #region Exchange types
    #endregion

    #region Implementation
    public class EmailExportClient : IEmailExportClient
    {
        public CultureInfo Culture { get; set; }
        public string Servername { get; set; }
        public int Portnumber { get; set; }
        public  bool Secure { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SendEmail(
            string from, List<string> to, List<string> cc, List<string> bcc, 
            string subject, string body, string documentName, string documentPath)
        {
            using (MailMessage mail = new MailMessage())
            {
                SmtpClient SmtpServer = new SmtpClient(Servername);

                mail.From = new MailAddress(from.Trim());
                if (to != null) foreach (string s in to) mail.To.Add(new MailAddress(s.Trim()));
                if (cc != null) foreach (string s in cc) mail.CC.Add(new MailAddress(s.Trim()));
                if (bcc != null) foreach (string s in bcc) mail.Bcc.Add(new MailAddress(s.Trim()));
                mail.Subject = subject;
                mail.Body = body;

                if (documentPath != null)
                {
                    Attachment attachment = new Attachment(documentPath);
                    if (documentName != null) attachment.Name = documentName;
                    mail.Attachments.Add(attachment);
                }

                SmtpServer.Port = Portnumber;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.EnableSsl = Secure;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Username, Password);

                SmtpServer.Send(mail);
            }
        }
    }
    #endregion
}
