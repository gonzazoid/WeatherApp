using System;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using WeatherApp.Models;

namespace MvcMovie.Controllers
{

    public class HomeController : Controller
    {

        WeatherAppContext dbContext;

        public HomeController(WeatherAppContext _context)
        {
            dbContext = _context;
        }

        public JsonResult Err400()
        {
                Response.StatusCode = 400;
                return Json("bad request");
        }

        public JsonResult Err500()
        {
                Response.StatusCode = 500;
                return Json("internal error");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult ViewHistory()
        {
            return View();
        }

        [HttpPost]
        async public Task<JsonResult> getLocations([FromBody] GetLocationsReq req)
        {
            if (req == null || !ModelState.IsValid)
            {
                return Err400();
            }
            // Overflow is checked in the fluentvalidate rules
            int skip = (int)((req.page - 1) * req.rows);

            var places = dbContext.places;
            var ordered_query = req.sord == "asc" ? places.OrderBy(x => x.Title) : places.OrderByDescending(x => x.Title);
            var query = await ordered_query
                .Skip(skip)
                .Take((int)req.rows) 
                .ToListAsync();

            var count = await places.CountAsync();
            // TODO strong typing
            dynamic res = new System.Dynamic.ExpandoObject();
            res.total = (int)Math.Ceiling((double)count/req.rows);
            res.page = req.page;
            res.records = query.Count;
            res.rows = query;
            Console.WriteLine(Json(query));
            return Json(res);
        }

        [HttpPost]
        async public Task<JsonResult> setLocation([FromBody] SetLocationReq req)
        {
            if (req == null || !ModelState.IsValid)
            {
                return Err400();
            }
            try
            {
                switch(req.oper){
                    case "add":
                        Place newPlace = new Place(req.title, req.openWeatherProvidersAlias, req.yahooProvidersAlias);
                        dbContext.places.Add(newPlace);
                        await dbContext.SaveChangesAsync();
                        req.placeId = newPlace.PlaceId;
                        break;
                    case "edit":
                        Place place = dbContext.places.Find(req.placeId);
                        if(place == null){
                            return Err400();
                        }
                        place.Title = req.title;
                        place.OpenWeatherProvidersAlias = req.openWeatherProvidersAlias;
                        place.YahooProvidersAlias = req.yahooProvidersAlias;
                        dbContext.Entry(place).State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();
                        break;
                    case "delete":
                        Console.WriteLine("DELETE!!!" + Json(req));

                        var delPlace = dbContext.places.Where(x => x.PlaceId == req.placeId)
                                                       .FirstOrDefault(x => x.PlaceId == req.placeId);
                        if(delPlace == null)
                        {
                            return Err400();
                        }

                        // TODO how it work on large dataset???
                        dbContext.Remove(delPlace);
                        await dbContext.SaveChangesAsync();
                        break;
                    default:
                        // this code must never be executed because of fluentvalidation rules
                        return Err400();
                }
            }
            catch(DbUpdateException ex)
            {
                return Err500();
            }

            return Json(req);
        }

        [HttpPost]
        async public Task<JsonResult> getHistory([FromBody] GetHistoryReq req)
        {
            if (req == null || !ModelState.IsValid)
            {
                return Err400();
            }

            // Overflow is checked in the fluentvalidate rules
            int skip = (int)((req.page - 1) * req.rows);

            // TODO ISSUE Entity Framework Core does not translate GroupBy into SQL yet, that is a future feature.
            // https://github.com/aspnet/EntityFrameworkCore/issues/6245
            var states = dbContext.weather_states.Where(x => x.PlaceId == req.placeId).GroupBy(x => x.stamp);
            var ordered_query = req.sord == "asc" ? states.OrderBy(x => x.Key) : states.OrderByDescending(x => x.Key);
            var query = await ordered_query
                .Skip(skip)
                .Take((int)req.rows) 
                .ToListAsync();


            var count = await dbContext.weather_states
                .Where(x => x.PlaceId == req.placeId)
                .GroupBy(x => x.stamp)
                .CountAsync();


            var place = dbContext.places
                .FirstOrDefault(x => x.PlaceId == req.placeId);

            if(place == null)
            {
                return Err400();
            }

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            foreach (var group in query)
            {
                var row = new Dictionary<string, object>();
                row["stamp"] = group.Key;
                foreach(var state in group)
                {
                    row[state.provider + "Pressure"] = state.pressure;
                    row[state.provider + "Temp"] = state.temp;
                }
                rows.Add(row);
            }

            dynamic res = new System.Dynamic.ExpandoObject();
            res.total = (int)Math.Ceiling((double)count/req.rows);
            res.page = req.page;
            res.records = count;
            res.rows = rows;
            res.locationName = place.Title;

            return Json(res);

        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
