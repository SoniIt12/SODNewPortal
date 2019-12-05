
//One way submit data
function OnewaySubmit11(ddlorigin, ddldestination, chkoneway, chkroundtrip, datepicker, passenger)
{
    var sodRequestList = new Array();
    var obj = new Object();
    if (chkoneway.prop("checked"))
        obj.TravelRequestTypeId = "1";
    if (chkroundtrip.prop("checked"))
        obj.TravelRequestTypeId = "2";

    obj.OriginPlace = ddlorigin.options[ddlorigin.selectedIndex].value;
    obj.DestinationPlace = ddldestination.options[ddldestination.selectedIndex].value;
    obj.TravelDate = datepicker.value;
    obj.Passengers = passenger.value;;
    sodRequestList.push(obj);
    debugger;

    var sodRequestsList = JSON.stringify({ sodRequestsList: sodRequestList });
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: 'POST',
        url: '/user/SodOneWayRequestSubmit',
        data: sodRequestsList,
        success: function (data) {
            var url = '@Url.Action("TrSelection", "TravelSelection")';
            window.location.href = url;
        },
        error: function () {
            $("#spanfail").html("Error on Save data.");
        }
    });
}