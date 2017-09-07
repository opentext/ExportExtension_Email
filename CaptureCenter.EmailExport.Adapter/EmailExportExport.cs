using System;
using System.IO;
using System.Collections.Generic;
using ExportExtensionCommon;
using DOKuStar.Diagnostics.Tracing;

namespace CaptureCenter.EmailExport
{
    public class EmailExportExport : SIEEExport
    {
        private IEmailExportClient emailExportClient;

        public EmailExportExport(IEmailExportClient emailExportClient)
        {
            this.emailExportClient = emailExportClient;
        }

        public override void ExportDocument(SIEESettings settings, SIEEDocument document, string name, SIEEFieldlist fieldlist)
        {
            EmailExportSettings mySettings = settings as EmailExportSettings;
            mySettings.InitializeEmailExportClient(emailExportClient);

            emailExportClient.SendEmail(
                fieldlist.GetFieldByName("Email_From").Value,
                parseEmailAddresses(fieldlist.GetFieldByName("Email_To")),
                parseEmailAddresses(fieldlist.GetFieldByName("Email_CC")),
                parseEmailAddresses(fieldlist.GetFieldByName("Email_BCC")),
                fieldlist.GetFieldByName("Email_Subject").Value,
                fieldlist.GetFieldByName("Email_Body").Value,
                name + ".pdf", document.PDFFileName
            );

            string username = mySettings.Username;
            string password = PasswordEncryption.Decrypt(mySettings.Password);

       }

        private List<string> parseEmailAddresses(SIEEField field)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(field.Value)) result.Add(field.Value);
            foreach (string s in field.ValueList)
                if (!string.IsNullOrEmpty(s)) result.Add(s);
            return result;
        }
    }
}
