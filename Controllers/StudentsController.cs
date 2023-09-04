using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Models;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize(Roles = "teacher")] 
    public class StudentsController : ControllerBase
    {
        private static List<StudentModel> students = new List<StudentModel> ();
        

        private readonly IHttpClientFactory _httpClientFactory;

        public StudentsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(students); 
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            var student = students.Find(s => s.Id == id);
            if (student == null)
            {
                return NotFound("Student with given ID does not exist");
            }
            return Ok(student);
        }

        [HttpPost]
        public IActionResult CreateStudent(StudentModel student)
        {
            student.Id = students.Count + 1;
            students.Add(student);
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, StudentModel student)
        {
            var existingStudent = students.Find(s => s.Id == id);
            if (existingStudent == null)
            {
                return NotFound("Student with given ID does not exist");
            }

            // Update the student properties
            existingStudent.Name = student.Name;
            existingStudent.Age = student.Age;
            existingStudent.Grade = student.Grade;

            return Ok("Student data updated successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = students.Find(s => s.Id == id);
            if (student == null)
            {
                return NotFound("Student with given ID does not exist");
            }

            students.Remove(student);
            return Ok("Student data deleted successfully");
        }
    }
}
