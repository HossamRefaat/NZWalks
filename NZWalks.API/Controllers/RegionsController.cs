﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository _regionRepository;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(IRegionRepository regionRepository,
            AutoMapper.IMapper mapper, ILogger<RegionsController> logger)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
              //Get data from database - domain model 
                var regions = await _regionRepository.GetAllAsync();

                //map domain model to DTOs using AutoMapper

                var regionsDto = _mapper.Map<List<RegionDto>>(regions);
                return Ok(regionsDto);

        }

        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Reader")]

        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            //Get data from database - domain model 
            var region = await _regionRepository.GetByIdAsync(id);

            if (region == null) return NotFound();            

            return Ok(_mapper.Map<RegionDto>(region));
        }

        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]

        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {

            var regionDomainModel = _mapper.Map<Region>(addRegionRequestDto);

            regionDomainModel = await _regionRepository.CreateRegionAsync(regionDomainModel);

            var regionsDto = _mapper.Map<RegionDto>(regionDomainModel);

            //nameof(Get) => The action method that retrieves the created resource.
            //new {id = regionsDto.Id} => Route values (used to generate the URL). ex: https://..../{id}
            //regionsDto => The response body (created resource).
            return CreatedAtAction(nameof(Get), new {id = regionsDto.Id}, regionsDto);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]

        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionDomainModel = _mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await _regionRepository.UpdateRegionAsync(id, regionDomainModel);
            
            if(regionDomainModel == null) return NotFound();

            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await _regionRepository.DeleteRegionAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            var regionsDto = _mapper.Map<RegionDto>(region);

            return Ok(regionsDto);
        }
    }
}
