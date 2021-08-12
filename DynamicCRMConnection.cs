
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using log4net;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client;

namespace Task_003
{
    public class DynamicCRMConnection
    {
        private static readonly ILog log = LogManager.GetLogger("Task_003.DynamicCRMConnection");
        public  IOrganizationService CreateCRMConnection()
        {
            try
            {
                var connString = ConfigurationManager.ConnectionStrings["DynamicCRMconnectionstring"].ConnectionString;
                if (!String.IsNullOrEmpty(connString))
                {
                    CrmServiceClient conn = new CrmServiceClient(connString);
                    if (conn.IsReady)
                    {
                        log.Info("Your Connection to " + conn.OrganizationDetail.FriendlyName + "is ready");
                        IOrganizationService service = conn.OrganizationWebProxyClient == null ? (IOrganizationService)conn.OrganizationServiceProxy : (IOrganizationService)conn.OrganizationWebProxyClient;
                        return service;
                    }
                    else if (conn.OrganizationDetail is null)
                    {
                        log.Info("Your Connection to Dynamic 365 CRM  is faild");
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    log.Info("Check your ConnectionString");
                    return null;
                }
            }
            catch (Exception ex)
            {

                log.Error(ex.Message, ex);
                throw;
            }
      
        }
        public CrmOrganizationServiceContext CreateCRMContext(IOrganizationService service)
        {
            if (service!= null )
            {
                var context = new CrmOrganizationServiceContext(service);
                return context;
            }
            else
            {
                return null;
            }
        }
    }
}
