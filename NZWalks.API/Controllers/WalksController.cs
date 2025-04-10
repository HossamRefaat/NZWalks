﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWalkRepository _walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            _mapper = mapper;
            _walkRepository = walkRepository;
        }


        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalksRequestDto addWalksRequestDto)
        {
            var walkDomainModel = _mapper.Map<Walk>(addWalksRequestDto);
            await _walkRepository.AddWalkAsync(walkDomainModel);
            return Ok(_mapper.Map<WalkDto>(walkDomainModel));
        }

        //GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageSize=10&pageNumber=1   
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var walks = await _walkRepository.GetWalksAsync(filterOn, filterQuery,
                sortBy, isAscending ?? true, pageNumber, pageSize);

            return Ok(_mapper.Map<IEnumerable<WalkDto>>(walks));    
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var walk = await _walkRepository.GetWalkByIdAsync(id);
            if (walk == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<WalkDto>(walk));
        }

        [HttpPut("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walkDomainModel = _mapper.Map<Walk>(updateWalkRequestDto);

            walkDomainModel = await _walkRepository.UpdateWalkAsync(id, walkDomainModel);

            if (walkDomainModel == null) return NotFound();

            return Ok(_mapper.Map<WalkDto>(walkDomainModel));
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walk = await _walkRepository.DeleteWalkAsync(id);
            if (walk == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<WalkDto>(walk));
        }
    }
}
