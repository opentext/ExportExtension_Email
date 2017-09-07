using System;
using System.Drawing;
using ExportExtensionCommon;

namespace CaptureCenter.EmailExport
{
    public class EmailExportFactory : SIEEFactory
    {
        public override SIEESettings CreateSettings() { return new EmailExportSettings(); }
        public override SIEEUserControl CreateWpfControl() { return new EmailExportControlWPF(); }
        public override SIEEViewModel CreateViewModel(SIEESettings settings)
        {
            return new EmailExportViewModel(settings, new EmailExportClient());
        }
        public override SIEEExport CreateExport() { return new EmailExportExport(new EmailExportClient()); }
        public override SIEEDescription CreateDescription() { return new EmailExportDescription(); }
    }

    class EmailExportDescription : SIEEDescription
    {
        public override string TypeName { get { return "EmailExport"; } }

        public override string GetLocation(SIEESettings s)
        {
            EmailExportSettings es = s as EmailExportSettings;
            return es.Username + "@" + es.Servername;
        }

        public override Image Image { get { return Properties.Resources.Icon; } }
    }
}
