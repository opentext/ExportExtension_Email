using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ExportExtensionCommon;
using System.Windows.Input;

namespace CaptureCenter.EmailExport
{
    public partial class EmailExportControlWPF : SIEEUserControl
    {
        public EmailExportControlWPF()
        {
            InitializeComponent();
            this.Loaded += bindCommands;
        }

        #region Connection tab
        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((EmailExportViewModel)DataContext).CT.PasswordChangedHandler();
        }
        #endregion

        #region Tab handling
        private Dictionary<string, bool> tabActivation = null;

        private void initializeTabActivation()
        {
            tabActivation = new Dictionary<string, bool>();
            foreach (string name in ((EmailExportViewModel)DataContext).Tabnames.Keys) tabActivation[name] = false;
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((EmailExportViewModel)DataContext).Tabnames == null) return;
            if (tabActivation == null) initializeTabActivation();
            foreach (string tabName in tabActivation.Keys)
            {
                TabItem pt = (TabItem)LogicalTreeHelper.FindLogicalNode((DependencyObject)sender, tabName);
                if (pt.IsSelected)
                {
                    if (tabActivation[tabName]) return;
                    tabActivation[tabName] = true;
                    try { ((EmailExportViewModel)DataContext).ActivateTab(tabName); }
                    finally { tabActivation[tabName] = false; }
                    return;
                }
            }
        }

        private void Button_AddTokenToFile(object sender, RoutedEventArgs e)
        {
            EmailExportViewModel vm = ((EmailExportViewModel)DataContext);
            vm.DT.AddTokenToFileHandler((string)((Button)sender).Tag);
        }
        #endregion

        #region Commands
        public static RoutedUICommand TestConnection { get { return testConnection; } }
        private static RoutedUICommand testConnection = new RoutedUICommand(
            "Test connection", "testConnection", typeof(EmailExportControlWPF));

        public static RoutedUICommand Version { get { return version; } }
        private static RoutedUICommand version = new RoutedUICommand(
            "Show version", "showVersion", typeof(EmailExportControlWPF),
            new InputGestureCollection(new List<InputGesture>() {
                new KeyGesture(Key.V, ModifierKeys.Alt)
        }));

        public static RoutedUICommand Login { get { return login; } }
        private static RoutedUICommand login = new RoutedUICommand(
            "Login", "Login to the World", typeof(EmailExportControlWPF),
            new InputGestureCollection(new List<InputGesture>() {
                new KeyGesture(Key.L, ModifierKeys.Control)
        }));

        private void bindCommands(object sender, RoutedEventArgs ee)
        {
            CommandBindings.Add(new CommandBinding(
                EmailExportControlWPF.TestConnection,
                (s, e) => { ((EmailExportViewModel)DataContext).CT.TestButtonHandler(); }));
            CommandBindings.Add(new CommandBinding(
                EmailExportControlWPF.Version,
                (s, e) => { ((EmailExportViewModel)DataContext).CT.ShowVersion(); }));
            //CommandBindings.Add(new CommandBinding(
            //    EmailExportControlWPF.Login,
            //    (s, e) => { ((EmailExportViewModel)DataContext).LoginButtonHandler(); }));

            //CommandBindings.Add(new CommandBinding(
            //    ProcessSuiteExportConnectorCommands.Select,
            //    (s, e) => { ((VmProcessSuiteSettings)this.DataContext).SelectEntityButtonHandler(); },
            //    (s, e) => { e.CanExecute = ((VmProcessSuiteSettings)DataContext).EntitiesVM.CanSelectEntityButtonHandler(); }));
        }
        #endregion
    }
}
