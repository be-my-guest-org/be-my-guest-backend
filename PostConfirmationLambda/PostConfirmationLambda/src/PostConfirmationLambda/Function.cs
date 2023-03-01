using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PostConfirmationLambda;

public class Function
{
    private const string TableNameEnvVar = "TABLE_NAME";

    public void FunctionHandler(PostUserConfirmationEvent postUserConfirmationEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"User signup event received {JsonSerializer.Serialize(postUserConfirmationEvent)}");

        var dynamoDbClient = new AmazonDynamoDBClient();

        var userDto = postUserConfirmationEvent.ToUserDto();
        var serializedUserDto = JsonSerializer.Serialize(userDto);
        var userAttributes = Document.FromJson(serializedUserDto).ToAttributeMap();
        var createItemRequest = new PutItemRequest
        {
            TableName = Environment.GetEnvironmentVariable(TableNameEnvVar),
            Item = userAttributes,
            ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)",
        };

        dynamoDbClient.PutItemAsync(createItemRequest);
    }
}