
var globalcount = 0;

//datepicker
$(function () {
    $("#txtCheckInDate").datepicker({
        numberOfMonths: 2,
        defaultDate: new Date(),
        minDate: new Date(),
        maxDate: "+10M +00D",
        dateFormat: "dd/mm/yy",
        onSelect: function (selectedDate) {
            var instance = $(this).data("datepicker");
            var date = $.datepicker.parseDate(instance.settings.dateFormat || $.datepicker._defaults.dateFormat, selectedDate, instance.settings);
            date.setDate(date.getDate());
            toDate.datepicker("option", "minDate", date);
        },
        onClose: function (event, ui) { $("#txtCheckOutDate").focus(); }
    });
    var toDate = $("#txtCheckOutDate").datepicker({
        numberOfMonths: 2,
        minDate: new Date(),
        defaultDate: new Date(),
        maxDate: "+10M +00D",
        dateFormat: "dd/mm/yy"
    });
});

//dynamic hotel tables creation
$(document).ready(function () {
    getCityDetail();
    setTimeout(function () {
        $('.select2').select2({
            placeholder: 'Select City'
        });
    }, 1000);
   
    var appenddata1 = "<option value =''></option>";
    function getCityDetail() {
         appenddata1 = ""
        $.ajax({
            method: "GET",
            url: "../city/GetCityListData",
            success: function (data1) {
                for (var i = 0; i < data1.length; i++) {
                    appenddata1 += "<option value = '" + data1[i].CityCode + " '>" + data1[i].CityCode + "-" + data1[i].CityName + " </option>";

                }
                $("#txtCity").html(appenddata1);
            },
            error: function (response) {
            }
        });
    };
    var counter = 2;
    $("#addButtonnew").click(function () {
        var maxcount = 4;
        if (counter > maxcount) {
            alert("Maximum " + maxcount + " hotels are allowed.");
            return false;
        }
        var newTextBoxDiv = $(document.createElement('div'))
             .attr("id", 'TextBoxDiv' + counter);
        newTextBoxDiv.after().html('<br><div style="border-top:solid 1px #b33; height:auto; padding-top:10px;"">' +
        '<div class="row"><div class="col-md-4"><span>City Code</span>' +
        '<input maxlength="3" style="width: 90%; height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;" type="text" name="textbox' + counter +
         '" id="textboxcity' + counter + '" maxlength="3"></div>' +
        '<div class="col-md-4"><span>Check-In Date </span><br>' +
        '<input style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;" type="text" name="textbox' + counter +
         '" id="textboxcheckin' + counter + '" value="" ></div>' +
        '<div class="col-md-4"><span>Check-Out Date </span>' +
        '<input style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;" type="text"  name="textbox' + counter +
         '" id="textboxcheckout' + counter + '" value="" ></div>' +
        '</div>   ' +
         '<div class="row"><div class="col-md-4"><span>Check-In Time</span>' +
        '<input  style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;" type="text" name="textbox' + counter +
         '" id="textboxcheckintym' + counter + '" value=""></div>' +
        '<div class="col-md-4"><span>Check-Out Time </span><br>' +
        '<input style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;" type="text" name="textbox' + counter +
         '" id="textboxcheckouttym' + counter + '" value="" ></div>' +
         '<div class="col-md-4"><span>Entitlement</span>' +
            '<select id="entitlementtxt"' + counter + '" style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;' +
            ' onchange=""><option>Single</option><option>Sharing</option></select></div>' +
        '</div>   ' +
         '<div class="row"><div class="col-md-4"><span>Flight No. (Optional) </span>' +
        '<input  style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;" type="text" name="textbox' + counter +
         '" id="textboxflightno' + counter + '" value=""></div>' +
         '<div class="col-md-4"><span>PNR (Optional) </span>' +
        '<input  style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;" type="text" name="textbox' + counter +
         '" id="textboxpnr' + counter + '" value=""></div>' +
         '<div class="col-md-4"><span>Travel Type</span>' +
            '<select id="traveltype"' + counter + '" style="width: 90%;height: 30px; padding: 5px 10px; font-size: 12px; line-height: 1.5; border-radius: 3px;' +
            ' onchange=""><option>Surface/ Transport</option><option>Other Airlines Booking</option></select>' +
        '</div>   ' +
        '</div>'
       );

        $('#addContainers').append(newTextBoxDiv);
        $(function () {
            $('#textboxcheckin' + counter).datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy",
                onSelect: function (selectedDate) {
                    var instance = $(this).data("datepicker");
                    var date = $.datepicker.parseDate(instance.settings.dateFormat || $.datepicker._defaults.dateFormat, selectedDate, instance.settings);
                    date.setDate(date.getDate());
                    toDate.datepicker("option", "minDate", date);
                },
                onClose: function (event, ui) { $("#textboxcheckout" + counter).focus(); }
            });
            var toDate = $("#textboxcheckout" + counter).datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
            $('#textboxcheckintym' + counter).wickedpicker({
                twentyFour: false, title:
                   'Select Time', showSeconds: false
            });
            $('#textboxcheckouttym' + counter).wickedpicker({
                twentyFour: false, title:
                   'Select Time', showSeconds: false
            });
        });

        counter++;
        globalcount = counter;
    });
    
    //Remove hotels tabs by click on minus '-' button 
    $("#removeButton").click(function () {
        if (counter == 2) {
            alert("No more tabs to remove");
            return false;
        }
        counter--;
        globalcount = counter;
        $("#TextBoxDiv" + counter).remove();
    });
       
});

