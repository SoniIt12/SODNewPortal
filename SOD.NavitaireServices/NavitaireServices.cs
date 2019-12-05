using System;
using System.Data;
using System.Web;
using SOD.NavitaireServices.BookingManager;
using SOD.NavitaireServices.SessionManager;

namespace SOD.NavitaireServices
{
    public class NavitaireServices
    {
        ////******** Navitaire API Integration ************************
       
      public static string Login()
        {
            ISessionManager sessionManager = new SessionManagerClient();
            LogonRequest logonRequest = new LogonRequest();
            string username = "nonspicdes"; //ConfigurationManager.AppSettings["userid_fapi"].ToString();
            string password = "Spiced@123";//ConfigurationManager.AppSettings["password_fapi"].ToString();
            string domain = "WWW";
            logonRequest.logonRequestData = new LogonRequestData();
            logonRequest.logonRequestData.DomainCode = domain;
            logonRequest.logonRequestData.AgentName = username;
            logonRequest.logonRequestData.Password = password;
            LogonResponse logonResponse = sessionManager.Logon(logonRequest);
            string signature = "";
            if (logonResponse != null && logonResponse.Signature != null && logonResponse.Signature != string.Empty)
            {
                signature = logonResponse.Signature;
            }
            else
            {
                //Response.Write("Agent Authentication Failed.");
                //ScriptManager.RegisterStartupScript(Page, GetType(), "ab", "alert('Agent Authentication Failed.');", true);

            }
            return signature;
        }
        public static void Logout(string signature)
        {
            ISessionManager sessionManager = new SessionManagerClient();
            LogoutRequest logoutRequest = new LogoutRequest();
            logoutRequest.Signature = signature;
            sessionManager.Logout(logoutRequest);
        }

        public static void GetAvailability1(string source, string destination, string ddate,string returndate, string waytype, string noofPassenger)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            string signature = Login();
            GetAvailabilityRequest gar = new GetAvailabilityRequest();
            TripAvailabilityRequest tar = new TripAvailabilityRequest();
            AvailabilityRequest ar = new AvailabilityRequest();
            ar.DepartureStation = source;// "DEL";// HttpContext.Current.Session["origin"].ToString();
            ar.ArrivalStation = destination;// "BOM";// HttpContext.Current.Session["destination"].ToString();
            //string[] date = ddate.Split('/');// HttpContext.Current.Session["ddate"].ToString().Split('/');
            //int year = int.Parse(date[2]), month = int.Parse(date[1]), day = int.Parse(date[0]);
            int year = int.Parse(ddate.Substring(0, 4)), month = int.Parse(ddate.Substring(4, 2)), day = int.Parse(ddate.Substring(6));
            ar.BeginDate = new DateTime(year, month, day); //Convert.ToDateTime("2015-07-07");
            ar.EndDate = new DateTime(year, month, day);
            ar.FlightType = FlightType.Direct;
            //ar.FlightNumber = b.FlightNo.Length == 3 ? " " + b.FlightNo : b.FlightNo;
            int pax = Convert.ToInt16(noofPassenger);//+ Convert.ToInt16(page.child);
            ar.PaxCount = Convert.ToSByte(pax);
            ar.Dow = DOW.Daily;
            ar.CurrencyCode = "INR";
            ar.AvailabilityType = AvailabilityType.Default; //change 15 oct 2015
            ar.MaximumConnectingFlights = 5;
            ar.AvailabilityFilter = AvailabilityFilter.Default;//change
            ar.FareClassControl = FareClassControl.LowestFareClass;
            ar.MinimumFarePrice = 0;
            ar.MaximumFarePrice = 0;
            ar.SSRCollectionsMode = SSRCollectionsMode.None;
            ar.InboundOutbound = InboundOutbound.None;
            ar.NightsStay = 0;
            ar.IncludeAllotments = false;
            ar.FareTypes = new string[1];// change
            ar.FareTypes[0] = "R";

            //ar.FareTypes = new string[1];// change
            //ar.FareTypes[0] = "BD";
            //ar.ProductClasses = new string[1];
            //ar.ProductClasses[0] = "BD";
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
                rar.DepartureStation = destination;// "DEL";// HttpContext.Current.Session["origin"].ToString();
                rar.ArrivalStation = source;// "BOM";// HttpContext.Current.Session["destination"].ToString();
                //string[] date = ddate.Split('/');// HttpContext.Current.Session["ddate"].ToString().Split('/');
                //int year = int.Parse(date[2]), month = int.Parse(date[1]), day = int.Parse(date[0]);
                int ryear = int.Parse(returndate.Substring(0, 4)), rmonth = int.Parse(returndate.Substring(4, 2)), rday = int.Parse(returndate.Substring(6));
                rar.BeginDate = new DateTime(ryear, rmonth, rday); //Convert.ToDateTime("2015-07-07");
                rar.EndDate = new DateTime(ryear, rmonth, rday);
                rar.FlightType = FlightType.Direct;
                //ar.FlightNumber = b.FlightNo.Length == 3 ? " " + b.FlightNo : b.FlightNo;
                int rpax = Convert.ToInt16(noofPassenger);//+ Convert.ToInt16(page.child);
                rar.PaxCount = Convert.ToSByte(rpax);
                rar.Dow = DOW.Daily;
                rar.CurrencyCode = "INR";
                rar.AvailabilityType = AvailabilityType.Default; //change 15 oct 2015
                rar.MaximumConnectingFlights = 5;
                rar.AvailabilityFilter = AvailabilityFilter.Default;//change
                rar.FareClassControl = FareClassControl.LowestFareClass;
                rar.MinimumFarePrice = 0;
                rar.MaximumFarePrice = 0;
                rar.SSRCollectionsMode = SSRCollectionsMode.None;
                rar.InboundOutbound = InboundOutbound.None;
                rar.NightsStay = 0;
                rar.IncludeAllotments = false;
                rar.FareTypes = new string[1];// change
                rar.FareTypes[0] = "R";

