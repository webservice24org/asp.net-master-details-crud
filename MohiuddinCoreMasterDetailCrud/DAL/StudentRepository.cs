using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MohiuddinCoreMasterDetailCrud.Models;
using MohiuddinCoreMasterDetailCrud.Models.ViewModels;

namespace MohiuddinCoreMasterDetailCrud.DAL
{
    public class StudentRepository
    {
        private readonly MohiuddinCoreMasterDetailsContext _context;
        private readonly IWebHostEnvironment _webHost;

        public StudentRepository(MohiuddinCoreMasterDetailsContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }
        public async Task<List<StudentViewModel>> GetStudentsAsync()
        {
            var students = await _context.Students
                .Select(s => new StudentViewModel
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    Dob = s.Dob,
                    MobileNo = s.Mobile,
                    ImageUrl = s.ImageUrl,
                    IsEnrolled = s.IsEnroll,
                    CourseId = s.CourseId
                })
                .ToListAsync();

            return students;
        }

        public async Task<StudentViewModel> GetStudentDetailsAsync(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Course)
                .Include(s => s.Modules)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
                return null;

            var studentViewModel = new StudentViewModel
            {
                StudentId = student.StudentId,
                StudentName = student.StudentName,
                Dob = student.Dob,
                MobileNo = student.Mobile,
                ImageUrl = student.ImageUrl,
                IsEnrolled = student.IsEnroll,
                CourseId = student.CourseId,
                CourseName = student.Course?.CourseName, 
                Modules = student.Modules.Select(m => new ModuleViewModel
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    Duration = m.Duration
                }).ToList()
            };

            return studentViewModel;
        }


        public async Task<List<SelectListItem>> GetCoursesAsync()
        {
            var courses = await _context.Courses
                .Select(cor => new SelectListItem
                {
                    Value = cor.CourseId.ToString(),
                    Text = cor.CourseName
                })
                .ToListAsync();

            return courses;
        }


        public async Task<int> SaveStudentAsync(StudentViewModel studentViewModel)
        {
            var modulesTable = new DataTable();
            modulesTable.Columns.Add("ModuleName", typeof(string));
            modulesTable.Columns.Add("Duration", typeof(int));

            foreach (var module in studentViewModel.Modules)
            {
                modulesTable.Rows.Add(module.ModuleName, module.Duration);
            }

            var parameters = new[]
            {
                new SqlParameter("@StudentName", SqlDbType.NVarChar) { Value = studentViewModel.StudentName },
                new SqlParameter("@Dob", SqlDbType.DateTime) { Value = studentViewModel.Dob },
                new SqlParameter("@Mobile", SqlDbType.NVarChar) { Value = studentViewModel.MobileNo },
                new SqlParameter("@ImageUrl", SqlDbType.NVarChar) { Value = studentViewModel.ImageUrl ?? (object)DBNull.Value },
                new SqlParameter("@IsEnroll", SqlDbType.Bit) { Value = studentViewModel.IsEnrolled },
                new SqlParameter("@CourseId", SqlDbType.Int) { Value = studentViewModel.CourseId },
                new SqlParameter("@Modules", SqlDbType.Structured)
                {
                    TypeName = "dbo.ParamModuleType",
                    Value = modulesTable
                },
                new SqlParameter("@StudentId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC @StudentId = dbo.InsertStudentSP @StudentName, @Dob, @Mobile, @ImageUrl, @IsEnroll, @CourseId, @Modules", parameters);

            var studentId = (int)parameters[7].Value;

            return studentId;
        }

        public async Task<bool> UpdateStudentAsync(StudentViewModel studentViewModel)
        {
            var modulesTable = new DataTable();
            modulesTable.Columns.Add("ModuleName", typeof(string));
            modulesTable.Columns.Add("Duration", typeof(int));

            foreach (var module in studentViewModel.Modules)
            {
                modulesTable.Rows.Add(module.ModuleName, module.Duration);
            }

            var currentStudent = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == studentViewModel.StudentId);
            if (currentStudent == null)
                return false;

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@StudentId", SqlDbType.Int) { Value = studentViewModel.StudentId },
                new SqlParameter("@StudentName", SqlDbType.NVarChar) { Value = studentViewModel.StudentName },
                new SqlParameter("@Dob", SqlDbType.DateTime) { Value = studentViewModel.Dob },
                new SqlParameter("@Mobile", SqlDbType.NVarChar) { Value = studentViewModel.MobileNo },
                new SqlParameter("@IsEnroll", SqlDbType.Bit) { Value = studentViewModel.IsEnrolled },
                new SqlParameter("@CourseId", SqlDbType.Int) { Value = studentViewModel.CourseId },
                new SqlParameter("@Modules", SqlDbType.Structured)
                {
                    TypeName = "dbo.ParamModuleType",
                    Value = modulesTable
                }
            };

            if (!string.IsNullOrEmpty(studentViewModel.ImageUrl) && currentStudent.ImageUrl != studentViewModel.ImageUrl)
            {
                var oldImagePath = Path.Combine(_webHost.WebRootPath, "Images", currentStudent.ImageUrl);
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }

                parameters.Add(new SqlParameter("@ImageUrl", SqlDbType.NVarChar) { Value = studentViewModel.ImageUrl ?? (object)DBNull.Value });
            }
            else
            {
                parameters.Add(new SqlParameter("@ImageUrl", SqlDbType.NVarChar) { Value = currentStudent.ImageUrl ?? (object)DBNull.Value });
            }

            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdateStudentSP @StudentId, @StudentName, @Dob, @Mobile, @ImageUrl, @IsEnroll, @CourseId, @Modules", parameters.ToArray());

            return true;
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Modules)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
                throw new InvalidOperationException("Student not found.");

            _context.Modules.RemoveRange(student.Modules);

            if (!string.IsNullOrEmpty(student.ImageUrl))
            {
                string imagePath = Path.Combine(_webHost.WebRootPath, "Images", student.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

    }
}
