using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaManager_Roua.Models.Cinema;

namespace CinemaManager_Hamza.Controllers
{
    public class MoviesController : Controller
    {
        private readonly CinemaDbContext _context;

        public MoviesController(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MoviesAndTheirProds()
        {
            var Cinemamovies = await _context.Movies.Include(m => m.Producer).ToListAsync();

            return View(Cinemamovies);
        }

        public ActionResult ProdsAndTheirMovies() 
        {
            var CinemaDbContext = _context.Producers.Include(p => p.Movies);
            return View(CinemaDbContext); 
        }
       
        public async Task<IActionResult> Index()
        {
            var cinemaDbContext = _context.Movies.Include(m => m.Producer);
            return View(await cinemaDbContext.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

       
        public IActionResult Create()
        {
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id");
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,ProducerId")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id", movie.ProducerId);
            return View(movie);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id", movie.ProducerId);
            return View(movie);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ProducerId")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id", movie.ProducerId);
            return View(movie);
        }

       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

       
        public IActionResult MoviesAndTheirProds_UsingModel()
        {
            var result = from m in _context.Movies
                         join p in _context.Producers on m.ProducerId equals p.Id
                         select new Producerd_Movie
                         {
                             mTitle = m.Title,
                             mGenre = m.Genre,
                             pName = p.Name,
                             pNationality = p.Nationality
                         };

            return View(result.ToList());
        }
        public async Task<IActionResult> MyMovies(int id)
        {
            var movies = await _context.Movies
                .Include(m => m.Producer)
                .Where(m => m.ProducerId == id)
                .ToListAsync();

            return View(movies);
        }
        public ActionResult SearchByTitle(string critere)
        {
            var result = from m in _context.Movies
                         where m.Title.Contains(critere) || string.IsNullOrEmpty(critere)
                         select m;

            return View("Index", result.ToList());
        }

        public ActionResult SearchById(string id)
        {
            var result = from m in _context.Movies
                         where m.Title.Contains(id)
                         select m;

            return View("Index", result.ToList());
        }
        public ActionResult SearchByGenre(string genre)
        {
            var result = from m in _context.Movies
                         where m.Genre.Contains(genre)
                         select m;

            return View("Index", result.ToList());
        }
        public ActionResult SearchBy2(string genre, string title)
        {
            var genres = _context.Movies.Select(m => m.Genre).Distinct().ToList();
            ViewBag.Genres = new SelectList(genres);

            var query = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(m => m.Title.Contains(title));

            if (!string.IsNullOrEmpty(genre) && genre != "All")
                query = query.Where(m => m.Genre == genre);

            return View(query.ToList());
        }


    }
}