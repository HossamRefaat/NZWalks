using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext _dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            //Get data from database - domain model 
            var regions = _dbContext.Regions.ToList();

            //map domain model to DTOs
            var regionsDto = new List<RegionDto>();
            foreach (var region in regions)
            {
                regionsDto.Add(new RegionDto
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl
                });
            }

            return Ok(regionsDto);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            //Get data from database - domain model 
            var region = _dbContext.Regions.FirstOrDefault(r => r.Id == id);

            if (region == null) return NotFound();

            //map domain model to DTOs
            var regionsDto = new RegionDto()
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };

            return Ok(regionsDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };

            _dbContext.Regions.Add(regionDomainModel);
            _dbContext.SaveChanges();

            var regionsDto = new RegionDto()
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            //nameof(Get) => The action method that retrieves the created resource.
            //new {id = regionsDto.Id} => Route values (used to generate the URL). ex: https://..../{id}
            //regionsDto => The response body (created resource).
            return CreatedAtAction(nameof(Get), new {id = regionsDto.Id}, regionsDto);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var region = _dbContext.Regions.FirstOrDefault(x => x.Id == id);
            if(region == null)
            {
                return NotFound();
            }

            region.Code = updateRegionRequestDto.Code;
            region.Name = updateRegionRequestDto.Name;
            region.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            _dbContext.SaveChanges();

            var regionsDto = new RegionDto()
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };

            return Ok(regionsDto);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var region = _dbContext.Regions.FirstOrDefault(x => x.Id == id);
            if (region == null)
            {
                return NotFound();
            }
            _dbContext.Regions.Remove(region);
            _dbContext.SaveChanges();

            var regionsDto = new RegionDto()
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };

            return Ok(regionsDto);
        }
    }
}
