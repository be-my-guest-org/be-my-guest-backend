using BeMyGuest.Api;
using BeMyGuest.Api.Middlewares;
using BeMyGuest.Application;
using BeMyGuest.Infrastructure;
using Moschen.AwsLambdaAuthenticationHandler.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddAuthentication(AwsJwtAuthorizerDefaults.AuthenticationScheme)
    .AddJwtAuthorizer(options =>
    {
        // In the case of local run, this option enables the extraction of claims from the token
        options.ExtractClaimsFromToken = true;

        // Validates the presence of the token
        options.RequireToken = true;
    });

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();