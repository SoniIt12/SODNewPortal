using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOD.CommonWebMethod
{
    public static class OKToBoard
    {

        /// <summary>
        /// Check Is OK to Board
        /// </summary>
        /// <param name="TravelRequestTypeId"></param>
        /// <param name="sodflightList"></param>
        /// <returns></returns>
        public static bool CheckIsOTB(int TravelRequestTypeId, List<FlightDetailModels> sodflightList)
        {
               var status = false;
               if (TravelRequestTypeId.Equals(1) || TravelRequestTypeId.Equals(2))
                {
                    var bookingtype = TravelRequestTypeId.Equals(1) ? "oneway" : "roundtrip";
                    if (sodflightList.Count == 1)
                        bookingtype = "oneway";
                   
                   status = CommonWebMethods.IsOktoBoardRequired(sodflightList[0].DestinationPlace.Trim(), bookingtype);
                }
                else
                {
                    var bookingtype = "multicity";
                    if (sodflightList.Count == 1) bookingtype = "oneway";
                    else if (sodflightList.Count == 2) bookingtype = "roundtrip";

                    foreach (var flight in sodflightList)
                    {
                        if (CommonWebMethods.IsOktoBoardRequired_forMulticitySectors(flight.OriginPlace.Trim(), flight.DestinationPlace.Trim(), bookingtype))
                        {
                            status = true;
                            break;
                        }
                    }
                }
               return status;
         }
    }
}