using Microsoft.EntityFrameworkCore.Migrations;

namespace CollegeSchedule.Migrations
{
    public partial class CollegeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupCourse = table.Column<int>(type: "int", nullable: false),
                    GroupShift = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacherFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationsSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsultationNumber = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Room = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsultationSubjectId = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationsSchedules_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationsSchedules_Subjects_ConsultationSubjectId",
                        column: x => x.ConsultationSubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationsSchedules_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamsSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamNumber = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamSubjectId = table.Column<int>(type: "int", nullable: true),
                    Room = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamsSchedules_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamsSchedules_Subjects_ExamSubjectId",
                        column: x => x.ExamSubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamsSchedules_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PracticeSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PracticeNumber = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PracticeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticeSchedules_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PracticeSchedules_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfTheWeek = table.Column<int>(type: "int", nullable: false),
                    SubjectNumber = table.Column<int>(type: "int", nullable: true),
                    StartOfLesson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndOfLesson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectDenominatorId = table.Column<int>(type: "int", nullable: true),
                    SubjectNumeratorId = table.Column<int>(type: "int", nullable: true),
                    TeacherDenominatorId = table.Column<int>(type: "int", nullable: true),
                    TeacherNumeratorId = table.Column<int>(type: "int", nullable: true),
                    RoomDenominator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomNumerator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Subjects_SubjectDenominatorId",
                        column: x => x.SubjectDenominatorId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Subjects_SubjectNumeratorId",
                        column: x => x.SubjectNumeratorId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Teachers_TeacherDenominatorId",
                        column: x => x.TeacherDenominatorId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Teachers_TeacherNumeratorId",
                        column: x => x.TeacherNumeratorId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeachersSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfTheWeek = table.Column<int>(type: "int", nullable: false),
                    Shift = table.Column<int>(type: "int", nullable: false),
                    SubjectNumber = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    ScheduleDenominatorId = table.Column<int>(type: "int", nullable: true),
                    ScheduleNumeratorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachersSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeachersSchedules_Schedules_ScheduleDenominatorId",
                        column: x => x.ScheduleDenominatorId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeachersSchedules_Schedules_ScheduleNumeratorId",
                        column: x => x.ScheduleNumeratorId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeachersSchedules_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "password", "userName" },
                values: new object[] { 1, "Adm1n$Pa$$2018+-", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsSchedules_ConsultationSubjectId",
                table: "ConsultationsSchedules",
                column: "ConsultationSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsSchedules_GroupId",
                table: "ConsultationsSchedules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsSchedules_TeacherId",
                table: "ConsultationsSchedules",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamsSchedules_ExamSubjectId",
                table: "ExamsSchedules",
                column: "ExamSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamsSchedules_GroupId",
                table: "ExamsSchedules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamsSchedules_TeacherId",
                table: "ExamsSchedules",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSchedules_GroupId",
                table: "PracticeSchedules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSchedules_TeacherId",
                table: "PracticeSchedules",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_GroupId",
                table: "Schedules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SubjectDenominatorId",
                table: "Schedules",
                column: "SubjectDenominatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SubjectNumeratorId",
                table: "Schedules",
                column: "SubjectNumeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_TeacherDenominatorId",
                table: "Schedules",
                column: "TeacherDenominatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_TeacherNumeratorId",
                table: "Schedules",
                column: "TeacherNumeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachersSchedules_ScheduleDenominatorId",
                table: "TeachersSchedules",
                column: "ScheduleDenominatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachersSchedules_ScheduleNumeratorId",
                table: "TeachersSchedules",
                column: "ScheduleNumeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachersSchedules_TeacherId",
                table: "TeachersSchedules",
                column: "TeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationsSchedules");

            migrationBuilder.DropTable(
                name: "ExamsSchedules");

            migrationBuilder.DropTable(
                name: "PracticeSchedules");

            migrationBuilder.DropTable(
                name: "TeachersSchedules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
