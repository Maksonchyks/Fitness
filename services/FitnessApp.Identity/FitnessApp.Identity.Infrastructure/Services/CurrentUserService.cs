using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace FitnessApp.Identity.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
                return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public bool IsAuthenticated => UserId.HasValue;

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public IEnumerable<string> Roles
        {
            get
            {
                var roleClaims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
                return roleClaims?.Select(rc => rc.Value) ?? Enumerable.Empty<string>();
            }
        }
    }
}
