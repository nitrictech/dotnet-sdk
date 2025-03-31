using Nitric.Sdk.Service;
using Application = Nitric.Sdk.Nitric;

private WebsocketContext ValidateRequest(WebsocketContext ctx, Func<WebsocketContext, WebsocketContext> next)
{
    // Validate Request
    return next(ctx);
}

private WebsocketContext HandleRequest(WebsocketContext ctx, Func<WebsocketContext, WebsocketContext> next)
{
    // Handle Request
    return next(ctx);
}

var websocket = Application.Websocket("public");

websocket.On(WebsocketEventType.Message, ValidateRequest, HandleRequest);

Application.Run();