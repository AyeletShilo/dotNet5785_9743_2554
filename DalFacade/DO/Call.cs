
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

}
