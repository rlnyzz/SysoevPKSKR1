using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyspevPKSKR1.Models
{
    [Table("genre")]
    public class Genre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_genre")]
        public int Id_genre { get; set; }

        [Column("name_genre")]
        [StringLength(255)]
        public string Name_genre { get; set; }

        [Column("description_genre")]
        [StringLength(255)]
        public string Description_genre { get; set; }

       
        public ICollection<Book> Books { get; set; }

        [NotMapped]
        public string Name => Name_genre?.Trim();
        
        
        [NotMapped]
        public string ShortDescription => Description_genre?.Length > 50 
            ? Description_genre.Substring(0, 47) + "..." 
            : Description_genre;
    }
}