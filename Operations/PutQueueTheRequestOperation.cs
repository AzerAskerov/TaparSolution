using AWSServerless2.Models.DBTable;
using Microsoft.Extensions.Options;

namespace AWSServerless2.Operations
{
    public class PutQueueTheRequestOperation : BaseOperation<PutQueueTheRequestOperationModel>
    {
       
        public override async void DoExecute()
        {
           await Parameter._db.SaveOrUpdateQueue(Parameter.queue);
        }

      
        
    }




    public class PutQueueTheRequestOperationModel:BaseOperationModel
    {
        public QueueTable queue { get; set; }
    }

}
