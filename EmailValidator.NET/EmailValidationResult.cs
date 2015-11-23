namespace EmailValidator.NET
{
    public enum EmailValidationResult
    {
        Undefined = 0,
        OK = 1,
        InvalidFormat = 10,
        AddressIsEmpty = 11,
        DomainNotResolved = 20,
        NoMailForDomain = 21,
        MailServerUnavailable = 30,
        MailboxUnavailable = 31,
        MailboxStorageExceeded = 32
    }
}