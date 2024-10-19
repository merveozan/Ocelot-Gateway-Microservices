using JwtAuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        // JWT security key and validity period
        // we can generate from https://jwt-keys.21no.de/ 
        public const string JWT_SECURITY_KEY = "vKq/TSD4QGbmIVDZHjw5LwteUjs7cYJ/OJdc9Ww8YmM="; // A secret key used for encoding and decoding JWT tokens
        private const int JWT_TOKEN_VALIDITY_MINS = 20; // Token validity in minutes
        private readonly List<UserAccount> userAccountList;

        public JwtTokenHandler()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount { UserName="admin", Password="admin123", Role="Admin" },
                new UserAccount { UserName="user01", Password="user01", Role="User" }
            };
        }

        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrEmpty(authenticationRequest.UserName) || string.IsNullOrEmpty(authenticationRequest.Password))
            {
                return null;
            }

            // Validation - Find the user in the list
            var userAccount = userAccountList.FirstOrDefault(x =>
                x.UserName == authenticationRequest.UserName && x.Password == authenticationRequest.Password);

            if (userAccount == null)
            {
                return null; // If user doesn't exist, return null
            }

            // Set token expiration time
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);

            // Create token key using the secret key
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);

            // Set up claims (Username and Role) included in the token
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, authenticationRequest.UserName),
                new Claim("Role", userAccount.Role)
            });


            // Generate signing credentials using the token key and HMAC SHA256 algorithm
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            // Create the security token descriptor (contains all the JWT token information)
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            // Use JwtSecurityTokenHandler to create and write the token
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            // Write the token as a string
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            // Return the response containing the token and expiration time
            return new AuthenticationResponse
            {
                UserName = userAccount.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token

            };
        }
    }
}
