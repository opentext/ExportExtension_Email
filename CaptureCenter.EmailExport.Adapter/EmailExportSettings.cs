using System;
using System.Collections.Generic;
using ExportExtensionCommon;
using System.Globalization;

namespace CaptureCenter.EmailExport
{
    [Serializable]
    public class EmailExportSettings : SIEESettings
    {
        #region Contruction
        public EmailExportSettings()
        {
            // Connection tab
            Servername = "";
            Portnumber = 25;
            Secure = false;
            Username = Password = "";
            TestEmailAddress = "";

            // Document tab
            UseSpecification = true;
            Specification = "<BATCHID>_<DOCUMENTNUMBER>";
        }
        #endregion

        #region Properties Connection
        private string servername;
        public string Servername
        {
            get { return servername; }
            set { SetField(ref servername, value); }
        }

        private int portnumber;
        public int Portnumber
        {
            get { return portnumber; }
            set { SetField(ref portnumber, value); }
        }

        private bool secure;
        public bool Secure
        {
            get { return secure; }
            set { SetField(ref secure, value); }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { SetField(ref username, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { SetField(ref password, value); ; }
        }

        private string testEmailAddress;
        public string TestEmailAddress
        {
            get { return string.IsNullOrEmpty(testEmailAddress) ? Username : testEmailAddress; }
            set { SetField(ref testEmailAddress, value); ; }
        }
        #endregion

        #region Properties Document
        private bool useInputFileName;
        public bool UseInputFileName
        {
            get { return useInputFileName; }
            set { SetField(ref useInputFileName, value); }
        }

        private bool useSpecification;
        public bool UseSpecification
        {
            get { return useSpecification; }
            set { SetField(ref useSpecification, value); RaisePropertyChanged(specification_Name); }
        }

        private string specification_Name = "Specification";
        private string specification;
        public string Specification
        {
            get { return useSpecification ? specification : null; }
            set { SetField(ref specification, value); }
        }
        #endregion

        #region Functions
        public void InitializeEmailExportClient(IEmailExportClient emailExportClient)
        {
            emailExportClient.Servername = Servername;
            emailExportClient.Portnumber = Portnumber;
            emailExportClient.Secure = Secure;
            emailExportClient.Username = Username;
            emailExportClient.SetPassword(PasswordEncryption.Decrypt(Password));
        }

        public override SIEEFieldlist CreateSchema()
        {
            SIEEFieldlist schema = new SIEEFieldlist();
            schema.Add(new SIEEField { Name = "Email_From", ExternalId = "Email_From" });
            schema.Add(new SIEEField { Name = "Email_To", ExternalId = "Email_To" });
            schema.Add(new SIEEField { Name = "Email_CC", ExternalId = "Email_CC" });
            schema.Add(new SIEEField { Name = "Email_BCC", ExternalId = "Email_BCC" });
            schema.Add(new SIEEField { Name = "Email_Subject", ExternalId = "Email_Subject" });
            schema.Add(new SIEEField { Name = "Email_Body", ExternalId = "Email_Body" });
            return schema;
        }
        
        public override object Clone()
        {
            return this.MemberwiseClone() as EmailExportSettings;
        }

        public override string GetDocumentNameSpec()
        {
            return Specification;
        }
        #endregion
    }
}