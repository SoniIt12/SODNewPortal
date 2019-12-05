using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using SOD.BookingManager;
using SOD.Model;
using SOD.SessionManager;
using System.Xml.Serialization;
using System.IO;
using SOD.Logging;
using System.Configuration;
using System.Linq;

namespace SOD.CommonWebMethod
{
    ////******** Navitaire API Integration 4X************************
    public class NavitaireServices
    {
        //Global Variable Declaration for the Class
        public static string mDept = ConfigurationManager.AppSettings["Flight_Safety"].ToString().ToLower();
       
        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns></returns>
        public static string Login()
        {
            ISessionManager sessionManager = new SessionManagerClient();
            LogonRequest logonRequest = new LogonRequest();
            string signature = "";
            string username = ConfigurationManager.AppSettings["userid_naviapi"].ToString();
            string password = ConfigurationManager.AppSettings["password_naviapi"].ToString();
            string domain = "WWW";
            try
            {
                logonRequest.logonRequestData = new LogonRequestData();
                logonRequest.logonRequestData.DomainCode = domain;
                logonRequest.logonRequestData.AgentName = username;
                logonRequest.logonRequestData.Password = password;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                LogonResponse logonResponse = sessionManager.Logon(logonRequest);

                if (logonResponse != null && logonResponse.Signature != null && logonResponse.Signature != string.Empty)
                {
                    signature = logonResponse.Signature;
                }
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, username + "_Login", "NavitaireServoces.cs");
            }
            return signature;
        }
       
        /// <summary>
        /// Logout method
        /// </summary>
        /// <param name="signature"></param>
        public static void Logout(string signature)
        {
            ISessionManager sessionManager = new SessionManagerClient();
            LogoutRequest logoutRequest = new LogoutRequest();
            logoutRequest.Signature = signature;
            sessionManager.Logout(logoutRequest);
        }
       

        /// <summary>
        /// To check Availability of flight
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="ddate"></param>
        /// <param name="returndate"></param>
        /// <param name="waytype"></param>
        /// <param name="noofPassenger"></param>
        /// <returns></returns>
        public static string GetAvailability1(string source, string destination, string ddate, string returndate, string waytype, string noofPassenger,string bookingfor)
        {
                IBookingManager bookingManager = new BookingManagerClient();
                string signature = Login();
                GetAvailabilityRequest gar = new GetAvailabilityRequest();
                TripAvailabilityRequest tar = new TripAvailabilityRequest();
                AvailabilityRequest ar = new AvailabilityRequest();
                ar.DepartureStation = source; 
                ar.ArrivalStation = destination; 
                string[] date = ddate.Split('/'); 
                int year = int.Parse(date[2]), month = int.Parse(date[1]), day = int.Parse(date[0]);

                ar.BeginDate = new DateTime(year, month, day);
                //ar.BeginDateSpecified = true;
                ar.EndDate = new DateTime(year, month, day);
                //ar.EndDateSpecified = true;
                ar.FlightType = FlightType.All;
                 
                //ar.FlightTypeSpecified = true;
                int pax = Convert.ToInt16(noofPassenger);
                ar.PaxCount = Convert.ToSByte(pax);
                //ar.PaxCountSpecified = true;
                ar.Dow = DOW.Daily;
                //ar.DowSpecified = true;
                ar.CurrencyCode = "INR";
                ar.AvailabilityType = bookingfor.Equals("Confirm") ? AvailabilityType.Overbook : AvailabilityType.Standby; 
                ar.MaximumConnectingFlights = 5;
                ar.AvailabilityFilter = AvailabilityFilter.ExcludeUnavailable;
                ar.FareClassControl = FareClassControl.LowestFareClass;
                ar.MinimumFarePrice = 0;
                ar.MaximumFarePrice = 0;
                ar.SSRCollectionsMode = SSRCollectionsMode.None;
                ar.InboundOutbound = InboundOutbound.None;
                ar.NightsStay = 0;
                ar.IncludeAllotments = false;
                ar.FareTypes = new string[1]; 
                ar.FareTypes[0] = "Z";

                PaxPriceType[] priceTypes = new PaxPriceType[ar.PaxCount];
                for (int i = 0; i < ar.PaxCount; i++)
                {
                    priceTypes[i] = new PaxPriceType();
                    priceTypes[i].PaxType = "ADT";
                    priceTypes[i].PaxDiscountCode = String.Empty;
                }
                ar.PaxPriceTypes = priceTypes;
                ar.IncludeTaxesAndFees = false;
                ar.FareRuleFilter = FareRuleFilter.Default;
                ar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                if (!string.IsNullOrEmpty(returndate))// for round trip
                {
                    AvailabilityRequest rar = new AvailabilityRequest();
                    rar.DepartureStation = destination; 
                    rar.ArrivalStation = source; 
                    string[] rdate = returndate.Split('/'); 
                    int ryear = int.Parse(rdate[2]), rmonth = int.Parse(rdate[1]), rday = int.Parse(rdate[0]);
                    rar.BeginDate = new DateTime(ryear, rmonth, rday);  
                    rar.EndDate = new DateTime(ryear, rmonth, rday);
                    rar.FlightType = FlightType.All;  
                    int rpax = Convert.ToInt16(noofPassenger); 
                    rar.PaxCount = Convert.ToSByte(rpax);
                    rar.Dow = DOW.Daily;
                    rar.CurrencyCode = "INR";
                    rar.AvailabilityType = bookingfor.Equals("Confirm") ? AvailabilityType.Overbook : AvailabilityType.Standby; 
                    //rar.AvailabilityType = AvailabilityType.Standby; 
                    rar.MaximumConnectingFlights = 5;
                    rar.AvailabilityFilter = AvailabilityFilter.ExcludeUnavailable;
                    rar.FareClassControl = FareClassControl.LowestFareClass;
                    rar.MinimumFarePrice = 0;
                    rar.MaximumFarePrice = 0;
                    rar.SSRCollectionsMode = SSRCollectionsMode.None;
                    rar.InboundOutbound = InboundOutbound.None;
                    rar.NightsStay = 0;
                    rar.IncludeAllotments = false;
                    rar.FareTypes = new string[1];
                    rar.FareTypes[0] = "Z";

                    PaxPriceType[] rpriceTypes = new PaxPriceType[ar.PaxCount];
                    for (int i = 0; i < ar.PaxCount; i++)
                    {
                        rpriceTypes[i] = new PaxPriceType();
                        rpriceTypes[i].PaxType = "ADT";
                        rpriceTypes[i].PaxDiscountCode = String.Empty;
                    }
                    rar.PaxPriceTypes = rpriceTypes;
                    rar.IncludeTaxesAndFees = false;
                    rar.FareRuleFilter = FareRuleFilter.Default;
                    rar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                    tar.AvailabilityRequests = new AvailabilityRequest[2];
                    tar.AvailabilityRequests[0] = ar;
                    tar.AvailabilityRequests[1] = rar;
                }
                else
                {
                    tar.AvailabilityRequests = new AvailabilityRequest[1];
                    tar.AvailabilityRequests[0] = ar;
                }
                tar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                gar.TripAvailabilityRequest = tar;
                gar.Signature = signature;
                gar.ContractVersion = 0;
                SaveXML(gar, "GetAvailabilityRequest.xml");
                GetAvailabilityResponse gare = bookingManager.GetAvailability(gar);
                SaveXML(gare, "GetAvailabilityResponse.xml");
                return GetPrice(signature, gare, noofPassenger);
        }
       

