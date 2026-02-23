using Main.Common.Repository;
using MediatR;

namespace Main.Common.Base;
internal class BaseRequestHandlerParam
{
    readonly IMediator _mediator;
    readonly IUnitOfWork _unitOfWork;


    public BaseRequestHandlerParam(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public IMediator Mediator => _mediator;
    public IUnitOfWork UnitOfWork => _unitOfWork;

}
