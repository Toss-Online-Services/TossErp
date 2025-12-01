using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Collaborations;
using Toss.Application.Common.Interfaces.Security;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Notifications.Commands.CreateComment;
using Toss.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Toss.Application.Collaborations.Commands.SubmitFeedback;

public record SubmitFeedbackCommand : IRequest<int>
{
    public int SalesId { get; init; }
    public int Rating { get; init; } // 1-5
    public string Note { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string? CaptchaToken { get; init; }
}

public class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ICollabLinkService _collabLinkService;
    private readonly ICaptchaVerifier _captchaVerifier;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISender _mediator;

    public SubmitFeedbackCommandHandler(
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

    public async Task<int> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        // Validate token
        var isValid = await _collabLinkService.ValidateLinkAsync(
            request.Token,
            CollabLinkPurpose.Feedback,
            "Sale",
            request.SalesId,
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

        // Validate rating
        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new ValidationException("Rating must be between 1 and 5.");
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

        // Create feedback comment with rating embedded in body
        var feedbackBody = $"{request.Note}\n\n[Rating: {request.Rating}/5]";

        var commentId = await _mediator.Send(new CreateCommentCommand
        {
            LinkedType = "Sale",
            LinkedId = request.SalesId,
            Body = feedbackBody,
            Type = CommentType.Feedback,
            CreatedBy = null // Anonymous feedback
        }, cancellationToken);

        return commentId;
    }
}

