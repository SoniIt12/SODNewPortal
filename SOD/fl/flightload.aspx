<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="flightload.aspx.cs" Inherits="SOD.fl.flightload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Flight Load</title>
    <link href="../css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
 

    <div class="container">
  <h2>Flight Load</h2>
  <form class="form-horizontal" id="form1" runat="server">
    <div class="form-group">
      <label class="control-label col-sm-2" for="txtsector">Sector:</label>
      <div class="col-sm-10">
          <asp:TextBox ID="txtsector" runat="server" CssClass="form-control"></asp:TextBox>
      </div>
    </div>

    <div class="form-group">
      <label class="control-label col-sm-2" for="txtflightNo">Flight No.</label>
      <div class="col-sm-10">       
          <asp:TextBox ID="txtflightNo" CssClass="form-control" runat="server"></asp:TextBox>   
      </div>
    </div>

    <div class="form-group">
      <label class="control-label col-sm-2" for="clddate">Date.</label>
      <div class="col-sm-10">       
          <asp:Calendar ID="clddate" runat="server" SelectedDayStyle-BackColor="Gray" TodayDayStyle-BackColor="Green" ></asp:Calendar>
     
      </div>
    </div>
    <div class="form-group">        
      <div class="col-sm-offset-2 col-sm-10">
           <asp:Button ID="btnGet" CssClass="btn btn-default" runat="server" Text="Flight Load" OnClick="btnGet_Click"></asp:Button>
    
      </div>
    </div>
      <div class="form-group">        
      <div class="col-sm-offset-2 col-sm-10">
           <asp:Label ID="lbldisplay" runat="server"></asp:Label>
      </div>
    </div>
  </form>
</div>

</body>
</html>
