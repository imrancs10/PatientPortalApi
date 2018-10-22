using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalAPI.Infrastructure
{
    public interface ISendMessageStrategy
    {
        void SendMessages();
    }
}