﻿using DataLayer;
using PatientPortalAPI.BAL.Appointments;
using PatientPortalAPI.BAL.Masters;
using PatientPortalAPI.BAL.Patient;
using PatientPortalAPI.Global;
using PatientPortalAPI.Infrastructure.Adapter.WebService;
using PatientPortalAPI.Infrastructure.Authentication;
using PatientPortalAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatientPortalAPI.Infrastructure.Utility
{
    public abstract class BaseViewPage : WebViewPage
    {
        //public virtual new CustomPrincipal User
        //{
        //    get { return base.User as CustomPrincipal; }
        //}

        //public virtual HospitalDetail GetHospitalDetail()
        //{
        //    HospitalDetails _details = new HospitalDetails();
        //    return _details.GetHospitalDetail();
        //}
        //public virtual int GetAppointmentCount()
        //{
        //    AppointDetails _details = new AppointDetails();
        //    return _details.PatientAppointmentCount(User.Id);
        //}
    }

    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual new CustomPrincipal User
        {
            get { return base.User as CustomPrincipal; }
        }
        public virtual HospitalDetail GetHospitalDetail()
        {
            HospitalDetails _details = new HospitalDetails();
            return _details.GetHospitalDetail();
        }

        public virtual int GetAppointmentCount()
        {
            if (User != null)
            {
                AppointDetails _details = new AppointDetails();
                return _details.PatientAppointmentCount(User.Id);
            }
            return 0;
        }

        public virtual PatientInfo GetPatientInfo()
        {
            if (User != null)
            {
                PatientDetails _details = new PatientDetails();
                return _details.GetPatientDetailById(User.Id);
            }
            return null;
        }

        public virtual PDModel GetPatientOPDDetail()
        {
            if (User != null)
            {
                string crNumber = string.IsNullOrEmpty(WebSession.PatientCRNo) ? WebSession.PatientRegNo : WebSession.PatientCRNo;
                var opdDetail = (new WebServiceIntegration()).GetPatientOPDDetail(crNumber);
                return opdDetail;
            }
            return null;
        }
    }
}