using System;
using System.Collections.Generic;

namespace BachBinHoangManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Role { get; set; }

    public int Status { get; set; }

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpired { get; set; }

    public string? VerifyToken { get; set; }

    public DateTime? VerifyTokenExpired { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? LastLogin { get; set; }
}
