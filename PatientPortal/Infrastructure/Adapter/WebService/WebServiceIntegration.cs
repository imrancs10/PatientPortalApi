using PatientPortal.HISWebReference;
using PatientPortal.OPDWebReference;
using PatientPortal.Infrastructure.Utility;
using PatientPortal.Models;
using PatientPortal.PateintInfoService;
using System.IO;
using System.Text;

namespace PatientPortal.Infrastructure.Adapter.WebService
{
    public class WebServiceIntegration
    {
        public HISPatientInfoModel GetPatientInfoBYCRNumber(string crNumber)
        {
            GetPatientDetails service = new GetPatientDetails();
            var result = service.getPatientDetails(crNumber);
            if (result.ToLower().Contains("no record"))
                return null;
            Serializer serilizer = new Serializer();
            result = result.Replace("<NewDataSet>", "").Replace("</NewDataSet>", "");
            return serilizer.Deserialize<HISPatientInfoModel>(result, "Table1");
        }

        public string GetPatientInfoinsert(HISPatientInfoInsertModel insertModel)
        {
            //Serializer serilizer = new Serializer();
            string xml = getXMLDataForRegistration(insertModel);//serilizer.SerializeToXML(insertModel);
            GetPatient_Info_insert pateintInfoService = new GetPatient_Info_insert();
            return pateintInfoService.GetPatientInfoinsert(xml);
        }

        private string getXMLDataForRegistration(HISPatientInfoInsertModel insertModel)
        {
            string str = "<?xml version=\"1.0\" standalone=\"yes\"?>" +
                        "<NewDataSet>" +
                          "<PatientInfo>" +
                               "<PatientId>" + insertModel.PatientId + "</PatientId>" +
                                "<RegistrationNumber>" + insertModel.RegistrationNumber + "</RegistrationNumber>" +
                                "<MobileNumber>" + insertModel.MobileNumber + "</MobileNumber>" +
                                "<Password>" + insertModel.Password + "</Password>" +
                                "<Email>" + insertModel.Email + "</Email>" +
                                "<Title>" + insertModel.Title + "</Title>" +
                                "<FirstName>" + insertModel.FirstName + "</FirstName>" +
                                "<MiddleName>" + insertModel.MiddleName + "</MiddleName>" +
                                "<LastName>" + insertModel.LastName + "</LastName>" +
                                "<DOB>" + insertModel.DOB + "</DOB>" +
                                "<Gender>" + insertModel.Gender + "</Gender>" +
                                "<MaritalStatus>" + insertModel.MaritalStatus + "</MaritalStatus>" +
                                "<Address>" + insertModel.Address + "</Address>" +
                                "<City>" + insertModel.City + "</City>    " +
                                "<PinCode>" + insertModel.PinCode + "</PinCode>" +
                                "<Religion>" + insertModel.Religion + "</Religion>" +
                                "<DepartmentId>" + insertModel.DepartmentId + "</DepartmentId>  " +
                                "<State>" + insertModel.State + "</State>	" +
                                "<FatherOrHusbandName>" + insertModel.FatherOrHusbandName + "</FatherOrHusbandName>	" +
                                "<Amount>" + insertModel.Amount + "</Amount>" +
                                "<PatientTransactionId>" + insertModel.PatientTransactionId + "</PatientTransactionId>" +
                                "<CreateDate>" + insertModel.CreateDate + "</CreateDate>	" +
                                "<ValidUpto>" + insertModel.ValidUpto + "</ValidUpto>" +
                           "</PatientInfo >" +
                        "</NewDataSet>";

            return str;
        }

        public PDModel GetPatientOPDDetail(string crNumber)
        {
            GetPatOpdDetails service = new GetPatOpdDetails();
            var result = service.GetPatientOPDDetails(crNumber);
            if (result.ToLower().Contains("no record"))
                return null;
            Serializer serilizer = new Serializer();
            result = result.Replace("<NewDataSet>", "").Replace("</NewDataSet>", "");
            return serilizer.Deserialize<PDModel>(result, "Table1");
        }
    }
}