using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using ExportExtensionCommon;

namespace CaptureCenter.EmailExport
{
    [TestClass]
    public class TestEmailExportClient
    {
        #region Test infrastructure

        public class EmailExportTestSystem
        {
            public string TestSystemName { get; set; }
            public bool Active { get; set; }
            public string Servername { get; set; }
            public int Portnumber { get; set; }
            public bool Secure { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private List<EmailExportTestSystem> testsystems = new List<EmailExportTestSystem>()
        {
           new EmailExportTestSystem()
           {
                TestSystemName = "Default",
                Active = true,
                Servername = "smtp.gmail.com",
                Portnumber = 587,
                Secure = true,
                Username = "xyz@gmail.com",
                Password = "xxxxxxxxx",
            },
        };

        private string testDocument;

        public TestEmailExportClient()
        {
            testsystems = SIEEUtils.GetLocalTestDefinintions(testsystems);
            testDocument = Path.GetTempFileName().Replace(".tmp", ".pdf");
            File.WriteAllBytes(testDocument, Properties.Resources.Document);
        }
        #endregion

        #region Send an email
        [TestMethod]
        [TestCategory("EmailExport client test")]
        public void t01_sendEmail()
        {
            foreach (EmailExportTestSystem ts in testsystems)
                if (ts.Active) t01_sendEmai(ts);
        }
        private void t01_sendEmai(EmailExportTestSystem ts)
        { 
            EmailExportClient emailExportClient = createClient(ts);

            emailExportClient.SendEmail(
                emailExportClient.Username, new List<string>() { emailExportClient.Username },
                null, null,
                "Test email from OCC unit test " + DateTime.Now.ToString(),
                "Hello World, \nRegards, OCC",
                "Dummy name", testDocument
            );
        }
        #endregion

        #region Utilities
        private EmailExportClient createClient(EmailExportTestSystem ts)
        {
            EmailExportClient client = new EmailExportClient();
            client.Servername = ts.Servername;
            client.Portnumber = ts.Portnumber;
            client.Secure = ts.Secure;
            client.Username = ts.Username;
            client.Password = ts.Password;
            return client;
        }
        #endregion
    }
}
