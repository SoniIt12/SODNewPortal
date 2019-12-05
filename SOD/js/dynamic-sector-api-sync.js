
//=================================================================
//Author: Satyam
//Purpose : Pull Data for Dynamic Sector API
//=================================================================


function sinkDynamicData() {
    $.ajax({
        url: '../admin/syncDynamicSec',
        type: "GET",
        processData: false,
        dataType: 'json',
        contentType: false,
        beforeSend: function () {
            $("#loaderMsg")[0].innerHTML = "Processing : Connected and pulling data from Dynamic Sector api.";
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
                $("#loaderMsg")[0].innerHTML = "Success :Data Sinking Process with Dynamic Sector api has been completed successfully.";
            alert('Success :Data Sinking Process with Dynamic Sector api has been completed successfully.');
        },
        error: function (er) {
            $("#loaderMsg")[0].innerHTML = "ERROR :Data Sinking Process fail with Dynamic Sector api.";
            alert('ERROR : Sink API Error ' + er);
            $("#btnSave").prop('disabled', false);
        }
    });
    return false;
}

