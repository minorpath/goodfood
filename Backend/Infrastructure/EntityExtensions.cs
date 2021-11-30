using System;

namespace Backend.Data
{
    public static class EntityExtensions
    {

        public static DishEntity ToEntity(this Dish dish) => new("lye", null)
        {
            Description = dish.Description,
            Comment = dish.Comment,
            ImageUrl = dish.ImageUrl,
            Urls = (dish.Urls != null) ? string.Join(";", dish.Urls) : null,
            Tags = (dish.Tags != null) ? string.Join(";", dish.Tags) : null
        };

        public static Dish ToDto(this DishEntity dish) => new()
        {
            Id = dish.RowKey,
            Description = dish.Description,
            Comment = dish.Comment,
            ImageUrl = dish.ImageUrl,
            Urls = dish.Urls?.Split(';', StringSplitOptions.RemoveEmptyEntries),
            Tags = dish.Tags?.Split(';', StringSplitOptions.RemoveEmptyEntries)
        };
    }
}
