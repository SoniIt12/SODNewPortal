
//=================================================================
//Author: Satyam
//Purpose : Pull Data from emdm API 
//=================================================================


function sinkData() {
    $.ajax({
        url: '../admin/sinkData',
        type: "GET",
        processData: false,
        dataType: 'json',
        contentType: false,
        beforeSend: function () {
            $("#loaderMsg")[0].innerHTML = "Processing : Connected and pulling data from emdm sap api.";
            $('.loader').show();
            $('.spinner').show();
            $("#btnSave").prop('disabled', true);
        },
        complete: function () {
            $("#loaderMsg")[0].innerHTML = "";
            $("#loaderMsg")[0].innerHTML = "Process complete.....";
            $('.loader').delay(100).hide(2000);
            $('.spinner').delay(100).hide(2000);
            $("#btnSave").prop('disabled', true);
        },
        success: function (response) {
            if (response != null || response != '')
                $("#loaderMsg")[0].innerHTML = "Success :Data Sinking Process with emdm sap api has been completed successfully.";
            alert('Success :Data Sinking Process with emdm sap api has been completed successfully.Total record sinked - ' + response);
        },
        error: function (er) {
            $("#loaderMsg")[0].innerHTML = "ERROR :Data Sinking Process fail with emdm sap api.";
            alert('ERROR : Sink API Error ' + er);
            $("#btnSave").prop('disabled', false);
        }
    });
    return false;
}

