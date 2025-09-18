using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CraneCalc.Domain.Enums;

namespace CraneCalc.Infrastructure.Entities;

public class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.User;
}