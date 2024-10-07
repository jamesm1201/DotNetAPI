using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAPI.Helpers
{
    public class AuthHelper{
        private readonly IConfiguration _config;
        public AuthHelper(IConfiguration config){
            _config = config;
        }
        
        public byte[] GetPasswordHash(string password, byte[] passwordSalt){
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey")
                .Value + Convert.ToBase64String(passwordSalt);
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                //How random hash will be
                prf: KeyDerivationPrf.HMACSHA256,
                //How many times it will be hashed
                iterationCount: 100000,
                numBytesRequested: 256/8
            );
        }

        public string CreateToken(int userId){
            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes
                    (tokenKeyString != null ? tokenKeyString: "")
                );
            
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                //How long token lasts for before relogging in
                Expires = DateTime.Now.AddDays(1)
            };

            //Handler uses descriptor to create token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}