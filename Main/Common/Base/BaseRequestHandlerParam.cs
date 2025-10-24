using MediatR;

namespace Main.Common.Base;
internal class BaseRequestHandlerParam
{
    readonly IMediator _mediator;

    public BaseRequestHandlerParam(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IMediator Mediator => _mediator;

}
