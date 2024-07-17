using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MohiuddinCoreMasterDetailCrud.Models;

namespace MohiuddinCoreMasterDetailCrud.Controllers
{
    public class CoursesController : Controller
    {
        private readonly MohiuddinCoreMasterDetailsContext _context;

        public CoursesController(MohiuddinCoreMasterDetailsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Insert([FromForm] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpGet]
        public JsonResult EditCourse(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return Json(new { success = false, message = "Course not found" });
            }
            return Json(new { success = true, data = course });
        }

        [HttpPost]
        public JsonResult UpdateCourse([FromForm] Course course)
        {
            if (ModelState.IsValid)
            {
                var existingCourse = _context.Courses.FirstOrDefault(c => c.CourseId == course.CourseId);
                if (existingCourse != null)
                {
                    existingCourse.CourseName = course.CourseName;
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Course not found" });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public JsonResult DeleteCourse(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return Json(new { success = false, message = "Course not found" });
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();

            return Json(new { success = true });
        }


        public JsonResult GetCourses()
        {
            var courses = _context.Courses.Select(c => new { c.CourseId, c.CourseName }).ToList();
            return Json(courses);
        }


    }
}
