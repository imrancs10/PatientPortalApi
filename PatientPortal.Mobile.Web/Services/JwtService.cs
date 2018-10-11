using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PatientPortal.Mobile.Data;
using PatientPortal.Mobile.Data.Entites;
//using PatientPortal.Mobile.Data.Entites;
using PatientPortal.Mobile.Web.Infrastructure.Utilities;
using PatientPortal.Mobile.Web.Models;
using PatientPortal.Mobile.Web.Services.Interface;
//using PatientPortal.Mobile.Web.Services.Interface;
using SessionManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Services
{
    public class JwtService : IJwtService
    {
        readonly Func<PatientPortalMobileContext> _dbContextProvider;
        readonly IPasswordHasherService _passwordHasherService;
        readonly IUserService _userService;
        //readonly IContactsService _contactsService;
        //readonly IFCMTokenService _fcmTokenService;
        readonly IConfiguration _configuration;
        const string TokenIssuer = "JwtToken:Issuer";
        const string TokenSecretConfKey = "JwtToken:Secret";
        const string TokenValidityConfKey = "JwtToken:TokenValidity";


        public JwtService(Func<PatientPortalMobileContext> dbContextProvider,
             IPasswordHasherService passwordHasherService,
            IUserService userService,
             IConfiguration configuration
             )

        {
            _dbContextProvider = dbContextProvider;
            _passwordHasherService = passwordHasherService;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<Errorable<JwtResponse>> Login(
            PatientInfo user,
            JwtRequest jwtRequest)
        {
            var jwtToken = String.Empty;

            if (ValidateMobileUser(jwtRequest.Password, user))
            {
                jwtToken = await GetJwtTokenForUser(user);
            }

            if (!string.IsNullOrEmpty(jwtToken))
            {
                var response = await CreateJwtResponse(jwtToken);
                return new Errorable<JwtResponse>(response);
            }

            return new Errorable<JwtResponse>(ErrorCode.InvalidUsernameOrPassword);
        }
        private string GetLoginToken(UserClaims userClaims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[TokenSecretConfKey]));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenValidityInMinuts = Convert.ToDouble(_configuration[TokenValidityConfKey]);
            var token = new JwtSecurityToken(
                issuer: Convert.ToString(_configuration[TokenIssuer]),
                claims: GetClaims(userClaims),
                expires: DateTime.Now.AddMinutes(tokenValidityInMinuts),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private IList<Claim> GetClaims(UserClaims userClaims)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("PatientId", userClaims.PatientId.ToString()));
            claims.Add(new Claim("RegistrationNumber", userClaims.RegistrationNumber.ToString()));
            claims.Add(new Claim("ValidUpto", userClaims.ValidUpto));
            return claims;
        }

        public bool ValidateMobileUser(string password
            , PatientInfo mobileUser
            )
        {
            var isValid = false;
            if (!String.IsNullOrEmpty(password))
            {
                isValid = mobileUser.Password == password;
            }
            return isValid;
        }

        public async Task<JwtResponse> CreateJwtResponse(string jwtToken)
        {
            var jwtResponse = new JwtResponse();
            jwtResponse.JwtToken = jwtToken;
            return jwtResponse;
        }

        public async Task<string> GetJwtTokenForUser(PatientInfo mobileUser)
        {
            return await GetJwtTokenForContactPointUser(mobileUser);
        }

        private async Task<string> GetJwtTokenForContactPointUser(PatientInfo mobileUser)
        {
            var claims = new UserClaims()
            {
                PatientId = mobileUser.PatientId,
                RegistrationNumber = mobileUser.RegistrationNumber,
                ValidUpto = mobileUser.ValidUpto != null ? mobileUser.ValidUpto.Value.ToShortDateString() : DateTime.Now.ToShortDateString()
            };
            return GetLoginToken(claims);
        }

        ////private async Task<string> GetJwtTokenForENSUser(
        ////    //MobileUser mobileUser
        ////    )
        ////{
        ////    //var contactDetail = await _contactsService.GetENSUserByUserId(mobileUser.ENSUserId.Value);
        ////    //var claims = new ENSUserClaims()
        ////    //{
        ////    //    MobileUserId = mobileUser.Id,
        ////    //    AccountId = mobileUser.AccountId,
        ////    //    UserId = contactDetail.ENSUserId.Value,
        ////    //    MemberId = contactDetail.MemberId.ToString(),
        ////    //    RecipientId = mobileUser.RecipientId
        ////    //};
        ////    //return GetLoginToken(claims);
        ////}


    }
}
