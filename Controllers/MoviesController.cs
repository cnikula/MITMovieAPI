using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveAPI.Models;

namespace MoveAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class MoviesController : Controller
    {
        private readonly MovieContext _movieContext;

        public MoviesController(MovieContext movieContext)
        {
            _movieContext = movieContext;
        }

        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpGet("CheckAuthenticated")]
        public IActionResult CheckAuthenticated()
        {
            return Ok("User Authenticated Successfully!");
        }

        // get : api/Movies
        /// <summary>
        /// Retrieves all movies
        /// </summary>
        /// <returns>List of moves</returns>
        /// <response code="200">Returns all movies</response>
        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            if (_movieContext.Movies is null) { return NotFound(); }

            return await _movieContext.Movies.ToListAsync();
        }

        // get : api/Movies/2
        /// <summary>
        /// Retrieves Movie with given Id
        /// </summary>
        /// <param name="id">Move Id</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            if (_movieContext.Movies is null) { return NotFound(); }

            var movie = await _movieContext.Movies.FindAsync(id);
            if (movie == null) { return NotFound(); }

            return movie;
        }

        // post : api/Movies
        /// <summary>
        /// Adds a new Movie
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            if (_movieContext.Movies is null) { return NotFound(); }

            _movieContext.Movies.Add(movie);
            await _movieContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // put : api/Movies/2
        /// <summary>
        /// Update Movie record
        /// </summary>
        /// <param name="id"> Move Id</param>
        /// <param name="movie"> Movie Data</param>
        /// <returns>
        /// </returns>
        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id) { return BadRequest(); }

            try
            {
                _movieContext.Entry(movie).State = EntityState.Modified;
                await _movieContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!MoviesExists(id))
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

        // delete : api/Movies/2
        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> Deletemovie(int id)
        {
            if (id <= 0) { return BadRequest(); }
            if (_movieContext.Movies is null) { return NotFound(); }

            var movie = await _movieContext.Movies.FindAsync(id);
            if (movie == null) { return NotFound(); }

            _movieContext.Movies.Remove(movie);
            await _movieContext.SaveChangesAsync();

            return NoContent();
        }

        private bool MoviesExists(long id)
        {
            return (_movieContext.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
