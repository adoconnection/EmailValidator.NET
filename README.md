# EmailValidator.NET
C# SMTP and format email validator

NuGet:
```
Install-Package EmailValidator.NET
```

Note, that there is no boadly suppord way to check email existance, so this validator should be used for reference only.
Works with:
* gmail.com
* yandex.ru

Example:

```cs
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

Drawbacks:
* some SMTP servers always reply email is good, however when you send final command, it turns out it is not
* some SMTP servers reply in a specific manner, delaing replies, so it seems like email is not exists
