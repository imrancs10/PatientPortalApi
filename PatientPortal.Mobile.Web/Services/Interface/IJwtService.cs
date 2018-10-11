using System.Threading.Tasks;
using PatientPortal.Mobile.Data.Entites;
using PatientPortal.Mobile.Web.Models;

namespace PatientPortal.Mobile.Web.Services.Interface
{
    public interface IJwtService
    {
        Task<Errorable<JwtResponse>> Login(PatientInfo user, JwtRequest jwtRequest);
        Task<string> GetJwtTokenForUser(PatientInfo user);
        Task<JwtResponse> CreateJwtResponse(string jwtToken);
    }
}