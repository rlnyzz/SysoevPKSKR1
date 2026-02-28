using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyspevPKSKR1.Models
{
    [Table("author")]
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_author")]
        public int Id_author { get; set; }

        [Column("firstname")]
        [StringLength(255)]
        public string Firstname { get; set; }

        [Column("lastname")]
        [StringLength(255)]
        public string Lastname { get; set; }

        [Column("birthdate")]
        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        [Column("country")]
        [StringLength(255)]
        public string Country { get; set; }

        
        public ICollection<Book> Books { get; set; }
    }
}
