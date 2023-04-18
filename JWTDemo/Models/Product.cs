using System.ComponentModel.DataAnnotations;

namespace JWTDemo.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int quantity { get; set; }


    }
}
