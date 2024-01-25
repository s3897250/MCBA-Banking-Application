namespace MCBAAdminAPI.Models.Repository
{
    public interface ILoginRepository
    {
        Task UpdateLoginStatus(string loginId, bool status);

    }
}
