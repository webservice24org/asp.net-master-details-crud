using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MohiuddinCoreMasterDetailCrud.Models;
using MohiuddinCoreMasterDetailCrud.Models.ViewModels;
using MohiuddinCoreMasterDetailCrud.DAL;
using System.Reflection;
using System.Threading.Tasks;

namespace MohiuddinCoreMasterDetailCrud.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHost;

        public StudentsController(MohiuddinCoreMasterDetailsContext db, IWebHostEnvironment webHost)
        {
            _studentRepository = new StudentRepository(db, webHost);
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            var studentViewModel = new StudentViewModel
            {
                Modules = new List<ModuleViewModel>()
            };

            return View(studentViewModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetStudents()
        {
            var students = await _studentRepository.GetStudentsAsync();
            return Json(students);
        }

        [HttpGet]
        public async Task<JsonResult> GetStudentDetails(int studentId)
        {
            var student = await _studentRepository.GetStudentDetailsAsync(studentId);
            if (student == null)
            {
                return Json(new { success = false, message = "Student not found." });
            }
            return Json(new { success = true, data = student });
        }

        public async Task<JsonResult> GetCourses()
        {
            var courses = await _studentRepository.GetCoursesAsync();
            return Json(courses);
        }

        [HttpPost]
        public async Task<JsonResult> InsertStudent(StudentViewModel studentViewModel)
        {
            try
            {
                string imageUrl = GetUploadedFileName(studentViewModel);
                if (imageUrl != null)
                {
                    studentViewModel.ImageUrl = imageUrl;
                    int studentId = await _studentRepository.SaveStudentAsync(studentViewModel);
                    return Json(new { success = true, message = "Student and modules added successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save profile image." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> StudentEdit(int studentId)
        {
            try
            {
                var studentViewModel = await _studentRepository.GetStudentDetailsAsync(studentId);
                if (studentViewModel == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }
                return Json(new { success = true, data = studentViewModel });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching student data: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateStudent(StudentViewModel studentViewModel)
        {
            try
            {
                if (studentViewModel.ProfileFile != null)
                {
                    var newFileName = GetUploadedFileName(studentViewModel);
                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        studentViewModel.ImageUrl = newFileName;
                    }
                }

                bool isUpdated = await _studentRepository.UpdateStudentAsync(studentViewModel);
                if (isUpdated)
                {
                    return Json(new { success = true, message = "Student updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Error updating student: Student not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating student: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<JsonResult> DeleteStudent(int studentId)
        {
            try
            {
                await _studentRepository.DeleteStudentAsync(studentId);
                return Json(new { success = true, message = "Student and related modules deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting student: " + ex.Message });
            }
        }

        private string GetUploadedFileName(StudentViewModel student)
        {
            string uniqueFileName = null;

            if (student.ProfileFile != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + student.ProfileFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    student.ProfileFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
