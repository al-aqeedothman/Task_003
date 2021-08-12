using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_003.Models;

namespace Task_003
{
   public class UserRepository
    {
        private static readonly ILog log = LogManager.GetLogger("Task_003.UserRepository");
        public Random random = new Random();
        private readonly IOrganizationService _service;
        public UserRepository(IOrganizationService service)
        {
            _service = service;
        }

        public UserModel GetRandomActiveUser()
        {
            try
            {
                QueryExpression query = new QueryExpression("systemuser");
                query.ColumnSet.AddColumns("fullname");              
                var user = _service.RetrieveMultiple(query);
                if (user.Entities != null)
                {
                    Console.WriteLine("Get All User");
                    foreach (Entity item in user.Entities)
                    {
                        if (item.Attributes.Count >= 2)
                        {
                            var fullName = item.Attributes["fullname"] == null ? "" : item.Attributes["fullname"];
                            Console.WriteLine($" User: {fullName }{"\n"}");
                            log.Info($" User: {fullName }{"\n"}");
                        }
                    }

                    var index = random.Next(0, user.Entities.Count());
                    var userEntity = user.Entities.ElementAt(index);
                    return new UserModel {
                        Id = (Guid)userEntity.Attributes["systemuserid"],
                        FullName = userEntity.Attributes["fullname"].ToString()
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
    }
}
