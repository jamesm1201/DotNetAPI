using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotNetAPI.Data;
using DotNetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAPI.Controllers
{
    public class AuthController : ControllerBase 
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;

        //Constructor sets up config and dapper
        public AuthController(IConfiguration config){
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("Register")]
        /**/
        public IActionResult Register(UserForRegistrationDto userForReg){
            //Checks if password confirmation matches password
            if(userForReg.Password == userForReg.PasswordConfirm){
                string sqlCheckUserExists = "SELECT * FROM TutorialAppSchema.Auth WHERE Email = '"
                 + userForReg.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                //Checks if email has already been used to register (in DB)
                if(existingUsers.Count() == 0){
                    byte[] passwordSalt = new byte[128/8];
                    //Generates random byte array for salt
                    using(RandomNumberGenerator rng = RandomNumberGenerator.Create()){
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = GetPasswordHash(userForReg.Password, passwordSalt);

                    string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth([Email],
                    [PasswordHash],
                    [PasswordSalt]) VALUES ('" + userForReg.Email +
                    "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if(_dapper.ExecuteSqlWithParams(sqlAddAuth, sqlParameters)){
                        string sqlAddUser = @"INSERT INTO TutorialAppSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]
                            ) VALUES (" +
                                "'" + userForReg.FirstName + 
                                "', '" + userForReg.FirstName + 
                                "', '" + userForReg.Email + 
                                "', '" + userForReg.Gender + 
                                "', 1)";
                        if(_dapper.ExecuteSql(sqlAddUser)){
                            return Ok();
                        }
                        throw new Exception("Failed to add user");
                    }
                    throw new Exception("Failed to register user");
                }
                throw new Exception("An account with that email already exists");
            }
            throw new Exception("Passwords do not match");
        }

        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin){
            string sqlForHashAndSalt = @"SELECT [PasswordHash], 
                PasswordSalt FROM TutorialAppSchema.Auth 
                WHERE Email = '" +
                userForLogin.Email + "'";
            UserForLoginConfirmDto userForConfirm = _dapper
                .LoadDataSingle<UserForLoginConfirmDto>(sqlForHashAndSalt);
            
            //uses same method as reg to compare hashed (DB stored salt and password)
            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirm.PasswordSalt);

            /*Loop throug byte arrays to check they are the same
            Can't directly compare == because it will compare pointers
            of hash objects*/
            for (int index = 0; index < passwordHash.Length; index++){
                if(passwordHash[index] != userForConfirm.PasswordHash[index] ){
                    return StatusCode(401, "Incorrect Password");
                }
            }
            //Creates a sql statement to get user ID from user logging in using email
            string userIdSql = @"
                    SELECT UserId FROM TutorialAppSchema.Users 
                        WHERE Email = '" + userForLogin.Email + "'";
            
            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            //Generates a token on login
            return Ok(new Dictionary<string, string> {
                {"token", CreateToken(userId)}
            });
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt){
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

        private string CreateToken(int userId){
            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes
                    (_config.GetSection("AppSettings:TokenKey").Value)
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