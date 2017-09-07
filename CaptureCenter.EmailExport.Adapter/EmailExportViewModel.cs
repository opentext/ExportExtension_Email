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
    public class EmailExportViewModel : SIEEViewModel
    {
        #region Construction
        public EmailExportSettings EmailExportSettings;
        public IEmailExportClient EmailExportClient;

        public EmailExportViewModel_CT CT { get; set; }
        public EmailExportViewModel_DT DT { get; set; }

        public EmailExportViewModel(SIEESettings settings, IEmailExportClient emailExportClient)
        {
            EmailExportSettings = settings as EmailExportSettings;
            this.EmailExportClient = emailExportClient;

            CT = new EmailExportViewModel_CT(this);
            DT = new EmailExportViewModel_DT(this);

            SelectedTab = 0;
            IsRunning = false;
        }

        public override void Initialize(UserControl control)
        {
            CT.Initialize(control);
            DT.Initialize(control);
            initializeTabnames(control);
        }

        public override SIEESettings Settings
        {
            get { return EmailExportSettings; }
        }
        #endregion

        #region Properties (general)
        // The settings in this view model just control the visibility and accessibility of the various tabs
        private int selectedTab;
        public int SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; SendPropertyChanged(); }
        }
        private bool dataLoaded;
        public bool DataLoaded
        {
            get { return dataLoaded; }
            set { dataLoaded = value; SendPropertyChanged(); }
        }
        #endregion

        #region Tab activation
        public Dictionary<string, bool> Tabnames;
        // Retrieve tabitem names from user control
        private void initializeTabnames(UserControl control)
        {
            Tabnames = new Dictionary<string, bool>();
            TabControl tc = (TabControl)LogicalTreeHelper.FindLogicalNode(control, "mainTabControl");
            foreach (TabItem tabItem in LogicalTreeHelper.GetChildren(tc)) Tabnames[tabItem.Name] = false;
        }

        public void ActivateTab(string tabName)
        {
            if (Tabnames[tabName]) return;
            IsRunning = true;
            try
            {
                switch (tabName)
                {
                    case "connectionTabItem":   { Tabnames[tabName] = CT.ActivateTab(); break; }
                    case "documentTabItem":     { Tabnames[tabName] = DT.ActivateTab(Settings.CreateSchema()); break; }
                }
            }
            catch (Exception e)
            {
                SIEEMessageBox.Show(e.Message, "Error in " + tabName, MessageBoxImage.Error);
                DataLoaded = false;
                SelectedTab = 0;
                TabNamesReset();
            }
            finally { IsRunning = false; }
        }

        private void TabNamesReset()
        {
            foreach (string tn in Tabnames.Keys.ToList()) Tabnames[tn] = false;
        }
        #endregion
    }
}