//Convert Time into 24 hrs format
function ConvertTime(time) {
    var AMPM = time.split(' ')[3];
    var hrs = time.split(' ')[0];
    var minutes = time.split(' ')[2];
    if (AMPM == "PM") {
        if (hrs != '12')
            hrs = hrs * 1 + 12;

    }
    else if (AMPM == "AM" && hrs == '12')
        hrs -= 12;
    else
        hrs = hrs;

    return hrs + ":" + minutes;

}

//submit hotel data
function SubmitHotelData() {
    var timeflag = true;
    if ($('#empDesign').val() == '') {
        alert('Please enter Your Designation.');
        $("#empDesign").focus();
        return false;
    }
    if ($('#empDept').val() == '') {
        alert('Please enter your Department.');
        $("#empDept").focus();
        return false;
    }
    if ($('#empEmail').val() == "") {
        alert("please enter email id.");
        $('#empEmail').focus();
        return false;
    }
    if ($('#empPhno').val() == "") {
        alert("please enter phone no.");
        $('#empPhno').focus();
        return false;
    }
    var emailRegex = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    var emailaddress = $("#empEmail").val();
    if (!emailRegex.test(emailaddress)) {
        alert("Please enter valid Email Id");
        $("#empEmail").focus();
        return false;
    } 
    if ($('#txtCity').val() == ''){
        alert('Please enter city code.');
        $("#txtCity").focus();
        return false;
    }
    if ($('#txtCheckInDate').val() == '') {
        alert('Please enter Check-In Date.');
        $("#txtCheckInDate").focus();
        return false;
    }
    if ($('#txtCheckOutDate').val() == ''){
        alert('Please enter Check-Out Date.');
        $("#txtCheckOutDate").focus();
        return false;
    }
    if ($('#txtCheckintime').val() == ''){
        alert('Please enter Check-In time.');
        $("#txtCheckintime").focus();
        return false;
    }
    if ($('#txtCheckouttime').val() == ''){
        alert('Please enter Check-Out time.');
        $("#txtCheckouttime").focus();
        return false;
    }
    if ($('#txtCheckInDate').val() == $('#txtCheckOutDate').val()) {
        var checkIntime = ConvertTime($('#txtCheckintime').val());
        var CheckoutTime = ConvertTime($('#txtCheckouttime').val());
        if (checkIntime == CheckoutTime) {
            alert('For Same Check-In date and Check-Out date, Check-In time and Check-Out time should be different .');
            $("#txtCheckintime").focus();
            timeflag = false;
        }
        if (checkIntime > CheckoutTime) {
            alert('For Same Check-In date and Check-Out date,Check-Out time should be greater than Check-In time.');
            $("#txtCheckintime").focus();
            timeflag = false;
        }
        //if ($('#txtCheckintime').val() == $('#txtCheckouttime').val()) {
        //    alert('For Same Check-in and Check-Out Date, check-in time and check-out time should be different .');
        //    $("#txtCheckintime").focus();
        //    return false;
        //}
    }
    if (!timeflag) return false;
    if ($('#reasontxt').val() == ''){
        alert('Please enter reason for hotel booking.');
        $("#reasontxt").focus();
        return false;
    }

    var city = document.getElementById("txtCity").value;
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: 'POST',
        url: '../EmployeeBookingDetail/CityCodeCheck',
        data: JSON.stringify({ citycode: city }),
        success: function (response) {
            if (response == "NotExist") {
                alert('City Code does not exist. Please enter a valid City Code. You may take help from City Code Help link.');
                $("#txtCity").focus();
                return false;
            }
            else {
                var result = confirm("Are you sure to submit your hotel request?");
                if (result) {
                    var sodbookingLst = new Array();
                    var sodflightLst = new Array();
                    var hotelDetailLst = new Array();

                    sodbookingLst = postsodbookingLst();
                    sodflightLst = postsodFlightLst();
                    hotelDetailLst = postsodhotelmasterlst();
                    $.ajax({
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        type: 'POST',
                        url: '../HotelOnly/SubmitHotelInfo',
                        data: JSON.stringify({ sodRequestsList: sodbookingLst, sodflightList: sodflightLst, hotelDetailList: hotelDetailLst }),
                        beforeSend: function () {
                            $('.loader').show();
                        },
                        complete: function () {
                            $('.loader').hide();
                        },
                        success: function (response) {
                            var url = '../HotelOnly/BookingResponse';
                            window.location.href = url;
                        },
                        error: function (response) {
                            alert(response);
                        }
                    });
                }
            }
        },
        error: function (response) {
            alert(response);
            return false;
        }
    });
        
}

