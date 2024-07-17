namespace MohiuddinCoreMasterDetailCrud.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime Dob { get; set; }
        public string Mobile { get; set; }
        public string ImageUrl { get; set; }
        public bool IsEnroll { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } 
        public ICollection<Module> Modules { get; set; } 
    }
}
