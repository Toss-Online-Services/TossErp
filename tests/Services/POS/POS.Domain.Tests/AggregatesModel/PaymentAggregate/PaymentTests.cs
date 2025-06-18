using FluentAssertions;
using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.PaymentAggregate;

public class PaymentTests
{
    private readonly Guid _saleId;
    private readonly decimal _amount;
    private readonly PaymentType _method;
    private readonly string _reference;
    private readonly string _cardLast4;
    private readonly string _cardType;

    public PaymentTests()
    {
        _saleId = Guid.NewGuid();
        _amount = 100.00m;
        _method = PaymentType.CreditCard;
        _reference = "REF123";
        _cardLast4 = "1234";
        _cardType = "Visa";
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesPaymentSuccessfully()
    {
        // Act
        var payment = new Payment(_saleId, _amount, _method, _reference, _cardLast4, _cardType);

        // Assert
        payment.SaleId.Should().Be(_saleId);
        payment.Amount.Should().Be(_amount);
        payment.Method.Should().Be(_method);
        payment.Reference.Should().Be(_reference);
        payment.CardLast4.Should().Be(_cardLast4);
        payment.CardType.Should().Be(_cardType);
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithEmptySaleId_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Payment(Guid.Empty, _amount, _method);
        action.Should().Throw<DomainException>().WithMessage("Sale ID cannot be empty");
    }

    [Fact]
    public void Constructor_WithZeroAmount_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Payment(_saleId, 0, _method);
        action.Should().Throw<DomainException>().WithMessage("Amount must be greater than zero");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Payment(_saleId, -100, _method);
        action.Should().Throw<DomainException>().WithMessage("Amount must be greater than zero");
    }

