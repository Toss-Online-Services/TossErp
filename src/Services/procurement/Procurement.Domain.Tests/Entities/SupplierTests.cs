using Xunit;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.Domain.Tests.Entities;

public class SupplierTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateSupplier()
    {
        // Arrange
        var code = "SUP001";
        var name = "Test Supplier";
        var tenantId = "test-tenant";

        // Act
        var supplier = Supplier.Create(code, name, tenantId);

        // Assert
        Assert.Equal(code, supplier.Code);
        Assert.Equal(name, supplier.Name);
        Assert.Equal(SupplierStatus.Active, supplier.Status);
        Assert.Equal(tenantId, supplier.TenantId);
    }

    [Fact]
    public void Create_WithEmptyCode_ShouldThrowException()
    {
        // Arrange
        var code = "";
        var name = "Test Supplier";
        var tenantId = "test-tenant";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Supplier.Create(code, name, tenantId));
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var code = "SUP001";
        var name = "";
        var tenantId = "test-tenant";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Supplier.Create(code, name, tenantId));
    }

    [Fact]
    public void UpdateContactInfo_WithValidData_ShouldUpdateContactInfo()
    {
        // Arrange
        var supplier = CreateSampleSupplier();
        var contactPerson = "John Doe";
        var email = "john@supplier.com";
        var phone = "+1234567890";

        // Act
        supplier.UpdateContactInfo(contactPerson, email, phone);

        // Assert
        Assert.Equal(contactPerson, supplier.ContactPerson);
        Assert.Equal(email, supplier.Email);
        Assert.Equal(phone, supplier.Phone);
    }

    [Fact]
    public void UpdateBusinessInfo_WithValidData_ShouldUpdateBusinessInfo()
    {
        // Arrange
        var supplier = CreateSampleSupplier();
        var address = "123 Business St";
        var city = "Business City";
        var state = "Business State";
        var postalCode = "12345";
        var country = "Business Country";
        var taxNumber = "TAX123456";

        // Act
        supplier.UpdateBusinessInfo(address, city, state, postalCode, country, taxNumber);

        // Assert
        Assert.Equal(address, supplier.Address);
        Assert.Equal(city, supplier.City);
        Assert.Equal(state, supplier.State);
        Assert.Equal(postalCode, supplier.PostalCode);
        Assert.Equal(country, supplier.Country);
        Assert.Equal(taxNumber, supplier.TaxNumber);
    }

    [Fact]
    public void UpdateFinancialInfo_WithValidData_ShouldUpdateFinancialInfo()
    {
        // Arrange
        var supplier = CreateSampleSupplier();
        var bankName = "Business Bank";
        var bankAccountNumber = "1234567890";
        var bankRoutingNumber = "987654321";
        var paymentTerms = "Net 30";

        // Act
        supplier.UpdateFinancialInfo(bankName, bankAccountNumber, bankRoutingNumber, paymentTerms);

        // Assert
        Assert.Equal(bankName, supplier.BankName);
        Assert.Equal(bankAccountNumber, supplier.BankAccountNumber);
        Assert.Equal(bankRoutingNumber, supplier.BankRoutingNumber);
        Assert.Equal(paymentTerms, supplier.PaymentTerms);
    }

    [Fact]
    public void Activate_ShouldChangeStatusToActive()
    {
        // Arrange
        var supplier = CreateSampleSupplier();
        supplier.Deactivate();

        // Act
        supplier.Activate();

        // Assert
        Assert.Equal(SupplierStatus.Active, supplier.Status);
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusToInactive()
    {
        // Arrange
        var supplier = CreateSampleSupplier();

        // Act
        supplier.Deactivate();

        // Assert
        Assert.Equal(SupplierStatus.Inactive, supplier.Status);
    }

    [Fact]
    public void Hold_ShouldChangeStatusToOnHold()
    {
        // Arrange
        var supplier = CreateSampleSupplier();

        // Act
        supplier.Hold();

        // Assert
        Assert.Equal(SupplierStatus.OnHold, supplier.Status);
    }

    [Fact]
    public void Blacklist_ShouldChangeStatusToBlacklisted()
    {
        // Arrange
        var supplier = CreateSampleSupplier();
        var reason = "Poor quality products";

        // Act
        supplier.Blacklist(reason);

        // Assert
        Assert.Equal(SupplierStatus.Blacklisted, supplier.Status);
        Assert.Contains(reason, supplier.Notes);
    }

    [Fact]
    public void AddNotes_ShouldAppendToExistingNotes()
    {
        // Arrange
        var supplier = CreateSampleSupplier();
        var initialNotes = "Initial notes";
        var additionalNotes = "Additional notes";
        supplier.AddNotes(initialNotes);

        // Act
        supplier.AddNotes(additionalNotes);

        // Assert
        Assert.Contains(initialNotes, supplier.Notes);
        Assert.Contains(additionalNotes, supplier.Notes);
    }

    private static Supplier CreateSampleSupplier()
    {
        var code = "SUP001";
        var name = "Test Supplier";
        var tenantId = "test-tenant";
        return Supplier.Create(code, name, tenantId);
    }
}
