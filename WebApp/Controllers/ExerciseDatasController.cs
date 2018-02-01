using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/ExerciseDatas")]
    public class ExerciseDatasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExerciseDatas
        [HttpGet]
        public IEnumerable<ExerciseData> GetTestData()
        {
            return _context.TestData;
        }

        // GET: api/ExerciseDatas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExerciseData([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exerciseData = await _context.TestData.SingleOrDefaultAsync(m => m.Id == id);

            if (exerciseData == null)
            {
                return NotFound();
            }

            return Ok(exerciseData);
        }

        // PUT: api/ExerciseDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExerciseData([FromRoute] Guid id, [FromBody] ExerciseData exerciseData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != exerciseData.Id)
            {
                return BadRequest();
            }

            _context.Entry(exerciseData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExerciseDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ExerciseDatas
        [HttpPost]
        public async Task<IActionResult> PostExerciseData([FromBody] ExerciseData exerciseData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TestData.Add(exerciseData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExerciseData", new { id = exerciseData.Id }, exerciseData);
        }

        // DELETE: api/ExerciseDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExerciseData([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exerciseData = await _context.TestData.SingleOrDefaultAsync(m => m.Id == id);
            if (exerciseData == null)
            {
                return NotFound();
            }

            _context.TestData.Remove(exerciseData);
            await _context.SaveChangesAsync();

            return Ok(exerciseData);
        }

        private bool ExerciseDataExists(Guid id)
        {
            return _context.TestData.Any(e => e.Id == id);
        }
    }
}