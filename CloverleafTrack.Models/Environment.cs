using System.ComponentModel;

namespace CloverleafTrack.Models;

[Flags]
public enum Environment
{
    None = 0,
    Outdoor = 1,
    Indoor = 2
}
