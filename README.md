# EmailValidator.NET
C# SMTP and format email validator

Example:

```
EmailValidator emailValidator = new EmailValidator();
EmailValidationResult result;

if (!emailValidator.Validate(email, out result))
{
    Console.WriteLine("Unable to check email") // no internet connection or mailserver is down / busy
}

switch (result)
{
    case EmailValidationResult.OK:
        Console.WriteLine("Mailbox exists");
        break;

    case EmailValidationResult.MailboxUnavailable:
        Console.WriteLine("Email server replied there is no such mailbox");
        break;

    case EmailValidationResult.MailboxStorageExceeded:
        Console.WriteLine("Mailbox overflow");
        break;

    case EmailValidationResult.NoMailForDomain:
        Console.WriteLine("Emails are not configured for domain (no MX records)");
        break;
}


```