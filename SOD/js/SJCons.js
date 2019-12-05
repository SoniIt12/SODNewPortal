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
        defaultDate: new Date(),
        minDate: new Date(),
        maxDate: "+10M +00D",
        dateFormat: "dd/mm/yy"        
    });
});

//dynamic hotel tables creation
$(document).ready(function () {
    getCityDetail();
    $('.select3').select2({
        placeholder: 'Select City'
    });
    setTimeout(function () {

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
    if ($('#txtFName').val() == '') {
        alert('Please enter Your First Name.');
        $("#txtFName").focus();
        return false;
    }
    if ($('#txtLName').val() == '') {
        alert('Please enter Your Last Name.');
        $("#txtLName").focus();
        return false;
    }
    if ($('#txtDesignation').val() == '') {
        alert('Please enter Your Designation.');
        $("#txtDesignation").focus();
        return false;
    }
    if ($('#txtDepartment').val() == '') {
        alert('Please enter your Department.');
        $("#txtDepartment").focus();
        return false;
    }
    if ($('#txtMobileNo').val() == '') {
        alert('Please enter Mobile No.');
        $("#txtMobileNo").focus();
        return false;
    }

    if ($('#txtEmails').val() == '') {
        alert('Please enter Email id.');
        $("#txtEmails").focus();
        return false;
    }
    var emailRegex = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    var emailaddress = $("#txtEmails").val();
    if (!emailRegex.test(emailaddress)) {
        alert("Please enter valid Email Id");
        $("#txtEmails").focus();
        return false;
    } 

    if ($('#txtCity').val() == '') {
        alert('Please enter city code.');
        $("#txtCity").focus();
        return false;
    }
    if ($('#txtCheckInDate').val() == '') {
        alert('Please enter Check-In Date.');
        $("#txtCheckInDate").focus();
        return false;
    }
    if ($('#txtCheckOutDate').val() == '') {
        alert('Please enter Check-Out Date.');
        $("#txtCheckOutDate").focus();
        return false;
    }
    if ($('#txtCheckintime').val() == '') {
        alert('Please enter Check-In time.');
        $("#txtCheckintime").focus();
        return false;
    }
    if ($('#txtCheckouttime').val() == '') {
        alert('Please enter Check-Out time.');
        $("#txtCheckouttime").focus();
        return false;
    }
    if ($('#reasontxt').val() == '') {
        alert('Please enter reason for hotel booking.');
        $("#reasontxt").focus();
        return false;
    }
    if ($('#txtHodName').val() == '') {
        alert('Please enter reason for hotel booking.');
        $("#txtHodName").focus();
        return false;
    }
    if ($('#txtHodEmail').val() == '') {
        alert('Please enter reason for hotel booking.');
        $("#txtHodEmail").focus();
        return false;
    }
    var Hodemailaddress = $("#txtHodEmail").val();
    if (!emailRegex.test(Hodemailaddress)) {
        alert("Please enter valid Hod Email Id");
        $("#txtEmails").focus();
        return false;
    }
    if ($('#traveltype option:selected').text() == "SJ") {
        if ($("#flightno").val() == '') {
            alert('Please enter Flight No.');
            $("#flightno").focus();
            return false;
        }
        if ($("#pnrinfo").val() == '') {
            alert('Please enter your Pnr No.');
            $("#pnrinfo").focus();
            return false;
        }
    }
    if ($('#traveltype option:selected').text() == "Train") {
        if ($('#trainInfo').val() != '' && $('#trainInfo').val() != null) {
            if ($('#trainInfo').val().length < 10) {
                alert('Please enter Valid Pnr No.');
                $("#trainInfo").focus();
                return false;
            }
        } 
    }
    else {
        if ($('#pnrinfo').val() != '' && $('#pnrinfo').val() != null) {
            if ($('#pnrinfo').val().length < 6) {
                alert('Please enter Valid Pnr No.');
                $("#pnrinfo").focus();
                return false;
            }
        } 
    }
    var timeflag = true;
    if ($('#txtCheckInDate').val() == $('#txtCheckOutDate').val()) {
        //if ($('#txtCheckintime').val() == $('#txtCheckouttime').val()) {
        //    alert('For Same Check-in and Check-Out Date, check-in time and check-out time should be different .');
        //    $("#txtCheckintime").focus();
        //    return false;
        //}
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
    }
    if (!timeflag) return false;
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
                    var HodDetails = new Array();
                    sodbookingLst = postsodbookingLst();
                    sodflightLst = postsodFlightLst();
                    hotelDetailLst = postsodhotelmasterlst();
                    HodDetails = PostHodDetails();
                    $.ajax({
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        type: 'POST',
                        url: '../SJSisBooking/SubmitHotelInfo',
                        data: JSON.stringify({ sodRequestsList: sodbookingLst, sodflightList: sodflightLst, hotelDetailList: hotelDetailLst, HodDetails: HodDetails }),
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
    objRm.TravelRequestTypeId = '5';
    objRm.SodBookingTypeId = '1';
    objRm.BookingFor = "Only Hotel";
    objRm.Meals = "NA";
    objRm.RequestDate = new Date();
    objRm.ReasonForTravel = $('#reasontxt').val();
    objRm.RequestedEmpCode = $('#txtEcn').val().trim();
    objRm.RequestedEmpName = $('#txtFName').val().trim() + " " + $('#txtLName').val().trim();
    objRm.RequestedEmpDesignation = $('#txtDesignation').val().trim();
    objRm.RequestedEmpDept = $('#txtDepartment').val().trim();
    objRm.EmailId = $('#txtEmails').val().trim();
    objRm.Phno = $('#txtMobileNo').val().trim();
    objRm.Passengers = 1;
    objRm.IsMandatoryTravel = 0;
    objRm.IsCabRequired = 0;
    objRm.IsAmountPaidByTraveller = 0;
    objRm.IsHotelRequired = 1;
    objRm.BookingStatus = "Open";
    objRm.SJSCHodEmailId = $('#txtHodEmail').val();
    objRm.StatusDate = new Date(1900, 0, 1, 0, 0, 0, 0);
    if ($('#txtGender').val().trim() == "Female") {
        objRm.Title = "Ms.";
    } else {
        objRm.Title = "Mr.";
    }
    if ($('#traveltype option:selected').text() == "Train") {
        if ($('#trainInfo').val() == '' || $('#trainInfo').val() == null) {
            objRm.Pnr == "NA";
        } else {
            objRm.Pnr = $('#trainInfo').val();
        }
    }
    else {
        if ($('#pnrinfo').val() == '' || $('#pnrinfo').val() == null) {
            objRm.Pnr == "NA";
        } else {
            objRm.Pnr = $('#pnrinfo').val();
        }
    }
  
    sodBookingList.push(objRm);
    return sodBookingList;
}

function postsodhotelmasterlst() {
    var hotelDetailsList = new Array();
    var objhd = new Object();
    objhd.TravelRequestTypeId = 4;
    objhd.HotelReferenceID = "";
    objhd.City = $('#txtCity').val().toUpperCase();
    objhd.EmployeeCode = $('#txtEcn').val();
    objhd.CheckInDate = ConvertMMddyyyy($('#txtCheckInDate').val()); //$('#txtCheckInDate').val();
    objhd.CheckOutDate = ConvertMMddyyyy($('#txtCheckOutDate').val()); // $('#txtCheckOutDate').val(); 
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
        if ($('input[name="radio"]:checked').val() == 'cabyes') {
            objhd.IsCabRequiredAsPerETA = true;
            objhd.CabPickupTime = $('#txtCabPickupTime').val();
        }
        else {
            objhd.IsCabRequiredAsPerETA = false;
            objhd.CabPickupTime = $('#txtCabPickupTime').val();
        }   
    }
    else {
        objhd.AirportTransport = false;
        objhd.IsCabRequiredAsPerETA = false;
    }
    objhd.HotelRequestId = 1;
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
     //objfd.TravelDate = $('#txtCheckInDate').val();
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

function PostHodDetails() {
    var HodDetails = new Array();
    var objhd = new Object();
    objhd.HoDName = $('#txtHodName').val();
    objhd.HodEmail = $('#txtHodEmail').val();
    HodDetails.push(objhd);
    return HodDetails;
}

function ConvertMMddyyyy(dval) {
    var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
    return mval;
}

function toggleAirTransport() {
    if ($('#airtransport option:selected').text() == "Yes") {
        $("#CabRequiredDiv").prop("hidden", false);
        $("#cabTimingSpan").prop("hidden", false);
        
    } else {
        $("#CabRequiredDiv").prop("hidden", true);
        $("#cabTimingSpan").prop("hidden", true);
    }
}

function changeCabResponse() {
    if ($('input[name="radio"]:checked').val() == 'cabyes') {
        $("#ETANo").prop("hidden", true);
        $("#ETAYes").prop("hidden", false); 
        $("#cabTimingSpan").prop("hidden", false);
    }
    else if ($('input[name="radio"]:checked').val() == 'cabno') { 
        $("#ETANo").prop("hidden", false);
        $("#ETAYes").prop("hidden", true);
        $("#cabTimingSpan").prop("hidden", false);
    }
}

function TravelSelect() {
    if ($('#traveltype option:selected').text() == "SJ") {
        $("#txtFlight").hide();
        $("#txtTrain").hide();
        $("#txtPnr").hide();
        $("#txtFlightMand").show();
        $("#txtPnrMand").show();
    }
    else if ($('#traveltype option:selected').text() == "Train") {
        $("#txtFlight").hide();
        $("#txtTrain").show();
        $("#txtPnr").show();
        $("#txtFlightMand").hide();
        $("#txtPnrMand").hide();
        $("#pnrinfo").hide();
        $("#trainInfo").show();        
        $("#txtApt").hide();
        $("#txtTrainP").show();
       
    } else {
        $("#txtFlight").show();
        $("#txtTrain").hide();
        $("#txtFlightMand").hide();
        $("#txtPnr").show();
        $("#txtPnrMand").hide();
        $("#trainInfo").hide();
        $("#pnrinfo").show();
        $("#txtTrainP").hide();
        $("#txtApt").show();
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
var app = angular.module("SjsisIndex", []);
app.controller("SjsisIndexCtrl", function ($scope, $http) {
    $scope.SignUpsubmitted = false;
    $scope.submitPersonalInfo = false;
    $scope.submitHodInfo = false;
    $scope.wrongPssword = false;
    $scope.samePassword = false;

    $scope.GetVerticals = function () {
        $http({
            method: "Get",
            url: "../SjSisBooking/GetVerticals",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $scope.VList = response.data;
            $scope.GetUserData();
        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    }
//    $('.SeeMore2').click(function () {
//        var $this = $(this);
//        $this.toggleClass('SeeMore2');
//        if ($this.hasClass('glyphicon')) {

//            $this.removeClass("glyphicon-plus").addClass("glyphicon-minus");
//;
//        } else {
//            $this.removeClass("glyphicon-minus").addClass("glyphicon-plus");
//        }
//    });

    $scope.GetUserData = function () {
       
        $('.loader').show();
        $http({
            method: "Get",
            url: "../SjSisBooking/GetRegUserData",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('.loader').hide();
            $scope.userInfo = response.data;            
            if (response.data.Title == "Mr") {
                $scope.Title = "Male"
            }
            else {
                $scope.Title = "Female";
            }
            $scope.HodTitleWithName = response.data.HodTitle + " " + response.data.HodName;
        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    }
    

    $scope.ShowModalWithData = function () {
        $http({
            method: "Get",
            url: "../SjSisBooking/GetRegUserData",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('#myModal').modal('show');
            $scope.UserPastInfo = response.data;
        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    }

    $scope.PersonalInfoUpdate = function (data) {
        $('.loader').show();
        $scope.submitPersonalInfo = true;
        var personalInfo = new Array();
        var obj = new Object();
        obj.Title = data.Title;
        obj.FirstName = data.FirstName;
        obj.LastName = data.LastName;
        obj.EmpCode = data.EmpCode;
        obj.SJSCVerticalID = data.SJSCVerticalID;
        obj.Department = data.Department;
        obj.Designation = data.Designation;
        obj.MobileNo = data.MobileNo;
        obj.EmailID = data.EmailID;
        personalInfo.push(obj)
        
        if ($scope.PersonalInfoForm.$valid) {
            var result = confirm("Are you sure to change your Personal details?");
            if (result) {
                $http({
                    method: "POST",
                    url: "../SjSisBooking/UpdateUserInfo",
                    data: JSON.stringify({ UserData: personalInfo[0] }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    $('.loader').hide();
                    $('#myModal').modal('hide');
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                    alert(JSON.parse(response.data));
                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                    $('.loader').hide();
                });
            }

            else {
                $('.loader').hide();
                return;
            }
        }
        else {
            $('.loader').hide();
            return;
        }
        $scope.submitPersonalInfo = false;
    };

    $scope.HodInfoUpdate = function (data) {
        $('.loader').show();
        $scope.submitHodInfo = true;
        var hodinfo = new Array();
        var obj = new Object();
        obj.HodTitle = data.HodTitle;
        obj.HodName = data.HodName;
        obj.HodEmailId = data.HodEmailId;
        obj.EmailID = data.EmailID;
        hodinfo.push(obj);
        if ($scope.HodInfoForm.$valid) {
            var result = confirm("Are you sure to change your Hod Details?");
            if (result) {
                $http({
                    method: "POST",
                    url: "../SjSisBooking/UpdateUserInfo",
                    data: JSON.stringify({ UserData: hodinfo[0] }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {   
                    $('.loader').hide();
                    $('#myModal').modal('hide');
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                    alert(JSON.parse(response.data));
                    window.location.reload();
                    }, function myError(response) {
                        alert('Error: Invalid request processing...');
                        $('.loader').hide();
                });
            }
            else {
                $('.loader').hide();
                return;
            }
        }
        else {
            $('.loader').hide();
            return;
        }
        $scope.submitHodInfo = false;
    };


    $scope.PasswordInfoUpdate = function (data) {        
        $scope.submitted = true;
        $scope.wrongCurPssword = false;
        $scope.samePassword = false;
        if ($scope.PasswordInfoForm.$valid && (data.NewPwd == data.ConfirmPwd)) {
            $('.loader').show();
            var result = confirm("Are you sure to change your Password?");
            if (result) {
                
                $http({
                    method: "POST",
                    url: "../SjSisBooking/checkCurrentPassword",
                    data: JSON.stringify({ UserData: data }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    $('.loader').hide();
                    if (JSON.parse(response.data) == "Invalid") {                        
                        $scope.wrongCurPssword = true;
                        $scope.wrongCurPsswordtxt = " Your Current Password is Wrong.";
                        return;
                    }
                    else {
                        if (data.NewPwd == data.OldPwd) {                            
                            $scope.samePassword = true;
                            $scope.wrongCurPssword = false;
                            $scope.samePasswordtxt = " Your New Password Should be different from Current Password.";
                            return;
                        }
                        $http({
                            method: "POST",
                            url: "../SjSisBooking/UpdatePasswordInfo",
                            data: JSON.stringify({ UserData: data }),
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8'
                        }).then(function mySucces(response) {
                            $('#myModal').modal('hide');
                            $('body').removeClass('modal-open');
                            $('.modal-backdrop').remove();
                            alert(JSON.parse(response.data));
                        }, function myError(response) {
                            alert('Error: Invalid request processing...');
                            //$('.loader').hide();
                        });
                    }                   
                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                    $('.loader').hide();
                });
            }
            else {
                return;
                $('.loader').hide();

            }
        }
        else {
            return;
        }
        $scope.submitted = false;
        $scope.samePassword = false;
        $scope.wrongCurPssword = false;
        $scope.PasswordInfoForm.$setPristine();
    };

    $scope.sendEmailNotification = function () {
        $http({
            method: "POST",
            url: "../bulk/sendEmailNotification",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {

        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    }
    $scope.resetForm = function () {
        //$scope.GetUserData();
        $scope.Pwd = {};  
        $('.loader').hide();
    }
    $scope.GetVerticals();
    //Log out function
    $scope.LogOut = function () {
        $http({
            method: "POST",
            url: "../SjSisBooking/Logout",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            window.location = "../sjsc/Login";            
        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    }
});
