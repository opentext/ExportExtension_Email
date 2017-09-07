using System;
using RightDocs.Common;
using ExportExtensionCommon;
using CaptureCenter.EmailExport;

namespace DOKuStar.EmailExport
{
    [CustomExportDestinationDescription("EmailExportWriter", "ExportExtensionInterface", "SIEE based Writer for Email Export", "OpenText")]
    public class EmailExportWriter: EECExportDestination
    {
        public EmailExportWriter() : base()
        {
            Initialize(new EmailExportFactory());
        }
    }
}