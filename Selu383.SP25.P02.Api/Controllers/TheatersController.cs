using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Theaters;
using Selu383.SP25.P02.Api.Models;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/theaters")]
    [ApiController]
    public class TheatersController : ControllerBase
    {
        private readonly DbSet<Features.Theaters.Theater> theaters;
        private readonly DataContext dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TheatersController(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            this.dataContext = dataContext;
            theaters = dataContext.Set<Features.Theaters.Theater>();
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<TheaterDto>>> GetAllTheaters()
        {
            var theaterList = await theaters.ToListAsync();
            return Ok(theaterList.Select(t => MapToDto(t)).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TheaterDto>> GetTheaterById(int id)
        {
            var theater = await theaters.FindAsync(id);
            if (theater == null)
            {
                return NotFound();
            }
            return Ok(MapToDto(theater));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<TheaterDto>> CreateTheater(TheaterDto dto, Features.Theaters.Theater theater)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            var manager = !dto.ManagerId.HasValue ? null : await _userManager.FindByIdAsync(dto.ManagerId.ToString());
            if (dto.ManagerId.HasValue && manager == null)
            {
                return BadRequest("Invalid ManagerId.");
            }

            var theater = new Features.Theaters.Theater
            {
                Name = dto.Name,
                Address = dto.Address,
                SeatCount = dto.SeatCount,
                ManagerId = dto.ManagerId
            };

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Models.Theater> entityEntry = theaters.Add(theater);
            await dataContext.SaveChangesAsync();

            dto.Id = theater.Id;
            return CreatedAtAction(nameof(GetTheaterById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        [Authorize] 
        public async Task<ActionResult<TheaterDto>> UpdateTheater(int id, TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            var theater = await theaters.FindAsync(id);
            if (theater == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (!isAdmin && currentUser.Id != theater.ManagerId)
            {
                return Forbid();
            }

            if (dto.ManagerId.HasValue)
            {
                var newManager = await _userManager.FindByIdAsync(dto.ManagerId.ToString());
                if (newManager == null)
                {
                    return BadRequest("Invalid ManagerId.");
                }

                if (!isAdmin)
                {
                    return Forbid(); 
                }
            }

            theater.Name = dto.Name;
            theater.Address = dto.Address;
            theater.SeatCount = dto.SeatCount;
            theater.ManagerId = dto.ManagerId;

            await dataContext.SaveChangesAsync();

            return Ok(MapToDto(theater));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult> DeleteTheater(int id)
        {
            var theater = await theaters.FindAsync(id);
            if (theater == null)
            {
                return NotFound();
            }

            theaters.Remove(theater);
            await dataContext.SaveChangesAsync();
            return Ok();
        }

        private static bool IsInvalid(TheaterDto dto)
        {
            return string.IsNullOrWhiteSpace(dto.Name) ||
                   dto.Name.Length > 120 ||
                   string.IsNullOrWhiteSpace(dto.Address) ||
                   dto.SeatCount <= 0;
        }

        private static TheaterDto MapToDto(Features.Theaters.Theater theater)
        {
            return new TheaterDto
            {
                Id = theater.Id,
                Name = theater.Name,
                Address = theater.Address,
                SeatCount = theater.SeatCount,
                ManagerId = theater.ManagerId
            };
        }
    }
}
