﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Theaters;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/theaters")]
    [ApiController]
    public class TheatersController : ControllerBase
    {
        private readonly DbSet<Theater> theaters;
        private readonly DataContext dataContext;

        public TheatersController(DataContext dataContext)
        {
            this.dataContext = dataContext;
            theaters = dataContext.Set<Theater>();
        }

        [HttpGet]
        public IQueryable<TheaterDto> GetAllTheaters()
        {
            return GetTheaterDtos(theaters);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<TheaterDto> GetTheaterById(int id)
        {
            var result = GetTheaterDtos(theaters.Where(x => x.Id == id)).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public ActionResult<TheaterDto> CreateTheater(TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            var theater = new Theater
            {
                Name = dto.Name,
                Address = dto.Address,
                SeatCount = dto.SeatCount,
            };
            theaters.Add(theater);

            dataContext.SaveChanges();

            dto.Id = theater.Id;

            return CreatedAtAction(nameof(GetTheaterById), new { id = dto.Id }, dto);
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<TheaterDto> UpdateTheater(int id, TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            var theater = theaters.FirstOrDefault(x => x.Id == id);
            if (theater == null)
            {
                return NotFound();
            }

            theater.Name = dto.Name;
            theater.Address = dto.Address;
            theater.SeatCount = dto.SeatCount;

            dataContext.SaveChanges();

            dto.Id = theater.Id;

            return Ok(dto);
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteTheater(int id)
        {
            var theater = theaters.FirstOrDefault(x => x.Id == id);
            if (theater == null)
            {
                return NotFound();
            }

            theaters.Remove(theater);

            dataContext.SaveChanges();

            return Ok();
        }

        private static bool IsInvalid(TheaterDto dto)
        {
            return string.IsNullOrWhiteSpace(dto.Name) ||
                   dto.Name.Length > 120 ||
                   string.IsNullOrWhiteSpace(dto.Address) ||
                   dto.SeatCount <= 0;
        }

        private static IQueryable<TheaterDto> GetTheaterDtos(IQueryable<Theater> theaters)
        {
            return theaters
                .Select(x => new TheaterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    SeatCount = x.SeatCount,
                });
        }
    }
}