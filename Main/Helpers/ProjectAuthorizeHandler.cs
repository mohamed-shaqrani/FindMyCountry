using Main.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Api.Repository;
using System.Security.Claims;

namespace Main.Helpers;

public sealed class ProjectAdminRequirement : IAuthorizationRequirement { }
public sealed class ProjectAuthorizeHandler : AuthorizationHandler<ProjectAdminRequirement>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectAuthorizeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdminRequirement requirement)
    {
        var UserRepo = _unitOfWork.GetRepository<User>();
        var UserEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        var UserEmailValue = UserEmail?.Value;
        if (UserEmailValue == null)
        {
            return;
        }

        var user = await UserRepo.GetAll(u => u.Email == UserEmailValue).FirstOrDefaultAsync();
        if (user is null)
        {
            return;
        }

    }
}

