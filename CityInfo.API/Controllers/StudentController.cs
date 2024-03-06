using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {   
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            return Ok(CollegeRepository.Students);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Student> GetStudentById(int id)
        {
            if(id <= 0)
            {
                return BadRequest(); // 400 - Client Error
            }
            var student = CollegeRepository.Students.Where(item => item.id == id).FirstOrDefault();
            if(student == null)
            {
                return NotFound($"Student {id} Not Found"); // 404
            }
            // Ok - 200 - Success
            return Ok(student);
        }

        [HttpGet("{name:alpha}")]
        public ActionResult<Student> GetStudentByName(string name)
        {
            if(String.IsNullOrEmpty(name)) {
               return  BadRequest();
            }
            var student = CollegeRepository.Students.Where(item => item.StudentName== name).FirstOrDefault();
            if(student == null)
            {
                return NotFound($"Student with {name} not found");
            }
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public ActionResult<bool> DeleteStudentById(int id)
        {

            if (id <= 0)
            {
                return BadRequest(); // 400 - Client Error
            }
            var student = CollegeRepository.Students.Where(item => item.id == id).FirstOrDefault();
            if (student == null)
            {
                return NotFound($"Student {id} Not Found"); // 404
            }
            CollegeRepository.Students.Remove(student);

            return Ok(true);
        }
    }
}
