using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Identity.Domain.Exceptions
{
    public class InvalidPasswordException : DomainException
    {
        public InvalidPasswordException() { }

        public InvalidPasswordException(string message) : base(message) { }

        public InvalidPasswordException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
