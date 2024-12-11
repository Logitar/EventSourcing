using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.EventSourcing.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateStreamAndEventTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Streams",
                columns: table => new
                {
                    StreamId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.StreamId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StreamId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NamespacedType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Streams_StreamId",
                        column: x => x.StreamId,
                        principalTable: "Streams",
                        principalColumn: "StreamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_ActorId",
                table: "Events",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Id",
                table: "Events",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_IsDeleted",
                table: "Events",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OccurredOn",
                table: "Events",
                column: "OccurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_Events_StreamId_Version",
                table: "Events",
                columns: new[] { "StreamId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_TypeName",
                table: "Events",
                column: "TypeName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Version",
                table: "Events",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_CreatedBy",
                table: "Streams",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_CreatedOn",
                table: "Streams",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_Id",
                table: "Streams",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streams_IsDeleted",
                table: "Streams",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_Type",
                table: "Streams",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_UpdatedBy",
                table: "Streams",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_UpdatedOn",
                table: "Streams",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_Version",
                table: "Streams",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Streams");
        }
    }
}