function postsodbookingLst() {
    var sodBookingList = new Array();
    //For Travel Request Master
    var objRm = new Object();
    objRm.TravelRequestTypeId = '4';
    objRm.SodBookingTypeId = '1';
    objRm.BookingFor = "Only Hotel";
    objRm.Meals = "NA";
    objRm.RequestDate = new Date();
    objRm.ReasonForTravel = $('#reasontxt').val();
    objRm.RequestedEmpId = $('#EmpId').html().trim();
    objRm.RequestedEmpCode = $('#empCode').val().trim();
    objRm.RequestedEmpName = $('#empName').html().trim();
    objRm.RequestedEmpDesignation = $('#empDesign').val().trim();
    objRm.RequestedEmpDept = $('#empDept').val().trim();
    objRm.EmailId = $('#empEmail').val().trim();
    objRm.Phno = $('#empPhno').val().trim();
    objRm.Passengers=1;
    objRm.IsMandatoryTravel=0;
    objRm.IsCabRequired=0;
    objRm.IsAmountPaidByTraveller=0;
    objRm.IsHotelRequired = 1;
    objRm.BookingStatus = "Open";
    objRm.StatusDate = new Date(1900, 0, 1, 0, 0, 0, 0);
    if ($('#empGender').html().trim() == "F") {
        objRm.Title = "Ms.";
    } else {
        objRm.Title = "Mr.";
    }
    if ($('#pnrinfo').val() == '' || $('#pnrinfo').val() == null) {
        objRm.Pnr == "NA";
    } else {
        objRm.Pnr = $('#pnrinfo').val();
    }
    sodBookingList.push(objRm);
    return sodBookingList;
}

