using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tele_Farming.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    profile_picture_path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.admin_id);
                });

            migrationBuilder.CreateTable(
                name: "Agent",
                columns: table => new
                {
                    agent_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contact_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bkash_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    profile_picture_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agent", x => x.agent_id);
                });

            migrationBuilder.CreateTable(
                name: "Complain",
                columns: table => new
                {
                    complain_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_resolved = table.Column<int>(type: "int", nullable: false),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complain", x => x.complain_id);
                });

            migrationBuilder.CreateTable(
                name: "Farmer",
                columns: table => new
                {
                    farmer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contact_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bkash_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    profile_picture_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmer", x => x.farmer_id);
                });

            migrationBuilder.CreateTable(
                name: "Specialist",
                columns: table => new
                {
                    specialist_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contact_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bkash_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    certificate_file_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nid_file_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    profile_picture_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_approved = table.Column<int>(type: "int", nullable: false),
                    PasswordResetCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialist", x => x.specialist_id);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    farmer_id = table.Column<int>(type: "int", nullable: true),
                    agent_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    post_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    post_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_accepted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_Post_Agent_agent_id",
                        column: x => x.agent_id,
                        principalTable: "Agent",
                        principalColumn: "agent_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Farmer_farmer_id",
                        column: x => x.farmer_id,
                        principalTable: "Farmer",
                        principalColumn: "farmer_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FarmerDetails",
                columns: table => new
                {
                    farmer_details_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_number = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmerDetails", x => x.farmer_details_id);
                    table.ForeignKey(
                        name: "FK_FarmerDetails_Post_post_id",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meeting",
                columns: table => new
                {
                    meeting_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    specialist_id = table.Column<int>(type: "int", nullable: false),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    meeting_link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    short_message = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    meeting_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    meeting_status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meeting", x => x.meeting_id);
                    table.ForeignKey(
                        name: "FK_Meeting_Post_post_id",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Meeting_Specialist_specialist_id",
                        column: x => x.specialist_id,
                        principalTable: "Specialist",
                        principalColumn: "specialist_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Images",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    image_path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Images", x => x.id);
                    table.ForeignKey(
                        name: "FK_Post_Images_Post_post_id",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Time",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Time", x => x.id);
                    table.ForeignKey(
                        name: "FK_Post_Time_Post_post_id",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Combined_Read_Write_Posts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: true),
                    farmer_id = table.Column<int>(type: "int", nullable: true),
                    agent_id = table.Column<int>(type: "int", nullable: true),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    specialist_id = table.Column<int>(type: "int", nullable: true),
                    FarmerDetailsfarmer_details_id = table.Column<int>(type: "int", nullable: true),
                    meeting_id = table.Column<int>(type: "int", nullable: true),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    month = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    year = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    minute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    am_pm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComplainedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hasMeeting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Combined_Read_Write_Posts_Agent_agent_id",
                        column: x => x.agent_id,
                        principalTable: "Agent",
                        principalColumn: "agent_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Combined_Read_Write_Posts_Farmer_farmer_id",
                        column: x => x.farmer_id,
                        principalTable: "Farmer",
                        principalColumn: "farmer_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Combined_Read_Write_Posts_FarmerDetails_FarmerDetailsfarmer_details_id",
                        column: x => x.FarmerDetailsfarmer_details_id,
                        principalTable: "FarmerDetails",
                        principalColumn: "farmer_details_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Combined_Read_Write_Posts_Meeting_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "Meeting",
                        principalColumn: "meeting_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Combined_Read_Write_Posts_Post_post_id",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Combined_Read_Write_Posts_Specialist_specialist_id",
                        column: x => x.specialist_id,
                        principalTable: "Specialist",
                        principalColumn: "specialist_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeetingFailures",
                columns: table => new
                {
                    meeting_failure_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    meeting_id = table.Column<int>(type: "int", nullable: false),
                    complain_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingFailures", x => x.meeting_failure_id);
                    table.ForeignKey(
                        name: "FK_MeetingFailures_Complain_complain_id",
                        column: x => x.complain_id,
                        principalTable: "Complain",
                        principalColumn: "complain_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingFailures_Meeting_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "Meeting",
                        principalColumn: "meeting_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    meeting_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    payment_status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_Payment_Meeting_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "Meeting",
                        principalColumn: "meeting_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    meeting_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_Review_Meeting_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "Meeting",
                        principalColumn: "meeting_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Combined_Read_Write_Posts_agent_id",
                table: "Combined_Read_Write_Posts",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combined_Read_Write_Posts_farmer_id",
                table: "Combined_Read_Write_Posts",
                column: "farmer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combined_Read_Write_Posts_FarmerDetailsfarmer_details_id",
                table: "Combined_Read_Write_Posts",
                column: "FarmerDetailsfarmer_details_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combined_Read_Write_Posts_meeting_id",
                table: "Combined_Read_Write_Posts",
                column: "meeting_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combined_Read_Write_Posts_post_id",
                table: "Combined_Read_Write_Posts",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combined_Read_Write_Posts_specialist_id",
                table: "Combined_Read_Write_Posts",
                column: "specialist_id");

            migrationBuilder.CreateIndex(
                name: "IX_FarmerDetails_post_id",
                table: "FarmerDetails",
                column: "post_id",
                unique: true,
                filter: "[post_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Meeting_post_id",
                table: "Meeting",
                column: "post_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meeting_specialist_id",
                table: "Meeting",
                column: "specialist_id");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingFailures_complain_id",
                table: "MeetingFailures",
                column: "complain_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeetingFailures_meeting_id",
                table: "MeetingFailures",
                column: "meeting_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_meeting_id",
                table: "Payment",
                column: "meeting_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_agent_id",
                table: "Post",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_farmer_id",
                table: "Post",
                column: "farmer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Images_post_id",
                table: "Post_Images",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Time_post_id",
                table: "Post_Time",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_meeting_id",
                table: "Review",
                column: "meeting_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Combined_Read_Write_Posts");

            migrationBuilder.DropTable(
                name: "MeetingFailures");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Post_Images");

            migrationBuilder.DropTable(
                name: "Post_Time");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "FarmerDetails");

            migrationBuilder.DropTable(
                name: "Complain");

            migrationBuilder.DropTable(
                name: "Meeting");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Specialist");

            migrationBuilder.DropTable(
                name: "Agent");

            migrationBuilder.DropTable(
                name: "Farmer");
        }
    }
}