        /// <summary>
       /// Get Price for Flight
       /// </summary>
       /// <param name="signature"></param>
       /// <param name="response"></param>
       /// <param name="noofPassenger"></param>
       /// <returns></returns>
        public static string GetPrice(string signature, GetAvailabilityResponse response, string noofPassenger)
        {
                DataSet ds = new DataSet();
                int counter = 0;
                foreach (JourneyDateMarket[] jdmArray in response.GetTripAvailabilityResponse.Schedules)
                {
                    DataTable flights = Table_Flights();
                    DataTable fare = Table_Fare();
                    foreach (JourneyDateMarket jdm in jdmArray)
                    {
                        foreach (Journey journey in jdm.Journeys)
                        {
                            /*Satyam 29 July 2016 : to Manage  connected Flights */
                            if (journey.Segments.Length > 1)
                            {
                                if (journey.Segments[0].Fares.Length != 0)
                                {
                                    DataRow dr = flights.NewRow();
                                    string fcode = journey.Segments[0].FlightDesignator.CarrierCode;
                                    dr["ShortDate"] = journey.Segments[0].STD.ToString("ddd, MMM d yyyy");
                                    dr["Date"] = journey.Segments[0].STD.ToShortDateString();
                                    dr["From"] = journey.Segments[0].DepartureStation;
                                    dr["To"] = journey.Segments[journey.Segments.Length - 1].ArrivalStation;
                                    //dr["FlightNo"] = journey.Segments[0].FlightDesignator.FlightNumber + " ," + fcode+" "+journey.Segments[journey.Segments.Length - 1].FlightDesignator.FlightNumber;
                                    dr["STD"] = journey.Segments[0].STD.ToString("dd/MM/yyy hh:mm tt");
                                    dr["STA"] = journey.Segments[journey.Segments.Length - 1].STA.ToString("dd/MM/yyy hh:mm tt");
                                    //dr["BaseFare"] = segment.Fares[0].PaxFares[0].ServiceCharges[0].Amount;
                                    dr["TimeDuration"] = TimeDuration1(journey.Segments[0].STD.ToString(), journey.Segments[journey.Segments.Length - 1].STA.ToString(), journey.Segments[0].DepartureStation, journey.Segments[journey.Segments.Length - 1].ArrivalStation);
                                    dr["FlightTypes"] = 3;
                                    string[] arr = GetEquipment_Flight(journey.Segments, signature).ToString().Split('@');
                                    dr["FlightInfos"] = arr[0];
                                    dr["FlightNo"] = arr[1];
                                    dr["FlightName"] = arr[2];
                                    flights.Rows.Add(dr);
                                }
                            }
                            /*End of Satyam Code  */
                            else
                            {
                                foreach (Segment segment in journey.Segments)
                                {
                                    if (segment.Fares.Length != 0)
                                    {
                                        DataRow dr = flights.NewRow();
                                        dr["ShortDate"] = segment.STD.ToString("ddd, MMM d yyyy");
                                        dr["Date"] = segment.STD.ToShortDateString();
                                        dr["From"] = segment.DepartureStation;
                                        dr["To"] = segment.ArrivalStation;
                                        //dr["FlightNo"] = segment.FlightDesignator.FlightNumber;
                                        dr["STD"] = segment.STD.ToString("dd/MM/yyy hh:mm tt");
                                        dr["STA"] = segment.STA.ToString("dd/MM/yyy hh:mm tt");
                                        //dr["BaseFare"] = segment.Fares[0].PaxFares[0].ServiceCharges[0].Amount;
                                        dr["TimeDuration"] = TimeDuration1(segment.STD.ToString(), segment.STA.ToString(),segment.DepartureStation,segment.ArrivalStation);
                                        if (journey.Segments[0].Legs.Length > 1)
                                        {
                                            dr["FlightTypes"] = 2;
                                        }
                                        else
                                        {
                                            dr["FlightTypes"] = 1;
                                        }
                                        string[] arr = GetEquipment_Flight(journey.Segments, signature).ToString().Split('@');
                                        dr["FlightInfos"] = arr[0];
                                        dr["FlightNo"] = arr[1];
                                        dr["FlightName"] = arr[2];
                                        flights.Rows.Add(dr);
                                    }
                                }
                            }
                        }
                    }
                    if (counter == 0)
                    {
                        HttpContext.Current.Session["flights"] = flights.DefaultView.ToTable();
                        ds.Tables.Add(flights);
                        ds.Tables[0].TableName = "flights";
                    }
                    else
                    {
                        HttpContext.Current.Session["Rflights"] = flights.DefaultView.ToTable();
                        ds.Tables.Add(flights);
                        ds.Tables[1].TableName = "Rflights";
                    }
                    counter++;
                }
                return ds.GetXml().ToString();
        }


        /// <summary>
        /// Calculate Total fare
        /// </summary>
        /// <param name="fare"></param>
        /// <param name="flightno"></param>
        /// <returns></returns>
        public static double TotalFare(DataTable fare, string flightno)
        {
            return Convert.ToDouble(fare.Compute("Sum(Amount)", "FlightNo=" + flightno));
        }
        

        /// <summary>
        /// Calculate Time duration
        /// </summary>
        /// <param name="D1"></param>
        /// <param name="D2"></param>
        /// <returns></returns>
        public static string TimeDuration(string D1, string D2)
        {
            DateTime d1 = Convert.ToDateTime(D1);
            DateTime d2 = Convert.ToDateTime(D2);
            TimeSpan difference = d2 - d1;
            string time = difference.Hours.ToString("00") + "h " + difference.Minutes.ToString("00") + "m";
            return time;
        }

        public static string TimeDuration1(string D1, string D2,string sector1, string sector2)
        {
            DateTime d1 = CommonWebMethod.CommonWebMethods.GetInternationTimeDurationUTC(sector1, Convert.ToDateTime(D1));
            DateTime d2 = CommonWebMethod.CommonWebMethods.GetInternationTimeDurationUTC(sector2, Convert.ToDateTime(D2));  
            TimeSpan difference = d2 - d1;
            string time = difference.Hours.ToString("00") + "h " + difference.Minutes.ToString("00") + "m";
            return time;
        }

