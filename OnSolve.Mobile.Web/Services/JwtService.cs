using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using SessionManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services
{
    public class JwtService : IJwtService
    {
        readonly Func<OnSolveMobileContext> _dbContextProvider;
        readonly IPasswordHasherService _passwordHasherService;
        readonly SessionManagerSoap _sessionManagerService;
        readonly IUserService _userService;
        readonly IContactsService _contactsService;
        readonly IFCMTokenService _fcmTokenService;
        readonly IConfiguration _configuration;
        const string TokenIssuer = "JwtToken:Issuer";
        const string TokenSecretConfKey = "JwtToken:Secret";
        const string TokenValidityConfKey = "JwtToken:TokenValidity";


        public JwtService(Func<OnSolveMobileContext> dbContextProvider,
             IPasswordHasherService passwordHasherService,
             SessionManagerSoap sessionManagerService,
             IUserService userService,
             IFCMTokenService fCMTokenService,
             IConfiguration configuration,
             IContactsService contactsService)

        {
            _dbContextProvider = dbContextProvider;
            _passwordHasherService = passwordHasherService;
            _sessionManagerService = sessionManagerService;
            _userService = userService;
            _fcmTokenService = fCMTokenService;
            _configuration = configuration;
            _contactsService = contactsService;
        }

        public async Task<Errorable<JwtResponse>> Login(MobileUser user, JwtRequest jwtRequest)
        {
            var jwtToken = String.Empty;

            if (ValidateMobileUser(jwtRequest.Password, user))
            {
                jwtToken = await GetJwtTokenForUser(user);
            }

            if (!string.IsNullOrEmpty(jwtToken))
            {
                if (!string.IsNullOrEmpty(jwtRequest.FCMToken))
                {
                    await _fcmTokenService.PostFCMTokenInfo(user.Id, jwtRequest.FCMToken);
                }
                var response = await CreateJwtResponse(jwtToken, user.AccountId);
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
            claims.Add(new Claim("MobileUserId", userClaims.MobileUserId.ToString()));
            claims.Add(new Claim("AccountId", userClaims.AccountId.ToString()));           
            claims.Add(new Claim("MemberId", userClaims.MemberId));
            claims.Add(new Claim("RecipientId", userClaims.RecipientId.ToString()));
            switch (userClaims)
            {
                case ENSUserClaims ensClaims:
                    claims.Add(new Claim("UserId", ensClaims.UserId.ToString()));
                    break;
            }                      
            return claims;
        }

        public async Task<bool> ValidateEnsUser(string username, string password)
        {
            var userDetail = await _userService.GetMobileUserContactDetail(username);
            if (userDetail != null && userDetail.Locked)
            {
                return false;
            }
            LoginRequest loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password,
                ServiceEntryPoint = ServiceEntryPointType.AlertingWebService
            };

            return await _sessionManagerService.CheckAuthenticationAsync(loginRequest);
        }

        public bool ValidateMobileUser(string password, MobileUser mobileUser)
        {
            var isValid = false;
            if (!String.IsNullOrEmpty(password))
            {
                var passwordHash = _passwordHasherService.GetPasswordHash(password, mobileUser.Salt);
                isValid = mobileUser.Password == passwordHash;
            }
            return isValid;
        }

        public async Task<JwtResponse> CreateJwtResponse(string jwtToken, int accountId)
        {
            var jwtResponse = new JwtResponse();
            jwtResponse.JwtToken = jwtToken;
            var accountDetail = await _userService.GetAccountDetails(accountId);
            jwtResponse.AccountName = accountDetail.AccountName;
            jwtResponse.CompanyName = accountDetail.CompanyName;
            return jwtResponse;
        }

        public async Task<string> GetJwtTokenForUser(MobileUser mobileUser)
        {
            if (mobileUser.ENSUserId.HasValue)
            {
                return await GetJwtTokenForENSUser(mobileUser);
            }
            return await GetJwtTokenForContactPointUser(mobileUser);
        }

        private async Task<string> GetJwtTokenForContactPointUser(MobileUser mobileUser)
        {
            var contactDetail = await _contactsService.GetContactPointDetail(mobileUser.RecipientId);
            var claims = new UserClaims()
            {
                MobileUserId = mobileUser.Id,
                AccountId = mobileUser.AccountId,               
                MemberId = (new Guid()).ToString(),
                RecipientId = mobileUser.RecipientId
            };
            return GetLoginToken(claims);
        }

        private async Task<string> GetJwtTokenForENSUser(MobileUser mobileUser)
        {
            var contactDetail = await _contactsService.GetENSUserByUserId(mobileUser.ENSUserId.Value);
            var claims = new ENSUserClaims()
            {
                MobileUserId = mobileUser.Id,
                AccountId = mobileUser.AccountId,
                UserId = contactDetail.ENSUserId.Value,
                MemberId = contactDetail.MemberId.ToString(),
                RecipientId = mobileUser.RecipientId
            };
            return GetLoginToken(claims);
        }


    }
}
