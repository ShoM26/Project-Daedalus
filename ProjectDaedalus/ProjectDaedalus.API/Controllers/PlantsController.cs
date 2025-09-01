using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.API.Dtos;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces; // assuming entities live in Core
using ProjectDaedalus.Infrastructure.Data; // DbContext

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly DaedalusContext _context;
        private readonly IPlantRepository _plantRepository;
        //GET all plants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetAllPlants()
        {
            try
            {
                var plants = await _plantRepository.GetAllAsync();
                if (plants == null)
                {
                    return NotFound($"No plants found");
                }

                return Ok(plants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //GET specific plant
        [HttpGet("{plantId}")]
        public async Task<IActionResult> GetPlantById(int plantId)
        {
            try
            {
                var p = await _plantRepository.GetByIdAsync(plantId);
                if (p == null)
                {
                    return NotFound($"Plant {plantId} not found");
                }

                // convert to dto
                var plant = new PlantDTO
                {
                    PlantId = p.PlantId,
                    ScientificName = p.ScientificName,
                    FamiliarName = p.FamiliarName,
                    FunFact = p.FunFact,
                    MoistureHighRange = p.MoistureHighRange,
                    MoistureLowRange = p.MoistureLowRange
                };
                return Ok(plant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //POST add new plant to database
        [HttpPost]
        public async Task<ActionResult<PlantDTO>> AddPlant([FromBody] PlantDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid payload");
            }

            try
            {
                var existingPlant = await _plantRepository.ScientificNameExistAsync(dto.ScientificName);
                if (existingPlant)
                {
                    return Conflict($"Plant with scientific name {dto.ScientificName} already exists");
                }

                var plant = new Plant
                {
                    ScientificName = dto.ScientificName,
                    FamiliarName = dto.FamiliarName,
                    MoistureHighRange = dto.MoistureHighRange,
                    MoistureLowRange = dto.MoistureLowRange,
                    FunFact = dto.FunFact
                };
                var createdPlant = await _plantRepository.AddAsync(plant);

                var resultDto = new PlantDTO
                {
                    PlantId = createdPlant.PlantId,
                    ScientificName = createdPlant.ScientificName,
                    FamiliarName = createdPlant.FamiliarName,
                    MoistureHighRange = createdPlant.MoistureHighRange,
                    MoistureLowRange = createdPlant.MoistureLowRange,
                    FunFact = createdPlant.FunFact
                };
                return CreatedAtAction(nameof(GetPlantById), resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //PUT update plant info
        [HttpPut("{plantId}")]
        public async Task<IActionResult> UpdatePlant(int plantId, [FromBody] PlantDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid payload");
            }
            // Find device by id
            try
            {
                var existingPlant = await _plantRepository.GetByIdAsync(plantId);
                if (existingPlant == null)
                {
                    return BadRequest($"Plant with id {plantId} not found");
                }

                if (!string.IsNullOrEmpty(dto.ScientificName) && dto.ScientificName != existingPlant.ScientificName)
                {
                    var duplicatePlant = await _plantRepository.GetByScientificNameAsync(dto.ScientificName);
                    if (duplicatePlant != null && duplicatePlant.PlantId != plantId)
                    {
                        return Conflict($"Plant with scientific name {dto.ScientificName} already exists");
                    }
                }

                if (!string.IsNullOrEmpty(dto.ScientificName))
                    existingPlant.ScientificName = dto.ScientificName;
                if (!string.IsNullOrEmpty(dto.FamiliarName))
                    existingPlant.FamiliarName = dto.FamiliarName;
                if (!string.IsNullOrEmpty(dto.FunFact))
                    existingPlant.FunFact = dto.FunFact;
                existingPlant.MoistureLowRange = dto.MoistureLowRange;
                existingPlant.MoistureHighRange = dto.MoistureHighRange;

                var updatedPlant = await _plantRepository.UpdateAsync(existingPlant);

                var resultDto = new PlantDTO
                {
                    ScientificName = updatedPlant.ScientificName,
                    FamiliarName = updatedPlant.FamiliarName,
                    MoistureHighRange = updatedPlant.MoistureHighRange,
                    MoistureLowRange = updatedPlant.MoistureLowRange,
                    FunFact = updatedPlant.FunFact
                };
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //DELETE remove a plant
        [HttpDelete("{plantId}")]
        public async Task<IActionResult> DeletePlant(int plantId)
        {
            try
            {
                var device = await _plantRepository.GetByIdAsync(plantId);
                if (device == null)
                {
                    return BadRequest($"Device with identifier '{plantId}' not found");
                }

                await _plantRepository.DeleteAsync(plantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //GET search plants by name
        [HttpGet("{scientificName}")]
        public async Task<IActionResult> GetPlantByScientificName(string scientificName)
        {
            try
            {
                var plant = await _plantRepository.GetByScientificNameAsync(scientificName);
                if (plant == null)
                {
                    return NotFound($"Plant with identifier '{scientificName}' not found");
                }

                return Ok(plant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

