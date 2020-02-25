using System;
using System.Linq;
using System.Net.Mail;
using DnsClient;
using DnsClient.Protocol;

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

            LookupClient dnsClient = new LookupClient() { UseTcpOnly = true };

            var mxRecords = dnsClient.Query(mailAddress.Host, QueryType.MX).AllRecords.MxRecords().ToList();

            if (mxRecords.Count == 0)
            {
                result = EmailValidationResult.NoMailForDomain;
                return true;
            }

            foreach (MxRecord mxRecord in mxRecords)
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient(mxRecord.Exchange.Value);
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
