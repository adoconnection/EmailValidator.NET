using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;

namespace EmailValidator.NET
{
    public class EmailValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="EmailValidatorException"></exception>
        public bool Validate(string email, out EmailValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                result = EmailValidationResult.AddressIsEmpty;
                return true;
            }

            email = email.Trim();

            MailAddress mailAddress = null;

            try
            {
                mailAddress = new MailAddress(email);
            }
            catch (ArgumentNullException e)
            {
                result = EmailValidationResult.AddressIsEmpty;
                return true;
            }
            catch (ArgumentException e)
            {
                result = EmailValidationResult.AddressIsEmpty;
                return true;
            }
            catch (FormatException e)
            {
                result = EmailValidationResult.InvalidFormat;
                return true;
            }

            if (mailAddress.Address != email)
            {
                result = EmailValidationResult.InvalidFormat;
                return true;
            }

            //////////////////

            DomainName domainName = DomainName.Parse(mailAddress.Host);
            DnsMessage dnsResponse = DnsClient.Default.Resolve(domainName, RecordType.Mx);

            IList<MxRecord> mxRecords = dnsResponse.AnswerRecords.OfType<MxRecord>().ToList();

            if (mxRecords.Count == 0)
            {
                result = EmailValidationResult.NoMailForDomain;
                return true;
            }

            foreach (MxRecord mxRecord in mxRecords)
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient(mxRecord.ExchangeDomainName.ToString());
                    SmtpStatusCode resultCode;

                    if (smtpClient.CheckMailboxExists(email, out resultCode))
                    {
                        switch (resultCode)
                        {
                            case SmtpStatusCode.Ok:
                                result = EmailValidationResult.OK;
                                return true;

                            case SmtpStatusCode.ExceededStorageAllocation:
                                result = EmailValidationResult.MailboxStorageExceeded;
                                return true;

                            case SmtpStatusCode.MailboxUnavailable:
                                result = EmailValidationResult.MailboxUnavailable;
                                return true;
                        }
                    }
                }
                catch (SmtpClientException)
                {
                }
                catch (ArgumentNullException)
                {
                }
            }

            if (mxRecords.Count > 0)
            {
                result = EmailValidationResult.MailServerUnavailable;
                return false;
            }

            result = EmailValidationResult.Undefined;
            return false;
        }
    }
}
