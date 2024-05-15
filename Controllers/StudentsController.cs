using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookLibrary.Data;
using BookLibrary.Model;
using BookLibrary.Repository.Interface;

namespace BookLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGenericRepository<Student> _genericRepository;

        public StudentsController(ApplicationDbContext context, IGenericRepository<Student> genericRepository)
        {
            _context = context;
            _genericRepository = genericRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentList()
        {
            var students = await _genericRepository.GetAll();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _genericRepository.Get(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            var result = await _genericRepository.Put(id, student);
            if (!result)
            {
                return NotFound(); 
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            var createdStudent = await _genericRepository.Post(student);
            if (createdStudent == null)
            {
                return BadRequest();
            }
            return CreatedAtAction("GetStudent", new { id = createdStudent.StudentId }, createdStudent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.StudentList.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            _context.StudentList.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool StudentExists(int id)
        {
            return _context.StudentList.Any(e => e.StudentId == id);
        }
    }
}
