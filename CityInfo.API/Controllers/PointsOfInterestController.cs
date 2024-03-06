using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _cityDataStore;

        public PointsOfInterestController(
                ILogger<PointsOfInterestController> logger,
                IMailService mailService,
                CitiesDataStore citiesDataStore
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetpointsOfInterests(int cityId)
        {
            //throw new Exception("Exception sample");
            try
            {
                //throw new Exception("Exception sample");
                var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                if(city == null)
                {
                    _logger.LogInformation($"City with id {cityId} not found.");
                    return NotFound();
                }   
                return Ok(city.PointsOfInterests);

            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest with city id - {cityId}", ex);
                return StatusCode(500, "Problem happened while handling the request");
            }
                
        }

        [HttpGet("{pointofinterestid}", Name = "getPointOfInterest")]

        public ActionResult<PointOfInterestDto> GetpointOfInterest(int cityId, int pointofinterestid)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            //find point of interest
            var pointOfInterest = city.PointsOfInterests.FirstOrDefault(c=> c.Id == pointofinterestid);
            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]

        public ActionResult<PointOfInterestDto> CreatePointofInterest(int cityId, PointOfInterestCreationDto pointOfInterest)
        { 
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = _cityDataStore.Cities.SelectMany(c=>c.PointsOfInterests).Max(p => p.Id);

            var newPointOfInterest = new PointOfInterestDto()
            {
                Id = maxPointOfInterestId += 1,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };

            city.PointsOfInterests.Add(newPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = newPointOfInterest.Id
                },
                newPointOfInterest);
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid, PointOfInterestupdateDto pointOfInterestupdate)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            //find point of interest
            var existingPointOfInterest = city.PointsOfInterests.FirstOrDefault(c => c.Id == pointofinterestid);
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            existingPointOfInterest.Name = pointOfInterestupdate.Name;
            existingPointOfInterest.Description = pointOfInterestupdate.Description;

            return NoContent();

        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestupdateDto> patchDocument)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            //find point of interest
            var existingPointOfInterest = city.PointsOfInterests.FirstOrDefault(c => c.Id == pointofinterestid);
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            var pointOfinterestToPatch = new PointOfInterestupdateDto
            {
                Name = existingPointOfInterest.Name,
                Description = existingPointOfInterest.Description,
            };

            patchDocument.ApplyTo(pointOfinterestToPatch, ModelState); // any error will catched by model state

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            //if we try to update just with path and operation and without value this code will get executed as this will try to validate the model
            if(!TryValidateModel(pointOfinterestToPatch))
            {
                return BadRequest(ModelState);
            }

            existingPointOfInterest.Name = pointOfinterestToPatch.Name;
            existingPointOfInterest.Description = pointOfinterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]

        public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            //find point of interest
            var existingPointOfInterest = city.PointsOfInterests.FirstOrDefault(c => c.Id == pointofinterestid);
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            city.PointsOfInterests.Remove(existingPointOfInterest);

            _mailService.Send("Point of interest deleted.", $"Point of interest {existingPointOfInterest.Name} with id {existingPointOfInterest.Id} has been deleted");

            return NoContent();  
        }
    }
}
