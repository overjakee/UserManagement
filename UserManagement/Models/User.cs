using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="กรุณากรอกชื่อผู้ใช้ระบบ")]
        [Range(0,100,ErrorMessage ="กรุอกไม่เกิน 100 ตัวอักษร")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "สามารถกรอกได้เฉพาะ a-z A-Z 0-9")]
        public string? UserName { get; set; }

        [Required]
        [Range(0, 300)]
        public string? Email { get; set; }

        [Range(0, 100)]
        public string? Name_TH {  get; set; }

        [Range(0, 100)]
        public string? Surname_TH { get; set; }

        [Range(0, 100)]
        public string? Name_EN { get; set; }

        [Range(0, 100)]
        public string? Surname_EN { get; set; }

        [Range(0, 300)]
        public string? Password {  get; set; }

        [Range(0, 300)]
        public string? SaltKey { get; set; }

        public int? GroupId { get; set; }
        [Required]
        public DateTime EffectiveStartDate { get; set; }
        [Required]
        public DateTime EffectiveFinishDate { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}
