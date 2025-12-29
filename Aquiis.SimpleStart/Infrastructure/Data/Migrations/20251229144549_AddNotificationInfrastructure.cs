using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    EnableInAppNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableEmailNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EmailLeaseExpiring = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailPaymentDue = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailPaymentReceived = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailApplicationStatusChange = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailMaintenanceUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailInspectionScheduled = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSMSNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    SMSPaymentDue = table.Column<bool>(type: "INTEGER", nullable: false),
                    SMSMaintenanceEmergency = table.Column<bool>(type: "INTEGER", nullable: false),
                    SMSLeaseExpiringUrgent = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableDailyDigest = table.Column<bool>(type: "INTEGER", nullable: false),
                    DailyDigestTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EnableWeeklyDigest = table.Column<bool>(type: "INTEGER", nullable: false),
                    WeeklyDigestDay = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecipientUserId = table.Column<string>(type: "TEXT", nullable: false),
                    SentOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReadOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SendInApp = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendSMS = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SMSSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    SMSSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailError = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SMSError = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000003"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000004"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000005"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000006"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000007"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000008"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000009"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000010"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000011"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000012"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000013"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000014"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000015"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000016"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000017"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000018"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000019"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000020"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000021"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000022"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000023"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000024"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000025"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000026"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000027"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000028"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000029"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000030"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000031"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000032"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                column: "LastModifiedBy",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_OrganizationId",
                table: "NotificationPreferences",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId",
                table: "NotificationPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId_OrganizationId",
                table: "NotificationPreferences",
                columns: new[] { "UserId", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Category",
                table: "Notifications",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OrganizationId",
                table: "Notifications",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientUserId",
                table: "Notifications",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SentOn",
                table: "Notifications",
                column: "SentOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000003"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000004"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000005"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000006"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000007"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000008"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000009"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000010"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000011"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000012"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000013"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000014"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000015"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000016"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000017"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000018"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000019"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000020"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000021"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000022"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000023"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000024"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000025"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000026"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000027"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000028"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000029"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000030"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000031"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000032"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                column: "LastModifiedBy",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                column: "LastModifiedBy",
                value: "");
        }
    }
}
