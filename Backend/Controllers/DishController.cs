using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
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

        // GET api/dish
        [HttpGet]
        public async Task<IEnumerable<Dish>> Get()
        {
            var entities = await _storage.GetAllAsync();
            return entities.Select(e => e.ToDto());
        }


        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]Dish dish)
        {
            var dishEntity = dish.ToEntity();
            await _storage.InsertAsync(dishEntity);
        }
    }
}
