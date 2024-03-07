using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        [MaxLength(200)]
        public string? Description { get; set; }

        [ForeignKey("CityId")]

        public City? City { get; set; } //navigation property

        public int CityId { get; set; } //not necessary to spacify this property in dependent class but recommended

        public PointOfInterest(string name) {
            Name = name;
        }
    }
}
