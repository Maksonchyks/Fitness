using FitnessApp.Communication.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("coaches")]
        public async Task<IActionResult> GetCoaches(CancellationToken ct)
        {
            var coaches = await _userRepository.GetCoachesAsync(ct);
            return Ok(coaches);
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetClients(CancellationToken ct)
        {
            var clients = await _userRepository.GetClientsAsync(ct);
            return Ok(clients);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var users = await _userRepository.GetAllActiveAsync(ct);
            return Ok(users);
        }
    }
}
