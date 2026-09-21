using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("usuarios")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        // NOVO CAMPO EMAIL
        [Column("email")]
        public string Email { get; set; }

        // senha (por enquanto em texto, depois pode virar hash real)
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        // número de tentativas erradas
        [Column("failed_attempts")]
        public int FailedAttempts { get; set; }

        // indica se a conta está bloqueada
        [Column("is_blocked")]
        public bool IsBlocked { get; set; }
    }
}