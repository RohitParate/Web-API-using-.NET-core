using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ICityInfoRepository _cityinfoRepository;

        public PointsOfInterestController(
                ILogger<PointsOfInterestController> logger,
                IMailService mailService,
                IMapper mapper,
                ICityInfoRepository cityInfoRepository
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _cityinfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetpointsOfInterests(int cityId)
        {
            if(!await _cityinfoRepository.CityExistsAsync(cityId)) {
                _logger.LogInformation("City not found");
                return NotFound();
            }
           var pointsOfInterestForCity = await _cityinfoRepository.GetpointOfInterestsForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
                
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]

        public async Task<ActionResult<PointOfInterestDto>> GetpointOfInterest(int cityId, int pointofinterestid)
        {
            if (!await _cityinfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City not found");
                return NotFound();
            }

            var pointOfInterest = await _cityinfoRepository.GetPointsOfInterestForCityAsync(cityId, pointofinterestid);

            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]

        public async  Task<ActionResult<PointOfInterestDto>> CreatePointofInterest(int cityId, PointOfInterestCreationDto pointOfInterest)
        {
            if (!await _cityinfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City not found");
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityinfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityinfoRepository.SaveChangesAsync();

            var CreatedPointOfInterest = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = CreatedPointOfInterest.Id,
                }, CreatedPointOfInterest);
          
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointofinterestid, PointOfInterestupdateDto pointOfInterestupdate)
        {
            if (!await _cityinfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City not found");
                return NotFound();
            }
            //find point of interest
            var existingPointOfInterest = await  _cityinfoRepository.GetPointsOfInterestForCityAsync(cityId, pointofinterestid);
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterestupdate, existingPointOfInterest);

            await _cityinfoRepository.SaveChangesAsync();


            return NoContent();

        }

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestupdateDto> patchDocument)
        {
            if (!await _cityinfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City not found");
                return NotFound();
            }
            //find point of interest
            var existingPointOfInterest = await _cityinfoRepository.GetPointsOfInterestForCityAsync(cityId, pointofinterestid);
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestupdateDto>(existingPointOfInterest);



            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // any error will catched by model state

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            //if we try to update just with path and operation and without value this code will get executed as this will try to validate the model
            if(!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, existingPointOfInterest);

            await _cityinfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]

        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointofinterestid)
        {
            if (!await _cityinfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City not found");
                return NotFound();
            }
            //find point of interest
            var existingPointOfInterest = await _cityinfoRepository.GetPointsOfInterestForCityAsync(cityId, pointofinterestid);
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            _cityinfoRepository.DeletePointOfInterest(existingPointOfInterest);
            await _cityinfoRepository.SaveChangesAsync();
            _mailService.Send("Point of interest deleted.", $"Point of interest {existingPointOfInterest.Name} with id {existingPointOfInterest.Id} has been deleted");

            return NoContent();  
        }
    }
}
