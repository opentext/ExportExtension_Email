using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ExportExtensionCommon;
using DOKuStar.Diagnostics.Tracing;
using System.IO;

namespace CaptureCenter.EmailExport
{
    public class EmailExportViewModel_CT :ModelBase
    {
        #region Construction
        private EmailExportViewModel vm { get; set; }
        private EmailExportSettings settings;

        public EmailExportViewModel_CT(EmailExportViewModel vm)
        {
            this.vm = vm;
            settings = vm.EmailExportSettings;
        }

        public void Initialize(UserControl control)
        {
            findPasswordBox(control);
        }

        public bool ActivateTab() { return true; }
        #endregion

        #region Properties ConnectionTab
        public string Servername
        {
            get { return settings.Servername; }
            set { settings.Servername = value; SendPropertyChanged(); }
        }

        public int Portnumber
        {
            get { return settings.Portnumber; }
            set { settings.Portnumber = value; SendPropertyChanged(); }
        }

        public bool Secure
        {
            get { return settings.Secure; }
            set { settings.Secure = value; SendPropertyChanged(); }
        }

        public string Username
        {
            get { return settings.Username; }
            set { settings.Username = value; SendPropertyChanged(); }
        }

        public string TestEmailAddress
        {
            get { return settings.TestEmailAddress; }
            set { settings.TestEmailAddress = value; SendPropertyChanged(); }
        }
        #endregion

        #region Password
        public string Password
        {
            get {
                if (settings.Password == null) return string.Empty;
                return PasswordEncryption.Decrypt(settings.Password);
            }
            set {
                settings.Password = PasswordEncryption.Encrypt(value);
                SendPropertyChanged("Password");
            }
        }

        private PasswordBox passwordBox;
        private void findPasswordBox(UserControl control)
        {
            passwordBox = (PasswordBox)LogicalTreeHelper.FindLogicalNode(control, "passwordBox");
        }
        public void PasswordChangedHandler()
        {
            Password = SIEEUtils.GetUsecuredString(passwordBox.SecurePassword);
        }
        #endregion

        #region Functions Connection
        public IEmailExportClient GetClient()
        {
            vm.EmailExportSettings.InitializeEmailExportClient(vm.EmailExportClient);
            return vm.EmailExportClient;
        }

        public void ShowVersion()
        {
            SIEEMessageBox.Show("EmailExport  connector Version 0.7", "Version", MessageBoxImage.Information);
        }

        private ConnectionTestResultDialog connectionTestResultDialog;
        private ConnectionTestHandler ConnectionTestHandler;

        // Set up objects, start tests (running in the backgroud) and launch the dialog
        public void TestButtonHandler()
        {
            VmTestResultDialog vmConnectionTestResultDialog = new VmTestResultDialog();
            ConnectionTestHandler = new EmailExportConnectionTestHandler(vmConnectionTestResultDialog);
            ConnectionTestHandler.CallingViewModel = this;

            connectionTestResultDialog = new ConnectionTestResultDialog(ConnectionTestHandler);
            connectionTestResultDialog.DataContext = vmConnectionTestResultDialog;
            connectionTestResultDialog.ShowInTaskbar = false;

            //The test environment is Winforms, we then set the window to topmost.
            //In OCC we we can set the owner property
            if (Application.Current == null)
                connectionTestResultDialog.Topmost = true;
            else
                connectionTestResultDialog.Owner = Application.Current.MainWindow;

            ConnectionTestHandler.LaunchTests();
            connectionTestResultDialog.ShowDialog();
        }

        #endregion
    }
}
