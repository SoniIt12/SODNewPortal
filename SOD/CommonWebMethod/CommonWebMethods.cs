using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;
using SOD.SpiceDynamicSector;
using SOD.Logging;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace SOD.CommonWebMethod
{
    /// <summary>
    /// This class will be used for get Original and Source Place from Spice Web Services
    /// </summary>
    public static class CommonWebMethods
    {

        //Credentials from Web.config
        static readonly string Uid = ConfigurationManager.AppSettings["OriginDestUser"].Trim();
        static readonly string Pwd = ConfigurationManager.AppSettings["OriginDestPwd"].Trim();
        
        /// <summary>
        /// Instance of Static Constructor
        /// </summary>
        static CommonWebMethods()
        {
            System.Web.HttpContext.Current.Session["IntSectorsTime"] = null;
        }
        /// <summary>
        /// Get Original Station
        /// </summary>
        /// <returns></returns>
        public static string GetOriginStation()
        {
            try
            {
                var sourcedate = string.Empty;
                string sourcepath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorSource.txt");
                FileInfo fileInfo = new FileInfo(sourcepath);
                using (StreamReader reader = new StreamReader(sourcepath))
                {
                    sourcedate = reader.ReadToEnd();
                }
                return sourcedate.ToString().Trim();
            }
            catch (Exception ex)
            {
                Logging.ErrorLog.AddEmailLogg(ex);
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetOriginStation", "CommonWebMethod/CommonWebMethods.cs");
                throw new Exception();
            }
        }

        /// <summary>
        /// Get State List
        /// </summary>
        /// <returns></returns>
        public static string GetStates()
        {
            try
            {
                var s = string.Empty;
                string tagName = "StateData";
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorStates.xml");
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(filePath);
                    XmlNodeList textlist = xml.GetElementsByTagName(tagName);
                    s = textlist[0].InnerText;
                }
                return s;
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetStates", "CommonWebMethod/CommonWebMethods.cs");
                throw new Exception();
            }
        }


        /// <summary>
        /// Get XML Node
        /// </summary>
        /// <returns></returns>
        public static string GetXMLNodeRights(string tagName)
        {
            try
            {
                var s = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//AdminSidePanel.xml");
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(filePath);
                    XmlNodeList textlist = xml.GetElementsByTagName(tagName);
                    s = textlist[0].InnerText;
                }
                return s;
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetXMLNodeRights", "CommonWebMethod/CommonWebMethods.cs");
                throw new Exception();
            }
        }


        /// <summary>
        /// Get Destination Station
        /// </summary>
        /// <param name="objArr"></param>
        /// <returns></returns>
        public static string GetArrivalStation(string objArr)
        {
            try
            {       var s=string.Empty;
                    string tagName=objArr.Split('|')[0].Trim();
                    string filePath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorArrival.xml");
                    FileInfo fileInfo=new FileInfo (filePath);
                    if (fileInfo.Exists)
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(filePath);
                        XmlNodeList textlist = xml.GetElementsByTagName(tagName);
                        s = textlist[0].InnerText;
                    }
                    return s;
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetArrivalStation", "CommonWebMethod/CommonWebMethods.cs");
                throw new Exception();
            }
        }


        /// <summary>
        /// Convert dd/MM/yyyy to MM/dd/yyyy
        /// </summary>
        /// <param name="strdate"></param>
        /// <returns></returns>
        public static string ConvertMMddyyyy(string strdate)
        {
           string []str=strdate.Split('/');
           string dt= str[1]+ "/" +str[0]+ "/"+ str[2];
           return dt;
        }

        /// <summary>
        /// Convert string to datetime
        /// </summary>
        /// <param name="strdate"></param>
        /// <returns></returns>
        public static string ConvertToCustomDate(string strdate)
        {
            string[] str = strdate.Split('-');
            string dt = str[1] + "/" + str[0] + "/" + str[2];
            return dt;
        }

        /// <summary>
        /// Sync Dynamic Sector API data to XML
        /// </summary>
        /// <returns></returns>
        public static string GetOriginStationSync()
        {
            string r = string.Empty;
            try
            { 
                DynamicSectors dynSecCln = new DynamicSectors();
                OriginStations[] orgStn = dynSecCln.GetOriginStations(Uid, Pwd);
                List<string> list_D = new List<string>();
                List<string> list_I = new List<string>();
             
                    foreach (OriginStations orgnStn in orgStn)
                    {
                        string key, value;
                        key = orgnStn.CityCode;
                        value = orgnStn.Origin + "-" + orgnStn.CityCode + "";
                        if (orgnStn.StationType == "Domestic")
                            list_D.Add(value + "|India");
                        else if (orgnStn.StationType == "International")
                            list_I.Add(value + "|International");

                    }
                    list_D.Sort();
                    list_I.Sort();
                    List<string> final = new List<string>();
                    final.AddRange(list_D);
                    final.AddRange(list_I);
                    var json = new JavaScriptSerializer().Serialize(final);

                    //Write Json Sector data in text file
                    string datapath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorSource.txt");
                    string logPath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorDateLog.txt");
                    System.IO.File.WriteAllText(datapath, json.ToString().Trim());
                    System.IO.File.WriteAllText(logPath, System.DateTime.Now.ToString("MM/dd/yyyy"));
                    if (orgStn.Length > 0)  
                    WriteArrivaStationTextData(json);
                    r =json.ToString().Length > 2 ? "Sync Successfully." : "";
            }
            catch (Exception ex)
            {
                Logging.ErrorLog.AddEmailLogg(ex);
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetOriginStationSync", "CommonWebMethod/CommonWebMethods.cs");
            }
            return r;
        }


        /// <summary>
        /// Write Arrival Station Data
        /// </summary>
        /// <param name="?"></param>
        public static void WriteArrivaStationTextData(string jsdata)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string[] arrSource = js.Deserialize<string[]>(jsdata);

                //string[] arrSource = jsdata.Split(',');

                string sector = "";
                string preroot="";
                string filename = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorArrival.xml");
                FileInfo info = new FileInfo(filename);
                info.Delete();
                for (int i = 0; i < arrSource.Length; i++)
                {
                    sector = arrSource[i].ToString().Split('-')[1];
                    //Get Arrival Stations
                    DynamicSectors dynSecCln = new DynamicSectors();
                    ArrivalStations[] arrStn = dynSecCln.GetArrivalStations(Uid, Pwd, sector.Split('|')[0]);


                    List<string> list_D = new List<string>();
                    List<string> list_I = new List<string>();
                    foreach (ArrivalStations orgnStn in arrStn)
                    {
                        string key, value;
                        key = orgnStn.JourneyArrivalStation;
                        value = orgnStn.ArrivalStationName + "-" + orgnStn.JourneyArrivalStation + "";

                        //Check-International and Domestic Sector
                        string[] InternationalSectorsCodes = ConfigurationManager.AppSettings["InternationalSectorsCode"].Split(',');
                        int index = Array.IndexOf(InternationalSectorsCodes, key);
                        if (index>-1)
                            list_I.Add(value + "|International");
                        else
                            list_D.Add(value + "|India");

                    }
                    list_D.Sort();
                    list_I.Sort();
                    List<string> final = new List<string>();
                    final.AddRange(list_D);
                    final.AddRange(list_I);
                    var json = new JavaScriptSerializer().Serialize(final);
                    string fileName = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorArrival.xml");
                    FileInfo fileInfo=new FileInfo (fileName);
                    if (!fileInfo.Exists)
                     {
                         using (XmlWriter writer = XmlWriter.Create(fileName))
                         {
                             writer.WriteStartDocument();
                             writer.WriteStartElement("Root");
                             writer.WriteStartElement(sector.Split('|')[0]);
                             writer.WriteElementString("SectorData", json.ToString());
                             writer.WriteEndElement();
                           
                             writer.WriteEndDocument();
                             preroot = sector.Split('|')[0];
                             writer.Flush();
                         }
                     }
                     else
                     {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(fileName);
                        //create node  
                        XmlNode node = doc.CreateNode(XmlNodeType.Element, sector.Split('|')[0],null);
                        //create title node
                        XmlNode nodeTitle = doc.CreateElement("SectorData");
                        nodeTitle.InnerText = json.ToString();
                        //add to parent node
                        node.AppendChild(nodeTitle);
                        //add to elements collection
                        doc.DocumentElement.AppendChild(node);
                        //save back
                        doc.Save(fileName);
                     }
                }
            }
            catch(Exception ex)
            {
                Logging.ErrorLog.AddEmailLogg(ex);
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "WriteArrivaStationTextData", "CommonWebMethod/CommonWebMethods.cs");
            }
            finally
            {

            }
        }


        /// <summary>
        /// Check Dynaic Secor Log
        /// </summary>
        /// <returns></returns>
        public static string DynamicSectorDateLog()
        {
            string logdate = string.Empty;
            string jsondata = string.Empty;
            string currentdate = System.DateTime.Now.ToString("MM/dd/yyyy");
            string logPath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorDateLog.txt");

            // Read Dynamic Sector Log
            using (StreamReader reader = new StreamReader(logPath))
            {
                logdate = reader.ReadToEnd();
            }

            string datapath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//DynamicSectorSource.txt");
            if (logdate == currentdate)
            {
                //Read Dynamic Sector API Data
                using (StreamReader reader = new StreamReader(datapath))
                {
                    jsondata = reader.ReadToEnd();
                }
            }
            return jsondata;
        }


        
        /// <summary>
        /// Get Hold Booking DateTime
        /// </summary>
        /// <param name="SodBookingTypeId"></param>
        /// <param name="BookingFor"></param>
        /// <param name="TravelDate"></param>
        /// <param name="DepartureTime"></param>
        /// <param name="RequestDate"></param>
        /// <returns></returns>
        public static DateTime GetHoldBookingDateTime(int SodBookingTypeId, string BookingFor, DateTime TravelDate, string DepartureTime, DateTime RequestDate)
        {
            string fligtDepartureTime = null;
            var HoldTime = new DateTime();
            try
            {
                //For NON SOD Standby Booking
                if (SodBookingTypeId.Equals(2) && BookingFor.ToLower().Equals("standby"))
                {
                    DateTime flightDepTime = Convert.ToDateTime(DepartureTime).AddMinutes(-1);
                    var day = TravelDate.ToString("dd");
                    var month = TravelDate.ToString("MM");
                    var year = TravelDate.ToString("yyyy");
                    var hour = flightDepTime.ToString("HH");
                    var minute = flightDepTime.ToString("mm");
                    DateTime dateTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), Convert.ToInt32(hour), Convert.ToInt32(minute), 0);
                    HoldTime = dateTime;    

                }

                //For  NON SOD Confirm Booking
                else if (SodBookingTypeId.Equals(2) && BookingFor.ToLower().Equals("confirm"))
                {
                    DateTime flightDepTimes = Convert.ToDateTime(DepartureTime);
                    var day = TravelDate.ToString("dd");
                    var month = TravelDate.ToString("MM");
                    var year = TravelDate.ToString("yyyy");
                    var hour = flightDepTimes.ToString("HH");
                    var minute = flightDepTimes.ToString("mm");

                    DateTime flightDepTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), Convert.ToInt32(hour), Convert.ToInt32(minute), 0);
                    TimeSpan DateTimedifference = flightDepTime - RequestDate;

                   if (DateTimedifference.TotalHours > 72)
                    {
                        HoldTime = RequestDate.AddHours(24);
                    }

                    else if (DateTimedifference.TotalHours >3 && DateTimedifference.TotalHours <=72)
                    {
                        HoldTime = RequestDate.AddHours(3);
                    }

                    else if (DateTimedifference.TotalHours <= 3)
                    {
                        if (DateTimedifference.TotalHours >1 && DateTimedifference.TotalHours <=2)
                        {
                            var holdminute = DateTimedifference.TotalMinutes - 60;
                            HoldTime = RequestDate.AddMinutes(holdminute);
                        }
                        else if (DateTimedifference.Hours<1)
                        {
                            //Confirm Booking will not be allowed :prior to (flight departure -1:00 hrs)
                            HoldTime = flightDepTime.AddHours(-1);
                        }
                        else
                            HoldTime = RequestDate.AddHours(1);
                    }
                }
            }
            catch (Exception ex)
            {
               ErrorLog objLogging = new ErrorLog();
               objLogging.AddDBLogging(ex, "GetHoldBookingDateTime", "CommonWebMethod.cs/GetHoldBookingDateTime");
            }

            return HoldTime;
        }
       


        
        /// <summary>
        /// Check is Ok to Board is required or not for the flight
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="flightType"></param>
        /// <returns></returns>
        public static bool IsOktoBoardRequired(String sector, string bookingtype)
        {
            var s = false;
            try
            {
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//OKtoBoardSectorsList.xml");
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(filePath);
                    XmlNodeList textlist = xml.GetElementsByTagName(sector);
                    if (textlist.Count > 0)
                    {
                        for (int i = 0; i < textlist[0].ChildNodes.Count; i++)
                        {
                            if (textlist[0].ChildNodes[i].Name.ToLower().Trim().Equals(bookingtype))
                            {
                                s = textlist[0].ChildNodes[i].FirstChild.InnerText.Equals("Yes") ? true : false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetAdmin", "CommonWebMethod.cs/IsOktoBoardRequired");
                throw new Exception();
            }
            return s;
        }




        /// <summary>
        /// For Multicity Sector
        /// </summary>
        /// <param name="OriginPlace"></param>
        /// <param name="DestinationPlace"></param>
        /// <param name="bookingtype"></param>
        /// <returns></returns>
        public static bool IsOktoBoardRequired_forMulticitySectors(String OriginPlace, String DestinationPlace, string bookingtype)
        {
            var s = false;
            try
            {
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//OKtoBoardSectorsList.xml");
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(filePath);
                    XmlNodeList textlist = xml.GetElementsByTagName(DestinationPlace);
                    if (textlist.Count > 0)
                    {
                        for (int i = 0; i < textlist[0].ChildNodes.Count;i++ )
                        {
                            if (textlist[0].ChildNodes[i].Name.ToLower().Trim().Equals(bookingtype))
                            {
                                s = textlist[0].ChildNodes[i].FirstChild.InnerText.Equals("Yes") ? true : false;
                                break;
                            }
                        }
                        
                        //Check-International  
                        string[] InternationalSectorsCodes = ConfigurationManager.AppSettings["InternationalSectorsCode"].Split(',');
                        int index = Array.IndexOf(InternationalSectorsCodes, OriginPlace);
                        s = (index > -1) ? false:true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetAdmin", "CommonWebMethod.cs/IsOktoBoardRequired");
                throw new Exception();
            }
            return s;
        }


        /// <summary>
        /// Get UTC Domestic/International Time duration for flights
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="flightType"></param>
        /// <returns></returns>
        public static DateTime  GetInternationTimeDurationUTC(String sector,DateTime date)
        {
            try
            {
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/DynamicSectorData//InternationalSectorsTime.xml");
                FileInfo fileInfo = null;
                if (System.Web.HttpContext.Current.Session["IntSectorsTime"]==null)
                {
                    fileInfo = new FileInfo(filePath);
                    System.Web.HttpContext.Current.Session["IntSectorsTime"] = fileInfo;
                }
                else
                {
                    fileInfo = (FileInfo)System.Web.HttpContext.Current.Session["IntSectorsTime"];
                }
                if (fileInfo.Exists)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(filePath);
                    XmlNodeList textlist = xml.GetElementsByTagName(sector);
                    if (textlist.Count > 0)
                    {
                        date = date.AddMinutes(Convert.ToDouble(textlist[0].ChildNodes[0].FirstChild.InnerText));
                    }
                    else
                    {
                        date = date.AddMinutes(-330); //Indian Time +5:30
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "CommonWebMethod.cs", "GetInternationTimeDurationUTC/InternationalSectorsTime");
                throw new Exception();
            }
            return date;
        }
     }
 }
