using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_003.Enum;

namespace Task_003
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Dynamic365net.Program");
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                var connectionInstance = new DynamicCRMConnection();
                var _Service = connectionInstance.CreateCRMConnection();
                var _Context = connectionInstance.CreateCRMContext(_Service);

                IncidentRepository _incidentRepository = new IncidentRepository(_Service);
                UserRepository _userRepository = new UserRepository(_Service);

                var incident = _incidentRepository.GetRandomActiveIncident();
                var user = _userRepository.GetRandomActiveUser();
                _incidentRepository.AssignedIncidentToUser(user, incident);
                _incidentRepository.CancelIncident(IncidentStatus.Canceled, IncidentState.Canceled, incident);
                _incidentRepository.ResolveIncident(IncidentStatus.ProblemSolved, incident);
                
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
            
            
         
        }

    }
}
