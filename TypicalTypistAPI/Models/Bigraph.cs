using System;
using System.Collections.Generic;

namespace TypicalTypistAPI.Models;

public partial class Bigraph
{
    public int BigraphId { get; set; }

    public string Bigraph1 { get; set; } = null!;

    public int? WordId { get; set; }

    public virtual Word? Word { get; set; }
}
