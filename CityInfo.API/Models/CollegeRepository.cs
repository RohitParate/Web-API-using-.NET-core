// In Memory repository

namespace CityInfo.API.Models
{
    public static class CollegeRepository
    {
        public static List<Student> Students { get; set; } = new List<Student>(){
                new Student
                {
                    id = 1,
                    StudentName = "Rohit",
                    Email = "student1email@gmail.com",
                    Address = "NGP, NAGPUR"
                },
                 new Student
                {
                    id = 2,
                    StudentName = "Rahul",
                    Email = "student2email@gmail.com",
                    Address = "NGP, NAGPUR"
                }
            };
    }
}
