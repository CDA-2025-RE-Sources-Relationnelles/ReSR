using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Extensions;

namespace ReSR.Infrastructure.Adapters;
internal class UserAuthService(
    TimeSpan authenticationTokenExpiry,
    string   tokenIssuer,
    string   tokenAudience,
    string   encodingKey
) : IAccountAuthService<User> {

    #region PROPERTIES

        private readonly TimeSpan authenticationTokenExpiry = authenticationTokenExpiry;
        private readonly string   tokenIssuer               = tokenIssuer;
        private readonly string   tokenAudience             = tokenAudience;

        private readonly SigningCredentials credentials = new (
            key       : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encodingKey)),
            algorithm : SecurityAlgorithms.HmacSha256
        );

    #endregion
    #region CONSTRUCTORS

        public UserAuthService(
            IConfiguration configuration
        ) : this(
            authenticationTokenExpiry : TimeSpan.Parse(configuration["Jwt:Expiry:User"]!),
            tokenIssuer               : configuration["Jwt:Issuer"]!,
            tokenAudience             : configuration["Jwt:Audience"]!,
            encodingKey               : configuration["Jwt:Key"]!
        ) {}

    #endregion
    #region METHODS

        public IResponse<string> TryGenerateToken(User account) {
            if (account.IsAnonymous)
                return Response.Failure<string>(new InvalidOperationException("Impossible de générer un token pour un compte anonymisé !"));

            IEnumerable<Claim> claims = [
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,  account.Username),
                new Claim(JwtRegisteredClaimNames.Name, account.Id.ToString()),
                new Claim(ClaimTypes.Email,             account.Email),
                new Claim(ClaimTypes.Role,              nameof(User))
            ];
            claims = claims.Concat(account.Permissions.GetUniqueValues().Select(x => new Claim(ClaimTypes.Role, x.ToString())));


            JwtSecurityToken token = new (
                issuer             : this.tokenIssuer,
                audience           : this.tokenAudience,
                claims             : claims,
                expires            : DateTime.UtcNow.Add(this.authenticationTokenExpiry),
                signingCredentials : this.credentials
            );

            return Response.Success(new JwtSecurityTokenHandler().WriteToken(token));
        }

    #endregion
    
}