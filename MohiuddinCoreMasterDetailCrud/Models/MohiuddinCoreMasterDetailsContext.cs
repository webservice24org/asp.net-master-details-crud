using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MohiuddinCoreMasterDetailCrud.Models;
using System.Threading.Tasks;
namespace MohiuddinCoreMasterDetailCrud.Models
{

    public partial class MohiuddinCoreMasterDetailsContext : DbContext
    {
        public MohiuddinCoreMasterDetailsContext() { }

        public MohiuddinCoreMasterDetailsContext(DbContextOptions<MohiuddinCoreMasterDetailsContext> options)
            : base(options) { }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A7F23CBDCC");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.ModuleId).HasName("PK__Modules__2B7477A7403D382B");

                entity.Property(e => e.ModuleName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Student).WithMany(p => p.Modules)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK__Modules__Student__29572725");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52B993A2ECB17");

                entity.Property(e => e.Dob).HasColumnType("datetime");
                entity.Property(e => e.ImageUrl).IsUnicode(false);
                entity.Property(e => e.Mobile)
                    .HasMaxLength(14)
                    .IsUnicode(false);
                entity.Property(e => e.StudentName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Course).WithMany(p => p.Students)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK__Students__Course__267ABA7A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }


}
