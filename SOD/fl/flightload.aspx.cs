using SOD.FlightLoadCapacity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SOD.fl
{
    public partial class flightload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                clddate.SelectedDate = DateTime.UtcNow.AddMinutes(330);
                clddate.TodaysDate = DateTime.UtcNow.AddMinutes(330);
            }

        }
        protected void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["userid_load"];
                string password = ConfigurationManager.AppSettings["password_load"];

                //FlightLoadCapacitySoapClient cl = new FlightLoadCapacitySoapClient();

                FlightLoadCapacity.FlightLoadCapacity cl = new FlightLoadCapacity.FlightLoadCapacity();

                string flightno = txtflightNo.Text.Length == 3 ? " " + txtflightNo.Text : txtflightNo.Text;
                DateTime ddate = clddate.SelectedDate;
                string source = txtsector.Text.Split('-')[0];
                string destination = txtsector.Text.Split('-')[1];
                LoadAndCapacity lc = cl.GetFlightLoadCapacity(username, password, flightno, ddate, source, destination);
                lbldisplay.Text = "Date: " + clddate.SelectedDate.ToString("dd/MM/yyyy") + "<br>FlightNo: " + flightno + " ,Sector: " + source + " - " + destination + " <br>Capacity: " + lc.Capacity.ToString() + " ,Load : " + lc.Load.ToString();
            }
            catch (Exception ex)
            {
                lbldisplay.Text = ex.ToString();
            }
        }
    }
}