using System;
using System.Collections.Generic;

namespace TypicalTypistAPI.Models;

public partial class UserTypingTest
{
    public int TestId { get; set; }

    public int UserId { get; set; }

    public DateTime TestDate { get; set; }

    public int? CharCount { get; set; }

    public int? IncorrectCount { get; set; }

    public string? Mode { get; set; }

    public decimal? Wpm { get; set; }

    public decimal? Accuracy { get; set; }

    public virtual User User { get; set; } = null!;
}