                //ar.FareTypes = new string[1];// change
                //ar.FareTypes[0] = "BD";
                //ar.ProductClasses = new string[1];
                //ar.ProductClasses[0] = "BD";
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
            //if (waytype == "1")
            GetPrice(signature, gare,noofPassenger);
            //if (waytype == "2")
            //    GetPriceReturn(signature, gare,page);
        }
        public static void GetPrice(string signature, GetAvailabilityResponse response, string noofPassenger)
        {
            IBookingManager bookingManager = new BookingManagerClient();

            int counter = 0;
            foreach (JourneyDateMarket[] jdmArray in response.GetTripAvailabilityResponse.Schedules)
            {
                DataTable flights = Table_Flights();
                DataTable fare = Table_Fare();
                foreach (JourneyDateMarket jdm in jdmArray)
                {
                    foreach (Journey journey in jdm.Journeys)
                    {
                        PriceItineraryRequest pir = new PriceItineraryRequest();
                        ItineraryPriceRequest ipr = new ItineraryPriceRequest();
                        ipr.PriceItineraryBy = PriceItineraryBy.JourneyBySellKey;
                        ipr.SellByKeyRequest = new SellJourneyByKeyRequestData();
                        ipr.SellByKeyRequest.ActionStatusCode = "NN";//change
                        SellKeyList[] list = new SellKeyList[1];
                        list[0] = new SellKeyList();
                        list[0].JourneySellKey = journey.JourneySellKey;
                        foreach (Segment segment in journey.Segments)
                        {
                            DataRow dr = flights.NewRow();
                            if (segment.Fares.Length > 0) //for international 23 oct
                            {
                                list[0].FareSellKey = segment.Fares[0].FareSellKey;
                                dr["ShortDate"] = segment.STD.ToString("ddd, MMM d yyyy");
                                dr["Date"] = segment.STD.ToShortDateString();
                                dr["From"] = segment.DepartureStation;
                                dr["To"] = segment.ArrivalStation;
                                dr["FlightNo"] = segment.FlightDesignator.FlightNumber;
                                dr["STD"] = segment.STD.ToString("dd/MM/yyy hh:mm tt");
                                dr["STA"] = segment.STA.ToString("dd/MM/yyy hh:mm tt");
                                dr["BaseFare"] = segment.Fares[0].PaxFares[0].ServiceCharges[0].Amount;
                                dr["TimeDuration"] = TimeDuration(segment.STD.ToString(), segment.STA.ToString());
                                flights.Rows.Add(dr);
                            }
                        }


                        ipr.SellByKeyRequest.JourneySellKeys = list;
                        int pax = Convert.ToInt16(noofPassenger);// + Convert.ToInt16(page.child);
                        ipr.SellByKeyRequest.PaxCount = Convert.ToSByte(pax);
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
                        PriceItineraryResponse pire = bookingManager.GetItineraryPrice(pir);
                        foreach (Journey j in pire.Booking.Journeys)
                        {
                            foreach (Segment segment in j.Segments)
                            {
                                foreach (BookingServiceCharge bsc in segment.Fares[0].PaxFares[0].ServiceCharges)
                                {
                                    DataRow drr = fare.NewRow();
                                    drr["FlightNo"] = segment.FlightDesignator.FlightNumber;
                                    drr["Heads"] = bsc.ChargeType.ToString() + bsc.ChargeCode.ToString();
                                    drr["Amount"] = bsc.Amount * Convert.ToDecimal(pax);
                                    fare.Rows.Add(drr);
                                }
                            }
                        }
                    }
                }
                foreach (DataRow dr in flights.Rows)
                {
                    dr["TotalFare"] = TotalFare(fare, dr["FlightNo"].ToString());// *(Convert.ToDouble(page.adult) + Convert.ToDouble(page.child));
                }
                if (counter == 0)
                {
                    flights.DefaultView.Sort = "TotalFare";
                    HttpContext.Current.Session["flights"] = flights.DefaultView.ToTable();
                    HttpContext.Current.Session["fare"] = fare;
                }
                else
                {
                    flights.DefaultView.Sort = "TotalFare";
                    HttpContext.Current.Session["Rflights"] = flights.DefaultView.ToTable();
                    HttpContext.Current.Session["Rfare"] = fare;
                }
                counter++;
            }
            Logout(signature);
        }

