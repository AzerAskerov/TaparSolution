using TaparSolution.Models.DBTable;

namespace TaparSolution.Operations
{
    public class GetRequestPriceOperation : BaseOperation<GetRequestPriceModel>
    {

        public int Price { get; set; }
        public override void DoExecute()
        {
            Price = (10 - Parameter._partner.rate) + 1;
        }
    }

    public class GetRequestPriceModel : BaseOperationModel
    {
     public  PartnerTable _partner { get; set; }
    }
}
