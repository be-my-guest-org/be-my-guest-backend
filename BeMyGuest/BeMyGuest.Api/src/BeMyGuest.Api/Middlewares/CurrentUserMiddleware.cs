using BeMyGuest.Common.User;

namespace BeMyGuest.Api.Middlewares;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CurrentUserData currentUserData)
    {
        currentUserData.Username = context.User.Claims.First(claim => claim.Type == "username").Value;
        currentUserData.UserId = context.User.Claims.First(claim => claim.Type == "sub").Value;

        await _next(context);
    }
}