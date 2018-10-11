using System.Threading.Tasks;
using PatientPortal.Mobile.Data.Entites;
using PatientPortal.Mobile.Web.Models;

namespace PatientPortal.Mobile.Web.Services.Interface
{
    public interface IJwtService
    {
        Task<Errorable<JwtResponse>> Login(
            //MobileUser user, 
            JwtRequest jwtRequest);
        //Task<string> GetJwtTokenForUser(MobileUser user);
        //Task<bool> ValidateEnsUser(string username, string password);
        //Task<JwtResponse> CreateJwtResponse(string jwtToken, int accountId);
    }
}