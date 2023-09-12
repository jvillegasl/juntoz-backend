using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderStoreApi.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderNumber { get; set; }

        [Required(ErrorMessage = "Client DNI is required.")]
        public string ClientDNI { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = null!;
    }


    public class NewOrder
    {
        [Required(ErrorMessage = "Client DNI is required.")]
        public string ClientDNI { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = null!;
    }
}
