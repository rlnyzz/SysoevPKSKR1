using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyspevPKSKR1.Models
{
    [Table("book")] 
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] 
        public int Id { get; set; }

        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; }

        [Column("author")] 
        [StringLength(255)]
        public string Author { get; set; }

        [Column("publishyear")]
        public int? Publishyear { get; set; }

        [Column("isbn")]
        public int? Isbn { get; set; }

        [Column("genre")]  
        [StringLength(255)]
        public string Genre { get; set; }

        [Column("quantityinstock")]
        public int? Quantityinstock { get; set; }

        
        [Column("author_id")]  
        public int? AuthorId { get; set; }

        [Column("genre_id")]  
        public int? GenreId { get; set; }

      
        [ForeignKey("AuthorId")]
        public Author AuthorNavigation { get; set; }

        [ForeignKey("GenreId")]
        public Genre GenreNavigation { get; set; }
    }
}