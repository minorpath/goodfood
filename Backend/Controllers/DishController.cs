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

        [HttpGet]
        [Route("", Name = "GetAllDishes")]
        public async Task<IEnumerable<Dish>> Get()
        {
            var entities = await _storage.GetAllAsync();
            return entities.Select(e => e.ToDto());
        }

        [HttpGet]
        [Route("{id}", Name = "GetDishById")]
        public async Task<IActionResult> Get(string id)
        {
            var entity = await _storage.GetAsync(id);
            return entity != null ? Ok(entity.ToDto()) : NotFound();
        }

        [HttpPost]
        [Route("", Name = "PostNewDish")]
        public async Task Post([FromBody]Dish dish)
        {
            var dishEntity = dish.ToEntity();
            await _storage.InsertAsync(dishEntity);
        }
    }
}
