using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Assignment7
{
    public class Ratings
    {
        public string itemId;
        public string type;
    }
    public class Function
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        public async Task<List<Ratings>> FunctionHandler(DynamoDBEvent input, ILambdaContext context)
        {
            Table table = Table.LoadTable(client, "RatingsByType");
            List<Ratings> ratings = new List<Ratings>();
            List<DynamoDBEvent.DynamodbStreamRecord> records = (List<DynamoDBEvent.DynamodbStreamRecord>)input.Records;
            //if I have one record
            if(records.Count > 0)
            {
                //grab the first record
                DynamoDBEvent.DynamodbStreamRecord record = records[0];
                //Only want to handle inserts
                if(record.EventName.Equals("INSERT"))
                {
                    //grab the document
                    Document myDoc = Document.FromAttributeMap(record.Dynamodb.NewImage);
                    //then turn it into a rating
                    Ratings myRating = JsonConvert.DeserializeObject<Ratings>(myDoc.ToJson());
                    //calculate ratings average
                   



                    //for(int i = 0; )

                    //do an update item request
                    var request = new UpdateItemRequest
                    {
                        TableName = "RatingsByType",
                        Key = new Dictionary<string, AttributeValue>
                    {
                        { "type",  new AttributeValue { S = myRating.type } }
                    },
                        AttributeUpdates = new Dictionary<string, AttributeValueUpdate>()
                    {
                        {
                                "count",
                                new AttributeValueUpdate {Action = "ADD" , Value = new AttributeValue { N = "1" } }
                        },
                    },
                    };
                    await client.UpdateItemAsync(request);

                }
            }

            return ratings;
        }
    }
}
