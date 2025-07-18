using System.ComponentModel.DataAnnotations;

namespace API.DTO.GraphQL;

public class CreateProductGraphQLDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(100, double.PositiveInfinity)]
        public long Price { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Range(0, 200)]
        public int QuantityInStock { get; set; }
    }
