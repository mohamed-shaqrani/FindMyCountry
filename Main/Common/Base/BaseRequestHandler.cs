using Main.Common.Repository;
using MediatR;

namespace Main.Common.Base;

internal abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected IMediator _mediator;
    protected IUnitOfWork _unitOfWork;
    public BaseRequestHandler(BaseRequestHandlerParam requestHandlerParam)
    {
        _mediator = requestHandlerParam.Mediator;
        _unitOfWork = requestHandlerParam.UnitOfWork;
    }
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    public TResponse HandleNonAsync(TRequest request, CancellationToken cancellationToken)
    {
        return HandleNonAsync(request, cancellationToken);
    }

}
