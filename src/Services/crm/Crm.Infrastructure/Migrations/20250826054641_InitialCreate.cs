using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "crm");

            migrationBuilder.CreateTable(
                name: "CustomerInteractions",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FollowUpDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInteractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: true),
                    Industry = table.Column<string>(type: "text", nullable: true),
                    EmployeeCount = table.Column<int>(type: "integer", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    TaxId = table.Column<string>(type: "text", nullable: true),
                    AssignedTo = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    SubscriptionStatus = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubscriptionEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastActivityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Company = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AddressStreet = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AddressCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AddressState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AddressPostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AddressCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Industry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CompanySize = table.Column<int>(type: "integer", nullable: true),
                    EstimatedValueAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    EstimatedValueCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastContactedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QualifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QualifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConvertedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConvertedCustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConvertedOpportunityId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConvertedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    CampaignName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NextFollowUp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContactAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastActivityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyTransactions",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RelatedOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: true),
                    Stage = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ExpectedCloseDate = table.Column<DateTime>(type: "date", nullable: false),
                    ActualCloseDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SalesTeam = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastActivityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContactAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NextFollowUp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WinReason = table.Column<string>(type: "text", nullable: true),
                    LossReason = table.Column<string>(type: "text", nullable: true),
                    CompetitorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    CampaignName = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CloseReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StageProgressDays = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    ActualValueAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ActualValueCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    EstimatedValueAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EstimatedValueCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Probability = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    AssignedTo = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Outcome = table.Column<string>(type: "text", nullable: true),
                    NextAction = table.Column<string>(type: "text", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "crm",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Communication",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CommunicatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    From = table.Column<string>(type: "text", nullable: true),
                    To = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    Attachments = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communication_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "crm",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    JobTitle = table.Column<string>(type: "text", nullable: true),
                    Department = table.Column<string>(type: "text", nullable: true),
                    ContactType = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastContactedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contact_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "crm",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Document",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedBy = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "crm",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Note",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "crm",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_CustomerId",
                schema: "crm",
                table: "Activity",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Communication_CustomerId",
                schema: "crm",
                table: "Communication",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_CustomerId",
                schema: "crm",
                table: "Contact",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteraction_CreatedAt",
                schema: "crm",
                table: "CustomerInteractions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteraction_CreatedBy",
                schema: "crm",
                table: "CustomerInteractions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteraction_CustomerId",
                schema: "crm",
                table: "CustomerInteractions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteraction_FollowUpDate",
                schema: "crm",
                table: "CustomerInteractions",
                column: "FollowUpDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteraction_Status",
                schema: "crm",
                table: "CustomerInteractions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteraction_Type",
                schema: "crm",
                table: "CustomerInteractions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Document_CustomerId",
                schema: "crm",
                table: "Document",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_AssignedTo",
                schema: "crm",
                table: "Leads",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_CampaignId",
                schema: "crm",
                table: "Leads",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_ConvertedAt",
                schema: "crm",
                table: "Leads",
                column: "ConvertedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_CreatedAt",
                schema: "crm",
                table: "Leads",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_IsDeleted",
                schema: "crm",
                table: "Leads",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_LastContactedAt",
                schema: "crm",
                table: "Leads",
                column: "LastContactedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_QualifiedAt",
                schema: "crm",
                table: "Leads",
                column: "QualifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Source",
                schema: "crm",
                table: "Leads",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Status",
                schema: "crm",
                table: "Leads",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Tenant_AssignedTo",
                schema: "crm",
                table: "Leads",
                columns: new[] { "TenantId", "AssignedTo" });

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Tenant_Status",
                schema: "crm",
                table: "Leads",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Lead_TenantId",
                schema: "crm",
                table: "Leads",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransaction_CreatedAt",
                schema: "crm",
                table: "LoyaltyTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransaction_CustomerId",
                schema: "crm",
                table: "LoyaltyTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransaction_ExpiryDate",
                schema: "crm",
                table: "LoyaltyTransactions",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransaction_RelatedOrderId",
                schema: "crm",
                table: "LoyaltyTransactions",
                column: "RelatedOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransaction_Type",
                schema: "crm",
                table: "LoyaltyTransactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Note_CustomerId",
                schema: "crm",
                table: "Note",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_ActualCloseDate",
                schema: "crm",
                table: "Opportunities",
                column: "ActualCloseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_AssignedTo",
                schema: "crm",
                table: "Opportunities",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_CreatedAt",
                schema: "crm",
                table: "Opportunities",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Customer_Stage",
                schema: "crm",
                table: "Opportunities",
                columns: new[] { "CustomerId", "Stage" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_CustomerId",
                schema: "crm",
                table: "Opportunities",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_ExpectedCloseDate",
                schema: "crm",
                table: "Opportunities",
                column: "ExpectedCloseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_IsDeleted",
                schema: "crm",
                table: "Opportunities",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_LastActivityDate",
                schema: "crm",
                table: "Opportunities",
                column: "LastActivityDate");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_LeadId",
                schema: "crm",
                table: "Opportunities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Priority",
                schema: "crm",
                table: "Opportunities",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Source",
                schema: "crm",
                table: "Opportunities",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Stage",
                schema: "crm",
                table: "Opportunities",
                column: "Stage");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Tenant_AssignedTo",
                schema: "crm",
                table: "Opportunities",
                columns: new[] { "TenantId", "AssignedTo" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Tenant_Stage",
                schema: "crm",
                table: "Opportunities",
                columns: new[] { "TenantId", "Stage" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_TenantId",
                schema: "crm",
                table: "Opportunities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Type",
                schema: "crm",
                table: "Opportunities",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activity",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Communication",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Contact",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "CustomerInteractions",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Document",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Leads",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "LoyaltyTransactions",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Note",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Opportunities",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "crm");
        }
    }
}
