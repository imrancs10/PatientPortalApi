using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Infrastructure
{
    public interface IMessageSystem
    {
        void Send(Message msg);
    }
}