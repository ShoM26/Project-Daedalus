using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.API.Attributes;
using ProjectDaedalus.API.Dtos.Plant;
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

        public PlantsController(DaedalusContext context, IPlantRepository plantRepository)
        {
            _context = context;
            _plantRepository = plantRepository;
        }
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
        [HttpGet("plants/{plantId}/byId")]
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
                var plant = new PlantDto
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
        [HttpPost("internal")]
        [InternalApi]
        public async Task<ActionResult<PlantDto>> AddPlant([FromBody] PlantDto dto)
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

                var resultDto = new PlantDto
                {
                    PlantId = createdPlant.PlantId,
                    ScientificName = createdPlant.ScientificName,
                    FamiliarName = createdPlant.FamiliarName,
                    MoistureHighRange = createdPlant.MoistureHighRange,
                    MoistureLowRange = createdPlant.MoistureLowRange,
                    FunFact = createdPlant.FunFact
                };
                return CreatedAtAction(
                    nameof(GetPlantById), new { plantId = resultDto.PlantId }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //PUT update plant info
        [HttpPut("internal/{plantId}")]
        [InternalApi]
        public async Task<IActionResult> UpdatePlant(int plantId, [FromBody] PlantDto dto)
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

                var resultDto = new PlantDto
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
        [HttpGet("plants/{scientificName}/byName")]
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

        [HttpPost("bulk-insert")]
        public async Task<ActionResult<BulkRegistrationApiResponse>> BulkPlantInsert([FromBody] List<PlantDto> plantDtos)
        {
            try
            {
                if (plantDtos == null || plantDtos.Count == 0)
                {
                    return BadRequest(new BulkRegistrationApiResponse
                    {
                        Success = false,
                        Message = "No plants were provided",
                        Data = new BulkRegistrationResult
                        {
                            TotalPlants = 0,
                            Message = "Plant list is null or empty"
                        }
                    });
                }

                var plants = plantDtos.Select(dto => new Plant
                {
                    ScientificName = dto.ScientificName?.Trim(),
                    FamiliarName = dto.FamiliarName?.Trim(),
                    MoistureHighRange = dto.MoistureHighRange,
                    MoistureLowRange = dto.MoistureLowRange,
                    FunFact = dto.FunFact?.Trim()
                }).ToList();

                var repositoryResult = await _plantRepository.BulkInsertAsync(plants);

                var apiResult = new BulkRegistrationResult
                {
                    TotalPlants = repositoryResult.TotalPlants,
                    SuccessfulRegistrations = repositoryResult.SuccessfulRegistrations,
                    FailedRegistrations = repositoryResult.FailedRegistrations,
                    Message = repositoryResult.ErrorMessage
                };

                var response = new BulkRegistrationApiResponse
                {
                    Success = apiResult.SuccessfulRegistrations == apiResult.TotalPlants,
                    Message = apiResult.SuccessfulRegistrations > 0
                        ? "Bulk Registration Complete with errors"
                        : "Bulk Registration Complete",
                    Data = apiResult
                };

                if (repositoryResult.HasErrors && repositoryResult.SuccessfulRegistrations == 0)
                {
                    return BadRequest(response);
                }
                else if (repositoryResult.HasErrors)
                {
                    return Ok(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BulkRegistrationApiResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = new BulkRegistrationResult
                    {
                        TotalPlants = plantDtos.Count,
                        FailedRegistrations =  plantDtos.Count,
                        Message = $"Server error: {ex.Message}"
                    }
                });
            }
        }
    }
}

public class BulkRegistrationApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public BulkRegistrationResult Data { get; set; }
}
public class BulkRegistrationResult
{
    public int TotalPlants { get; set; }
    public int SuccessfulRegistrations { get; set; }
    public int FailedRegistrations { get; set; }
    public string Message { get; set; }
}

