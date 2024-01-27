using MCBAAdminAPI.Models.DataManager;
using MCBAAdminAPI.Models.Repository;
using MCBAAdminAPI.Data;
using Microsoft.AspNetCore.Mvc;
namespace MCBAAdminAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginRepository _loginRepository;

    public LoginController(ILoginRepository loginRepository)
    {
        _loginRepository = loginRepository;
    }

    [HttpPost("lock/{loginId}")]
    public async Task<IActionResult> LockAccount(string loginId)
    {
        await _loginRepository.UpdateLoginStatus(loginId, true);
        return NoContent();
    }

    [HttpPost("unlock/{loginId}")]
    public async Task<IActionResult> UnlockAccount(string loginId)
    {
        await _loginRepository.UpdateLoginStatus(loginId, false);
        return NoContent();
    }
}
