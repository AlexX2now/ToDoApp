using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    //Сущность, с которой и производится работа
    public class TodoItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        public bool? IsCompleted { get; set; } = false;
        [Required]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
