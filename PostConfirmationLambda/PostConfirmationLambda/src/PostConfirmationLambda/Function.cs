using System.Text.Json;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PostConfirmationLambda;

public class Function
{
    public void FunctionHandler(JsonElement postUserConfirmationEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"User signup event received {JsonSerializer.Serialize(postUserConfirmationEvent)}");
    }
}