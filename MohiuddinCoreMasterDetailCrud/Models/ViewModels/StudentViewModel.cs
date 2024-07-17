using System.ComponentModel.DataAnnotations;

namespace MohiuddinCoreMasterDetailCrud.Models.ViewModels
{
    public class StudentViewModel
    {
        public int StudentId { get; set; }

        [Required]
        [Display(Name ="Student Name")]
        public string StudentName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name ="Date of Birth")]
        public DateTime Dob { get; set; }

        [Required]
        [Display (Name="Select Course")]
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        [Required]
        [Phone]
        [Display (Name="Mobile No")]
        public string MobileNo { get; set; }

        [Display(Name = "Enrollment")]
        public bool IsEnrolled { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile ProfileFile { get; set; }
        public string ImageUrl { get; set; }

        public IList<ModuleViewModel> Modules { get; set; } = new List<ModuleViewModel>();
    }
}
