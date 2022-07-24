using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;
using Amazon;
using TaparSolution.Models.DBTable;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using TaparSolution.Helpers;

namespace TaparSolution
{
    public class DynamoDbClient 
    {
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        public readonly DynamoDBContext _context;

        private static DynamoDbClient dynamodbinstance { get; set; }

        public static DynamoDbClient GetInstance()
        {
            return dynamodbinstance;
        }

        public DynamoDbClient(IOptions<MyConfig> config)
        {
            
            //localstack ignores secrets
            _amazonDynamoDBClient = new AmazonDynamoDBClient(config.Value.awsacessId , config.Value.awsSecretAccessKey,
              new AmazonDynamoDBConfig
              {
                  ServiceURL = "https://zojql2doe5.execute-api.us-east-2.amazonaws.com/Prod",
                  RegionEndpoint = RegionEndpoint.GetBySystemName(config.Value.AwsRegion)

              });

            _context = new DynamoDBContext(_amazonDynamoDBClient, new DynamoDBContextConfig
            {
                TableNamePrefix = ""
            });
            dynamodbinstance = this;
        }

      
        public async Task SaveOrUpdateMessage(ComposedMessageTable message)
        {
            await _context.SaveAsync(message);
        }

        public async Task SaveOrUpdateRequest(ClientRequestTable req)
        {
            await _context.SaveAsync(req);
        }

        public async Task SaveOrUpdatePartner(PartnerTable partner)
        {
            await _context.SaveAsync(partner);
        }

        public async Task SaveOrUpdateReqRespCompotition(ReqResCompositionTable compotition)
        {
            await _context.SaveAsync(compotition);
        }

        public async Task SaveOrUpdateQueue(QueueTable _queue)
        {
           await _context.SaveAsync(_queue);
        }

        public async Task<ClientRequestTable> GetRequestByOid(long req_oid)
        {
           return await _context.LoadAsync<ClientRequestTable>(req_oid);
        }
        public async Task<ReqResCompositionTable> GetReqRespCompotitionByOid(long comp_oid)
        {
            return await _context.LoadAsync<ReqResCompositionTable>(comp_oid);
        }


        public async Task<List<ReqResCompositionTable>> GetReqRespComotitionByPartnerAndRequest(long reqid, long partnerid)
        {
            var search = _context.ScanAsync<ReqResCompositionTable>
   (
     new[] {
        new ScanCondition
          (
            nameof(ReqResCompositionTable.requestid),
            ScanOperator.Equal,
            reqid
          ),
         new ScanCondition
          (
            nameof(ReqResCompositionTable.partnerid),
            ScanOperator.Equal,
            partnerid
          )
     }
   );

            return await search.GetRemainingAsync();

        }
        public async Task<List<Brandtable>> GetBrandList( string searchString)
        {
            var search = _context.ScanAsync<Brandtable>
   (
     new[] {
        new ScanCondition
          (
            nameof(Brandtable.Brand),
            ScanOperator.BeginsWith,
            searchString
          )
     }
   );

            return await search.GetRemainingAsync();

        }

        public async Task<List<ComposedMessageTable>> GetLastMessage(string chatid)
        {
            var search = _context.ScanAsync<ComposedMessageTable>
   (
     new[] {
        new ScanCondition
          (
            nameof(ComposedMessageTable.chat_id),
            ScanOperator.Equal,
            chatid
          )
     }
   );

            return await search.GetRemainingAsync();


        }

        public async Task<List<ComposedMessageTable>> GetAllPartnerMessageByCurrentRequest(long reqid)
        {
            var search = _context.ScanAsync<ComposedMessageTable>
   (
     new[] {
        new ScanCondition
          (
            nameof(ComposedMessageTable.request_oid),
            ScanOperator.Equal,
            reqid
          ),
        new ScanCondition
        (
            nameof(ComposedMessageTable.origin),
            ScanOperator.Equal,
            "Partner"
            )
     }
   );

            return await search.GetRemainingAsync();


        }

        public async Task<List<PartnerTable>> GetPartnerByUserId(long userid)
        {
            var search = _context.ScanAsync<PartnerTable>
   (
     new[] {
        new ScanCondition
          (
            nameof(PartnerTable.partnerid),
            ScanOperator.Equal,
            userid
          )
     }
   );

            return await search.GetRemainingAsync();


        }

        public async Task<List<PartnerTable>> GetPartnerByBrandAndRegionSubscription(string brand, List<string> regions)
        {

            var objectValues = regions.Select(x => (object)x.ToUpper()).ToArray();
            var search = _context.ScanAsync<PartnerTable>
   (
     new[] {
        new ScanCondition
          (
            nameof(PartnerTable.subscribedBrands),
            ScanOperator.Contains,
            brand.ToUpper()
          ),
        new ScanCondition

        (nameof(PartnerTable.region),
        ScanOperator.In,
        objectValues
            ),
        new ScanCondition
        (nameof(PartnerTable.balance),
        ScanOperator.GreaterThan,
        0)
     }
   );

            return await search.GetRemainingAsync();


        }

        public async Task<List<QueueTable>> GetQueueById(long id)
        {


            var search = _context.ScanAsync<QueueTable>
   (
     new[] {
        new ScanCondition
          (
            nameof(QueueTable.queueid),
            ScanOperator.Equal,
            id
          )

     }
   );

            return await search.GetRemainingAsync();


        }


        public async Task<List<QueueTable>> GetUnproccededQueue()
        {

            
            var search = _context.ScanAsync<QueueTable>
   (
     new[] {
        new ScanCondition
          (
            nameof(QueueTable.status),
            ScanOperator.Equal,
            queuestatus.created
          )
          ,
        new ScanCondition

            (
            nameof(QueueTable.proccess_after),
            ScanOperator.LessThan,
            DateTImeHelper.GetCurrentDate()
            )
     }
   );

            return await search.GetRemainingAsync();


        }

    
    }
}
