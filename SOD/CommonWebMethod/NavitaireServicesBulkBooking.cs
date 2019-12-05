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
    ////******** Navitaire API Integration ************************
    public class NavitaireServicesBulkBooking
    {
        //Global Variable Declaration for the Class
        public static string mDept = ConfigurationManager.AppSettings["Flight_Safety"].ToString().ToLower();
        public static string gflightType = string.Empty;

        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns></returns>
        public static string Login()
        {
            ISessionManager sessionManager = new SessionManagerClient();
            LogonRequest logonRequest = new LogonRequest();
            string username = ConfigurationManager.AppSettings["userid_naviapi"].ToString();
            string password = ConfigurationManager.AppSettings["password_naviapi"].ToString();
            string signature = "";
            try
            {
                string domain = "WWW";
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
                objLogging.AddDBLogging(ex, username + "_Login", "NavitaireServocesBulkBooking.cs");
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
        public static string GetAvailability1(string source, string destination, string ddate, string returndate, string waytype, string noofPassenger)
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
            ar.EndDate = new DateTime(year, month, day);
            ar.FlightType = FlightType.All;
            int pax = Convert.ToInt16(noofPassenger);
            ar.PaxCount = Convert.ToSByte(pax);
            ar.Dow = DOW.Daily;
            ar.CurrencyCode = "INR";
            ar.AvailabilityType = AvailabilityType.Standby;
            ar.MaximumConnectingFlights = 5;
            ar.AvailabilityFilter = AvailabilityFilter.Default;
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
                rar.AvailabilityType = AvailabilityType.Standby;
                rar.MaximumConnectingFlights = 5;
                rar.AvailabilityFilter = AvailabilityFilter.Default;
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
            GetAvailabilityResponse gare = bookingManager.GetAvailability(gar);
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
                        /*Satyam 29 July 2016 : to Manage direct Flights */
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
                                dr["TimeDuration"] = TimeDuration(journey.Segments[0].STD.ToString(), journey.Segments[journey.Segments.Length - 1].STA.ToString());
                                dr["FlightTypes"] = 3;
                                string[] arr = GetEquipment_Flight(journey.Segments, signature).ToString().Split('@');
                                dr["FlightInfos"] = arr[0];
                                dr["FlightNo"] = arr[1];
                                dr["FlightName"] = arr[2];
                                flights.Rows.Add(dr);
                            }
                        }
                        /*End of Satyam Code */
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
                                    dr["TimeDuration"] = TimeDuration(segment.STD.ToString(), segment.STA.ToString());
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
                ar.AvailabilityType = AvailabilityType.Standby;
                ar.MaximumConnectingFlights = 5;
                ar.AvailabilityFilter = AvailabilityFilter.Default;
                ar.FareClassControl = FareClassControl.LowestFareClass;
                ar.MinimumFarePrice = 0;
                ar.MaximumFarePrice = 0;
                ar.SSRCollectionsMode = SSRCollectionsMode.None;
                ar.InboundOutbound = InboundOutbound.None;
                ar.NightsStay = 0;
                ar.IncludeAllotments = false;
                ar.FareTypes = new string[1];// change
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
                            dr["TimeDuration"] = TimeDuration(journey.Segments[0].STD.ToString(), journey.Segments[journey.Segments.Length - 1].STA.ToString());
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
                                dr["TimeDuration"] = TimeDuration(segment.STD.ToString(), segment.STA.ToString());
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
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            int counter = 0;
            IBookingManager bookingManager = new BookingManagerClient();
            GetEquipmentPropertiesRequest getreqRequest = new GetEquipmentPropertiesRequest();
            EquipmentListRequest equRequest = new EquipmentListRequest();
            equRequest.EquipmentRequests = new EquipmentRequest[segs.Length];

            foreach (Segment seg in segs)
            {
                if (seg.Legs.Length > 1)
                {
                    for (int i = 0; i < seg.Legs.Length; i++)
                    {
                        equRequest.EquipmentRequests[counter] = new EquipmentRequest();
                        equRequest.EquipmentRequests[counter].ArrivalStation = seg.Legs[i].ArrivalStation;
                        equRequest.EquipmentRequests[counter].DepartureStation = seg.Legs[i].DepartureStation;
                        equRequest.EquipmentRequests[counter].EquipmentType = seg.Legs[i].LegInfo.EquipmentType;
                        equRequest.EquipmentRequests[counter].EquipmentTypeSuffix = seg.Legs[i].LegInfo.EquipmentTypeSuffix;
                        equRequest.EquipmentRequests[counter].DepartureDate = seg.Legs[i].STD;
                        equRequest.EquipmentRequests[counter].CarrierCode = seg.Legs[i].FlightDesignator.CarrierCode;
                        equRequest.EquipmentRequests[counter].FlightNumber = seg.Legs[i].FlightDesignator.FlightNumber;
                        equRequest.EquipmentRequests[counter].MarketingOverride = false;
                        getreqRequest.EquipmentListRequest = equRequest;
                        getreqRequest.Signature = signature;
                        getreqRequest.ContractVersion = 0;
                        if (i == 0)
                        {
                            str.Append(seg.Legs[i].FlightDesignator.CarrierCode + "" + seg.Legs[i].FlightDesignator.FlightNumber + "  <br/>");
                            flightno = flightno + seg.Legs[i].FlightDesignator.CarrierCode + "" + seg.Legs[i].FlightDesignator.FlightNumber + " , ";
                        }
                        else
                            str.Append(seg.Legs[i].FlightDesignator.CarrierCode + "" + seg.Legs[i].FlightDesignator.FlightNumber + "  (Same Aircraft) <br/>");

                        str.Append(seg.Legs[i].DepartureStation + "-" + seg.Legs[i].STD + " to " + seg.Legs[i].ArrivalStation + "-" + seg.Legs[i].STA + "  <br/>");
                        str.Append("The aircraft type is " + " " + "#AT" + counter + "  <br/>");
                        str.Append("Estimated travel time :" + TimeDuration(seg.Legs[i].STD.ToString(), seg.Legs[i].STA.ToString()) + "  <br/>");
                        if (i < seg.Legs.Length - 1)
                            str.Append("Layover Time :" + TimeDuration(seg.Legs[i + 1].STD.ToString(), seg.Legs[i].STA.ToString()) + "  <br/>");
                    }
                }
                else
                {
                    equRequest.EquipmentRequests[counter] = new EquipmentRequest();
                    equRequest.EquipmentRequests[counter].ArrivalStation = seg.ArrivalStation;
                    equRequest.EquipmentRequests[counter].DepartureStation = seg.DepartureStation;
                    equRequest.EquipmentRequests[counter].EquipmentType = seg.Legs[0].LegInfo.EquipmentType;
                    equRequest.EquipmentRequests[counter].EquipmentTypeSuffix = seg.Legs[0].LegInfo.EquipmentTypeSuffix;
                    equRequest.EquipmentRequests[counter].DepartureDate = seg.STD;
                    equRequest.EquipmentRequests[counter].CarrierCode = seg.FlightDesignator.CarrierCode;
                    equRequest.EquipmentRequests[counter].FlightNumber = seg.FlightDesignator.FlightNumber;
                    equRequest.EquipmentRequests[counter].MarketingOverride = false;
                    getreqRequest.EquipmentListRequest = equRequest;
                    getreqRequest.Signature = signature;
                    getreqRequest.ContractVersion = 0;

                    flightno = flightno + seg.FlightDesignator.CarrierCode + "" + seg.FlightDesignator.FlightNumber + " , ";
                    if (counter == 0)
                        str.Append(seg.FlightDesignator.CarrierCode + "" + seg.FlightDesignator.FlightNumber + "  <br/>");
                    else
                        str.Append(seg.FlightDesignator.CarrierCode + "" + seg.FlightDesignator.FlightNumber + "  (Change of Aircraft) <br/>");
                    str.Append(seg.DepartureStation + "-" + seg.STD + " to " + seg.ArrivalStation + "-" + seg.STA + "  <br/>");
                    str.Append("The aircraft type is " + " " + "#AT" + counter + "  <br/>");
                    str.Append("Estimated travel time :" + TimeDuration(seg.STD.ToString(), seg.STA.ToString()) + "  <br/>");
                    if (counter < segs.Length - 1)
                        str.Append("Layover Time :" + TimeDuration(segs[counter + 1].STD.ToString(), seg.STA.ToString()) + "  <br/>");
                }
                counter++;
            }

            GetEquipmentPropertiesResponse resp = bookingManager.GetEquipmentProperties(getreqRequest);
            counter = 0;
            foreach (EquipmentResponse res in resp.EquipmentListResponse.EquipmentResponses)
            {
                str.Replace("#AT" + counter, res.EquipmentInfo.Name);
                if (counter == 0)
                    flightName = res.EquipmentInfo.Name;
                else
                    flightName = flightName + "," + res.EquipmentInfo.Name;
                counter++;
            }
            str.Append("@" + flightno.Substring(0, flightno.Length - 2));
            str.Append("@" + flightName);
            return str.ToString();
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
                filename = HttpContext.Current.Session["RequestId"].ToString() + "_" + filename;
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType(), new Type[] { bfair.GetType() });
                TextWriter txtWriter = new StreamWriter(HttpContext.Current.Server.MapPath("~/BulkXml") + "//" + filename);
                xmlSerializer.Serialize(txtWriter, obj);
                txtWriter.Close();
            }
            catch (Exception ex)
            {

            }
        }


        //************ PNR Generation ****************************************************************
        /// <summary>
        /// Generate PNR
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        public static List<BulkEmployeeList> Generate_PNR(List<BulkUploadModels> bulkUpload, string reqId)
        {
            var pnrList = new List<BulkEmployeeList>();
            HttpContext.Current.Session["RequestId"] = reqId;
            try
            {
                //var sodflightList = bulkUpload.GroupBy(item => new { item.FlightNo, item.Sector, item.TravelDate,item.BookingType })
                // .Select(lst => lst.ToList()).ToList();
                
                foreach (var bulkList in bulkUpload)
                {
                    try
                    {
                        string signature = Login();
                        var paxCount = "1";
                        GetAvailabilityResponse availabilityrespone = GetAvailability_PNR(signature, bulkList, paxCount);
                        SellResponse sellresponse = Selling_PNR(signature, availabilityrespone, bulkList.AgencyCode, bulkList.BookingType, bulkList.EmpCode);
                        var equipmentType = availabilityrespone.GetTripAvailabilityResponse.Schedules[0][0].Journeys[0].Segments[0].Legs[0].LegInfo.EquipmentType.ToString().Trim();
                        //GetBooking From State Response only for test running:28-09-2018
                        GetBookingFromStateResponse bookingResp_AfterSell = GetBookingFrom_State(signature);
                        SellResponse sellresponse1 = Sell_SSR(signature, bookingResp_AfterSell, bulkList,equipmentType);
                        if (sellresponse1.BookingUpdateResponseData.OtherServiceInformations == null)
                            sellresponse = sellresponse1;
                        //GetBooking From State Response
                        GetBookingFromStateResponse bookingResp = GetBookingFrom_State(signature);
                        AddPaymentToBookingResponse paymentresponse = AddPayment_PNR_FromStateResponse(signature, bookingResp, bulkList.AgencyCode, bulkList.EmpCode);
                        var sdAgencyCode = ConfigurationManager.AppSettings["Bulk_Booking_SOD_SDFINAgencyCode"].ToString();
                        //Override FEE in case of COCKPIT & CREW Booking
                        if (bulkList.AgencyCode != sdAgencyCode)
                        {
                            OverrideFeeResponse resp = new OverrideFeeResponse();
                            resp = OverrideFee(signature, 0, 0, 0);
                            AddPaymentToBookingResponse paymentresponse1 = AddPayment_PNR_OverrideFee(signature, resp, bulkList.AgencyCode);
                        }
                        BookingCommitResponse commitresponse = BookingCommit_PNR(signature, bulkList, bulkList.AgencyCode);
                        string pnr = commitresponse.BookingUpdateResponseData.Success.RecordLocator;
                        string amount = commitresponse.BookingUpdateResponseData.Success.PNRAmount.TotalCost.ToString().Trim();
                        pnr = pnr + "|" + amount;
                        if (!pnr.Equals("ERR001"))
                        {
                            var list = new BulkEmployeeList();
                            list.EmpCode = bulkList.EmpCode;
                            list.PNRStatus = pnr;
                            list.BTrId = bulkList.BReqId;
                            list.SrNo = bulkList.SrNo;
                            pnrList.Add(list);
                        }
                    }
                    catch (Exception ex)
                    {
                        var list = new BulkEmployeeList();
                        list.EmpCode = bulkList.EmpCode;
                        list.PNRStatus = "ERR001|0.00";
                        list.BTrId = bulkList.BReqId;
                        list.SrNo = bulkList.SrNo;
                        pnrList.Add(list);
                        ErrorLog objLogging = new ErrorLog();
                        objLogging.AddDBLogging(ex, list.EmpCode + "_foreach-bulkUpload/Generate_PNR", "NavitaireServicesBulkBooking.cs");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "Generate_PNR", "CommonWebMethod/NavitaireServocesBulkBooking.cs");
            }
            return pnrList;
        }



        /// <summary>
        /// Get Booking from State
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static GetBookingFromStateResponse GetBookingFrom_State(string signature)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            GetBookingFromStateRequest req = new GetBookingFromStateRequest();
            GetBookingFromStateResponse res = new GetBookingFromStateResponse();
            req.Signature = signature;
            req.ContractVersion = 0;
            res = bookingManager.GetBookingFromState(req);

            return res;
        }



        /// <summary>
        /// Add Payment PNR GetBookingFromStateResponse
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="dept"></param>
        /// <param name="IsAmountPaidByTraveller"></param>
        public static AddPaymentToBookingResponse AddPayment_PNR_FromStateResponse(string signature, GetBookingFromStateResponse response, string agencyCode, string EmpCode)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            AddPaymentToBookingRequest addreq = new AddPaymentToBookingRequest();
            AddPaymentToBookingRequestData addreqdata = new AddPaymentToBookingRequestData();
            AddPaymentToBookingResponse aresponse = new AddPaymentToBookingResponse();

            addreqdata.MessageState = MessageState.New;
            addreqdata.WaiveFee = false;
            addreqdata.ReferenceType = PaymentReferenceType.Default;
            addreqdata.PaymentMethodType = RequestPaymentMethodType.AgencyAccount;
            addreqdata.PaymentMethodCode = "AG";
            addreqdata.QuotedCurrencyCode = "INR";
            addreqdata.QuotedAmount = response.BookingData.BookingSum.BalanceDue;
            addreqdata.Status = BookingPaymentStatus.New;
            addreqdata.AccountNumberID = 0;
            addreqdata.AccountNumber = agencyCode;

            //addreqdata.Expiration = DateTime.Today;
            addreqdata.ParentPaymentID = 0;
            addreqdata.Installments = 0;
            addreqdata.Deposit = false;

            addreq.addPaymentToBookingReqData = addreqdata;
            addreq.ContractVersion = 0;
            addreq.Signature = signature;

            SaveXML(addreq, EmpCode + "_AddPaymentToBookingRequestBulk.xml");
            aresponse = bookingManager.AddPaymentToBooking(addreq);
            SaveXML(aresponse, EmpCode + "_AddPaymentToBookingResponseBulk.xml");

            return aresponse;
        }



        /// <summary>
        /// GetBooking From StateResponse
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="contVersion"></param>
        /// <returns></returns>
        public static GetBookingFromStateResponse GetBookingFrom_StateResponse(string signature, int contVersion)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            GetBookingFromStateRequest req = new GetBookingFromStateRequest();
            GetBookingFromStateResponse resp = new GetBookingFromStateResponse();

            req.Signature = signature;
            req.ContractVersion = contVersion;
            resp = bookingManager.GetBookingFromState(req);
            return resp;
        }



        /// <summary>
        /// Overried Fee for Meal
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="PassengerNumber"></param>
        /// <param name="FeeNumber"></param>
        /// <param name="NetAmount"></param>
        /// Currently not in Use
        public static OverrideFeeResponse OverrideFee(string signature, short PassengerNumber, short FeeNumber, decimal NetAmount)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            OverrideFeeRequest ofeereq = new OverrideFeeRequest();
            ofeereq.FeeRequest = new FeeRequest();
            OverrideFeeResponse resp = new OverrideFeeResponse();

            FeeRequest req = new FeeRequest();
            req.PassengerNumber = PassengerNumber;
            req.FeeNumber = FeeNumber;
            req.NetAmount = NetAmount;
            req.CurrencyCode = "INR";
            ofeereq.Signature = signature;
            ofeereq.FeeRequest = req;
            ofeereq.ContractVersion = 0;

            resp = bookingManager.OverrideFee(ofeereq);
            SaveXML(ofeereq, "OverrideFee_Request_Bulk.xml");
            SaveXML(resp, "OverrideFee_Response_Bulk.xml");

            return resp;
        }



        /// <summary>
        /// Check Availability PNR
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <returns></returns>
        public static GetAvailabilityResponse GetAvailability_PNR(string signature, BulkUploadModels item, string paxCount)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            GetAvailabilityRequest gar = new GetAvailabilityRequest();
            TripAvailabilityRequest tar = new TripAvailabilityRequest();

            tar.AvailabilityRequests = new AvailabilityRequest[1];
            AvailabilityRequest ar = new AvailabilityRequest();
            ar.DepartureStation = item.Sector.Split('-')[0].Trim();
            ar.ArrivalStation = item.Sector.Split('-')[1].Trim();
            string[] date = item.TravelDate.Split('/');
            var year = Convert.ToInt32(date[2]);
            var month = Convert.ToInt32(date[1]);
            var day = Convert.ToInt32(date[0]);

            DateTime d1 = new DateTime(year, month, day);
            ar.BeginDate = d1;
            ar.EndDate = d1;
            ar.FlightType = FlightType.All;

            //Manage Flight-----------------------------------------------
            if (item.FlightNo.Length >= 5)
            {
                string flight = item.FlightNo.Split('/')[0];
                if (flight.Length == 4)
                    ar.FlightNumber = flight;

                else if (flight.Length == 3)
                    ar.FlightNumber = " " + flight;

                else if (flight.Length == 2)
                    ar.FlightNumber = "  " + flight;
                else
                    ar.FlightNumber = "   " + flight;
            }
            else if (item.FlightNo.Length == 4)
                ar.FlightNumber = item.FlightNo.Split('/')[0];
            else if (item.FlightNo.Length == 3)
                ar.FlightNumber = " " + item.FlightNo.Split('/')[0];
            else if (item.FlightNo.Length == 2)
                ar.FlightNumber = "  " + item.FlightNo.Split('/')[0];
            else
                ar.FlightNumber = "   " + item.FlightNo.Split('/')[0];
            //-------------------------------------------------------------

            ar.PaxCount = short.Parse(paxCount);
            ar.Dow = DOW.Daily;
            ar.CurrencyCode = "INR";
            ar.AvailabilityType = item.BookingType.Equals("Confirm") ? AvailabilityType.Overbook : AvailabilityType.Standby;
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
            tar.AvailabilityRequests[0] = ar;


            tar.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
            tar.LowFareMode = false;
            gar.TripAvailabilityRequest = tar;
            gar.Signature = signature;
            gar.ContractVersion = 0;

            GetAvailabilityResponse gare = new GetAvailabilityResponse();
            gare = bookingManager.GetAvailability(gar);
            SaveXML(gar, item.EmpCode + "_GetAvailabilityRequest_Bulk.xml");
            SaveXML(gare, item.EmpCode + "_GetAvailabilityResponse_Bulk.xml");

            return gare;
        }

        /// <summary>
        /// Showing breakup of fair
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="sodRequestsList"></param>
        /// <returns></returns>
        public static PriceItineraryResponse GetPrice_PNR(string signature, GetAvailabilityResponse response, string paxCount, string bookingfor, string fltoMatch, string EmpCode)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            PriceItineraryRequest pir = new PriceItineraryRequest();
            ItineraryPriceRequest ipr = new ItineraryPriceRequest();
            PriceItineraryResponse pire = new PriceItineraryResponse();

            ipr.PriceItineraryBy = PriceItineraryBy.JourneyBySellKey;
            ipr.SellByKeyRequest = new SellJourneyByKeyRequestData();
            ipr.SellByKeyRequest.ActionStatusCode = bookingfor.Equals("Confirm") ? "SS" : "LL";
            SellKeyList[] list = new SellKeyList[response.GetTripAvailabilityResponse.Schedules.Length];
            int l = 0;

            foreach (JourneyDateMarket[] jdmArray in response.GetTripAvailabilityResponse.Schedules)
            {
                list[l] = new SellKeyList();
                foreach (JourneyDateMarket jdm in jdmArray)
                {
                    foreach (Journey journey in jdm.Journeys)
                    {
                        string flightNo = "";
                        list[l].JourneySellKey = journey.JourneySellKey;
                        foreach (Segment segment in journey.Segments)
                        {
                            if (flightNo == "")
                                flightNo = segment.FlightDesignator.FlightNumber.Trim();
                            else
                                flightNo = flightNo + "/" + segment.FlightDesignator.FlightNumber.Trim();
                        }
                        if (flightNo == fltoMatch)
                        {
                            foreach (Segment segment in journey.Segments)
                            {
                                if (segment.Fares.Length > 0)
                                {
                                    list[l].FareSellKey = string.IsNullOrEmpty(list[l].FareSellKey) ? segment.Fares[0].FareSellKey : list[l].FareSellKey + "^" + segment.Fares[0].FareSellKey;
                                }
                            }
                            gflightType = GetFlightType(signature, journey.Segments);
                        }
                    }
                }
                l++;
            }

            ipr.SellByKeyRequest.JourneySellKeys = list;
            ipr.SellByKeyRequest.PaxCount = short.Parse(paxCount);
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
            pire = bookingManager.GetItineraryPrice(pir);

            SaveXML(pir, EmpCode + "_PriceItineraryRequest_Bulk.xml");
            SaveXML(pire, EmpCode + "_PriceItineraryResponse_Bulk.xml");
            return pire;
        }


        /// <summary>
        /// Selling PNR Method
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <param name="bookingfor"></param>
        /// <returns></returns>
        public static SellResponse Selling_PNR(string signature, GetAvailabilityResponse response, string agencyCode, string bookingfor, string EmpCode)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            SellRequest sr = new SellRequest();
            SellRequestData srd = new SellRequestData();
            srd.SellBy = SellBy.JourneyBySellKey;
            SellJourneyByKeyRequest sjr = new SellJourneyByKeyRequest();
            SellJourneyByKeyRequestData skd = new SellJourneyByKeyRequestData();
            SellResponse sre = new SellResponse();
            skd.ActionStatusCode = bookingfor.Equals("Confirm") ? "SS" : "LL";

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
                        if (bookingfor.ToLower() == "standby")
                            list[l].StandbyPriorityCode = "NR";
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
            ps.AgentCode = agencyCode;
            ps.OrganizationCode = agencyCode;
            ps.DomainCode = "WWW";
            ps.LocationCode = "WWW";
            skd.SourcePOS = ps;

            skd.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
            skd.IsAllotmentMarketFare = false;

            sjr.SellJourneyByKeyRequestData = skd;
            srd.SellJourneyByKeyRequest = sjr;
            sr.Signature = signature;
            sr.SellRequestData = srd;
            sre = bookingManager.Sell(sr);

            SaveXML(sr, EmpCode + "_SellRequest_Bulk.xml");
            SaveXML(sre, EmpCode + "_SellResponse_Bulk.xml");
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
        public static SellResponse Sell_SSR(string signature, GetBookingFromStateResponse response, BulkUploadModels bulkList, string equipmentType)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            SellRequest selre = new SellRequest();
            SellRequestData selredata = new SellRequestData();
            SellResponse sre = new SellResponse();
            selredata.SellBy = SellBy.SSR;
            selredata.SellSSR = new SellSSR();
            selredata.SellSSR.SSRRequest = new SSRRequest();
            selredata.SellSSR.SSRRequest.SegmentSSRRequests = new SegmentSSRRequest[response.BookingData.Journeys.Length];
            int l = 0;

            Journey jouney = response.BookingData.Journeys[0];
            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l] = new SegmentSSRRequest();
            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].FlightDesignator = jouney.Segments[0].FlightDesignator;
            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].STD = jouney.Segments[0].STD;

            //Meal will be allocated for only the first Leg as per business policy
            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].DepartureStation = jouney.Segments[0].Legs[0].DepartureStation;
            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].ArrivalStation = jouney.Segments[0].Legs[0].ArrivalStation;

            int mealCount = 0;
            if (bulkList.Meal.Trim() != "NA")
                mealCount = mealCount + 1;

            if (bulkList.Beverage.Trim() != "NA")
                mealCount = mealCount + 1;

            selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs = new PaxSSR[mealCount];
            bool flag = false;

            for (short i = 0; i < mealCount; i++)
            {
                selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i] = new PaxSSR();
                selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].State = MessageState.New;
                if (bulkList.BookingType.ToLower() == "standby")
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
                selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].PassengerNumber = 0;

                if (bulkList.Beverage.Trim() == "BVG" && flag == false)
                {
                    selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = bulkList.Beverage;
                    selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRNumber = 0;
                    selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRValue = 0;
                    flag = true;
                }
                else
                {
                    if (bulkList.Meal != "NA")
                    {
                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRCode = AllocateMeal(bulkList.Meal, equipmentType);
                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRNumber = 0;
                        selredata.SellSSR.SSRRequest.SegmentSSRRequests[l].PaxSSRs[i].SSRValue = 0;
                    }
                }
            }

            selredata.SellSSR.SSRRequest.CurrencyCode = "INR";
            selredata.SellSSR.SSRRequest.CancelFirstSSR = false;
            selredata.SellSSR.SSRRequest.SSRFeeForceWaiveOnSell = false;
            selre.ContractVersion = 0;
            selre.Signature = signature;
            selre.SellRequestData = selredata;

            SaveXML(selre, bulkList.EmpCode + "_SELLSSRRequest_Bulk.xml");
            sre = bookingManager.Sell(selre);
            SaveXML(sre, bulkList.EmpCode + "_SELLSSRResponse_Bulk.xml");
            return sre;
        }


        /// <summary>
        /// To Get Flight Type
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="seg"></param>
        /// <returns></returns>
        public static string GetFlightType(string signature, Segment[] segs)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            GetEquipmentPropertiesRequest getreqRequest = new GetEquipmentPropertiesRequest();
            EquipmentListRequest equRequest = new EquipmentListRequest();
            equRequest.EquipmentRequests = new EquipmentRequest[1];
            string flightName = string.Empty;

            foreach (Segment seg in segs)
            {
                equRequest.EquipmentRequests[0] = new EquipmentRequest();
                equRequest.EquipmentRequests[0].ArrivalStation = seg.ArrivalStation;
                equRequest.EquipmentRequests[0].DepartureStation = seg.DepartureStation;
                equRequest.EquipmentRequests[0].EquipmentType = seg.Legs[0].LegInfo.EquipmentType;
                equRequest.EquipmentRequests[0].EquipmentTypeSuffix = seg.Legs[0].LegInfo.EquipmentTypeSuffix;
                equRequest.EquipmentRequests[0].DepartureDate = seg.STD;
                equRequest.EquipmentRequests[0].CarrierCode = seg.FlightDesignator.CarrierCode;
                equRequest.EquipmentRequests[0].FlightNumber = seg.FlightDesignator.FlightNumber;
                equRequest.EquipmentRequests[0].MarketingOverride = false;
                getreqRequest.EquipmentListRequest = equRequest;
                getreqRequest.Signature = signature;
                getreqRequest.ContractVersion = 0;
            }
            GetEquipmentPropertiesResponse resp = bookingManager.GetEquipmentProperties(getreqRequest);
            foreach (EquipmentResponse res in resp.EquipmentListResponse.EquipmentResponses)
            {
                flightName = res.EquipmentInfo.Name.ToString();
            }

            return flightName;
        }


        /// <summary>
        /// Allocate Meal
        /// </summary>
        /// <returns></returns>
        public static string AllocateMeal(string mealCodeRequest, string equipmentType)
        {
            string mealResponseCode = string.Empty;
            string[] equiptypes = ConfigurationManager.AppSettings["Q400EquipmentType"].Split(',');
            foreach (var equipName in equiptypes)
            {
                if (equipmentType == equipName)
                {
                    if (mealCodeRequest.Equals("VGML"))
                        mealResponseCode = "VGSW";
                    else
                        mealResponseCode = "NVSW";
                }

            }

            if (mealResponseCode.Equals(string.Empty))
                mealResponseCode = mealCodeRequest;

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
        /// Add PaymentToBookingResponse Method with Override Fee Response
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        /// Currently not in Use
        public static AddPaymentToBookingResponse AddPayment_PNR_OverrideFee(string signature, OverrideFeeResponse response, string agencyCode)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            AddPaymentToBookingRequest addreq = new AddPaymentToBookingRequest();
            AddPaymentToBookingRequestData addreqdata = new AddPaymentToBookingRequestData();
            AddPaymentToBookingResponse aresponse = new AddPaymentToBookingResponse();

            addreqdata.MessageState = MessageState.New;
            addreqdata.WaiveFee = false;
            addreqdata.ReferenceType = PaymentReferenceType.Default;
            addreqdata.PaymentMethodType = RequestPaymentMethodType.AgencyAccount;
            addreqdata.PaymentMethodCode = "AG";
            addreqdata.QuotedCurrencyCode = "INR";
            addreqdata.QuotedAmount = response.Booking.BookingSum.TotalCost;
            addreqdata.Status = BookingPaymentStatus.New;
            addreqdata.AccountNumberID = 0;
            addreqdata.AccountNumber = agencyCode;
            //addreqdata.Expiration = DateTime.Today;
            addreqdata.ParentPaymentID = 0;
            addreqdata.Installments = 0;
            addreqdata.Deposit = false;

            addreq.addPaymentToBookingReqData = addreqdata;
            addreq.ContractVersion = 0;
            addreq.Signature = signature;

            SaveXML(addreq, "AddPaymentToBookingRequest_Bulk.xml");
            aresponse = bookingManager.AddPaymentToBooking(addreq);
            SaveXML(aresponse, "AddPaymentToBookingResponse_Bulk.xml");

            return aresponse;
        }


        /// <summary>
        /// Add PaymentToBookingResponse Method
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        /// Currenlty not in use
        public static AddPaymentToBookingResponse AddPayment_PNR(string signature, SellResponse response, string agencyCode)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            AddPaymentToBookingRequest addreq = new AddPaymentToBookingRequest();
            AddPaymentToBookingRequestData addreqdata = new AddPaymentToBookingRequestData();
            AddPaymentToBookingResponse aresponse = new AddPaymentToBookingResponse();

            addreqdata.MessageState = MessageState.New;
            addreqdata.WaiveFee = false;
            addreqdata.ReferenceType = PaymentReferenceType.Default;
            addreqdata.PaymentMethodType = RequestPaymentMethodType.AgencyAccount;
            addreqdata.PaymentMethodCode = "AG";
            addreqdata.QuotedCurrencyCode = "INR";
            addreqdata.QuotedAmount = response.BookingUpdateResponseData.Success.PNRAmount.TotalCost;
            addreqdata.Status = BookingPaymentStatus.New;
            addreqdata.AccountNumberID = 0;
            addreqdata.AccountNumber = agencyCode;
            //addreqdata.Expiration = DateTime.Today;
            addreqdata.ParentPaymentID = 0;
            addreqdata.Installments = 0;
            addreqdata.Deposit = false;
            addreq.addPaymentToBookingReqData = addreqdata;
            addreq.ContractVersion = 0;
            addreq.Signature = signature;

            SaveXML(addreq, "AddPaymentToBookingRequest_Bulk.xml");
            aresponse = bookingManager.AddPaymentToBooking(addreq);
            SaveXML(aresponse, "AddPaymentToBookingResponse_Bulk.xml");

            return aresponse;
        }


        /// <summary>
        /// Booking Commit PNR :Final PNR Generation Method
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="sodRequestsList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        public static BookingCommitResponse BookingCommit_PNR(string signature, BulkUploadModels lstBulkUpload, string agencyCode)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            BookingCommitRequest breq = new BookingCommitRequest();
            BookingCommitRequestData breqdata = new BookingCommitRequestData();
            BookingCommitResponse bresponse = null;

            breqdata.State = MessageState.New;
            breqdata.CurrencyCode = "INR";
            breqdata.PaxCount = short.Parse("1");
            breqdata.BookingID = 0;
            breqdata.BookingParentID = 0;
            breqdata.BookingComments = new BookingComment[1];
            breqdata.BookingComments[0] = new BookingComment();

            var userInfo = Services.ADO.SodCommonServices.GetBulkBookingUserInfoByTransactionId(lstBulkUpload.TrnId, "", 1);

            breqdata.BookingComments[0].CommentText = agencyCode + " - " + lstBulkUpload.BookingType + " | " + userInfo[0].EmpCode + " | " + userInfo[0].EmpName + " | " + userInfo[0].Designation + " | " + userInfo[0].Department + " | " + userInfo[0].Phone + " | " + userInfo[0].Email;
            PointOfSale ps = new PointOfSale();
            ps.State = MessageState.New;
            ps.AgentCode = agencyCode;
            ps.OrganizationCode = agencyCode;
            ps.DomainCode = "WWWW";
            ps.LocationCode = "WWW";
            breqdata.SourcePOS = ps;
            breqdata.Passengers = new Passenger[breqdata.PaxCount];

            breqdata.Passengers[0] = new Passenger();
            breqdata.Passengers[0].PassengerNumber = 0;
            breqdata.Passengers[0].FamilyNumber = 0;
            breqdata.Passengers[0].State = MessageState.Modified;
            breqdata.Passengers[0].Names = new BookingName[1];
            breqdata.Passengers[0].Names[0] = new BookingName();
            breqdata.Passengers[0].Names[0].State = MessageState.New;
            breqdata.Passengers[0].Names[0].FirstName = lstBulkUpload.FirstName;

            if (lstBulkUpload.LastName == "." || lstBulkUpload.LastName == "")
                breqdata.Passengers[0].Names[0].LastName = lstBulkUpload.FirstName;
            else
                breqdata.Passengers[0].Names[0].LastName = lstBulkUpload.LastName;

            breqdata.Passengers[0].Names[0].Title = lstBulkUpload.Title;
            breqdata.Passengers[0].PassengerInfos = new PassengerInfo[1];
            breqdata.Passengers[0].PassengerInfos[0] = new PassengerInfo();
            if (lstBulkUpload.Title.Trim().Equals("Mr"))
            {
                breqdata.Passengers[0].PassengerInfos[0].Gender = Gender.Male;
                breqdata.Passengers[0].PassengerInfos[0].WeightCategory = WeightCategory.Male;
            }
            else
            {
                breqdata.Passengers[0].PassengerInfos[0].Gender = Gender.Female;
                breqdata.Passengers[0].PassengerInfos[0].WeightCategory = WeightCategory.Female;
            }

            breqdata.BookingContacts = new BookingContact[1];
            breqdata.BookingContacts[0] = new BookingContact();
            breqdata.BookingContacts[0].CompanyName = "SpiceJet";
            breqdata.BookingContacts[0].Names = new BookingName[1];
            breqdata.BookingContacts[0].Names[0] = new BookingName();
            breqdata.BookingContacts[0].Names[0].Title = lstBulkUpload.Title;
            breqdata.BookingContacts[0].Names[0].FirstName = lstBulkUpload.FirstName;

            if (lstBulkUpload.LastName == "." || lstBulkUpload.LastName == "")
                breqdata.BookingContacts[0].Names[0].LastName = lstBulkUpload.FirstName;
            else
                breqdata.BookingContacts[0].Names[0].LastName = lstBulkUpload.LastName;


            breqdata.BookingContacts[0].AddressLine1 = "SpiceJet Ltd.| 319 | Udyog Vihar | Phase IV";
            breqdata.BookingContacts[0].City = "Gurgaon";
            breqdata.BookingContacts[0].ProvinceState = "HR";
            breqdata.BookingContacts[0].CountryCode = "IN";

            breqdata.BookingContacts[0].EmailAddress = lstBulkUpload.EmailId;
            breqdata.BookingContacts[0].HomePhone = lstBulkUpload.MobileNo;

            breqdata.BookingContacts[0].PostalCode = "122016";
            breqdata.BookingContacts[0].NotificationPreference = NotificationPreference.None;
            breqdata.BookingContacts[0].SourceOrganization = agencyCode;

            //New Add
            breqdata.BookingContacts[0].DistributionOption = DistributionOption.Email;
            breqdata.BookingContacts[0].TypeCode = "P";
            breqdata.RestrictionOverride = false;
            breqdata.ChangeHoldDateTime = false;
            breqdata.WaiveNameChangeFee = false;
            breqdata.WaivePenaltyFee = false;
            breqdata.WaiveSpoilageFee = false;
            //4x
            breqdata.DistributeToContacts = true;
            breq.BookingCommitRequestData = breqdata;
            breq.ContractVersion = 0;
            breq.Signature = signature;
            bresponse = bookingManager.BookingCommit(breq);

            SaveXML(breq, lstBulkUpload.EmpCode + "_BookingCommitRequest_Bulk.xml");
            SaveXML(bresponse, lstBulkUpload.EmpCode + "_BookingCommitResponse_Bulk.xml");
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
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, response.BookingUpdateResponseData.Success.RecordLocator + "_SentItinerary_PNR", "NavitaireServocesBulkBooking.cs");
            }
        }
        //************* End of PNR Generation*****************************************************
    }
}