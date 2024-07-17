namespace MohiuddinCoreMasterDetailCrud.Models
{
    public class Module
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int Duration { get; set; }
        public int StudentId { get; set; } 
        public Student Student { get; set; } 
    }
}