    [Fact]
    public void Process_WhenPending_ChangesStatusToProcessing()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);

        // Act
        payment.Process();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Processing);
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Process_WhenNotPending_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();

        // Act & Assert
        var action = () => payment.Process();
        action.Should().Throw<DomainException>().WithMessage("Can only process pending payments");
    }

    [Fact]
    public void Complete_WhenProcessing_ChangesStatusToCompleted()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        var transactionId = "TRX123";
        var authorizationCode = "AUTH123";

        // Act
        payment.Complete(transactionId, authorizationCode);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.TransactionId.Should().Be(transactionId);
        payment.AuthorizationCode.Should().Be(authorizationCode);
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Complete_WhenNotProcessing_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);

        // Act & Assert
        var action = () => payment.Complete("TRX123", "AUTH123");
        action.Should().Throw<DomainException>().WithMessage("Can only complete processing payments");
    }

    [Fact]
    public void Fail_WhenProcessing_ChangesStatusToFailed()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        var errorMessage = "Payment failed";

        // Act
        payment.Fail(errorMessage);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.ErrorMessage.Should().Be(errorMessage);
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Fail_WhenNotProcessing_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);

        // Act & Assert
        var action = () => payment.Fail("Payment failed");
        action.Should().Throw<DomainException>().WithMessage("Can only fail processing payments");
    }

    [Fact]
    public void Retry_WhenFailed_ChangesStatusToPending()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        payment.Fail("Payment failed");

        // Act
        payment.Retry();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.RetryCount.Should().Be(1);
        payment.LastRetryAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Retry_WhenNotFailed_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);

        // Act & Assert
        var action = () => payment.Retry();
        action.Should().Throw<DomainException>().WithMessage("Can only retry failed payments");
    }

    [Fact]
    public void Retry_WhenMaxRetriesReached_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        payment.Fail("Payment failed");
        payment.Retry();
        payment.Process();
        payment.Fail("Payment failed");
        payment.Retry();
        payment.Process();
        payment.Fail("Payment failed");
        payment.Retry();

        // Act & Assert
        var action = () => payment.Retry();
        action.Should().Throw<DomainException>().WithMessage("Maximum retry attempts reached");
    }

    [Fact]
    public void ProcessPartialRefund_WhenCompleted_UpdatesPartialRefundAmount()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        payment.Complete("TRX123", "AUTH123");
        var refundAmount = 50.00m;
        var reason = "Customer returned item";

        // Act
        payment.ProcessPartialRefund(refundAmount, reason);

        // Assert
        payment.PartialRefundAmount.Should().Be(refundAmount);
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ProcessPartialRefund_WhenNotCompleted_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);

        // Act & Assert
        var action = () => payment.ProcessPartialRefund(50.00m, "Customer returned item");
        action.Should().Throw<DomainException>().WithMessage("Can only refund completed payments");
    }

    [Fact]
    public void ProcessPartialRefund_WithZeroAmount_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        payment.Complete("TRX123", "AUTH123");

        // Act & Assert
        var action = () => payment.ProcessPartialRefund(0, "Customer returned item");
        action.Should().Throw<DomainException>().WithMessage("Refund amount must be greater than zero");
    }

    [Fact]
    public void ProcessPartialRefund_WithNegativeAmount_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        payment.Complete("TRX123", "AUTH123");

        // Act & Assert
        var action = () => payment.ProcessPartialRefund(-50.00m, "Customer returned item");
        action.Should().Throw<DomainException>().WithMessage("Refund amount must be greater than zero");
    }

    [Fact]
    public void ProcessPartialRefund_WithAmountExceedingRemaining_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        payment.Complete("TRX123", "AUTH123");

        // Act & Assert
        var action = () => payment.ProcessPartialRefund(150.00m, "Customer returned item");
        action.Should().Throw<DomainException>().WithMessage("Refund amount cannot exceed remaining amount");
    }

    [Fact]
    public void AddPaymentSplit_WhenPending_AddsSplitSuccessfully()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        var split = new PaymentSplit(50.00m, PaymentType.Cash);

        // Act
        payment.AddPaymentSplit(split);

        // Assert
        payment.PaymentSplits.Should().ContainSingle();
        payment.PaymentSplits[0].Amount.Should().Be(50.00m);
        payment.PaymentSplits[0].Method.Should().Be(PaymentType.Cash);
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddPaymentSplit_WhenNotPending_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        var split = new PaymentSplit(50.00m, PaymentType.Cash);

        // Act & Assert
        var action = () => payment.AddPaymentSplit(split);
        action.Should().Throw<DomainException>().WithMessage("Can only add splits to pending payments");
    }

    [Fact]
    public void AddPaymentSplit_WithTotalExceedingAmount_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        var split1 = new PaymentSplit(60.00m, PaymentType.Cash);
        var split2 = new PaymentSplit(50.00m, PaymentType.CreditCard);

        // Act & Assert
        payment.AddPaymentSplit(split1);
        var action = () => payment.AddPaymentSplit(split2);
        action.Should().Throw<DomainException>().WithMessage("Total split amount cannot exceed payment amount");
    }

    [Fact]
    public void Reconcile_WhenCompleted_UpdatesReconciliationDetails()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        payment.Process();
        payment.Complete("TRX123", "AUTH123");
        var reference = "REC123";

        // Act
        payment.Reconcile(reference);

        // Assert
        payment.IsReconciled.Should().BeTrue();
        payment.ReconciledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        payment.ReconciliationReference.Should().Be(reference);
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Reconcile_WhenNotCompleted_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);

        // Act & Assert
        var action = () => payment.Reconcile("REC123");
        action.Should().Throw<DomainException>().WithMessage("Can only reconcile completed payments");
    }

    [Fact]
    public void UpdateGatewayResponse_UpdatesResponseSuccessfully()
    {
        // Arrange
        var payment = new Payment(_saleId, _amount, _method);
        var response = new PaymentGatewayResponse("Success", "TRX123", "AUTH123");

        // Act
        payment.UpdateGatewayResponse(response);

        // Assert
        payment.GatewayResponse.Should().Be(response);
        payment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
} 
