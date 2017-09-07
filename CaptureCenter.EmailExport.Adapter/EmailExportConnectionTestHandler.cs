using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ExportExtensionCommon;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;

namespace CaptureCenter.EmailExport
{
    public class EmailExportConnectionTestHandler : ConnectionTestHandler
    {
        public EmailExportConnectionTestHandler(VmTestResultDialog vmTestResultDialog) : base(vmTestResultDialog)
        {
            TestList.Add(new TestFunctionDefinition()
                { Name = "Try to reach Server (ping)", Function = TestFunction_Ping, ContinueOnError = true });
            //TestList.Add(new TestFunctionDefinition()
            //    { Name = "Try to say EHLO", Function = TestFunction_ehlo, ContinueOnError = true });
            TestList.Add(new TestFunctionDefinition()
                { Name = "Try to send test email", Function = TestFunction_TestEmail });
        }

        #region The test fucntions
        private bool TestFunction_Ping(ref string errorMsg)
        {
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(((EmailExportViewModel_CT)CallingViewModel).Servername);
                if (reply.Status != IPStatus.Success)
                {
                    errorMsg = "Return status = " + reply.Status.ToString();
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
                if (e.InnerException != null) errorMsg += "\n" + e.InnerException.Message;
                return false;
            }
        }

        private bool TestFunction_ehlo(ref string errorMsg)
        {
            EmailExportViewModel_CT viewModel_CT = CallingViewModel as EmailExportViewModel_CT;
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    string server = viewModel_CT.Servername;
                    int port = viewModel_CT.Portnumber;
                    client.Connect(server, port);
                    // As GMail requires SSL we should use SslStream
                    // If your SMTP server doesn't support SSL you can
                    // work directly with the underlying stream
                    using (var stream = client.GetStream())
                    using (var sslStream = new SslStream(stream))
                    {
                        sslStream.AuthenticateAsClient(server);
                        using (var writer = new StreamWriter(sslStream))
                        using (var reader = new StreamReader(sslStream))
                        {
                            writer.WriteLine("EHLO " + server);
                            writer.Flush();
                            Console.WriteLine(reader.ReadLine());
                            // GMail responds with: 220 mx.google.com ESMTP
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = "Could not EHLO. \n" + e.Message;
                if (e.InnerException != null)
                    errorMsg += "\n" + e.InnerException.Message;
                return false;
            }
            return true;
        }

        private bool TestFunction_TestEmail(ref string errorMsg)
        {
            EmailExportViewModel_CT viewModel_CT = CallingViewModel as EmailExportViewModel_CT;
            try
            {
                IEmailExportClient client = viewModel_CT.GetClient();
                client.SendEmail(
                    viewModel_CT.TestEmailAddress, new List<string>() { viewModel_CT.TestEmailAddress }, 
                    null, null, "OCC Email Export Selftest", "Hello World!", null, null
                );
                return true;
            }
            catch (Exception e)
            {
                errorMsg = "Could not send test email\n" + e.Message;
                if (e.InnerException != null)
                    errorMsg += "\n" + e.InnerException.Message;
                return false;
            }
        }
        #endregion
    }
}
