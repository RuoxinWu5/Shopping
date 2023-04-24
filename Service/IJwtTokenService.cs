using Data.Model;

namespace Service
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user);
    }
}