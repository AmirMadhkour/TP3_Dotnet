using CinemaManager_Hamza.Models.Cinema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaManager_Hamza.Controllers
{
    public class ProducersController : Controller
    {
        private readonly CinemaDbContext _context;

        public ProducersController(CinemaDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var producers = _context.Producers.Include(p => p.Movies).ToList();
            return View(producers);
        }


        public ActionResult Details(int id)
        {
            return View(_context.Producers.Find(id));
        }

        public ActionResult Create()
        {
            var genres = new List<string> { "Action", "Drama", "Comedy", "Horror", "Thriller" };
            ViewBag.Genres = new SelectList(genres);

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Producer producer, string movieTitle, string movieGenre)
        {
            if (ModelState.IsValid)
            {
                _context.Producers.Add(producer);
                _context.SaveChanges();
  
                if (!string.IsNullOrEmpty(movieTitle) && !string.IsNullOrEmpty(movieGenre))
                {
                    var movie = new Movie
                    {
                        Title = movieTitle,
                        Genre = movieGenre,
                        ProducerId = producer.Id
                    };
                    _context.Movies.Add(movie);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(producer);
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Producer p)
        {
            try
            {
                _context.Producers.Update(p);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Producer p)
        {
            try
            {
                _context.Producers.Remove(p);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ProdsAndTheirMovies()
        {
            var producersWithMovies = _context.Producers.Include(p => p.Movies).ToList();

            return View(producersWithMovies);
        }

        public IActionResult ProdsAndTheirMovies_UsingModel()
        {
            var result = from p in _context.Producers
                         from m in p.Movies
                         select new Producer_Movie
                         {
                             pName = p.Name,
                             pNationality = p.Nationality,
                             mTitle = m.Title,
                             mGenre = m.Genre
                         };

            return View(result.ToList());
        }

    }
}
