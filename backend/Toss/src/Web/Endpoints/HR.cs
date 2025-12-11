using Microsoft.AspNetCore.Mvc;
using Toss.Application.HR.Commands.CreateEmployee;
using Toss.Application.HR.Commands.UpdateEmployee;
using Toss.Application.HR.Commands.DeleteEmployee;
using Toss.Application.HR.Commands.RecordClockAttendance;
using Toss.Application.HR.Commands.RecordDaysAttendance;
using Toss.Application.HR.Commands.RunPayroll;
using Toss.Application.HR.Queries.GetEmployees;
using Toss.Application.HR.Queries.GetEmployeeById;
using Toss.Application.HR.Queries.GetAttendanceForPeriod;
using Toss.Application.HR.Queries.GetPayrollRuns;
using Toss.Application.HR.Queries.ExportPayroll;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class HR : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Employee CRUD
        group.MapGet("employees", GetEmployees)
            .WithName("GetEmployees");

        group.MapGet("employees/{id}", GetEmployeeById)
            .WithName("GetEmployeeById");

        group.MapPost("employees", CreateEmployee)
            .WithName("CreateEmployee");

        group.MapPut("employees/{id}", UpdateEmployee)
            .WithName("UpdateEmployee");

        group.MapDelete("employees/{id}", DeleteEmployee)
            .WithName("DeleteEmployee");

        // Attendance
        group.MapPost("attendance/clock", RecordClockAttendance)
            .WithName("RecordClockAttendance");

        group.MapPost("attendance/days", RecordDaysAttendance)
            .WithName("RecordDaysAttendance");

        group.MapGet("attendance", GetAttendanceForPeriod)
            .WithName("GetAttendanceForPeriod");

        // Payroll
        group.MapPost("payroll/run", RunPayroll)
            .WithName("RunPayroll");

        group.MapGet("payroll/runs", GetPayrollRuns)
            .WithName("GetPayrollRuns");

        group.MapGet("payroll/export", ExportPayroll)
            .WithName("ExportPayroll");
    }

    public async Task<IResult> GetEmployees(ISender sender, [AsParameters] GetEmployeesQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetEmployeeById(ISender sender, int id)
    {
        var result = await sender.Send(new GetEmployeeByIdQuery { Id = id });
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CreateEmployee(ISender sender, CreateEmployeeCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/hr/employees/{id}", new { id });
    }

    public async Task<IResult> UpdateEmployee(ISender sender, int id, UpdateEmployeeCommand command)
    {
        var result = await sender.Send(command with { Id = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> DeleteEmployee(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteEmployeeCommand { Id = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> RecordClockAttendance(ISender sender, RecordClockAttendanceCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/hr/attendance/{id}", new { id });
    }

    public async Task<IResult> RecordDaysAttendance(ISender sender, RecordDaysAttendanceCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/hr/attendance/{id}", new { id });
    }

    public async Task<IResult> GetAttendanceForPeriod(ISender sender, [AsParameters] GetAttendanceForPeriodQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> RunPayroll(ISender sender, [AsParameters] RunPayrollCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    public async Task<IResult> GetPayrollRuns(ISender sender, [AsParameters] GetPayrollRunsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> ExportPayroll(
        ISender sender,
        [FromQuery] DateTimeOffset fromDate,
        [FromQuery] DateTimeOffset toDate,
        [FromBody] List<int>? employeeIds)
    {
        var exportQuery = new ExportPayrollQuery
        {
            FromDate = fromDate,
            ToDate = toDate,
            EmployeeIds = employeeIds
        };

        var csvBytes = await sender.Send(exportQuery);
        var fileName = $"payroll_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv";
        return Results.File(csvBytes, "text/csv", fileName);
    }
}

