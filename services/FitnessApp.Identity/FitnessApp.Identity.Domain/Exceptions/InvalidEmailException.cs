using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Identity.Domain.Exceptions
{
    public class InvalidEmailException : DomainException
    {
        public InvalidEmailException() { }

        public InvalidEmailException(string message) : base(message) { }

        public InvalidEmailException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
