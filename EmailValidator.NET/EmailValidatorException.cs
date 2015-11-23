using System;
using System.Runtime.Serialization;

namespace EmailValidator.NET
{
    public class EmailValidatorException :  Exception
    {
        public EmailValidatorException()
        {
        }

        public EmailValidatorException(string message) : base(message)
        {
        }

        public EmailValidatorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmailValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}