using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Service
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;
        private const string BuyerRoleName = "Buyer";
        private const string SellerRoleName = "Seller";

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(_issuer));
            _audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(_audience));
            _secretKey = _configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException(nameof(_secretKey));
        }
        public string GenerateJwtToken(User user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Type == UserType.BUYER ? BuyerRoleName : SellerRoleName)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}