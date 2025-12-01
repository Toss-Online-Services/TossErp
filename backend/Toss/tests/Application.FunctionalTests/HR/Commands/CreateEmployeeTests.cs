using Toss.Application.Common.Exceptions;
using Toss.Application.HR.Commands.CreateEmployee;
using Toss.Domain.Entities.HR;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.HR.Commands;

using static Testing;

public class CreateEmployeeTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireName()
    {
        var command = new CreateEmployeeCommand
        {
            RateType = EmployeeRateType.Hourly,
            Rate = 50
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRequirePositiveRate()
    {
        var command = new CreateEmployeeCommand
        {
            Name = "Test Employee",
            RateType = EmployeeRateType.Hourly,
            Rate = 0
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateEmployee()
    {
        await RunAsDefaultUserAsync();

        var command = new CreateEmployeeCommand
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "0123456789",
            RateType = EmployeeRateType.Hourly,
            Rate = 50.00m,
            Notes = "Test employee"
        };

        var employeeId = await SendAsync(command);

        var employee = await FindAsync<Employee>(employeeId);

        employee.ShouldNotBeNull();
        employee!.Name.ShouldBe(command.Name);
        employee.Email.ShouldBe(command.Email);
        employee.RateType.ShouldBe(command.RateType);
        employee.Rate.ShouldBe(command.Rate);
        employee.IsActive.ShouldBeTrue();
    }
}

