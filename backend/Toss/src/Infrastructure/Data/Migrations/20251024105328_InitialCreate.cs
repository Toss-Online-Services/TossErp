using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Toss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Street = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Street2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StateProvince = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Coordinates_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Coordinates_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Coordinates_Area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Coordinates_Zone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VehicleType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VehicleRegistration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    Phone_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: true),
                    ContactPhone_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AreaGroup = table.Column<string>(type: "text", nullable: true),
                    OpeningTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ClosingTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    WhatsAppAlertsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    GroupBuyingEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AIAssistantEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Location_Area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Location_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Location_Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Location_Zone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shops_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyRegNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VATNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContactPerson = table.Column<string>(type: "text", nullable: true),
                    ContactPhone_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AddressId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PaymentTermsDays = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SharedDeliveryRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RunNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: true),
                    GroupBuyPoolId = table.Column<int>(type: "integer", nullable: true),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AssignedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualDepartureTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualArrivalTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AreaGroup = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartLocation_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    StartLocation_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    StartLocation_Area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartLocation_Zone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TotalDeliveryCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDistance = table.Column<decimal>(type: "numeric", nullable: false),
                    ParticipantCount = table.Column<int>(type: "integer", nullable: false),
                    CostPerStop = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedDeliveryRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedDeliveryRuns_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SKU = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MinimumStockLevel = table.Column<int>(type: "integer", nullable: false),
                    ReorderQuantity = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AddressId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AllowsMarketing = table.Column<bool>(type: "boolean", nullable: false),
                    TotalPurchaseAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPurchaseCount = table.Column<int>(type: "integer", nullable: false),
                    LastPurchaseDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FirstPurchaseDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TotalPurchases = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Customers_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AggregatedPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    APONumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PONumber = table.Column<string>(type: "text", nullable: false),
                    GroupBuyPoolId = table.Column<int>(type: "integer", nullable: false),
                    PoolId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    TotalQuantity = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequiredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedPurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedPurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PONumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    OrderDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedBy = table.Column<string>(type: "text", nullable: true),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    GroupBuyPoolId = table.Column<int>(type: "integer", nullable: true),
                    IsPartOfGroupBuy = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryStops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SharedDeliveryRunId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    DeliveryInstructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CostShare = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ArrivalTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ActualDeliveryTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: true),
                    PoolParticipationId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DeliveryLocation_Area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DeliveryLocation_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    DeliveryLocation_Longitude = table.Column<double>(type: "double precision", nullable: false),
                    DeliveryLocation_Zone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryStops_SharedDeliveryRuns_SharedDeliveryRunId",
                        column: x => x.SharedDeliveryRunId,
                        principalTable: "SharedDeliveryRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryStops_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false),
                    MinimumStock = table.Column<int>(type: "integer", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    AcknowledgedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AcknowledgedBy = table.Column<string>(type: "text", nullable: true),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAlerts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAlerts_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false),
                    ReservedStock = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ReorderPoint = table.Column<int>(type: "integer", nullable: false),
                    ReorderQuantity = table.Column<int>(type: "integer", nullable: false),
                    AverageCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LastStockDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLevels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockLevels_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    MovementType = table.Column<int>(type: "integer", nullable: false),
                    QuantityBefore = table.Column<int>(type: "integer", nullable: false),
                    QuantityChange = table.Column<int>(type: "integer", nullable: false),
                    QuantityAfter = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ReferenceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MovementDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SupplierSKU = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    MinOrderQuantity = table.Column<int>(type: "integer", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierProducts_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    InteractionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    InteractionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    InteractionBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RequiresFollowUp = table.Column<bool>(type: "boolean", nullable: false),
                    FollowUpDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsFollowedUp = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerInteractions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LinkCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FullUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SourceId = table.Column<int>(type: "integer", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CustomerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CustomerEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayLinks_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PayLinks_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    SaleDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaymentReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VoidReason = table.Column<string>(type: "text", nullable: true),
                    VoidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sales_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupBuyPools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PoolNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    InitiatorShopId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    MinimumQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaximumQuantity = table.Column<int>(type: "integer", nullable: true),
                    CurrentQuantity = table.Column<int>(type: "integer", nullable: false),
                    TargetParticipants = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BulkDiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FinalUnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OpenDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CloseDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConfirmedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AreaGroup = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MaxDistanceKm = table.Column<double>(type: "double precision", nullable: true),
                    EstimatedShippingCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualShippingCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    SharedDeliveryRunId = table.Column<int>(type: "integer", nullable: true),
                    AggregatedPurchaseOrderId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupBuyPools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupBuyPools_AggregatedPurchaseOrders_AggregatedPurchaseOr~",
                        column: x => x.AggregatedPurchaseOrderId,
                        principalTable: "AggregatedPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GroupBuyPools_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupBuyPools_Shops_InitiatorShopId",
                        column: x => x.InitiatorShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupBuyPools_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ProductSKU = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuantityOrdered = table.Column<int>(type: "integer", nullable: false),
                    QuantityReceived = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceiptNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReceivedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsPartialReceipt = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    QualityCheckPassed = table.Column<bool>(type: "boolean", nullable: false),
                    QualityNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipts_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProofOfDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeliveryStopId = table.Column<int>(type: "integer", nullable: false),
                    ProofType = table.Column<int>(type: "integer", nullable: false),
                    PIN = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SignatureData = table.Column<string>(type: "text", nullable: true),
                    SignedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RecipientName = table.Column<string>(type: "text", nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CaptureLocation_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    CaptureLocation_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    CaptureLocation_Area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CaptureLocation_Zone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProofData = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProofOfDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProofOfDeliveries_DeliveryStops_DeliveryStopId",
                        column: x => x.DeliveryStopId,
                        principalTable: "DeliveryStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupplierProductId = table.Column<int>(type: "integer", nullable: false),
                    MinQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaxQuantity = table.Column<int>(type: "integer", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPricings_SupplierProducts_SupplierProductId",
                        column: x => x.SupplierProductId,
                        principalTable: "SupplierProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SourceId = table.Column<int>(type: "integer", nullable: true),
                    SaleId = table.Column<int>(type: "integer", nullable: true),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: true),
                    TransactionRef = table.Column<string>(type: "text", nullable: true),
                    PaymentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    PayerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PayerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PayerEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    GatewayReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GatewayResponse = table.Column<string>(type: "text", nullable: true),
                    InitiatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AuthorizedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PayLinkId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_PayLinks_PayLinkId",
                        column: x => x.PayLinkId,
                        principalTable: "PayLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    PurchaseDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PurchaseAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ItemCount = table.Column<int>(type: "integer", nullable: false),
                    TopProductCategory = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CustomerId1 = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPurchases_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerPurchases_Customers_CustomerId1",
                        column: x => x.CustomerId1,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerPurchases_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    InvoiceDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    PaidDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceiptNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IssuedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CustomerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ReceiptContent = table.Column<string>(type: "text", nullable: true),
                    IsPrinted = table.Column<bool>(type: "boolean", nullable: false),
                    PrintedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Receipts_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ProductSKU = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoolParticipations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupBuyPoolId = table.Column<int>(type: "integer", nullable: false),
                    PoolId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    QuantityCommitted = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingShare = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    JoinedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    ConfirmedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolParticipations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoolParticipations_GroupBuyPools_GroupBuyPoolId",
                        column: x => x.GroupBuyPoolId,
                        principalTable: "GroupBuyPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoolParticipations_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedPurchaseOrders_APONumber",
                table: "AggregatedPurchaseOrders",
                column: "APONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedPurchaseOrders_OrderDate",
                table: "AggregatedPurchaseOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedPurchaseOrders_Status",
                table: "AggregatedPurchaseOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedPurchaseOrders_SupplierId",
                table: "AggregatedPurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteractions_CustomerId_RequiresFollowUp",
                table: "CustomerInteractions",
                columns: new[] { "CustomerId", "RequiresFollowUp" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInteractions_InteractionDate",
                table: "CustomerInteractions",
                column: "InteractionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchases_CustomerId_PurchaseDate",
                table: "CustomerPurchases",
                columns: new[] { "CustomerId", "PurchaseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchases_CustomerId1",
                table: "CustomerPurchases",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchases_PurchaseDate",
                table: "CustomerPurchases",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchases_SaleId",
                table: "CustomerPurchases",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AddressId",
                table: "Customers",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_LastPurchaseDate",
                table: "Customers",
                column: "LastPurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ShopId_Email",
                table: "Customers",
                columns: new[] { "ShopId", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryStops_SharedDeliveryRunId_SequenceNumber",
                table: "DeliveryStops",
                columns: new[] { "SharedDeliveryRunId", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryStops_ShopId",
                table: "DeliveryStops",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryStops_Status",
                table: "DeliveryStops",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Email",
                table: "Drivers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_IsActive_IsAvailable",
                table: "Drivers",
                columns: new[] { "IsActive", "IsAvailable" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_AggregatedPurchaseOrderId",
                table: "GroupBuyPools",
                column: "AggregatedPurchaseOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_AreaGroup",
                table: "GroupBuyPools",
                column: "AreaGroup");

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_InitiatorShopId",
                table: "GroupBuyPools",
                column: "InitiatorShopId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_PoolNumber",
                table: "GroupBuyPools",
                column: "PoolNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_ProductId",
                table: "GroupBuyPools",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_Status",
                table: "GroupBuyPools",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_Status_CloseDate",
                table: "GroupBuyPools",
                columns: new[] { "Status", "CloseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyPools_SupplierId",
                table: "GroupBuyPools",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate",
                table: "Invoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_IsPaid",
                table: "Invoices",
                column: "IsPaid");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SaleId",
                table: "Invoices",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_PayLinks_CustomerId",
                table: "PayLinks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PayLinks_IsActive_ExpiresAt",
                table: "PayLinks",
                columns: new[] { "IsActive", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PayLinks_LinkCode",
                table: "PayLinks",
                column: "LinkCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayLinks_ShopId",
                table: "PayLinks",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_PayLinks_SourceType_SourceId",
                table: "PayLinks",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InitiatedAt",
                table: "Payments",
                column: "InitiatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PayLinkId",
                table: "Payments",
                column: "PayLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentReference",
                table: "Payments",
                column: "PaymentReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ShopId",
                table: "Payments",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SourceType_SourceId",
                table: "Payments",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PoolParticipations_GroupBuyPoolId_ShopId",
                table: "PoolParticipations",
                columns: new[] { "GroupBuyPoolId", "ShopId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoolParticipations_JoinedDate",
                table: "PoolParticipations",
                column: "JoinedDate");

            migrationBuilder.CreateIndex(
                name: "IX_PoolParticipations_ShopId",
                table: "PoolParticipations",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Name",
                table: "ProductCategories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProofOfDeliveries_CapturedAt",
                table: "ProofOfDeliveries",
                column: "CapturedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProofOfDeliveries_DeliveryStopId",
                table: "ProofOfDeliveries",
                column: "DeliveryStopId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ProductId",
                table: "PurchaseOrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_GroupBuyPoolId",
                table: "PurchaseOrders",
                column: "GroupBuyPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderDate",
                table: "PurchaseOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PONumber",
                table: "PurchaseOrders",
                column: "PONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_ShopId_OrderDate",
                table: "PurchaseOrders",
                columns: new[] { "ShopId", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_Status",
                table: "PurchaseOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipts_PurchaseOrderId",
                table: "PurchaseReceipts",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipts_ReceiptNumber",
                table: "PurchaseReceipts",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipts_ReceivedDate",
                table: "PurchaseReceipts",
                column: "ReceivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_IssuedDate",
                table: "Receipts",
                column: "IssuedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ReceiptNumber",
                table: "Receipts",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_SaleId",
                table: "Receipts",
                column: "SaleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ShopId",
                table: "Receipts",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId",
                table: "SaleItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleDate",
                table: "Sales",
                column: "SaleDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleNumber",
                table: "Sales",
                column: "SaleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ShopId_SaleDate",
                table: "Sales",
                columns: new[] { "ShopId", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Status",
                table: "Sales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDeliveryRuns_DriverId",
                table: "SharedDeliveryRuns",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDeliveryRuns_GroupBuyPoolId",
                table: "SharedDeliveryRuns",
                column: "GroupBuyPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDeliveryRuns_RunNumber",
                table: "SharedDeliveryRuns",
                column: "RunNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SharedDeliveryRuns_ScheduledDate",
                table: "SharedDeliveryRuns",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDeliveryRuns_Status",
                table: "SharedDeliveryRuns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_AddressId",
                table: "Shops",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_AreaGroup",
                table: "Shops",
                column: "AreaGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_OwnerId",
                table: "Shops",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlerts_ProductId",
                table: "StockAlerts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlerts_ShopId_IsAcknowledged",
                table: "StockAlerts",
                columns: new[] { "ShopId", "IsAcknowledged" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLevels_ProductId",
                table: "StockLevels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLevels_ShopId_ProductId",
                table: "StockLevels",
                columns: new[] { "ShopId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_MovementDate",
                table: "StockMovements",
                column: "MovementDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId",
                table: "StockMovements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ReferenceType_ReferenceId",
                table: "StockMovements",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ShopId_ProductId",
                table: "StockMovements",
                columns: new[] { "ShopId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPricings_SupplierProductId",
                table: "SupplierPricings",
                column: "SupplierProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPricings_ValidFrom_ValidTo",
                table: "SupplierPricings",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProducts_ProductId",
                table: "SupplierProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProducts_SupplierId_ProductId",
                table: "SupplierProducts",
                columns: new[] { "SupplierId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_AddressId",
                table: "Suppliers",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Email",
                table: "Suppliers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Name",
                table: "Suppliers",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CustomerInteractions");

            migrationBuilder.DropTable(
                name: "CustomerPurchases");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PoolParticipations");

            migrationBuilder.DropTable(
                name: "ProofOfDeliveries");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "PurchaseReceipts");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "StockAlerts");

            migrationBuilder.DropTable(
                name: "StockLevels");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "SupplierPricings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PayLinks");

            migrationBuilder.DropTable(
                name: "GroupBuyPools");

            migrationBuilder.DropTable(
                name: "DeliveryStops");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "SupplierProducts");

            migrationBuilder.DropTable(
                name: "AggregatedPurchaseOrders");

            migrationBuilder.DropTable(
                name: "SharedDeliveryRuns");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
