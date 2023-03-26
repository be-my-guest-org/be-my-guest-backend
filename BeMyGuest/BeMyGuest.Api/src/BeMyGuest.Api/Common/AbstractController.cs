using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BeMyGuest.Api.Common;

public abstract class AbstractController : ControllerBase
{
    protected static StatusCodeResult InternalServerError()
    {
        return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
    }
}