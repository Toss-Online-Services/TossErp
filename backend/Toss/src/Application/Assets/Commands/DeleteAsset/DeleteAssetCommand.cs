using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Assets.Commands.DeleteAsset;

public record DeleteAssetCommand(int Id) : IRequest;

public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public DeleteAssetCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == request.Id
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (asset == null)
        {
            throw new NotFoundException("Asset", request.Id.ToString());
        }

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

