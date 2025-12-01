using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Collaborations;
using Toss.Application.Common.Interfaces.Security;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Notifications.Commands.CreateComment;
using Toss.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Toss.Application.Collaborations.Commands.SubmitOffer;

public record SubmitOfferCommand : IRequest<int>
{
    public int PurchaseRequestId { get; init; }
    public decimal? Price { get; init; }
    public string Note { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string? CaptchaToken { get; init; }
}

public class SubmitOfferCommandHandler : IRequestHandler<SubmitOfferCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ICollabLinkService _collabLinkService;
    private readonly ICaptchaVerifier _captchaVerifier;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISender _mediator;

    public SubmitOfferCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        ICollabLinkService collabLinkService,
        ICaptchaVerifier captchaVerifier,
        IHttpContextAccessor httpContextAccessor,
        ISender mediator)
    {
        _context = context;
        _businessContext = businessContext;
        _collabLinkService = collabLinkService;
        _captchaVerifier = captchaVerifier;
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
    }

    public async Task<int> Handle(SubmitOfferCommand request, CancellationToken cancellationToken)
    {
        // Validate token
        var isValid = await _collabLinkService.ValidateLinkAsync(
            request.Token,
            CollabLinkPurpose.Offer,
            "PurchaseRequest",
            request.PurchaseRequestId,
            cancellationToken);

        if (!isValid)
        {
            throw new ForbiddenAccessException("Invalid or expired collaboration link token.");
        }

        // Get link info to determine business context
        var linkInfo = await _collabLinkService.GetLinkInfoAsync(request.Token, cancellationToken);
        if (linkInfo == null)
        {
            throw new ForbiddenAccessException("Collaboration link not found.");
        }

        // Set business context for the comment creation
        if (!_businessContext.HasBusiness || _businessContext.CurrentBusinessId != linkInfo.BusinessId)
        {
            _businessContext.SetBusiness(linkInfo.BusinessId, string.Empty, string.Empty);
        }

        // Verify captcha if provided
        if (!string.IsNullOrWhiteSpace(request.CaptchaToken))
        {
            var remoteIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var captchaValid = await _captchaVerifier.VerifyAsync(request.CaptchaToken, remoteIp, cancellationToken);
            
            if (!captchaValid)
            {
                throw new ValidationException("Captcha verification failed.");
            }
        }

        // Create offer comment with price embedded if provided
        var offerBody = request.Note;
        if (request.Price.HasValue)
        {
            offerBody = $"{request.Note}\n\n[Price: {request.Price.Value:C}]";
        }

        var commentId = await _mediator.Send(new CreateCommentCommand
        {
            LinkedType = "PurchaseRequest",
            LinkedId = request.PurchaseRequestId,
            Body = offerBody,
            Type = CommentType.Offer,
            CreatedBy = null // Anonymous offer
        }, cancellationToken);

        return commentId;
    }
}

