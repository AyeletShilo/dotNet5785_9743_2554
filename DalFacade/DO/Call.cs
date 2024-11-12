
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DO;

public record Call
(
   int Id,//?//
   Type callType,
   string callAddress,
   double Latitude,
   double Longitude,
   DateTime OpenTime,
   string? description=null,
   DateTime? MaxTime=null
)
{
    public Call() : this(0, Type.a/*?*/, "", 0, 0, DateTime.MinValue) { }
}
