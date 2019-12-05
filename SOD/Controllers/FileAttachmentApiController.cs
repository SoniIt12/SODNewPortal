using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace SOD.Controllers
{

    public class FileAttachmentApiController : ApiController
    {
        private readonly IOATrepository _oaTRepository;
        public FileAttachmentApiController()
        {
            _oaTRepository = new OATrepository(new SodEntities());
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("PostFileAttachmentWithData")]
        public async Task<HttpResponseMessage> Post()
        {
            var jsonmsg = "";
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            try
            {
                var root = HttpContext.Current.Server.MapPath("~/OatUploadAttachments/IthResponseAttachment");
                //Directory.CreateDirectory(root);
                var provider = new MultipartFormDataStreamProvider(root);
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                var model = result.FormData["model"];
                if (model == null)
                {
                    jsonmsg = "Your Response is not sent successfully";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, jsonmsg);
                }
                JObject json = JObject.Parse(model);
                List<ITHTransactionDetailModal> ithResponse = new List<ITHTransactionDetailModal>();
                var s = Newtonsoft.Json.JsonConvert.DeserializeObject(json["model"].ToString());
                dynamic Dataofjson = JsonConvert.DeserializeObject(s.ToString());
                int length = json["model"].Count();
                for (int i = 0; i < length; i++)
                {
                    ITHTransactionDetailModal ith = new ITHTransactionDetailModal();
                    ith.TrnId = Dataofjson[0].TrnId;
                    ith.PassengerID = Dataofjson[i].PassengerID;
                    ith.OATRequestID = Dataofjson[i].OATRequestID;
                    ith.FlightType = Dataofjson[0].FlightType;
                    ith.OriginPlace = Dataofjson[i].OriginPlace;
                    ith.DestinationPlace = Dataofjson[i].DestinationPlace;
                    ith.DepartureDate = Dataofjson[i].DepartureDate;
                    ith.DepartureTime = Dataofjson[i].DepartureTime;
                    ith.ArrivalDate = Dataofjson[i].ArrivalDate;
                    ith.ArrivalTime = Dataofjson[i].ArrivalTime;
                    ith.AirCraftName = Dataofjson[i].AirCraftName;
                    ith.FlightNumber = Dataofjson[i].FlightNumber;
                    ith.Amount = Dataofjson[i].Amount;
                    //ith.IsInternational = Dataofjson[0].IsInternational;
                    ith.UploadedImage = Dataofjson[i].ITHUploadRefFiles;
                    ithResponse.Add(ith);
                }
                //get the posted files  
                for (var i = 0; i < result.FileData.Count; i++)
                {
                    var fileName = result.FileData[i].Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    var fileType = result.FileData[i].Headers.ContentType.MediaType;
                    FileStream so = File.OpenRead(result.FileData[i].LocalFileName);
                    byte[] photo;
                    using (Stream stream = so)
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            photo = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    foreach (var lst in ithResponse)
                    {
                        if (lst.UploadedImage != null)
                        {
                            if (lst.UploadedImage.Contains(fileName))
                            {
                                lst.ITHUploadRefFile = photo;
                                lst.ITHUploadRefType = fileType;
                                break;
                            }
                        }                        
                    }
                    //ithResponse[i].ITHUploadRefFile = photo;
                }
                var res = _oaTRepository.SubmitIthDetailResponse(ithResponse);
                if (res > 0)
                {
                    jsonmsg = "Your response has been successfully sent to spicejet OAT Traveldesk.";
                }
            }
            catch (Exception ex)
            {
                jsonmsg = ex.InnerException.ToString();
                throw ex;
            }
            return Request.CreateResponse(HttpStatusCode.OK, jsonmsg);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("PostItenarybooking")]
        public async Task<HttpResponseMessage> PostItenary()
        {
            var jsonmsg = "";
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            try
            {
                var root = HttpContext.Current.Server.MapPath("~/OatUploadAttachments/BookedItenary");
                Directory.CreateDirectory(root);
                var provider = new MultipartFormDataStreamProvider(root);
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                var model = result.FormData["model"];
                if (model == null)
                {
                    jsonmsg = "Your Response is not sent successfully";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, jsonmsg);
                }
                JObject json = JObject.Parse(model);
                List<OATUploadItenaryModal> ItenaryList = new List<OATUploadItenaryModal>();
                var s = Newtonsoft.Json.JsonConvert.DeserializeObject(json["model"].ToString());
                dynamic Dataofjson = JsonConvert.DeserializeObject(s.ToString());
                int length = json["model"].Count();
                for (int i = 0; i < length; i++)
                {
                    OATUploadItenaryModal ith = new OATUploadItenaryModal();
                    ith.OATRequestId = Dataofjson[0].OATTravelRequestId;
                    ith.PassengerID = Dataofjson[i].PassengerID;
                    ith.OriginPlace = Dataofjson[i].OriginPlace;
                    ith.DestinationPlace = Dataofjson[i].DestinationPlace;
                    ith.DepartureDate = Dataofjson[i].DepartureDate;
                    ith.DepartureTime = Dataofjson[i].DepartureTime;
                    ith.AirlineName = Dataofjson[i].AirCraftName;
                    ith.PNR = Dataofjson[i].PNR;
                    ith.Amount = Dataofjson[i].Amount;
                    ith.AmountCurrencyCode = "INR";                   
                    ith.Remarks = Dataofjson[i].Remarks;
                    ith.Status = "Close";
                    ith.EntryDate = DateTime.Now;
                    ItenaryList.Add(ith);
                }
                var c = 0;
                foreach (var file in result.FileData)
                {
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    string filename = System.DateTime.Now.ToString("yyyyMMddHHss") + "_" + file.Headers.ContentDisposition.FileName.ToString().Replace("\"", "");
                    fi.CopyTo(root + "\\" + filename, true);
                    fi.Delete();
                    //string url = root + "\\" + filename;
                    ItenaryList[c].FilePath = "\\OatUploadAttachments\\BookedItenary";// +"\\" +System.Web.HttpUtility.UrlEncode(filename);

                    ItenaryList[c].FileName = filename;
                    c++;
                }

                var res = _oaTRepository.SubmitItenaryListOfBooking(ItenaryList);
                if (res > 0)
                {
                    var controller = DependencyResolver.Current.GetService<OatDeskController>();
                    var oatReqId = Convert.ToInt64(Dataofjson[0].OATTravelRequestId);
                    var bookingType = _oaTRepository.getBookingType(oatReqId);
                    if (bookingType == "Others (On Behalf of Employee / Non-Employee)")
                    {
                        //var oatReqID = Dataofjson[0].OATTravelRequestId;
                        var TrnId = 0;
                        var emailSubject = "C-LeveL Approval Notification :" + System.DateTime.Now.ToString();
                        var SubjectQuote = " ";
                        var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + "SKEY" + "&type=ca";
                        var bidingQuote = "<b>Note: Highlighted Colored border is Travel desk, User & HOD Choice. Please provide your acceptance for the same fares.</b>";
                        
                        //SendMailToHod(oatReqID, TrnId, emailSubject, SubjectQuote, bidingQuote, "User")
                        controller.SendMailToHod(oatReqId, TrnId, emailSubject, SubjectQuote, bidingQuote, "C-Level");
                        //update UpdateC-levelNotification in IthMaster 
                        _oaTRepository.UpdateClevelNotification(oatReqId);
                    }
                    // send mail to user
                    controller.sendFinalConfirmationToUser(oatReqId);

                    jsonmsg = "Your response has been successfully sent to spicejet OAT Traveldesk.";
                }

            }
            catch (Exception ex)
            {
                jsonmsg = ex.InnerException.ToString();
                throw ex;
            }
            return Request.CreateResponse(HttpStatusCode.OK, jsonmsg);
        }
        
    }
}

