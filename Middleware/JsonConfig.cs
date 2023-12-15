using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

public class PreserveReferencesAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<JsonSerializerOptions>();
        options.ReferenceHandler = ReferenceHandler.Preserve;

        base.OnResultExecuting(context);
    }
}