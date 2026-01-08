using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Identity.Domain.Services
{
    public interface IPasswordHasher
    {
        (string Hash, string Salt) Hash(string plainPassword);
        bool Verify(string plainPassword, string hash, string salt);
    }
}
