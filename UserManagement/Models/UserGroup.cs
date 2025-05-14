using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class UserGroup
    {
        [Key]
        public int ID { get; set; }
        public string Name_TH { get; set; }
        public string Name_EN { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}
