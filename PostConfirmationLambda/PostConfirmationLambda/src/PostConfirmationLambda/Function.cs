using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace PostConfirmationLambda;

public class Function
{
    private const string TableNameEnvVar = "DynamoDb__TableName";

    public JsonElement FunctionHandler(JsonElement postConfirmationEvent, ILambdaContext context)
    {
        context.Logger.LogInformation(
            $"User signup event received {JsonSerializer.Serialize(postConfirmationEvent)}");

        var postUserConfirmationEvent = postConfirmationEvent.Deserialize<PostUserConfirmationEvent>()!;

        var dynamoDbClient = new AmazonDynamoDBClient();

        var userDto = postUserConfirmationEvent.ToUserDto();
        var serializedUserDto = JsonSerializer.Serialize(userDto);
        var userAttributes = Document.FromJson(serializedUserDto).ToAttributeMap();
        var createItemRequest = new PutItemRequest
        {
            TableName = Environment.GetEnvironmentVariable(TableNameEnvVar), Item = userAttributes,
            // ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)",
        };

        var response = dynamoDbClient.PutItemAsync(createItemRequest).Result;

        context.Logger.LogInformation($"Response from dynamoDb {response.HttpStatusCode}");

        return postConfirmationEvent;
    }
}