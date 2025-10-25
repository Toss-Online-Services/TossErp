using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Vendors;

namespace Toss.Application.Vendors.Commands.CreateVendorNote;

public record CreateVendorNoteCommand : IRequest<int>
{
    public int VendorId { get; init; }
    public string Note { get; init; } = string.Empty;
}

public class CreateVendorNoteCommandHandler : IRequestHandler<CreateVendorNoteCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateVendorNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateVendorNoteCommand request, CancellationToken cancellationToken)
    {
        // Validate vendor exists
        var vendor = await _context.Vendors.FindAsync(new object[] { request.VendorId }, cancellationToken);
        if (vendor == null)
            throw new NotFoundException(nameof(Vendor), request.VendorId.ToString());

        var vendorNote = new VendorNote
        {
            VendorId = request.VendorId,
            Note = request.Note
        };

        _context.VendorNotes.Add(vendorNote);
        await _context.SaveChangesAsync(cancellationToken);

        return vendorNote.Id;
    }
}

