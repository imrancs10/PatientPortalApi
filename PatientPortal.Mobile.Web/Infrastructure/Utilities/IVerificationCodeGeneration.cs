namespace PatientPortal.Mobile.Web.Infrastructure.Utilities
{
    public interface IVerificationCodeGeneration
    {
        string GenerateResetCode();

        string GenerateRandomString();
    }
}