using MCBAAdminAPI.Models.Repository;
using MCBAAdminAPI.Data;
using Microsoft.EntityFrameworkCore;
namespace MCBAAdminAPI.Models.DataManager
{
    public class LoginManager : ILoginRepository
    {
        private readonly MCBAAdminContext _context;

        public LoginManager(MCBAAdminContext context)
        {
            _context = context;
        }
        // Updates the lock status of a specific login.
        public async Task UpdateLoginStatus(string loginId, bool lockStatus)
        {
            var login = await _context.Logins.FirstOrDefaultAsync(l => l.LoginID == loginId);
            if (login != null)
            {
                login.IsLocked = lockStatus;
                await _context.SaveChangesAsync();
            }

        }
    }
}
