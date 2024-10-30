using System;
using System.Collections.Generic;

namespace TypicalTypistAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public DateTime Joined { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? ImageId { get; set; }

    public bool? Active { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<UserBigraphStat> UserBigraphStats { get; set; } = new List<UserBigraphStat>();

    public virtual ICollection<UserKeyStat> UserKeyStats { get; set; } = new List<UserKeyStat>();

    public virtual ICollection<UserStat> UserStats { get; set; } = new List<UserStat>();

    public virtual ICollection<UserTypingTest> UserTypingTests { get; set; } = new List<UserTypingTest>();
}