function  postsodhotelmasterlst()
{
    var hotelDetailsList = new Array();
    var objhd = new Object();
    objhd.TravelRequestTypeId = 4;
    objhd.HotelReferenceID = "";
    objhd.City = $('#txtCity').val().toUpperCase();
    objhd.EmployeeCode = $('#empCode').val().trim();
    objhd.CheckInDate = ConvertMMddyyyy($('#txtCheckInDate').val());  //$('#txtCheckInDate').val();////prod
    objhd.CheckOutDate = ConvertMMddyyyy($('#txtCheckOutDate').val())  //$('#txtCheckOutDate').val();////prod;
    objhd.CheckinTime = $('#txtCheckintime').val();
    objhd.CheckoutTime = $('#txtCheckouttime').val();
    objhd.NoOfGuest = "1"; 
    objhd.EntryDate = new Date();
    objhd.HotelCode = "NA";
    objhd.HotelName = "NA";
    objhd.HotelAddress = "NA";
    objhd.HotelPhoneNo = "NA";
    objhd.IsAllocated = "0";
    objhd.Remarks_Status = "NA";
    objhd.PrimaryEmail = "NA";
    objhd.SecondaryEmail = "NA";
    if ($('#airtransport :selected').text() == "Yes") {
        objhd.AirportTransport = true;
        objhd.IsCabRequiredAsPerETA = false;
        objhd.CabPickupTime = $('#txtCabPickupTime').val();
    }
    else {
        objhd.AirportTransport = false;
        objhd.IsCabRequiredAsPerETA = true;
    }
    objhd.HotelRequestId =1;
    hotelDetailsList.push(objhd);
    return hotelDetailsList;
}


function postsodFlightLst() {
    var sodFlightList = new Array();
    var objfd = new Object();
    objfd.OriginPlace = "NA";
    objfd.DestinationPlace = $('#txtCity').val().toUpperCase();
    //Local
    objfd.TravelDate = ConvertMMddyyyy($('#txtCheckInDate').val());
    //objfd.TravelDate = $('#txtCheckInDate').val(); //prod
    if ($('#flightno').val() == '') {
        objfd.FlightNo = "NA";
    } else {
        objfd.FlightNo = $('#flightno').val();
    }
    objfd.DepartureTime = "00:00 AM";
    objfd.ArrivalTime = "00:00 AM";
    objfd.FlightName = "NA";
    objfd.FlightTypes = 0;
    sodFlightList.push(objfd);

    return sodFlightList;
}

function ConvertMMddyyyy(dval) {
    var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
    return mval;
}

function toggleAirTransport() {
    if ($('#airtransport option:selected').text() == "Yes") {
        $("#cabTimingText").prop("hidden", false);
    } else {
        $("#cabTimingText").prop("hidden", true);
    }
}
function validateNumber(evt) {
    var e = evt || window.event;
    var key = e.keyCode || e.which;

    if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
        // numbers   
        key >= 48 && key <= 57 ||
        // Numeric keypad
        key >= 96 && key <= 105 ||
        // Backspace and Tab and Enter
        key == 8 || key == 9 || key == 13 ||
        // Home and End
        key == 35 || key == 36 ||
        // left and right arrows
        key == 37 || key == 39 ||
        // Del and Ins
        key == 46 || key == 45) {
        // input is VALID
    }
    else {
        // input is INVALID
        e.returnValue = false;
        if (e.preventDefault) e.preventDefault();
    }
}
var app = angular.module("hotelOnly", []);
app.controller("hotelOnlyCtrl", function ($scope, $http) {
    $scope.email = $("#empEmail").val();
    $scope.Fname = $("#txtempName").val();
    $scope.empCode = $("#empCode").val();
    $scope.title = $('#empGender').html().trim()
    $scope.phone = $("#empPhno").val();
    $scope.designation = $("#empDesign").val();
    $scope.department = $("#empDept").val();

});
