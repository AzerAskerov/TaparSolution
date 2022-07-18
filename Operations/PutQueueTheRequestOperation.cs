using TaparSolution.Models.DBTable;
using Microsoft.Extensions.Options;

namespace TaparSolution.Operations
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
