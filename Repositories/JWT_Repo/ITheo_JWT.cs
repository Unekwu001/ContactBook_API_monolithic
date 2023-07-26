using ContactBook_API.Models;

namespace ContactBook_API.Repositories.JWT_Repo
{

    public interface ITheo_JWT
    {
        string GenerateJwtToken(User user);
    }

}
