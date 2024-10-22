using IdentityServerApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityServerApi.Services
{
    public class JwtTokenHandler
    {
        private readonly IConfiguration _configuration;
        private readonly List<UserAccount> _userAccounts;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenValidityInMinutes;

        public JwtTokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["JwtSettings:SecretKey"];
            _issuer = _configuration["JwtSettings:Issuer"];
            _audience = _configuration["JwtSettings:Audience"];
            _tokenValidityInMinutes = int.Parse(_configuration["JwtSettings:TokenValidityInMinutes"] ?? "20");

            _userAccounts = new List<UserAccount>
            {
                new UserAccount { UserName = "admin", Password = "admin123", Role = "Admin" },
                new UserAccount { UserName = "user01", Password = "user01", Role = "User" }
            };
        }

        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest.UserName) || string.IsNullOrWhiteSpace(authenticationRequest.Password))
            {
                return null;
            }

            // Validate the user credentials.
            var userAccount = _userAccounts.FirstOrDefault(x =>
                x.UserName.Equals(authenticationRequest.UserName, StringComparison.OrdinalIgnoreCase) &&
                x.Password == authenticationRequest.Password);

            if (userAccount == null)
            {
                return null; // User does not exist.
            }

            // Set token expiration time.
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(_tokenValidityInMinutes);

            // Create token key using the secret key.
            var tokenKey = Encoding.ASCII.GetBytes(_secretKey);

            // Set up claims (Username and Role) included in the token.
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, authenticationRequest.UserName),
                new Claim("Role", userAccount.Role)
            });

            // Generate signing credentials using the token key and HMAC SHA256 algorithm.
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            // Create the security token descriptor (contains all the JWT token information).
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials,
                Issuer = _issuer,
                Audience = _audience
            };

            // Use JwtSecurityTokenHandler to create and write the token.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            // Write the token as a string.
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            // Return the response containing the token and expiration time.
            return new AuthenticationResponse
            {
                UserName = userAccount.UserName,
                ExpiresIn = (int)(tokenExpiryTimeStamp - DateTime.UtcNow).TotalSeconds,
                JwtToken = token
            };
        }
    }
}
