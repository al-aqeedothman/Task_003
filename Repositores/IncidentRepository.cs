using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Task_003.Models;
using Task_003.Enum;

namespace Task_003
{
   public class IncidentRepository
    {
        private static readonly ILog log = LogManager.GetLogger("Task_003.IncidentREpository");
        public Random random = new Random();
        private readonly IOrganizationService _service;
        public IncidentRepository(IOrganizationService service)
        {
            _service = service;
        }
        public IncidentModel GetRandomActiveIncident()
        {
            try
            {
                QueryExpression query = new QueryExpression("incident");
                query.ColumnSet.AddColumns("title");
                query.Criteria = new FilterExpression();
                query.Criteria.AddCondition("incident", "statecode", ConditionOperator.Equal, (int)IncidentState.Active);

                var incidents = _service.RetrieveMultiple(query);
                if (incidents.Entities !=null)
                {
                    Console.WriteLine(" All Active incident ");
                    foreach (Entity item in incidents.Entities)
                    {
                        if (item.Attributes.Count >= 2)
                        {
                            var incidentName = item.Attributes["title"] == null ? "" : item.Attributes["title"];
                            Console.WriteLine($" incident: {incidentName } {"\n"}");
                        }
                    }

                    var index = random.Next(0, incidents.Entities.Count());
                    var incidentEntity = incidents.Entities.ElementAt(index);
                    return new IncidentModel
                    {
                        Id = (Guid)incidentEntity.Attributes["incidentid"],
                        Title = incidentEntity.Attributes["title"].ToString()
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }
        public void AssignedIncidentToUser(UserModel user, IncidentModel incident)
        {
            try
            {
                if (user != null && incident !=null)
                {
                    AssignRequest assignRequest = new AssignRequest();
                    assignRequest.Assignee = new EntityReference("systemuser", user.Id);
                    assignRequest.Target = new EntityReference("incident", incident.Id);
                    var response = _service.Execute(assignRequest);
                    Console.WriteLine("Assign the  case: " + incident.Title + "to " + user.FullName + " succes!");
                }              
            }

            catch (Exception ex)

            {
                Console.WriteLine("Assign the  case: " + incident?.Title + "to " + user?.FullName + " failed!");
                log.Error(ex.Message , ex);
                return ;

            }
        }

        public void ResolveIncident(IncidentStatus status,  IncidentModel incident)
        {
            try
            {
                Entity IncidentResolution = new Entity("incidentresolution");
                IncidentResolution.Attributes["subject"] = "Subject Closed";
                IncidentResolution.Attributes["incidentid"] = new EntityReference("incident", incident.Id);
               
                CloseIncidentRequest closeRequest = new CloseIncidentRequest();
                closeRequest.IncidentResolution = IncidentResolution;
               
                closeRequest.Status = new OptionSetValue((int)status);                
                CloseIncidentResponse closeResponse = (CloseIncidentResponse)_service.Execute(closeRequest);
                log.Info($" Incident: {incident.Title } Resolved successfully");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }
        public void CancelIncident(IncidentStatus status , IncidentState state , IncidentModel incident)
        {
            try
            {

                SetStateRequest request = new SetStateRequest();
                request.EntityMoniker = new EntityReference("incident", incident.Id);
                request.State = new OptionSetValue((int)state);
                request.Status = new OptionSetValue((int)status);
                SetStateResponse objResponse = (SetStateResponse)_service.Execute(request);
                log.Info($" Incident: {incident.Title } cancelled successfully");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }
    }
}
