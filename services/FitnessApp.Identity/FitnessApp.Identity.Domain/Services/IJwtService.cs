using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Entities;

namespace FitnessApp.Identity.Domain.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateJwtToken(string token);
        Guid? GetUserIdFromToken(string token);
    }
}
