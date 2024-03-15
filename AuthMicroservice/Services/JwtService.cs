using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Interfaces.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text;
using Shared.Configuration.Extensions;

namespace AuthMicroservice.Services {
    public interface IJwtService {
        string GenerateToken(ITokenPayload payload);
        ITokenPayload VerifyToken(string token);
    }

    public class JwtService : IJwtService {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly SymmetricSecurityKey _securityKey;
        private readonly JwtHeader _jwtHeader;

        public JwtService(IConfiguration configuration) {
            _configuration = configuration;
            _jwtHandler = new JwtSecurityTokenHandler();
            _securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetOrThrow("Auth:JwtToken:SecurityKey")));
            var credentials = new SigningCredentials(_securityKey, _configuration.GetOrThrow("Auth:JwtToken:EncryptionAlgorithm"));
            _jwtHeader = new JwtHeader(credentials);
        }

        public string GenerateToken(ITokenPayload payload) {
            var jwtPayload = new JwtPayload() {
                { _configuration.GetOrThrow("Auth:JwtToken:Fields:UserId"), Convert.ToInt32(payload.UserId) },
                { _configuration.GetOrThrow("Auth:JwtToken:Fields:Expiration"), DateTime.UtcNow.AddSeconds(_configuration.GetOrThrow<double>("Auth:JwtToken:Expiration")).Ticks },
            };
            var jwtToken = new JwtSecurityToken(_jwtHeader, jwtPayload);
            var token = _jwtHandler.WriteToken(jwtToken);
            return token;
        }

        public ITokenPayload VerifyToken(string token) {
            _jwtHandler.ValidateToken(token, new TokenValidationParameters() {
                IssuerSigningKey = _securityKey,
                ValidateAudience = false,
                ValidateIssuer = false,
            }, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            return new() {
                UserId = Convert.ToUInt32(jwtSecurityToken.Payload[_configuration.GetOrThrow("Auth:JwtToken:Fields:UserId")])
            };
        }
    }
}