using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FluentResponse.Interfaces;
using FluentResponse;

namespace ReSR.Infrastructure.Adapters;
internal class ManagerAuthService(
    TimeSpan authenticationTokenExpiry,
    string   tokenIssuer,
    string   tokenAudience,
    string   encodingKey
) : IAccountAuthService<Manager> {

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

        public ManagerAuthService(
            IConfiguration configuration
        ) : this(
            authenticationTokenExpiry : TimeSpan.Parse(configuration["Jwt:Expiry:Manager"]!),
            tokenIssuer               : configuration["Jwt:Issuer"]!,
            tokenAudience             : configuration["Jwt:Audience"]!,
            encodingKey               : configuration["Jwt:Key:Manager"]!
        ) {}

    #endregion
    #region METHODS

        public IResponse<string> TryGenerateToken(Manager account) {
            Claim[] claims = [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(ClaimTypes.Email,            account.Email),
                new Claim(ClaimTypes.Role,             account.Permissions.ToString()),
            ];


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