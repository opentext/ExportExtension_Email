using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using DOKuStar.Diagnostics.Tracing;
using ExportExtensionCommon;

namespace CaptureCenter.EmailExport
{

    [TestClass]
    public class TestEmailExportAdapter
    {
        #region Document tab handling
        [TestMethod]
        [TestCategory("EmailExport adapter test")]
        public void t01_DocumentNameTab()
        {
            OccEnvironment env = new OccEnvironment();

            string targetDir = Path.GetTempPath();
            string document = Path.Combine(targetDir, "Test_EmailExportExport.pdf");
   
            SIEEDocument doc = env.Batch[0];
            doc.PDFFileName = document;


            var td = new[]
            {
                new { n=01, result="abc", annotation="abc", filename= "xyz", spec=""},
                new { n=02, result="xyz", annotation="", filename= "xyz", spec=""},
                new { n=03, result="4711", annotation="", filename= "xyz", spec="<BATCHID>"},
                new { n=04, result="jschacht@opentext.com", annotation="", filename= "xyz", spec="<:Email_From>"},
           };
            int doOnly = 0;

            for (int i = 0; i != td.Length; i++)
            {
                if (doOnly != 0 && td[i].n != doOnly) continue;

                doc.ScriptingName = (td[i].annotation == "") ? null : td[i].annotation;
                doc.InputFileName = td[i].filename;
                if (td[i].spec != "")
                {
                    env.ViewModel.DT.UseInputFileName = false;
                    env.ViewModel.DT.UseSpecification = true;
                    env.ViewModel.DT.Specification = td[i].spec;
                }
                else
                {
                    env.ViewModel.DT.UseInputFileName = true;
                    env.ViewModel.DT.UseSpecification = false;
                    env.ViewModel.DT.Specification = null;
                }

                string pdfFilename = null;
                string pdfFilepath = null;

                EmailExportClient_Mock.SendEmail_CallBack = (to, cc, bcc, documentName, documentPath) =>
                {
                    pdfFilename = documentName;
                    pdfFilepath = documentPath;
                };
                env.Export.ExportBatch(env.ViewModel.Settings, env.Batch);
                
                Assert.AreEqual(td[i].result + ".pdf", pdfFilename);
                Assert.AreEqual(document, pdfFilepath);
            }
            Assert.AreEqual(doOnly, 0);
        }
        #endregion

        #region Connection tab handling
        [TestMethod]
        [TestCategory("EmailExport adapter test")]
        public void t02_ConnectionTab()
        {
            EmailExportSettings settings = new EmailExportSettings();
            EmailExportViewModel vm = new EmailExportViewModel(settings, new EmailExportClient());
            Assert.AreEqual(string.Empty, vm.CT.Servername);
            Assert.AreEqual(25, vm.CT.Portnumber);
            Assert.IsFalse(vm.CT.Secure);
            Assert.AreEqual(string.Empty, vm.CT.Username);
            Assert.AreEqual(string.Empty, vm.CT.Servername);
            Assert.AreEqual(string.Empty, vm.CT.Password);
            Assert.AreEqual(string.Empty, vm.CT.TestEmailAddress);

            vm.CT.Username = "user";
            vm.CT.TestEmailAddress = "user@provider.com";
            Assert.AreEqual("user@provider.com", vm.CT.TestEmailAddress);
            vm.CT.TestEmailAddress = null;
            Assert.AreEqual("user", vm.CT.TestEmailAddress);
        }
        #endregion

        #region Multiple email addresses
        [TestMethod]
        [TestCategory("EmailExport adapter test")]
        public void t03_MultipleAddresses()
        {
            OccEnvironment env = new OccEnvironment();

            List<string> toResult = new List<string>();
            List<string> ccResult = new List<string>();
            List<string> bccResult = new List<string>();

            EmailExportClient_Mock.SendEmail_CallBack = (to, cc, bcc, documentName, documentPath) =>
            {
                toResult = to;
                ccResult = cc;
                bccResult = bcc;
            };
            var td = new[]
            {
                new { n=01, value="", list=0, result=0},
                new { n=02, value="js@xy.com", list=0, result=1},
                new { n=03, value="", list=1, result=1},
                new { n=04, value="js@xy.com", list=1, result=2},
                new { n=05, value="js@xy.com", list=3, result=4},
           };
            int doOnly = 0;

            for (int i = 0; i != td.Length; i++)
            {
                if (doOnly != 0 && td[i].n != doOnly) continue;
                setField(env.Batch[0], "Email_To", td[i].value, td[i].list);
                setField(env.Batch[0], "Email_CC", td[i].value, td[i].list);
                setField(env.Batch[0], "Email_BCC", td[i].value, td[i].list);

                env.Export.ExportBatch(env.ViewModel.Settings, env.Batch);

                Assert.AreEqual(td[i].result, toResult.Count);
                Assert.AreEqual(td[i].result, ccResult.Count);
                Assert.AreEqual(td[i].result, bccResult.Count);
            }

                
        }

        private void setField(SIEEDocument doc, string fieldname, string value, int count)
        {
            SIEEField f = doc.Fieldlist.GetFieldByName(fieldname);
            f.Value = value;
            f.ValueList = new List<string>();
            for (int i = 0; i < count; i++) f.ValueList.Add("someone@somewhere.com");

        }
        #endregion

        #region Password handling
        [TestMethod]
        [TestCategory("EmailExport adapter test")]
        public void t04_PasswordHandling()
        {
            string s = "Hello World";
            string s_Encrypted = PasswordEncryption.Encrypt(s);
            string s_Decrypted = PasswordEncryption.Decrypt(s_Encrypted);
            Assert.AreEqual(s, s_Decrypted);

            OccEnvironment env = new OccEnvironment();
            env.ViewModel.CT.Password = s;
            s_Decrypted = env.ViewModel.CT.Password;
            Assert.AreEqual(s, s_Decrypted);

            s_Decrypted = env.ViewModel.CT.GetClient().Password;
            Assert.AreEqual(s, s_Decrypted);
        }
        #endregion

        #region Utilities
        private class OccEnvironment
        {
            public IEmailExportClient Client { get; set; }
            public EmailExportExport Export { get; set; }
            public EmailExportSettings Settings { get; set; }
            public EmailExportViewModel ViewModel { get; set; }
            public SIEEBatch Batch { get; set; }

            public OccEnvironment()
            {
                Client = new EmailExportClient_Mock();
                Export = new EmailExportExport(Client);
                Settings = createSettings();
                ViewModel = new EmailExportViewModel(Settings, Client);
                Batch = createBatch(Settings);
            }
        }

        private static EmailExportSettings createSettings()
        {
            EmailExportSettings settings = new EmailExportSettings();
            settings.Servername = "localhost";
            settings.Username = "Johannes";
            settings.Password = PasswordEncryption.Encrypt("My Passowrd");
            return settings;
        }

        private static SIEEBatch createBatch(SIEESettings settings)
        {
            SIEEBatch batch = new SIEEBatch();
            SIEEDocument doc = new SIEEDocument();
            batch.Add(doc);
            doc.Fieldlist = settings.CreateSchema();
            doc.Fieldlist.GetFieldByName("Email_From").Value = "jschacht@opentext.com";
            doc.PDFFileName = "docuemnt";
            doc.BatchId = "4711";
            return batch;
        }
        #endregion
    }
}
