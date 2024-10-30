using System;
using System.Collections.Generic;

namespace TypicalTypistAPI.Models;

public partial class UserBigraphStat
{
    public long BigraphStatId { get; set; }

    public int UserId { get; set; }

    public string StartingKey { get; set; } = null!;

    public string Bigraph { get; set; } = null!;

    public int? TotalTyped { get; set; }

    public decimal? Accuracy { get; set; }

    public decimal? Speed { get; set; }

    public virtual User User { get; set; } = null!;
}
