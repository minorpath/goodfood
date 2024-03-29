﻿using HeinjoFood.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace HeinjoFood.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishController : ControllerBase
    {
        private readonly StorageManager _storage;

        public DishController(StorageManager storage)
        {
            _storage = storage;
        }

        [HttpGet]
        [Route("search", Name = "SearchDishesByTag")]
        public async Task<IEnumerable<Dish>> SearchByTag([FromQuery]string tags)
        {
            var entities = await _storage.SearchAsync(tags);
            return entities.Select(e => e.ToDto());
        }

        [HttpGet]
        [Route("", Name = "GetAllDishes")]
        public async Task<IEnumerable<Dish>> Get()
        {
            var entities = await _storage.GetAllAsync();
            return entities.Select(e => e.ToDto());
        }

        [HttpGet]
        [Route("{id}", Name = "GetDishById")]
        [ProducesResponseType(typeof(Dish), 200)]
        public async Task<IActionResult> Get(string id)
        {
            var entity = await _storage.GetAsync(id);
            return entity != null ? Ok(entity.ToDto()) : NotFound();
        }

        /// <summary>
        /// Adds new Dish
        /// </summary>
        [HttpPost]
        [Route("", Name = "PostNewDish")]
        [ProducesResponseType(typeof(Dish), 201)]
        public async Task<IActionResult> Post([FromBody]Dish dish)
        {
            var dishEntity = dish.ToEntity();
            var createdEntity = await _storage.InsertAsync(dishEntity);
            var createdDto = createdEntity.ToDto();
            return CreatedAtRoute("GetDishById", new { id = createdDto.Id }, createdDto);
        }


        [HttpDelete]
        [Route("{id}", Name = "DeleteDishById")]
        public async Task<IActionResult> Delete(string id)
        {
            var dishEntity = await _storage.GetAsync(id);
            if( dishEntity == null) return NotFound();
            await _storage.DeleteAsync(dishEntity);
            return Ok();
        }
    }
}
