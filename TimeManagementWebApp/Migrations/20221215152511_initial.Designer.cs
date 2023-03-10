// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TimeManagementWebApp.Data;

#nullable disable

namespace TimeManagementWebApp.Migrations
{
    [DbContext(typeof(TimeManagementDbContext))]
    [Migration("20221215152511_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TimeManagementClassLibrary.Module", b =>
                {
                    b.Property<int>("ModuleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ModuleId"));

                    b.Property<int>("ClassHoursPerWeek")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("DaysOfWeek")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("NumCredits")
                        .HasColumnType("int");

                    b.Property<double>("SelfStudyHoursPerWeek")
                        .HasColumnType("float");

                    b.Property<int>("SemesterId")
                        .HasColumnType("int");

                    b.HasKey("ModuleId");

                    b.HasIndex("SemesterId");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.Semester", b =>
                {
                    b.Property<int>("SemesterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SemesterId"));

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("NumWeeks")
                        .HasColumnType("int");

                    b.Property<string>("SemesterName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("SemesterId");

                    b.HasIndex("StudentId");

                    b.ToTable("Semesters");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.Student", b =>
                {
                    b.Property<string>("StudentId")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StudentId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.StudyHours", b =>
                {
                    b.Property<int>("StudyHoursId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudyHoursId"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("ModuleId")
                        .HasColumnType("int");

                    b.Property<double>("RemainingStudyHours")
                        .HasColumnType("float");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.HasKey("StudyHoursId");

                    b.HasIndex("ModuleId");

                    b.ToTable("StudyHours");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.Module", b =>
                {
                    b.HasOne("TimeManagementClassLibrary.Semester", "Semester")
                        .WithMany("Modules")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Semester");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.Semester", b =>
                {
                    b.HasOne("TimeManagementClassLibrary.Student", "Student")
                        .WithMany("Semesters")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.StudyHours", b =>
                {
                    b.HasOne("TimeManagementClassLibrary.Module", "Module")
                        .WithMany("StudyHours")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.Module", b =>
                {
                    b.Navigation("StudyHours");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.Semester", b =>
                {
                    b.Navigation("Modules");
                });

            modelBuilder.Entity("TimeManagementClassLibrary.Student", b =>
                {
                    b.Navigation("Semesters");
                });
#pragma warning restore 612, 618
        }
    }
}
