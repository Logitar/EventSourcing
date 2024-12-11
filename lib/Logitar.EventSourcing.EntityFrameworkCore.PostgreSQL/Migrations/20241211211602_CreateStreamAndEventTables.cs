using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateStreamAndEventTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EventSourcing");

            migrationBuilder.CreateTable(
                name: "Streams",
                schema: "EventSourcing",
                columns: table => new
                {
                    StreamId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.StreamId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "EventSourcing",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StreamId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    ActorId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NamespacedType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Streams_StreamId",
                        column: x => x.StreamId,
                        principalSchema: "EventSourcing",
                        principalTable: "Streams",
                        principalColumn: "StreamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_ActorId",
                schema: "EventSourcing",
                table: "Events",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Id",
                schema: "EventSourcing",
                table: "Events",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_IsDeleted",
                schema: "EventSourcing",
                table: "Events",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OccurredOn",
                schema: "EventSourcing",
                table: "Events",
                column: "OccurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_Events_StreamId_Version",
                schema: "EventSourcing",
                table: "Events",
                columns: new[] { "StreamId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_TypeName",
                schema: "EventSourcing",
                table: "Events",
                column: "TypeName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Version",
                schema: "EventSourcing",
                table: "Events",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_CreatedBy",
                schema: "EventSourcing",
                table: "Streams",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_CreatedOn",
                schema: "EventSourcing",
                table: "Streams",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_Id",
                schema: "EventSourcing",
                table: "Streams",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streams_IsDeleted",
                schema: "EventSourcing",
                table: "Streams",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_Type",
                schema: "EventSourcing",
                table: "Streams",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_UpdatedBy",
                schema: "EventSourcing",
                table: "Streams",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_UpdatedOn",
                schema: "EventSourcing",
                table: "Streams",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_Version",
                schema: "EventSourcing",
                table: "Streams",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events",
                schema: "EventSourcing");

            migrationBuilder.DropTable(
                name: "Streams",
                schema: "EventSourcing");
        }
    }
}
