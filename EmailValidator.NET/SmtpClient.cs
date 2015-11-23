using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;

namespace EmailValidator.NET
{
    internal class SmtpClient
    {
        private struct SmtpResponse
        {
            public string Raw { get; set; }
            public SmtpStatusCode Code { get; set; }
        }

        private readonly string host;
        private readonly int port;

        public SmtpClient(string host, int port = 25)
        {
            this.host = host;
            this.port = port;
        }

        public bool CheckMailboxExists(string email, out SmtpStatusCode result)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient(this.host, this.port))
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    StreamReader streamReader = new StreamReader(networkStream);

                    this.AcceptResponse(streamReader, SmtpStatusCode.ServiceReady);

                    string mailHost = (new MailAddress(email)).Host;

                    this.SendCommand(networkStream, streamReader, "HELO " + mailHost, SmtpStatusCode.Ok);
                    this.SendCommand(networkStream, streamReader, "MAIL FROM:<check@" + mailHost + ">", SmtpStatusCode.Ok);
                    SmtpResponse response = this.SendCommand(networkStream, streamReader, "RCPT TO:<" + email + ">");
                    this.SendCommand(networkStream, streamReader, "QUIT", SmtpStatusCode.ServiceClosingTransmissionChannel);

                    result = response.Code;

                    return true;
                }
            }
            catch (SocketException e)
            {
                
            }

            result = SmtpStatusCode.GeneralFailure;
            return false;
        }

        private SmtpResponse SendCommand(NetworkStream networkStream, StreamReader streamReader, string command, params SmtpStatusCode[] goodReplys)
        {
            var dataBuffer = Encoding.ASCII.GetBytes(command + "\r\n");
            networkStream.Write(dataBuffer, 0, dataBuffer.Length);

            return this.AcceptResponse(streamReader, goodReplys);
        }

        private SmtpResponse AcceptResponse(StreamReader streamReader, params SmtpStatusCode[] goodReplys)
        {
            string response = streamReader.ReadLine();

            if (string.IsNullOrEmpty(response) || response.Length < 3)
            {
                throw new SmtpClientException("Invalid response");
            }

            SmtpStatusCode smtpStatusCode = this.GetResponseCode(response);

            if (goodReplys.Length > 0 && !goodReplys.Contains(smtpStatusCode))
            {
                throw new SmtpClientException(response);
            }

            return new SmtpResponse
            {
                Raw = response,
                Code = smtpStatusCode
            };
        }

        private SmtpStatusCode GetResponseCode(string response)
        {
            return (SmtpStatusCode) Enum.Parse(typeof(SmtpStatusCode), response.Substring(0, 3));
        }
    }
}