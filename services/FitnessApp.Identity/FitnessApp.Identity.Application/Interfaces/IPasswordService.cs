using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.ValueObjects;

namespace FitnessApp.Identity.Application.Interfaces
{
    public interface IPasswordService
    {
        Password CreatePassword(string plainPassword);
        bool VerifyPassword(string plainPassword, Password password);
    }
}
