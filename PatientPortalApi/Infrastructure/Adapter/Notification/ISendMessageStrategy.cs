using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Infrastructure
{
    public interface ISendMessageStrategy
    {
        void SendMessages();
    }
}