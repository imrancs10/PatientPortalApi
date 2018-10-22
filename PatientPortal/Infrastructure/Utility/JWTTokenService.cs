using PatientPortal.Models.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace PatientPortal.Infrastructure.Utility
{
    public class JWTTokenService
    {
        public string CreateToken(UserClaims userClaims)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddDays(7);

            //http://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            //ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            //{
            //    new Claim("RegistrationNumber", username)
            //});

            const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var now = DateTime.UtcNow;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

            //create the jwt
            var token = new JwtSecurityToken(
              issuer: Convert.ToString("http://localhost:64252"),
              claims: GetClaims(userClaims),
              expires: expires,
              signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private IList<Claim> GetClaims(UserClaims userClaims)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("PatientId", userClaims.PatientId.ToString()));
            claims.Add(new Claim("RegistrationNumber", userClaims.RegistrationNumber));
            claims.Add(new Claim("ValidUpTo", userClaims.ValidUpTo.ToShortDateString()));
            claims.Add(new Claim("CRNumber", userClaims.CRNumber != null ? userClaims.CRNumber : ""));
            return claims;
        }
    }
}