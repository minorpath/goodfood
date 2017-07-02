namespace Backend
{
    public class Dish
    {
        public string Id { get; set; }
        public string[] Tags { get; set; }
        public string ImageUrl { get; set; }
        public string[] Urls { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
    }
}