        private static double TotalFare(DataTable fare, string p)
        {
            return 0;
        }

        private static object TimeDuration(string p1, string p2)
        {
            throw new NotImplementedException();
        }

        public static void GetPriceReturn(string signature, GetAvailabilityResponse response, string noofPassenger)
        {
            IBookingManager bookingManager = new BookingManagerClient();
            DataTable flights = Table_Flights();
            DataTable fare = Table_Fare();
            foreach (JourneyDateMarket[] jdmArray in response.GetTripAvailabilityResponse.Schedules)
            {
                foreach (JourneyDateMarket jdm in jdmArray)
                {
                    foreach (Journey journey in jdm.Journeys)
                    {
                        PriceItineraryRequest pir = new PriceItineraryRequest();
                        ItineraryPriceRequest ipr = new ItineraryPriceRequest();
                        ipr.PriceItineraryBy = PriceItineraryBy.JourneyBySellKey;
                        ipr.SellByKeyRequest = new SellJourneyByKeyRequestData();
                        ipr.SellByKeyRequest.ActionStatusCode = "NN";
                        SellKeyList[] list = new SellKeyList[1];
                        list[0] = new SellKeyList();

                        list[0].JourneySellKey = journey.JourneySellKey;
                        foreach (Segment segment in journey.Segments)
                        {
                            DataRow dr = flights.NewRow();
                            if (segment.Fares.Length > 0)
                            {
                                list[0].FareSellKey = segment.Fares[0].FareSellKey;
                                dr["ShortDate"] = segment.STD.ToString("ddd MMM d yyyy");
                                dr["Date"] = segment.STD.ToShortDateString();
                                dr["From"] = segment.DepartureStation;
                                dr["To"] = segment.ArrivalStation;
                                dr["FlightNo"] = segment.FlightDesignator.FlightNumber;
                                dr["STD"] = segment.STD.ToString("dd/MM/yyy hh:mm tt");
                                dr["STA"] = segment.STA.ToString("dd/MM/yyy hh:mm tt");
                                dr["BaseFare"] = segment.Fares[0].PaxFares[0].ServiceCharges[0].Amount;
                                dr["TimeDuration"] = TimeDuration(segment.STD.ToString(), segment.STA.ToString());
                                flights.Rows.Add(dr);
                            }
                        }

                        ipr.SellByKeyRequest.JourneySellKeys = list;
                        int pax = Convert.ToInt16(noofPassenger);//+ Convert.ToInt16(page.child);
                        ipr.SellByKeyRequest.PaxCount = Convert.ToSByte(pax);
                        PaxPriceType[] priceTypes = new PaxPriceType[ipr.SellByKeyRequest.PaxCount];
                        for (int i = 0; i < ipr.SellByKeyRequest.PaxCount; i++)
                        {
                            priceTypes[i] = new PaxPriceType();
                            priceTypes[i].PaxType = "ADT";
                            priceTypes[i].PaxDiscountCode = string.Empty;
                        }
                        ipr.SellByKeyRequest.PaxPriceType = priceTypes;
                        ipr.SellByKeyRequest.CurrencyCode = "INR";
                        ipr.SellByKeyRequest.LoyaltyFilter = LoyaltyFilter.MonetaryOnly;
                        ipr.SellByKeyRequest.IsAllotmentMarketFare = false;
                        pir.ItineraryPriceRequest = ipr;
                        pir.Signature = signature;
                        PriceItineraryResponse pire = bookingManager.GetItineraryPrice(pir);
                        foreach (Journey j in pire.Booking.Journeys)
                        {
                            foreach (Segment segment in j.Segments)
                            {
                                foreach (BookingServiceCharge bsc in segment.Fares[0].PaxFares[0].ServiceCharges)
                                {
                                    DataRow drr = fare.NewRow();
                                    drr["FlightNo"] = segment.FlightDesignator.FlightNumber;
                                    drr["Heads"] = bsc.ChargeType.ToString() + bsc.ChargeCode.ToString();
                                    drr["Amount"] = bsc.Amount;
                                    fare.Rows.Add(drr);
                                }
                            }
                        }
                    }
                }
            }
            foreach (DataRow dr in flights.Rows)
            {
                dr["TotalFare"] = TotalFare(fare, dr["FlightNo"].ToString()) * (Convert.ToDouble(noofPassenger.ToString()));
            }
            HttpContext.Current.Session["Rflights"] = flights;
            HttpContext.Current.Session["Rfare"] = fare;
            Logout(signature);
        }
        
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
            return dt;
        }

        public static DataTable Table_Fare()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FlightNo", typeof(string));
            dt.Columns.Add("Heads", typeof(string));
            dt.Columns.Add("Amount", typeof(decimal));
            return dt;
        }
    }
}