using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key] // primary key annotation
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // new identity is created when city is created
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterests { get; set; } = new List<PointOfInterest>();


        public City(string name)
        {
            Name = name;
        }
    }
}