        /// <summary>
        /// Add Flights in to datatable
       /// </summary>
       /// <returns></returns>
        public static DataTable Table_Flights()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ShortDate", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("From", typeof(string));
            dt.Columns.Add("To", typeof(string));
            dt.Columns.Add("FlightNo", typeof(string));
            dt.Columns.Add("STD", typeof(string));
            dt.Columns.Add("STA", typeof(string));
            dt.Columns.Add("BaseFare", typeof(decimal));
            dt.Columns.Add("TotalFare", typeof(double));
            dt.Columns.Add("TimeDuration", typeof(string));
            dt.Columns.Add("FlightTypes", typeof(int));
            dt.Columns.Add("FlightInfos", typeof(string));
            dt.Columns.Add("FlightName", typeof(string)); 
            return dt;
        }


        /// <summary>
        /// Add Flight fare into datatable
        /// </summary>
        /// <returns></returns>
        public static DataTable Table_Fare()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FlightNo", typeof(string));
            dt.Columns.Add("Heads", typeof(string));
            dt.Columns.Add("Amount", typeof(decimal));
            return dt;
        }

        /// <summary>
        /// Get Multicity Flight Info
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetAvailabilityMulticity(List<TravelRequestModels> list)
        {
            try
            {
                IBookingManager bookingManager = new BookingManagerClient();
                string signature = Login();
                GetAvailabilityRequest gar = new GetAvailabilityRequest();
                TripAvailabilityRequest tar = new TripAvailabilityRequest();
                tar.AvailabilityRequests = new AvailabilityRequest[list.Count];

                int counter = 0;
                foreach (var item in list)
                {
                    AvailabilityRequest ar = new AvailabilityRequest();
                    ar.DepartureStation = item.OriginPlace;
                    ar.ArrivalStation = item.DestinationPlace;
                    string[] date = item.TravelDate.Split('/');
                    int year = int.Parse(date[2]), month = int.Parse(date[1]), day = int.Parse(date[0]);

                    ar.BeginDate = new DateTime(year, month, day);
                    ar.EndDate = new DateTime(year, month, day);
                    ar.FlightType = FlightType.All;
                    int pax = Convert.ToInt16(item.Passengers);
                    ar.PaxCount = Convert.ToSByte(pax);
                    ar.Dow = DOW.Daily;
                    ar.CurrencyCode = "INR";
                    ar.AvailabilityType = item.BookingFor.Equals("Confirm") ? AvailabilityType.Overbook : AvailabilityType.Standby; 
                    ar.MaximumConnectingFlights = 5;
                    ar.AvailabilityFilter = AvailabilityFilter.ExcludeUnavailable;
                    ar.FareClassControl = FareClassControl.LowestFareClass;
                    ar.MinimumFarePrice = 0;
                    ar.MaximumFarePrice = 0;
                    ar.SSRCollectionsMode = SSRCollectionsMode.None;
                    ar.InboundOutbound = InboundOutbound.None;
                    ar.NightsStay = 0;
                    ar.IncludeAllotments = false;
                    ar.FareTypes = new string[1]; 
                    ar.FareTypes[0] = "Z";
                  
                    PaxPriceType[] priceTypes = new PaxPriceType[ar.PaxCount];
                    for (int i = 0; i < ar.PaxCount; i++)
                    {
                        priceTypes[i] = new PaxPriceType();
                        priceTypes[i].PaxType = "ADT";
                        priceTypes[i].PaxDiscountCode = String.Empty;
                    }
                    ar.PaxPriceTypes = priceTypes;
                    ar.IncludeTaxesAndFees = false;
                    ar.FareRuleFilter = FareRuleFilter.Default;
                    ar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                    tar.AvailabilityRequests[counter] = ar;
                    counter++;
                }

                
                tar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                gar.TripAvailabilityRequest = tar;
                gar.Signature = signature;
                gar.ContractVersion = 0;
                GetAvailabilityResponse gare = bookingManager.GetAvailability(gar);
                return GetPriceMulticity(signature, gare, list[0].Passengers);
            }
            catch(Exception ex)
            {
                Logging.ErrorLog.AddEmailLogg(ex);
                throw;
            }
        }

        
        /// <summary>
        /// Get Price for Multicity
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="noofPassenger"></param>
        /// <returns></returns>
        public static string GetPriceMulticity(string signature, GetAvailabilityResponse response, string noofPassenger)
        {
            
                DataSet ds = new DataSet();
                int counter = 0;
                foreach (JourneyDateMarket[] jdmArray in response.GetTripAvailabilityResponse.Schedules)
                {
                    DataTable flights = Table_Flights();
                    DataTable fare = Table_Fare();
                    foreach (JourneyDateMarket jdm in jdmArray)
                    {
                        foreach (Journey journey in jdm.Journeys)
                        {
                            if (journey.Segments.Length > 1)
                            {
                                DataRow dr = flights.NewRow();
                                string fcode = journey.Segments[0].FlightDesignator.CarrierCode;
                                dr["ShortDate"] = journey.Segments[0].STD.ToString("ddd, MMM d yyyy");
                                dr["Date"] = journey.Segments[0].STD.ToShortDateString();
                                dr["From"] = journey.Segments[0].DepartureStation;
                                dr["To"] = journey.Segments[journey.Segments.Length - 1].ArrivalStation;
                                dr["STD"] = journey.Segments[0].STD.ToString("dd/MM/yyy hh:mm tt");
                                dr["STA"] = journey.Segments[journey.Segments.Length - 1].STA.ToString("dd/MM/yyy hh:mm tt");
                                dr["TimeDuration"] = TimeDuration1(journey.Segments[0].STD.ToString(), journey.Segments[journey.Segments.Length - 1].STA.ToString(), journey.Segments[0].DepartureStation, journey.Segments[journey.Segments.Length - 1].ArrivalStation);
                                dr["FlightTypes"] = 3;
                                string[] arr = GetEquipment_Flight(journey.Segments, signature).ToString().Split('@');
                                dr["FlightInfos"] = arr[0];
                                dr["FlightNo"] = arr[1];
                                dr["FlightName"] = arr[2];
                                flights.Rows.Add(dr);
                            }
                            else
                            {
                                foreach (Segment segment in journey.Segments)
                                {
                                    DataRow dr = flights.NewRow();
                                    dr["ShortDate"] = segment.STD.ToString("ddd, MMM d yyyy");
                                    dr["Date"] = segment.STD.ToShortDateString();
                                    dr["From"] = segment.DepartureStation;
                                    dr["To"] = segment.ArrivalStation;
                                    dr["STD"] = segment.STD.ToString("dd/MM/yyy hh:mm tt");
                                    dr["STA"] = segment.STA.ToString("dd/MM/yyy hh:mm tt");
                                    dr["TimeDuration"] = TimeDuration1(segment.STD.ToString(), segment.STA.ToString(),segment.DepartureStation,segment.ArrivalStation);
                                    if (journey.Segments[0].Legs.Length > 1)
                                    {
                                        dr["FlightTypes"] = 2;
                                    }
                                    else
                                    {
                                        dr["FlightTypes"] = 1;
                                    }
                                    string[] arr = GetEquipment_Flight(journey.Segments, signature).ToString().Split('@');
                                    dr["FlightInfos"] = arr[0];
                                    dr["FlightNo"] = arr[1];
                                    dr["FlightName"] = arr[2];
                                    flights.Rows.Add(dr);
                                }
                            }
                        }
                    }

                    HttpContext.Current.Session["flights"] = flights.DefaultView.ToTable();
                    ds.Tables.Add(flights);
                    ds.Tables[counter].TableName = "flights" + (counter + 1);
                    counter++;
                }
                Logout(signature);
                var dtSector = new DataTable();
                dtSector.Columns.Add("SectorCount", typeof(string));
                var drSector = dtSector.NewRow();
                drSector[0] = ds.Tables.Count;
                dtSector.Rows.Add(drSector);
                dtSector.AcceptChanges();
                dtSector.TableName = "dtsector";
                ds.Tables.Add(dtSector);
                return ds.GetXml().ToString();
        }

        
        
        /// <summary>
        /// Get Flight Type ex: airbus,boeing,etc...info
        /// </summary>
        /// <param name="segs"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static string GetEquipment_Flight(Segment[] segs, string signature)
        {
                string flightno = string.Empty;
                string flightName = string.Empty;
                System.Text.StringBuilder str=new System.Text.StringBuilder();
                int counter = 0;
               
               foreach (Segment seg in segs)
                {
                    if (seg.Legs.Length > 1)
                    {
                        for (int i = 0; i < seg.Legs.Length; i++)
                        {
                            if (i == 0)
                            {
                                str.Append(seg.Legs[i].FlightDesignator.CarrierCode + "" + seg.Legs[i].FlightDesignator.FlightNumber + "  <br/>");
                                flightno = flightno + seg.Legs[i].FlightDesignator.CarrierCode + "" + seg.Legs[i].FlightDesignator.FlightNumber + " , ";
                            }
                            else
                                str.Append(seg.Legs[i].FlightDesignator.CarrierCode + "" + seg.Legs[i].FlightDesignator.FlightNumber + "  (Same Aircraft) <br/>");
                           
                            str.Append(seg.Legs[i].DepartureStation + "-" + seg.Legs[i].STD + " to " + seg.Legs[i].ArrivalStation + "-" + seg.Legs[i].STA + "  <br/>");
                            //str.Append("The aircraft type is " + " " + "#AT" + counter + "  <br/>");
                            str.Append("The aircraft type is " + " " + GetEquipmentName(seg.Legs[i].LegInfo.Capacity) + "  <br/>");

                            str.Append("Estimated travel time :" + TimeDuration1(seg.Legs[i].STD.ToString(), seg.Legs[i].STA.ToString(), seg.Legs[i].DepartureStation, seg.Legs[i].ArrivalStation) + "  <br/>");
                            if (i < seg.Legs.Length - 1)
                                str.Append("Layover Time :" + TimeDuration1(seg.Legs[i + 1].STD.ToString(), seg.Legs[i].STA.ToString(), seg.Legs[i + 1].DepartureStation, seg.Legs[i].ArrivalStation) + "  <br/><br/>");
                            flightName = flightName + "," + GetEquipmentName(seg.Legs[i].LegInfo.Capacity);
                        }
                    }
                    else
                    {
                         
                        flightno = flightno + seg.FlightDesignator.CarrierCode + "" + seg.FlightDesignator.FlightNumber + " , ";
                        if (counter == 0)
                            str.Append(seg.FlightDesignator.CarrierCode + "" + seg.FlightDesignator.FlightNumber + "  <br/>");
                        else
                            str.Append(seg.FlightDesignator.CarrierCode + "" + seg.FlightDesignator.FlightNumber + "  (Change of Aircraft) <br/>");
                        str.Append(seg.DepartureStation + "-" + seg.STD + " to " + seg.ArrivalStation + "-" + seg.STA + "  <br/>");
                        //str.Append("The aircraft type is " + " " + "#AT" + counter + "  <br/>");
                        var flightNameinfo = GetEquipmentName(seg.Legs[0].LegInfo.Capacity);
                        str.Append("The aircraft type is " + " " + flightNameinfo + "  <br/>");
                        
                        str.Append("Estimated travel time :" + TimeDuration1(seg.STD.ToString(), seg.STA.ToString(),seg.DepartureStation,seg.ArrivalStation) + "  <br/>");
                        if (counter < segs.Length - 1)
                            str.Append("Layover Time :" + TimeDuration1(segs[counter + 1].STD.ToString(), seg.STA.ToString(), segs[counter + 1].DepartureStation, seg.ArrivalStation) + "  <br/><br/>");
                        flightName = flightNameinfo;
                    }
                    counter++;
             }

              str.Append("@" + flightno.Substring(0, flightno.Length - 2));
              str.Append("@" + flightName);
              return str.ToString();
        }
        

       /// <summary>
       /// Get Flight Name
       /// </summary>
       /// <param name="loadCapacity"></param>
       /// <returns></returns>
       private static string GetEquipmentName(Int16 loadCapacity)
        {
            var strName = string.Empty;
            if (loadCapacity >= 212)
            {
                strName = "BOEING 737-900 ER";
            }
            else if (loadCapacity == 189)
            {
                strName = "BOEING 737-800";
            }
            else if (loadCapacity == 78)
            {
                strName = "Q400";
            }
            else if (loadCapacity >= 110 && loadCapacity <= 189)//144 & 149
            {
                strName = "BOEING 737";
            }
            else if (loadCapacity >= 78 && loadCapacity <= 110)//78,88
                strName = "Q400";
            return strName;
        }


        /// <summary>
        /// Save XML info
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filename"></param>
        private static void SaveXML(object obj, string filename)
        {
            try
            {
                /*==========Purpose : To give Xml to spiceJet team when facing any isssue/concirn======*/
                BookingManager.AvailableFare bfair = new AvailableFare();
                filename =  HttpContext.Current.Session["RequestId"].ToString() + "_" + filename;
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType(), new Type[] { bfair.GetType() });
                TextWriter txtWriter = new StreamWriter(HttpContext.Current.Server.MapPath("~/xml") + "//" + filename);
                xmlSerializer.Serialize(txtWriter, obj);
                txtWriter.Close();
            }
            catch (Exception ex)
            {
                 
            }
        }

        //************ PNR Generation  ****************************************************************
        /// <summary>
        /// Generate PNR
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        public static string Generate_PNR(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> passengerList, List<PassengerMealAllocationModels> passengerMealsList)
        {
            try
            {
                string signature = Login();
                HttpContext.Current.Session["RequestId"] = sodRequestsList[0].TravelRequestId.ToString().Trim();
                GetAvailabilityResponse availabilityrespone = GetAvailability_PNR(signature, sodRequestsList, sodflightList);
                SellResponse sellresponse = Selling_PNR(signature, availabilityrespone, sodRequestsList[0].BookingFor, sodRequestsList[0].RequestedEmpDept.Trim().ToLower(), sodRequestsList[0].IsAmountPaidByTraveller, sodflightList);

                //GetBooking From State Response
                GetBookingFromStateResponse bookingResp_AfterSell = GetBookingFrom_State(signature);
                SellResponse sellresponse1 = Sell_SSR(signature, bookingResp_AfterSell, passengerMealsList, sodRequestsList[0].BookingFor, sodflightList, sodRequestsList[0].SodBookingTypeId);

                if(sellresponse1.BookingUpdateResponseData.OtherServiceInformations==null)
                {
                 sellresponse = sellresponse1;
                }
                //GetBooking From State Response
                GetBookingFromStateResponse bookingResp = GetBookingFrom_State(signature); 

                if (!sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                {
                    AddPaymentToBookingResponse paymentresponse = AddPayment_PNR_FromStateResponse(signature, bookingResp, sodRequestsList[0].RequestedEmpDept.Trim().ToLower(), sodRequestsList[0].IsAmountPaidByTraveller);
                }
                BookingCommitResponse commitresponse = BookingCommit_PNR(signature, sodRequestsList, passengerList,sodflightList);
                string pnr = commitresponse.BookingUpdateResponseData.Success.RecordLocator;
                string amount = commitresponse.BookingUpdateResponseData.Success.PNRAmount.TotalCost.ToString().Trim();
                return pnr = (pnr + "|" + amount);
                //getFlightBooking(pnr);
            }
            catch (Exception ex)
            {
               ErrorLog objLogging = new ErrorLog();
               return objLogging.AddDBLogging(ex, "Generate_PNR", "CommonWebMethod/NavitaireServices.cs");
            }
        }



        /// <summary>
        /// Get Booking from State
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static GetBookingFromStateResponse GetBookingFrom_State(string signature)
        {
            IBookingManager bookingManager=new  BookingManagerClient();
            GetBookingFromStateRequest req=new GetBookingFromStateRequest();
            GetBookingFromStateResponse res=new GetBookingFromStateResponse ();

            try
            {
            req.Signature=signature;
            req.ContractVersion=0;
            res=bookingManager.GetBookingFromState(req);
            }
            catch(Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "Generate_PNR", "NavitaireServices/GetBookingFrom_State.cs");
            }
            return res;
        }

        
        /// <summary>
        /// Check Availability PNR
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <returns></returns>
        public static GetAvailabilityResponse GetAvailability_PNR(string signature, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList)
        {
                IBookingManager bookingManager = new BookingManagerClient();
                GetAvailabilityRequest gar = new GetAvailabilityRequest();
                TripAvailabilityRequest tar = new TripAvailabilityRequest();
                tar.AvailabilityRequests = new AvailabilityRequest[sodflightList.Count];
                int counter = 0;
                foreach (var item in sodflightList)
                {
                    AvailabilityRequest ar = new AvailabilityRequest();
                    ar.DepartureStation = item.OriginPlace;
                    ar.ArrivalStation = item.DestinationPlace;
                    ar.BeginDate = item.TravelDate; 
                    ar.EndDate = item.TravelDate;

                    //4x
                    //ar.FlightType = FlightType.All;
                    if (item.FlightTypes == 1 || item.FlightTypes == 2)
                        ar.FlightType = FlightType.Direct;
                    else
                        ar.FlightType = FlightType.All;
                    
                    if (item.FlightNo.Length > 6)
                    {
                        string flight = item.FlightNo.Split(',')[0];
                        ar.FlightNumber = flight.Substring(2, flight.Length - 2).TrimEnd();
                    }
                    else
                    {
                        ar.FlightNumber = item.FlightNo.Substring(2, item.FlightNo.Length - 2).TrimEnd();
                    }

                    ar.PaxCount = short.Parse(sodRequestsList[0].Passengers.ToString());
                    ar.Dow = DOW.Daily;
                    ar.CurrencyCode = "INR";
                    
                    //Change4X
                    //ar.AvailabilityType = AvailabilityType.Default;
                    if (sodRequestsList[0].BookingFor.ToLower() == "standby")
                    {
                        ar.AvailabilityType = AvailabilityType.Standby;
                    }
                    else
                    {
                        ar.AvailabilityType = AvailabilityType.Overbook;
                    }
                    ar.MaximumConnectingFlights = 5;
                    ar.AvailabilityFilter = AvailabilityFilter.ExcludeUnavailable;
                    ar.FareClassControl = FareClassControl.LowestFareClass;
                    ar.MinimumFarePrice = 0;
                    ar.MaximumFarePrice = 0;
                    ar.SSRCollectionsMode = SSRCollectionsMode.None;
                    ar.InboundOutbound = InboundOutbound.None;
                    ar.NightsStay = 0;
                    ar.IncludeAllotments = false;
                    ar.FareTypes = new string[1];
                    ar.FareTypes[0] = "Z";
                    PaxPriceType[] priceTypes = new PaxPriceType[ar.PaxCount];
                    for (int i = 0; i < ar.PaxCount; i++)
                    {
                        priceTypes[i] = new PaxPriceType();
                        priceTypes[i].PaxType = "ADT";
                        priceTypes[i].PaxDiscountCode = String.Empty;
                    }
                    ar.PaxPriceTypes = priceTypes;
                    ar.IncludeTaxesAndFees = false;
                    ar.FareRuleFilter = FareRuleFilter.Default;
                    ar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                    tar.AvailabilityRequests[counter] = ar;
                    counter++;
                }
                tar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                tar.LowFareMode = false;
                gar.TripAvailabilityRequest = tar;
                gar.Signature = signature;
                gar.ContractVersion = 0;

                GetAvailabilityResponse gare = bookingManager.GetAvailability(gar);
                SaveXML(gar, "GetAvailabilityRequest.xml");
                SaveXML(gare, "GetAvailabilityResponse.xml");
                return gare;
        }

        /// <summary>
        /// Showing breakup of fair
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="sodRequestsList"></param>
        /// <returns></returns>
        public static PriceItineraryResponse GetPrice_PNR(string signature, GetAvailabilityResponse response, List<TravelRequestMasterModels> sodRequestsList)
        { 
                IBookingManager bookingManager = new BookingManagerClient();
                PriceItineraryRequest pir = new PriceItineraryRequest();
                ItineraryPriceRequest ipr = new ItineraryPriceRequest();
                ipr.PriceItineraryBy = PriceItineraryBy.JourneyBySellKey;
                ipr.SellByKeyRequest = new SellJourneyByKeyRequestData();
                //4x
                //ipr.SellByKeyRequest.ActionStatusCode = "NN"; 
                if (sodRequestsList[0].BookingFor.ToLower() == "standby")
                {
                    ipr.SellByKeyRequest.ActionStatusCode = "LL";
                }
                else
                {
                    ipr.SellByKeyRequest.ActionStatusCode = "SS";
                }
                SellKeyList[] list = new SellKeyList[response.GetTripAvailabilityResponse.Schedules.Length];

                int l = 0;
                foreach (JourneyDateMarket[] jdmArray in response.GetTripAvailabilityResponse.Schedules)
                {
                    list[l] = new SellKeyList();
                    foreach (JourneyDateMarket jdm in jdmArray)
                    {
                        foreach (Journey journey in jdm.Journeys)
                        {
                            list[l].JourneySellKey = journey.JourneySellKey;
                            foreach (Segment segment in journey.Segments)
                            {
                                if (segment.Fares.Length > 0)
                                {
                                    list[l].FareSellKey = string.IsNullOrEmpty(list[l].FareSellKey) ? segment.Fares[0].FareSellKey : list[l].FareSellKey + "^" + segment.Fares[0].FareSellKey;
                                }
                            }
                        }
                    }
                    l++;
                }

                ipr.SellByKeyRequest.JourneySellKeys = list;
                ipr.SellByKeyRequest.PaxCount = short.Parse(sodRequestsList[0].Passengers.ToString());
                PaxPriceType[] priceTypes = new PaxPriceType[ipr.SellByKeyRequest.PaxCount];
                for (int i = 0; i < ipr.SellByKeyRequest.PaxCount; i++)
                {
                    priceTypes[i] = new PaxPriceType();
                    priceTypes[i].PaxType = "ADT";
                    priceTypes[i].PaxDiscountCode = String.Empty;
                }
                ipr.SellByKeyRequest.PaxPriceType = priceTypes;
                ipr.SellByKeyRequest.CurrencyCode = "INR";
                ipr.SellByKeyRequest.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                ipr.SellByKeyRequest.IsAllotmentMarketFare = false;
                
                pir.ItineraryPriceRequest = ipr;
                pir.Signature = signature;
                SaveXML(pir, "PriceItineraryRequest.xml");
                PriceItineraryResponse pire = bookingManager.GetItineraryPrice(pir);
                SaveXML(pire, "PriceItineraryResponse.xml");
                return pire;
             
        }
       
        /// <summary>
        /// Selling PNR Method
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="bookingfor"></param>
        /// <returns></returns>
        public static SellResponse Selling_PNR(string signature, GetAvailabilityResponse response, string bookingfor, string dept, bool IsAmountPaidByTraveller, List<FlightDetailModels> sodflightList)
        {
                IBookingManager bookingManager = new BookingManagerClient();
                SellRequest sr = new SellRequest();
                SellRequestData srd = new SellRequestData();
                srd.SellBy = SellBy.JourneyBySellKey;
                SellJourneyByKeyRequest sjr = new SellJourneyByKeyRequest();
                SellJourneyByKeyRequestData skd = new SellJourneyByKeyRequestData();
                if (bookingfor.ToLower() == "standby")
                    skd.ActionStatusCode = "LL";
                else
                    skd.ActionStatusCode = "SS";
                SellKeyList[] list = new SellKeyList[response.GetTripAvailabilityResponse.Schedules.Length];
                var resflightNo=string.Empty;
                int l = 0;
                foreach (JourneyDateMarket[] jdmArray in response.GetTripAvailabilityResponse.Schedules)
                {
                    list[l] = new SellKeyList();
                    foreach (JourneyDateMarket jdm in jdmArray)
                    {
                        foreach (Journey journey in jdm.Journeys)
                        {
                            if (journey.Segments.Length >= 2)
                                resflightNo = "SG" + journey.Segments[0].FlightDesignator.FlightNumber + " , SG" + journey.Segments[1].FlightDesignator.FlightNumber;
                            else
                                resflightNo = "SG" + journey.Segments[0].FlightDesignator.FlightNumber;
                            if(sodflightList[l].FlightNo.Trim() == resflightNo.Trim())
                            {
                             list[l].JourneySellKey = journey.JourneySellKey;
                             foreach (Segment segment in journey.Segments)
                             {
                                if (segment.Fares.Length > 0)
                                {
                                    list[l].FareSellKey = string.IsNullOrEmpty(list[l].FareSellKey) ? segment.Fares[0].FareSellKey : list[l].FareSellKey + "^" + segment.Fares[0].FareSellKey;
                                }
                             }
                             if (bookingfor.ToLower() == "standby")
                                list[l].StandbyPriorityCode = "NR";
                             }
                        }
                    }
                    l++;
                }
                skd.JourneySellKeys = list;
                skd.PaxCount = short.Parse(response.GetTripAvailabilityResponse.Schedules[0][0].Journeys[0].Segments[0].Fares[0].PaxFares.Length.ToString());
                PaxPriceType[] priceTypes = new PaxPriceType[skd.PaxCount];
                for (int i = 0; i < skd.PaxCount; i++)
                {
                    priceTypes[i] = new PaxPriceType();
                    priceTypes[i].PaxType = "ADT";
                    priceTypes[i].PaxDiscountCode = String.Empty;
                }
                skd.PaxPriceType = priceTypes;
                skd.CurrencyCode = "INR"; 
                //change for POS
                PointOfSale ps = new PointOfSale();
                ps.State = MessageState.New;

                if (IsAmountPaidByTraveller.Equals(true))
                {
                    ps.AgentCode = null;
                    ps.OrganizationCode = null;  
                }
                else
                {
                     ps.AgentCode = dept == mDept ? "SDSAFTYDEL" : "SDFINANCED"; 
                     ps.OrganizationCode = dept == mDept ? "SDSAFTYDEL" : "SDFINANCED"; 
                }
               
                //4X
                ps.DomainCode = "WWW";
                //ps.DomainCode = "WWWW";
                ps.LocationCode = "WWW";
                skd.SourcePOS = ps;

                skd.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                skd.IsAllotmentMarketFare = false;

                sjr.SellJourneyByKeyRequestData = skd;
                srd.SellJourneyByKeyRequest = sjr;
                sr.Signature = signature;
                sr.SellRequestData = srd;
               
                SellResponse sre = new SellResponse();
                sre = bookingManager.Sell(sr);
                SaveXML(sr, "SellRequest.xml");
                SaveXML(sre, "SellResponse.xml");
                return sre;
        }

        /// <summary>
        /// Sell SSR method
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="meal"></param>
        /// <param name="bookingfor"></param>
        /// <param name="sodflightList"></param>
        /// <returns></returns>
        public static SellResponse Sell_SSR(string signature, GetBookingFromStateResponse response, List<PassengerMealAllocationModels> mealList, string bookingfor, List<FlightDetailModels> sodflightList,int sodType)
        {
                IBookingManager bookingManager = new BookingManagerClient();
                SellRequest selre = new SellRequest();
                SellRequestData selredata = new SellRequestData();
                selredata.SellBy = SellBy.SSR;
                selredata.SellSSR = new SellSSR();
                selredata.SellSSR.SSRRequest = new SSRRequest();
                if (response.BookingData != null)//for 4X Error Checking
                {
                    selredata.SellSSR.SSRRequest.SegmentSSRRequests = new SegmentSSRRequest[response.BookingData.Journeys.Length];
                    int l = 0;
                    foreach (Journey jouney in response.BookingData.Journeys)
                    {
                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l] = new SegmentSSRRequest();
                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].FlightDesignator = jouney.Segments[0].FlightDesignator;
                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].STD = jouney.Segments[0].STD;
                        if (jouney.Segments[0].Legs.Length > 1)
                        {
                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].DepartureStation = jouney.Segments[0].Legs[0].DepartureStation;
                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].ArrivalStation = jouney.Segments[0].Legs[0].ArrivalStation;
                        }
                        else
                        {
                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].DepartureStation = jouney.Segments[0].DepartureStation;
                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].ArrivalStation = jouney.Segments[0].ArrivalStation;
                        }

                        string sector = jouney.Segments[0].DepartureStation + "-" + jouney.Segments[jouney.Segments.Length - 1].ArrivalStation;
                        var mealCount = mealList.FindAll(s => s.Sector == sector && s.MealType != "Not Required").Count;
                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs = new PaxSSR[mealCount];

                        //for NON-Sod Need to rotate the loop according person wise
                        if (sodType == 2)
                        {
                            short j = 0;
                            short i = 0;
                            foreach (var meal in mealList)
                            {
                                //Implement ADD PAX
                                if (meal.Sector == sector)
                                {
                                    if (meal.MealType.Trim() != "Not Required")
                                    {
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i] = new PaxSSR();
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].State = MessageState.New;
                                        if (bookingfor.ToLower() == "standby")
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ActionStatusCode = "LL";
                                        else
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ActionStatusCode = "SS";
                                        if (jouney.Segments[0].Legs.Length > 1)
                                        {
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].DepartureStation = jouney.Segments[0].Legs[0].DepartureStation;
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ArrivalStation = jouney.Segments[0].Legs[0].ArrivalStation;
                                        }
                                        else
                                        {
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].DepartureStation = jouney.Segments[0].DepartureStation;
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ArrivalStation = jouney.Segments[0].ArrivalStation;
                                        }
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].PassengerNumber = j;
                                        //BL for Allocate Meal Date :11 August 2016
                                        if (meal.MealType.Trim() == "BVG")
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = meal.MealType;
                                        else if (meal.MealType.Trim() == "MNCH")
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = meal.MealType;
                                        else
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = AllocateMeal(signature, meal.MealType, sodflightList[l]);
                                        //selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = meal;
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRNumber = j;
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRValue = j;
                                        i++;
                                    }
                                    j++;
                                }
                            }
                        }
                        //Sod Implement Pax
                        else if (sodType == 1)
                        {
                            short i = 0;
                            foreach (var meal in mealList)
                            {
                                if (meal.Sector == sector)
                                {
                                    if (meal.MealType != "Not Required")
                                    {
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i] = new PaxSSR();
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].State = MessageState.New;
                                        if (bookingfor.ToLower().Trim() == "standby")
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ActionStatusCode = "LL";
                                        else
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ActionStatusCode = "SS";
                                        if (jouney.Segments[0].Legs.Length > 1)
                                        {
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ArrivalStation = jouney.Segments[0].Legs[0].ArrivalStation;
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].DepartureStation = jouney.Segments[0].Legs[0].DepartureStation;
                                        }
                                        else
                                        {
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].ArrivalStation = jouney.Segments[0].ArrivalStation;
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].DepartureStation = jouney.Segments[0].DepartureStation;
                                        }

                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].PassengerNumber = 0;
                                        //BL for Allocate Meal Date :11 August 2016
                                        if (meal.MealType.Trim() == "BVG")
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = meal.MealType;
                                        else if (meal.MealType.Trim() == "MNCH")
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = meal.MealType;
                                        else
                                            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = AllocateMeal(signature, meal.MealType, sodflightList[l]);
                                        //selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = meal;
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRNumber = 0;
                                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRValue = 0;
                                        i++;
                                    }
                                }
                            }
                        }
                        l++;
                    }
                }
                selredata.SellSSR.SSRRequest.CurrencyCode = "INR";
                selredata.SellSSR.SSRRequest.CancelFirstSSR = false;
                selredata.SellSSR.SSRRequest.SSRFeeForceWaiveOnSell = false;
                selre.ContractVersion = 0;
                selre.Signature = signature;
                selre.SellRequestData = selredata;
                SaveXML(selre, "SELLSSRRequest.xml");
                SellResponse sre = new SellResponse();
                sre = bookingManager.Sell(selre);
                SaveXML(sre, "SELLSSRResponse.xml");
                return sre;
        }

       
        /// <summary>
        /// Allocate Meal
        /// </summary>
        /// <returns></returns>
        public static string AllocateMeal(string signature,string mealCodeRequest, FlightDetailModels sodflightList)
        {
            //For Q400 Flights : Only For Sandwich
            //For Other Flights According time Meal will be allocated
            string FlightName = sodflightList.FlightName.Split(',')[0].Trim();
            string mealResponseCode = string.Empty;
            if (FlightName.Equals("Q400"))
            {
                if (mealCodeRequest.Equals("VGML"))
                    mealResponseCode = "VGSW";
                 else if (mealCodeRequest.Equals("NVML"))
                    mealResponseCode = "NVSW";
                else
                    mealResponseCode = mealCodeRequest;
            }
            else
            {
                mealResponseCode = mealCodeRequest;
            }
            return mealResponseCode;
        }


       /// <summary>
       /// Convert Hrs to Minute
       /// </summary>
       /// <param name="time"></param>
       /// <returns></returns>
        public static int ConvertHrsToMinute(String time)
        {
           int hr = Convert.ToDateTime(time).Hour;
           int min = Convert.ToDateTime(time).Minute;
           int totalTime = hr * 60 + min;
           return totalTime;
        }
       
      
        /// <summary>
        /// Add Payment PNR GetBookingFromStateResponse
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="dept"></param>
        /// <param name="IsAmountPaidByTraveller"></param>
        public static AddPaymentToBookingResponse AddPayment_PNR_FromStateResponse(string signature, GetBookingFromStateResponse response, string dept, bool IsAmountPaidByTraveller)
       {
                IBookingManager bookingManager = new BookingManagerClient();
                AddPaymentToBookingRequest addreq = new AddPaymentToBookingRequest();
                AddPaymentToBookingRequestData addreqdata = new AddPaymentToBookingRequestData();
               
                addreqdata.MessageState = MessageState.New;
                addreqdata.WaiveFee = false;
                addreqdata.ReferenceType = PaymentReferenceType.Default;
                addreqdata.PaymentMethodType = RequestPaymentMethodType.AgencyAccount;
                addreqdata.PaymentMethodCode = "AG";
                addreqdata.QuotedCurrencyCode = "INR";
                addreqdata.QuotedAmount = response.BookingData.BookingSum.BalanceDue;
                addreqdata.Status = BookingPaymentStatus.New;
                addreqdata.AccountNumberID = 0;
                if (IsAmountPaidByTraveller.Equals(true))
                    addreqdata.AccountNumber = null;
                else
                addreqdata.AccountNumber = dept == mDept ? "SDSAFTYDEL" : "SDFINANCED"; 
                //addreqdata.Expiration = DateTime.Today;
                addreqdata.ParentPaymentID = 0;
                addreqdata.Installments = 0;
                addreqdata.Deposit = false;

                addreq.addPaymentToBookingReqData = addreqdata;
                addreq.ContractVersion = 0;
                addreq.Signature = signature;
                AddPaymentToBookingResponse aresponse = new AddPaymentToBookingResponse();
                SaveXML(addreq, "AddPaymentToBookingRequest.xml");
                aresponse = bookingManager.AddPaymentToBooking(addreq);
                SaveXML(aresponse, "AddPaymentToBookingResponse.xml");
                return aresponse;

       }

        /// <summary>
        /// Add PaymentToBookingResponse Method
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static AddPaymentToBookingResponse AddPayment_PNR(string signature, SellResponse response, string dept, bool IsAmountPaidByTraveller)
        {
                IBookingManager bookingManager = new BookingManagerClient();
                AddPaymentToBookingRequest addreq = new AddPaymentToBookingRequest();
                AddPaymentToBookingRequestData addreqdata = new AddPaymentToBookingRequestData();
               
                addreqdata.MessageState = MessageState.New;
                addreqdata.WaiveFee = false;
                addreqdata.ReferenceType = PaymentReferenceType.Default;
                addreqdata.PaymentMethodType = RequestPaymentMethodType.AgencyAccount;
                addreqdata.PaymentMethodCode = "AG";
                addreqdata.QuotedCurrencyCode = "INR";
                addreqdata.QuotedAmount = response.BookingUpdateResponseData.Success.PNRAmount.TotalCost;
                addreqdata.Status = BookingPaymentStatus.New;
                addreqdata.AccountNumberID = 0;

                if (IsAmountPaidByTraveller.Equals(true))
                    addreqdata.AccountNumber = null;
                else
                addreqdata.AccountNumber = dept == mDept ? "SDSAFTYDEL" : "SDFINANCED"; //"SDFINANCED";
                //addreqdata.Expiration = DateTime.Today;
                addreqdata.ParentPaymentID = 0;
                addreqdata.Installments = 0;
                addreqdata.Deposit = false;

                addreq.addPaymentToBookingReqData = addreqdata;
                addreq.ContractVersion = 0;
                addreq.Signature = signature;
                AddPaymentToBookingResponse aresponse = new AddPaymentToBookingResponse();
                SaveXML(addreq, "AddPaymentToBookingRequest.xml");
                aresponse = bookingManager.AddPaymentToBooking(addreq);
                SaveXML(aresponse, "AddPaymentToBookingResponse.xml");
                return aresponse;
        }
       
       
        /// <summary>
        /// Booking Commit PNR :Final PNR Generation Method
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="sodRequestsList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        public static BookingCommitResponse BookingCommit_PNR(string signature, List<TravelRequestMasterModels> sodRequestsList, List<PassengerDetailModels> passengerList, List<FlightDetailModels> sodflightList)
        {       
                string comment =string.Empty;
                var userInfo =new List<EmployeeModel>();
                IBookingManager bookingManager = new BookingManagerClient();
                BookingCommitRequest breq = new BookingCommitRequest();
                BookingCommitRequestData breqdata = new BookingCommitRequestData();
                breqdata.State = MessageState.New;
                breqdata.CurrencyCode = "INR"; 
                breqdata.PaxCount = short.Parse(sodRequestsList[0].Passengers.ToString());
                breqdata.BookingID = 0;
                breqdata.BookingParentID = 0;
                breqdata.BookingComments = new BookingComment[1];
                breqdata.BookingComments[0] = new BookingComment();
                string dept = sodRequestsList[0].RequestedEmpDept.Trim().ToLower();
                string btype = sodRequestsList[0].SodBookingTypeId.Equals(1) ? "SOD" : "NON-SOD";
                
                if(sodRequestsList[0].IsVendorBooking.Equals(true))
                    comment = "NON-SOD Vendor Booking - " + sodRequestsList[0].BookingFor;
                else
                    comment = btype + "-" + sodRequestsList[0].BookingFor;
                if(sodRequestsList[0].RequestedEmpId==0)
                 userInfo = Services.ADO.SodCommonServices.GetEmployeeCommonDetailsSJSC(sodRequestsList[0].EmailId, 1);
                else
                 userInfo = Services.ADO.SodCommonServices.GetEmployeeCommonDetails(sodRequestsList[0].RequestedEmpId);
                breqdata.BookingComments[0].CommentText = comment + "| " + userInfo[0].EmpCode + "| " + userInfo[0].EmpName + "| " + userInfo[0].Designation + "| " + userInfo[0].Department + "| " + userInfo[0].Phone + "| " + userInfo[0].Email;

                PointOfSale ps = new PointOfSale();
                ps.State = MessageState.New;
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                {
                    ps.AgentCode = null;
                    ps.OrganizationCode = null;
                }
                else
                {
                    ps.AgentCode = dept == mDept ? "SDSAFTYDEL" : "SDFINANCED";
                    ps.OrganizationCode = dept == mDept ? "SDSAFTYDEL" : "SDFINANCED";
                }

                ps.DomainCode = "WWWW";
                ps.LocationCode = "WWW";
                breqdata.SourcePOS = ps;
                breqdata.Passengers = new Passenger[breqdata.PaxCount];
                short i = 0;
                foreach (var passenger in passengerList)
                {
                    breqdata.Passengers[i] = new Passenger();
                    breqdata.Passengers[i].PassengerNumber = i;
                    breqdata.Passengers[i].FamilyNumber = i;
                    breqdata.Passengers[i].State = MessageState.Modified;
                    breqdata.Passengers[i].Names = new BookingName[1];
                    breqdata.Passengers[i].Names[0] = new BookingName();
                    breqdata.Passengers[i].Names[0].State = MessageState.New;
                    breqdata.Passengers[i].Names[0].FirstName = passenger.TravelerFirstName.Trim();
                    //breqdata.Passengers[i].Names[0].MiddleName = "";
                    breqdata.Passengers[i].Names[0].LastName = passenger.TravelerLastName.Trim();
                    breqdata.Passengers[i].Names[0].Title = passenger.Title.Trim().Substring(0,2).ToUpper();
                    breqdata.Passengers[i].PassengerInfos =  new PassengerInfo[1];
                    breqdata.Passengers[i].PassengerInfos[0] = new PassengerInfo();
                    if (passenger.TravelerGender.Trim().Equals("M"))
                    {
                        breqdata.Passengers[i].PassengerInfos[0].Gender = Gender.Male;
                        breqdata.Passengers[i].PassengerInfos[0].WeightCategory =  WeightCategory.Male;
                    }
                    else
                    {
                        breqdata.Passengers[i].PassengerInfos[0].Gender =Gender.Female;
                        breqdata.Passengers[i].PassengerInfos[0].WeightCategory = WeightCategory.Female;
                    }
                    i++;
                }
                breqdata.BookingContacts = new BookingContact[1];
                breqdata.BookingContacts[0] = new BookingContact();
                breqdata.BookingContacts[0].CompanyName = "SpiceJet";
                breqdata.BookingContacts[0].Names = new BookingName[1];
                breqdata.BookingContacts[0].Names[0] = new BookingName();
                breqdata.BookingContacts[0].Names[0].Title = sodRequestsList[0].Title.Trim().Substring(0, 2).ToUpper(); 
                breqdata.BookingContacts[0].Names[0].FirstName = sodRequestsList[0].RequestedEmpName.Trim().Substring(0, sodRequestsList[0].RequestedEmpName.Trim().LastIndexOf(' '));
                breqdata.BookingContacts[0].Names[0].LastName = sodRequestsList[0].RequestedEmpName.Trim().Substring(sodRequestsList[0].RequestedEmpName.Trim().LastIndexOf(' ') + 1, sodRequestsList[0].RequestedEmpName.Trim().Length - sodRequestsList[0].RequestedEmpName.Trim().LastIndexOf(' ') - 1);

                if (sodRequestsList[0].IsVendorBooking.Equals(true))
                {
                    breqdata.BookingContacts[0].AddressLine1 = sodRequestsList[0].PassAddressLine1;
                    breqdata.BookingContacts[0].AddressLine2 = sodRequestsList[0].PassAddressLine2;
                    breqdata.BookingContacts[0].City = sodRequestsList[0].PassCity;
                    breqdata.BookingContacts[0].ProvinceState = "";
                    breqdata.BookingContacts[0].CountryCode = "IN";
                }
                else
                {
                    breqdata.BookingContacts[0].AddressLine1 = "SpiceJet Ltd.| 319 | Udyog Vihar | Phase IV";
                    breqdata.BookingContacts[0].City = "Gurugram - 122016";
                    breqdata.BookingContacts[0].ProvinceState = "HR";
                    breqdata.BookingContacts[0].CountryCode = "IN";
                }
                //4X
                breqdata.BookingContacts[0].CultureCode = "en-GB";
                breqdata.BookingContacts[0].EmailAddress = btype == "SOD" ? sodRequestsList[0].EmailId : sodRequestsList[0].PassEmailId;
                breqdata.BookingContacts[0].HomePhone = sodRequestsList[0].Phno;
                breqdata.BookingContacts[0].PostalCode = "";
                breqdata.BookingContacts[0].NotificationPreference = NotificationPreference.None;
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                {
                    breqdata.BookingContacts[0].SourceOrganization = null;
                }
                else
                {
                    breqdata.BookingContacts[0].SourceOrganization = dept == mDept ? "SDSAFTYDEL" : "SDFINANCED";
                }
                breqdata.BookingContacts[0].DistributionOption = DistributionOption.Email;
                breqdata.BookingContacts[0].TypeCode = "P";

                breqdata.RestrictionOverride = false;
                breqdata.ChangeHoldDateTime = false;
                breqdata.WaiveNameChangeFee = false;
                breqdata.WaivePenaltyFee = false;
                breqdata.WaiveSpoilageFee = false;
                //4X --for sending email direct
                breqdata.DistributeToContacts = true;
             
                //Hold Booking :For NON-SOD- Standby and Confirm Booking*************************** 
                if(sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                {
                    breqdata.ChangeHoldDateTime = true;
                    breqdata.BookingHold = new BookingHold();
                    breqdata.BookingHold.State = MessageState.New;
                    var PNRGenerationTime = System.DateTime.Now;
                    var HoldDateTimes = CommonWebMethods.GetHoldBookingDateTime(sodRequestsList[0].SodBookingTypeId, sodRequestsList[0].BookingFor, sodflightList[0].TravelDate, sodflightList[0].DepartureTime, PNRGenerationTime);
                    //HoldDateTimes=HoldDateTimes.AddHours(-5.30);//UTC Time
                    breqdata.BookingHold.HoldDateTime = HoldDateTimes;
                }              
                //*END******************************************************************************

                breq.BookingCommitRequestData = breqdata;
                breq.ContractVersion = 0;
                breq.Signature = signature;

                BookingCommitResponse bresponse = null;
                SaveXML(breq, "BookingCommitRequest.xml");
                bresponse = bookingManager.BookingCommit(breq); 
                SaveXML(bresponse, "BookingCommitResponse.xml");

                /*************Message OK to board Only allowed for SOD Booking*********************/
                if (sodRequestsList[0].SodBookingTypeId.Equals(1))
                {
                    if (sodRequestsList[0].TravelRequestTypeId.Equals(1) || sodRequestsList[0].TravelRequestTypeId.Equals(2))
                    {
                        var bookingtype = sodRequestsList[0].TravelRequestTypeId.Equals(1) ? "oneway" : "roundtrip";
                        if (sodflightList.Count == 1)
                            bookingtype = "oneway";
                        if (CommonWebMethods.IsOktoBoardRequired(sodflightList[0].DestinationPlace.Trim(), bookingtype))
                        {
                            OKtoBoard(signature, bresponse.BookingUpdateResponseData.Success.RecordLocator, passengerList[0].TravelerFirstName + " " + passengerList[0].TravelerLastName);
                        }
                    }
                    else
                    {
                        var bookingtype = "multicity";
                        if (sodflightList.Count == 1) bookingtype = "oneway";
                        else if (sodflightList.Count == 2) bookingtype ="roundtrip";

                        foreach (var flight in sodflightList)
                        {
                            if (CommonWebMethods.IsOktoBoardRequired_forMulticitySectors(flight.OriginPlace.Trim(), flight.DestinationPlace.Trim(), bookingtype))
                            {
                                OKtoBoard(signature, bresponse.BookingUpdateResponseData.Success.RecordLocator, passengerList[0].TravelerFirstName + " " + passengerList[0].TravelerLastName);
                                break;
                            }
                        }
                    }
                }
                /*******End*******************************************/
                SentItinerary_PNR(signature, bresponse);
                return bresponse;
        }
        
        /// <summary>
        /// Set Itinerary PNR
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        public static void SentItinerary_PNR(string signature, BookingCommitResponse response)
        {
            try
            {
                IBookingManager ibm = new BookingManagerClient();
                SendItineraryRequest str = new SendItineraryRequest();
                str.ContractVersion = 0;
                str.Signature = signature;
                str.RecordLocatorReqData = response.BookingUpdateResponseData.Success.RecordLocator;
                SendItineraryResponse stre = null;
                stre = ibm.SendItinerary(str);
                Logout(signature);
            }
            catch(Exception ex)
            {
                
            }
        }
         

        /// <summary>
        /// Ok To Board
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="pnr"></param>
        public static void OKtoBoard(string signature,string pnr,string passengerName)
        {
            try
            {
                IBookingManager ibm = new BookingManagerClient();
                AddBookingCommentsRequest abcRequest = new AddBookingCommentsRequest();
                AddBookingCommentsResponse abcResponse = null;
                string oktoBoardComment = ConfigurationManager.AppSettings["oktoBoardComment"].ToString();
                //abcRequest.AddBookingCommentsReqData =new AddBookingCommentsRequestData[1];
                abcRequest.AddBookingCommentsReqData = new AddBookingCommentsRequestData();
                abcRequest.AddBookingCommentsReqData.RecordLocator = pnr;
                abcRequest.AddBookingCommentsReqData.BookingComments = new BookingComment[1];
                abcRequest.AddBookingCommentsReqData.BookingComments[0] = new BookingComment();

                abcRequest.AddBookingCommentsReqData.BookingComments[0].State = MessageState.New;
                abcRequest.AddBookingCommentsReqData.BookingComments[0].CommentType = CommentType.Manifest;
                abcRequest.AddBookingCommentsReqData.BookingComments[0].CommentText = oktoBoardComment+ "  "+passengerName; 
                abcRequest.AddBookingCommentsReqData.BookingComments[0].PointOfSale =null;

                abcRequest.AddBookingCommentsReqData.BookingComments[0].CreatedDate = System.DateTime.Now;
                abcRequest.Signature = signature;
                abcResponse = ibm.AddBookingComments(abcRequest);
                SaveXML(abcResponse, "OKtoBoardResponse.xml");
            }
            catch (Exception ex)
            {

            }
        }

        //************* End of PNR Generation*****************************************************

        /// <summary>
        /// Get Flight Info :Hold Time Details
        /// </summary>
        /// <param name="pnr"></param>
        public static DateTime GetFlightBooking_HoldTime(string pnr)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            string signature = Login();
            GetBookingRequest bookingRequest = new GetBookingRequest();
            bookingRequest.GetBookingReqData = new GetBookingRequestData();
            bookingRequest.Signature = signature;
            bookingRequest.GetBookingReqData.GetBookingBy = GetBookingBy.RecordLocator;
            bookingRequest.GetBookingReqData.GetByRecordLocator = new GetByRecordLocator();
            bookingRequest.GetBookingReqData.GetByRecordLocator.RecordLocator = pnr; 
            GetBookingResponse response = bookingManager.GetBooking(bookingRequest);
            Booking booking = response.Booking;
            var HoldTime = response.Booking.BookingHold.HoldDateTime;
            Logout(signature);
            return HoldTime;
        }


        /// <summary>
        /// Check PNR-Flight Booking Details
        /// </summary>
        /// <param name="pnr"></param>
        public static void getFlightBooking(string pnr)
        {
                IBookingManager bookingManager = new BookingManagerClient();
                string signature = Login();
                GetBookingRequest bookingRequest = new GetBookingRequest();
                bookingRequest.GetBookingReqData = new GetBookingRequestData();
                bookingRequest.Signature = signature;
                bookingRequest.GetBookingReqData.GetBookingBy = GetBookingBy.RecordLocator;
                bookingRequest.GetBookingReqData.GetByRecordLocator = new GetByRecordLocator();
                bookingRequest.GetBookingReqData.GetByRecordLocator.RecordLocator = pnr;//  
                GetBookingResponse response = bookingManager.GetBooking(bookingRequest);
                Booking booking = response.Booking;
                string bookingtype = response.Booking.Journeys[0].Segments[0].ActionStatusCode;
                decimal dueamount = response.Booking.BookingSum.AuthorizedBalanceDue;
                decimal totalcost = response.Booking.BookingSum.TotalCost;
                HttpContext.Current.Session["RequestId"] = "500281";
                //Save XML
                SaveXML(response, "getFlightBooking_Response.xml");
                Logout(signature);
        }

        /// <summary>
        /// CTEst Method
        /// </summary>
        /// <param name="pnr"></param>
        public static string getFlightBooking_Test(string pnr)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            string signature = Login();
            GetBookingRequest bookingRequest = new GetBookingRequest();
            bookingRequest.GetBookingReqData = new GetBookingRequestData();
            bookingRequest.Signature = signature;
            bookingRequest.GetBookingReqData.GetBookingBy = GetBookingBy.RecordLocator;
            bookingRequest.GetBookingReqData.GetByRecordLocator = new GetByRecordLocator();
            bookingRequest.GetBookingReqData.GetByRecordLocator.RecordLocator = pnr;//  
            GetBookingResponse response = bookingManager.GetBooking(bookingRequest);
            Booking booking = response.Booking;
            string bookingtype = response.Booking.Journeys[0].Segments[0].ActionStatusCode;
            decimal dueamount = response.Booking.BookingSum.AuthorizedBalanceDue;
            decimal totalcost = response.Booking.BookingSum.TotalCost;
            Logout(signature);
            return totalcost.ToString();
        }

        /// <summary>
        /// Check Flight Boarding Status
        /// </summary>
        /// <param name="pnr"></param>
        public static bool GetPNR_FlightBoardingStatus(string pnr, string sector)
        {
            string brsector = string.Empty;
            bool liftStatus = false;
            try
            {
                IBookingManager bookingManager = new BookingManagerClient();
                string signature = Login();
                GetBookingRequest bookingRequest = new GetBookingRequest();
                bookingRequest.GetBookingReqData = new GetBookingRequestData();
                bookingRequest.Signature = signature;
                bookingRequest.GetBookingReqData.GetBookingBy = GetBookingBy.RecordLocator;
                bookingRequest.GetBookingReqData.GetByRecordLocator = new GetByRecordLocator();
                bookingRequest.GetBookingReqData.GetByRecordLocator.RecordLocator = pnr;
                GetBookingResponse response = bookingManager.GetBooking(bookingRequest);
                Journey[] journeyList = response.Booking.Journeys;
                foreach (Journey journey in journeyList)
                {
                    foreach (Segment s in journey.Segments)
                    {
                        brsector = string.Empty;
                        brsector = s.DepartureStation + "-" + s.ArrivalStation;
                        if (brsector == sector)
                        {
                            liftStatus = s.PaxSegments[0].LiftStatus.ToString().ToLower().Equals("boarded") ? true : false;
                            if (liftStatus == true)
                            {
                                Logout(signature);
                                return liftStatus;
                            }
                        }
                    }
                }
                Logout(signature);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return liftStatus;
        }
    
    }
}