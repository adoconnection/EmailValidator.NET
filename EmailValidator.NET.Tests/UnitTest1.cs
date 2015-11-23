using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmailValidator.NET.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAvailable()
        {
            EmailValidator emailValidator = new EmailValidator();

            IList<string> emails = new List<string>()
            {
                "mail@zelbike.ru",
                "adoconnection@gmail.com",
                "adoconnection@yandex.ru",
            };

            foreach (string email in emails)
            {
                EmailValidationResult result;

                if (!emailValidator.Validate(email, out result))
                {
                    Assert.Fail("Unable to test " + email);
                }

                Assert.AreEqual(EmailValidationResult.OK, result, email);
            }
        }

        [TestMethod]
        public void TestInvalidDomain()
        {
            EmailValidator emailValidator = new EmailValidator();

            IList<string> emails = new List<string>()
            {
                "mail@zelbike1.ru",
                "adoconnection@gmail4411.com",
                "adoconnection@yndx.ru",
                "mail@по-кружке.рф",
            };

            foreach (string email in emails)
            {
                EmailValidationResult result;

                if (!emailValidator.Validate(email, out result))
                {
                    Assert.Fail("Unable to test " + email);
                }

                Assert.AreEqual(EmailValidationResult.NoMailForDomain, result, email);
            }
        }

        [TestMethod]
        public void TestNoMailForDomain()
        {
            EmailValidator emailValidator = new EmailValidator();

            IList<string> emails = new List<string>()
            {
                "mail@forum.zelbike.ru",
            };

            foreach (string email in emails)
            {
                EmailValidationResult result;

                if (!emailValidator.Validate(email, out result))
                {
                    Assert.Fail("Unable to test " + email);
                }

                Assert.AreEqual(EmailValidationResult.NoMailForDomain, result, email);
            }
        }

        [TestMethod]
        public void TestAddressIsEmpty()
        {
            EmailValidator emailValidator = new EmailValidator();

            IList<string> emails = new List<string>()
            {
                "",
                "  ",
                null,
            };

            foreach (string email in emails)
            {
                EmailValidationResult result;

                if (!emailValidator.Validate(email, out result))
                {
                    Assert.Fail("Unable to test " + email);
                }

                Assert.AreEqual(EmailValidationResult.AddressIsEmpty, result, email);
            }
        }

        [TestMethod]
        public void TestInvalidFormat()
        {
            EmailValidator emailValidator = new EmailValidator();

            IList<string> emails = new List<string>()
            {
                "mail.zelbike1.ru",
                "adoconnec tion@gmail1.com",
                "adoconnection@yandex1@",
            };

            foreach (string email in emails)
            {
                EmailValidationResult result;

                if (!emailValidator.Validate(email, out result))
                {
                    Assert.Fail("Unable to test " + email);
                }

                Assert.AreEqual(EmailValidationResult.InvalidFormat, result, email);
            }
        }

        [TestMethod]
        public void TestMailboxUnavailable()
        {
            EmailValidator emailValidator = new EmailValidator();

            EmailValidationResult result;

            if (!emailValidator.Validate("Adoconnection112233@gmail.com", out result))
            {
                Assert.Fail("Unable to test email");
            }

            Assert.AreEqual(EmailValidationResult.MailboxUnavailable, result);
        }
    }
}
