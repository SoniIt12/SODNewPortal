// round trip, one-way, multicity radio button click
function Round_OneWayChange() {

            if ($('input[name="radio"]:checked').val() == 'round-trip') {
                
                $("#txtddate2").prop("disabled", false);
                $("#pick_up_loc2").prop("hidden", false);
                $("#arrival_flight").prop("hidden", false);
                $("#OnewayRoundDiv").prop("hidden", false);
                $("#multicityDiv").prop("hidden", true);
                $("#AddNewPassenger").prop("hidden", true);
                $("#roundborder").prop("hidden", false);
            }

            else if ($('input[name="radio"]:checked').val() == 'one-way') {
                $("#txtddate2").prop("disabled", true);
                $("#txtddate2").val("");
                $("#pick_up_loc2").prop("hidden", true);
                $("#arrival_flight").prop("hidden", true);
                $("#OnewayRoundDiv").prop("hidden", false);
                $("#multicityDiv").prop("hidden", true);
                $("#AddNewPassenger").prop("hidden", true);
                $("#roundborder").prop("hidden", false);
            }
            else if ($('input[name="radio"]:checked').val() == 'multicity') {
                $("#txtddate2").prop("disabled", true);
                $("#txtddate2").val("");
                $("#pick_up_loc2").prop("hidden", true);
                $("#arrival_flight").prop("hidden", true);
                $("#OnewayRoundDiv").prop("hidden", true);
                $("#multicityDiv").prop("hidden", false);
                $("#AddNewPassenger").prop("hidden", false);

                $("#roundborder").prop("hidden", true);
               
            }
        }

//convert MM/dd/yyyy
function ConvertMMddyyyy(dval) {
    var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
    return mval;
}

$(function () {
    var fromDate = $("#txtddate").datepicker({
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
        onClose: function (event, ui) { $("#txtddate2").focus(); }
    });

    var toDate = $("#txtddate2").datepicker({
        numberOfMonths: 2,
        defaultDate: new Date(),
        maxDate: "+10M +00D",
        dateFormat: "dd/mm/yy",
    });
});

$(function () {
    var fromDate = $("#txtddateMulti").datepicker({
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
        onClose: function (event, ui) { $("#txt1").focus(); }
    });
    var toDate = $("#txt1").datepicker({
        numberOfMonths: 2,
        defaultDate: new Date(),
        maxDate: "+10M +00D",
        dateFormat: "dd/mm/yy",
    });

});

$(document).ready(function () {
    $("#multicityDiv").prop("hidden", true);
    $('#hotel_div :input').attr('disabled', true);
    $('#cab_div :input').attr('disabled', true);
    handleStatusChanged();
    handleStatusChanged2();
});

//hotel checkbox selected
function handleStatusChanged() {
    $('#checkbox-hotel').on('change', function () {
        toggleStatus();
    });
    
}

// cab checkbox selected
function handleStatusChanged2() {
    $('#checkbox-cab').on('change', function () {
        toggleStatus2();
    });
}

function toggleStatus() {
    if ($('#checkbox-hotel').is(':checked')) {        
        $('#hotel_div :input').removeAttr('disabled');
    } else {
        $('#hotel_div :input').attr('disabled', true);
        $('#txt-city2').val('');
        $('#txtddateC1').val('');
        $('#txtddateC2').val('');
    }
}

function toggleStatus2() {
    if ($('#checkbox-cab').is(':checked')) {
        $('#cab_div :input').removeAttr('disabled');
    } else {
        $('#cab_div :input').attr('disabled', true);
        $('#txt-loc1').val('');
        $('#txt-loc2').val('');
        $('#txt-pick2').val('');
        $('#txt-locationDrop').val('');
    }
}

// datepicker
$(function () {
    var fromDate = $("#txtddateC1").datepicker({
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
        onClose: function (event, ui) { $("#txtddateC2").focus(); }
    });

    var toDate = $("#txtddateC2").datepicker({
        numberOfMonths: 2,
        defaultDate: new Date(),
        maxDate: "+10M +00D",
        dateFormat: "dd/mm/yy",
    });
});


//mobile number validation
$(document).ready(function () {
    //called when key is pressed in textbox
    $("#txt-mob").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $("#errmsg").html("Only Numbers Allowed").show().fadeOut("slow");
            return false;
        }
    });

    $("#checkbox-hotel").click(copyData);    

});


function copyData()
{
    if ($('#checkbox-hotel').is(':checked') && ($('input[name="radio"]:checked').val() == 'round-trip')) {
        var textcopy = $("#txt-destination").val();
        $("#txt-city2").val(textcopy);
        var textcheck1 = $("#txtddate").val();
        $("#txtddateC1").val(textcheck1);
        var textcheck2 = $("#txtddate2").val();
        $("#txtddateC2").val(textcheck2);
    }
    if ($('#checkbox-hotel').is(':checked') && ($('input[name="radio"]:checked').val() == 'one-way')) {
        var textcopy = $("#txt-destination").val();
        $("#txt-city2").val(textcopy);
        var textcheck1 = $("#txtddate").val();
        $("#txtddateC1").val(textcheck1);

        var parts = textcheck1.split("/");
        var trdate = new Date(parts[2], parts[1] - 1, parts[0]);
        var dateFormat = GetDateFormat(trdate.addDays(1));
        $("[id$=txtddateC2]").val(dateFormat);

    }
    if ($('#checkbox-hotel').is(':checked') && ($('input[name="radio"]:checked').val() == 'multicity')) {
        var textcopy = $("#txt-destinationMulti").val();
        $("#txt-city2").val(textcopy);
        var textcheck1 = $("#txtddateMulti").val();
        $("#txtddateC1").val(textcheck1);

        var parts = textcheck1.split("/");
        var trdate = new Date(parts[2], parts[1] - 1, parts[0]);
        var dateFormat = GetDateFormat(trdate.addDays(1));
        $("[id$=txtddateC2]").val(dateFormat);

    }
}

Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + parseInt(days));
    return this;
};

function GetDateFormat(date) {
    var month = (date.getMonth() + 1).toString();
    month = month.length > 1 ? month : '0' + month;
    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;
    return day + '/' + month + '/' + date.getFullYear();
}

//submit button click
$(document).ready(function () {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/; // email regex
    var emailaddress = $("#txt-email").val();
    var flagEmail = 0;

    $('#btnSearch_blkList').click(function () {
        if ($('input[name="radio"]:checked').val() == 'round-trip')
        {
            if ((!$.trim($('#txt-pickup').val())) || (!$.trim($('#txt-destination').val())) || (!$.trim($('#txtddate').val())) ||
                (!$.trim($('#txtddate2').val())) || (!$.trim($('#txt-info').val())) || 
                (!$.trim($('#txt-reason').val())) || (!$.trim($('#txt-info2').val())) || (!$.trim($('#txt-address').val()))
               || (!$.trim($('#txt-firstName').val())) || (!$.trim($('#txt-lastName').val()))  || (!$.trim($('#txt-mob').val()))
                || (!$.trim($('#txt-city').val())) || (!$.trim($('#txt-state').val())) || (!$.trim($('#txt-email').val()))) {
                if (!$.trim($('#txt-pickup').val())) {
                    $("#errtxt-pickup").html("Required").show();
                } 
                if (!$.trim($('#txt-destination').val())) {
                    $("#errtxt-destination").html("Required").show();
                }
                
                if (!$.trim($('#txtddate').val())) {
                    $("#errtxtddate").html("Required").show();
                }
                if (!$.trim($('#txtddate2').val())) {
                    $("#errtxtddate2").html("Required").show();
                }
                if (!$.trim($('#txt-info').val())) {
                    $("#errtxt-info").html("Required").show();
                }
                
                if (!$.trim($('#txt-reason').val())) {
                    $("#errtxt-reason").html("Required").show();
                }
                if (!$.trim($('#txt-info2').val())) {
                    $("#errtxt-info2").html("Required").show();
                }
                
                if (!$.trim($('#txt-address').val())) {
                    $("#errtxt-address").html("Required").show();
                }

                if ($('#checkbox-cab').is(':checked')) {
                    if (!$.trim($('#txt-loc1').val())) {
                        $("#errtxt-loc1").html("Required").show();
                    }
                    if (!$.trim($('#txt-loc2').val())) {
                        $("#errtxt-loc2").html("Required").show();
                    }
                    if (!$.trim($('#txt-pick2').val())) {
                        $("#errtxt-pick2").html("Required").show();
                    }
                    if (!$.trim($('#txt-locationDrop').val())) {
                        $("#errtxt-locationDrop").html("Required").show();
                    }

                }

                if (!$.trim($('#txt-firstName').val())) {
                    $("#errtxt-firstName").html("Required").show();
                }
                if (!$.trim($('#txt-lastName').val())) {
                    $("#errtxt-lastName").html("Required").show();
                }
                if (!$.trim($('#txt-email').val())) {
                    $("#errtxt-email").html("Required").show();
                }

                
                if (!$.trim($('#txt-mob').val())) {
                    $("#errmsg").html("Required").show();
                }
                if (!$.trim($('#txt-city').val())) {
                    $("#errtxt-city").html("Required").show();
                }
                if (!$.trim($('#txt-state').val())) {
                    $("#errtxt-state").html("Required").show();
                }
                
                if ($('#checkbox-hotel').is(':checked')) {
                    if (!$.trim($('#txt-city2').val())) {
                        $("#errtxt-city2").html("Required").show();
                    }
                    if (!$.trim($('#txtddateC1').val())) {
                        $("#errtxtddateC1").html("Required").show();
                    }
                    if (!$.trim($('#txtddateC2').val())) {
                        $("#errtxtddateC2").html("Required").show();
                    }
                }
            }
            else {
                var elists = new Array();
                var plists = new Array();
                var hlists = new Array();
                var clists = new Array();
                var masterList = new Array();

                //save master data for round trip
                var objMaster = new Object();
                objMaster.ReasonForTravel = $('#txt-reason').val();
                objMaster.Passengers = $('#txt-adult').val();
                objMaster.BookingStatus = "Open";
                if ($('#checkbox-cab').is(':checked')) {
                    objMaster.IsCabRequired = true;
                } else { objMaster.IsCabRequired = false; }
                if ($('#checkbox-hotel').is(':checked')) {
                    objMaster.IsHotelRequired = true;
                } else { objMaster.IsHotelRequired = false; }
                objMaster.Address = $('#txt-address').val();
                objMaster.City = $('#txt-city').val();
                objMaster.State = $('#txt-state').val();
                objMaster.EmailId = $('#txt-email').val();
                objMaster.Phno = $('#txt-mob').val();
                masterList.push(objMaster);

                //Save flight information for round trip
                var obj = new Object();
                //obj.TravelRequestId = 1;
                obj.OriginPlace = $('#txt-pickup').val();
                obj.DestinationPlace = $('#txt-destination').val();
                obj.DepartureDate = ConvertMMddyyyy($('#txtddate').val());
                obj.DepartureTime = $('#txt-depTime').val();
                obj.DepFlightInfo = $('#txt-info').val();
                obj.DepFlightNumber = $('#txt-number').val();
                obj.DepReason = $('#txt-reason').val();                
                elists.push(obj);

                var obj2 = new Object();
                //obj2.TravelRequestId = 1;
                obj2.OriginPlace = $('#txt-destination').val();
                obj2.DestinationPlace = $('#txt-pickup').val();
                obj2.DepartureDate = ConvertMMddyyyy($('#txtddate2').val());
                obj2.DepartureTime = $('#txt-retTime').val();
                obj2.DepFlightInfo = $('#txt-info2').val();
                obj2.DepFlightNumber = $('#txt-number2').val();
                obj2.DepReason = $('#txt-reason').val();
                elists.push(obj2);

                //Save passenger information for round trip
                var objP = new Object();
                //objP.TravelRequestId = 1;
                objP.FirstName = $('#txt-firstName').val();
                objP.LastName = $('#txt-lastName').val();
                if ($('#ddlbookingFor_roundtrip :selected').text() == "Mr.")
                { objP.Gender = "M" }
                else { objP.Gender = "F" };
                plists.push(objP);

               
                //Save hotel information for round trip
                if ($('#checkbox-hotel').is(':checked')) {
                    var objH = new Object();
                    //objH.TravelRequestId = 1;
                    objH.City = $('#txt-city2').val();
                    objH.CheckInDate = ConvertMMddyyyy($('#txtddateC1').val());
                    objH.CheckOutDate = ConvertMMddyyyy($('#txtddateC2').val());
                    objH.CheckinTime = $('#txt-in').val();
                    objH.CheckoutTime = $('#txt-out').val();
                    objH.Entitlement = $('#entitlement :selected').text();
                    hlists.push(objH);
                }

                //Save cab information for round trip
                if ($('#checkbox-cab').is(':checked')) {
                    var objC = new Object();
                    //objC.TravelRequestId = 1;
                    objC.PickupLoc = $('#txt-loc1').val();
                    objC.PickupTime = $('#txt-time1').val();
                    objC.DropLoc = $('#txt-loc2').val();
                    objC.DropTime = $('#txt-time2').val();
                    clists.push(objC);

                    var objC2 = new Object();
                    //objC2.TravelRequestId = 1;
                    objC2.PickupLoc = $('#txt-pick2').val();
                    objC2.PickupTime = $('#txt-picktime2').val();
                    objC2.DropLoc = $('#txt-locationDrop').val();
                    objC2.DropTime = $('#txt-timeDrop').val();
                    clists.push(objC2);
                }

                //email validation
                var flagEmail = 0;
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                var emailaddress = $("#txt-email").val();
                if (!emailReg.test(emailaddress)) {
                    $("#errtxt-email").html("Invalid Email").show();
                    flagEmail = 1;
                }


                // save data to database for round trip
                if (flagEmail == 0) {
                    $.ajax({
                        url: '../OAT/OALStoreDetails',
                        type: "POST",
                        processData: false,
                        data: JSON.stringify({ elist: elists, plist: plists, hlist: hlists, clist: clists, mlist: masterList }),
                        //data: elist,
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (response) {
                            if (response != null || response != '') {
                                window.location.href = "../Oat/oatbkresponse";
                            }
                        },
                        error: function (er) {
                        }
                    });
                }
               // location.reload();
            }
            
        }
            //one way
        else if ($('input[name="radio"]:checked').val() == 'one-way')
        {
            if ((!$.trim($('#txt-pickup').val())) || (!$.trim($('#txt-destination').val())) || (!$.trim($('#txtddate').val())) ||
                (!$.trim($('#txt-info').val())) || 
                (!$.trim($('#txt-reason').val())) || (!$.trim($('#txt-firstName').val())) || (!$.trim($('#txt-address').val()))
                || (!$.trim($('#txt-lastName').val())) || (!$.trim($('#txt-mob').val()))
                || (!$.trim($('#txt-city').val())) || (!$.trim($('#txt-state').val())) || (!$.trim($('#txt-email').val()))) {
                if (!$.trim($('#txt-pickup').val())) {
                    $("#errtxt-pickup").html("Required").show();
                }
                if (!$.trim($('#txt-destination').val())) {
                    $("#errtxt-destination").html("Required").show();
                }
                if (!$.trim($('#txtddate').val())) {
                    $("#errtxtddate").html("Required").show();
                }
                
                if (!$.trim($('#txt-info').val())) {
                    $("#errtxt-info").html("Required").show();
                }
                
                if (!$.trim($('#txt-reason').val())) {
                    $("#errtxt-reason").html("Required").show();
                }
                

                if (!$.trim($('#txt-firstName').val())) {
                    $("#errtxt-firstName").html("Required").show();
                }
                if (!$.trim($('#txt-lastName').val())) {
                    $("#errtxt-lastName").html("Required").show();
                }
                if (!$.trim($('#txt-email').val())) {
                    $("#errtxt-email").html("Required").show();
                }
                if (!$.trim($('#txt-address').val())) {
                    $("#errtxt-address").html("Required").show();
                }
                
                if (!$.trim($('#txt-mob').val())) {
                    $("#errmsg").html("Required").show();
                }
                if (!$.trim($('#txt-city').val())) {
                    $("#errtxt-city").html("Required").show();
                }
                if (!$.trim($('#txt-state').val())) {
                    $("#errtxt-state").html("Required").show();
                }
                if ($('#checkbox-cab').is(':checked')) {
                    if (!$.trim($('#txt-loc1').val())) {
                        $("#errtxt-loc1").html("Required").show();
                    }
                    if (!$.trim($('#txt-loc2').val())) {
                        $("#errtxt-loc2").html("Required").show();
                    }
                    
                }
                if ($('#checkbox-hotel').is(':checked')) {
                    if (!$.trim($('#txt-city2').val())) {
                        $("#errtxt-city2").html("Required").show();
                    }
                    if (!$.trim($('#txtddateC1').val())) {
                        $("#errtxtddateC1").html("Required").show();
                    }
                    if (!$.trim($('#txtddateC2').val())) {
                        $("#errtxtddateC2").html("Required").show();
                    }
                }
            }
            else {
                var elists = new Array();
                var plists = new Array();
                var hlists = new Array();
                var clists = new Array();
                var masterList = new Array();

                //save master data for one way
                var objMaster = new Object();
                objMaster.ReasonForTravel = $('#txt-reason').val();
                objMaster.Passengers = $('#txt-adult').val();
                objMaster.BookingStatus = "Open";
                if ($('#checkbox-cab').is(':checked')) {
                    objMaster.IsCabRequired = true;
                } else { objMaster.IsCabRequired = false; }
                if ($('#checkbox-hotel').is(':checked')) {
                    objMaster.IsHotelRequired = true;
                } else { objMaster.IsHotelRequired = false; }
                objMaster.Address = $('#txt-address').val();
                objMaster.City = $('#txt-city').val();
                objMaster.State = $('#txt-state').val();
                objMaster.EmailId = $('#txt-email').val();
                objMaster.Phno = $('#txt-mob').val();
                masterList.push(objMaster);


                //Save flight information for one way
                var obj = new Object();
                //obj.TravelRequestId = 1;
                obj.OriginPlace = $('#txt-pickup').val();
                obj.DestinationPlace = $('#txt-destination').val();
                obj.DepartureDate = ConvertMMddyyyy($('#txtddate').val());
                obj.DepartureTime = $('#txt-depTime').val();
                obj.DepFlightInfo = $('#txt-info').val();
                obj.DepFlightNumber = $('#txt-number').val();
                obj.DepReason = $('#txt-reason').val();
                elists.push(obj);

                //Save passenger information for one way
                var objP = new Object();
                //objP.TravelRequestId = 1;
                objP.FirstName = $('#txt-firstName').val();
                objP.LastName = $('#txt-lastName').val();
                if ($('#ddlbookingFor_roundtrip :selected').text() == "Mr.")
                { objP.Gender = "M" }
                else { objP.Gender = "F" };
                
                plists.push(objP);

                //Save hotel information for one way
                if ($('#checkbox-hotel').is(':checked')) {
                    
                    var objH = new Object();
                    objH.City = $('#txt-city2').val();
                    objH.CheckInDate = ConvertMMddyyyy($('#txtddateC1').val());
                    objH.CheckOutDate = ConvertMMddyyyy($('#txtddateC2').val());
                    objH.CheckinTime = $('#txt-in').val();
                    objH.CheckoutTime = $('#txt-out').val();
                    objH.Entitlement = $('#entitlement :selected').text();
                    hlists.push(objH);
                }

                //Save cab information for one way
                if ($('#checkbox-cab').is(':checked')) {
                    var objC = new Object();
                    objC.PickupLoc = $('#txt-loc1').val();
                    objC.PickupTime = $('#txt-time1').val();
                    objC.DropLoc = $('#txt-loc2').val();
                    objC.DropTime = $('#txt-time2').val();
                    clists.push(objC);
                }
                

                var flagEmail = 0;
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                var emailaddress = $("#txt-email").val();
                if (!emailReg.test(emailaddress)) {
                    $("#errtxt-email").html("Invalid Email").show();
                    flagEmail = 1;
                }

                //pass list data to save to database
                if (flagEmail == 0) {
                    $.ajax({
                        url: '../OAT/OALStoreDetails',
                        type: "POST",
                        processData: false,
                        data: JSON.stringify({ elist: elists, plist: plists, hlist: hlists, clist: clists, mlist: masterList }),
                        //data: elist,
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (response) {
                            if (response != null || response != '') {
                                         window.location.href = "../Oat/oatbkresponse";
                                        }
                                    },
                                    error: function (er) {
                                    }
                                });
                            }
                }
        }

        //multicity
        else if ($('input[name="radio"]:checked').val() == 'multicity') {

            if ((!$.trim($('#txt-pickupMulti').val())) || (!$.trim($('#txt-destinationMulti').val())) || (!$.trim($('#txtddateMulti').val())) ||
                (!$.trim($('#txt-infoMulti').val())) || 
                (!$.trim($('#txt-reasonMulti').val())) || (!$.trim($('#txt-address').val())) || (!$.trim($('#txt-firstName').val())) || (!$.trim($('#txt-lastName').val()))
                || (!$.trim($('#txt-mob').val()))
                || (!$.trim($('#txt-city').val())) || (!$.trim($('#txt-state').val())) || (!$.trim($('#txt-email').val()))) {

                if (!$.trim($('#txt-pickupMulti').val())) {
                    $("#errtxt-pickupMulti").html("Required").show();
                }
                if (!$.trim($('#txt-destinationMulti').val())) {
                    $("#errtxt-destinationMulti").html("Required").show();
                }
                if (!$.trim($('#txtddateMulti').val())) {
                    $("#errtxtddateMulti").html("Required").show();
                }

                if (!$.trim($('#txt-infoMulti').val())) {
                    $("#errtxt-infoMulti").html("Required").show();
                }
                
                if (!$.trim($('#txt-reasonMulti').val())) {
                    $("#errtxt-reasonMulti").html("Required").show();
                }

                if (!$.trim($('#txt-firstName').val())) {
                    $("#errtxt-firstName").html("Required").show();
                }
                if (!$.trim($('#txt-lastName').val())) {
                    $("#errtxt-lastName").html("Required").show();
                }
                if (!$.trim($('#txt-email').val())) {
                    $("#errtxt-email").html("Required").show();
                   
                }
                if (!$.trim($('#txt-address').val())) {
                    $("#errtxt-address").html("Required").show();
                }

                
                if (!$.trim($('#txt-mob').val())) {
                    $("#errmsg").html("Required").show();
                }
                if (!$.trim($('#txt-city').val())) {
                    $("#errtxt-city").html("Required").show();
                }
                if (!$.trim($('#txt-state').val())) {
                    $("#errtxt-state").html("Required").show();
                }

               
            }
            else {
                var elists = new Array();
                var plists = new Array();
                var hlists = new Array();
                var clists = new Array();
                var masterList = new Array();


                //Save flight information for multicity
                var obj = new Object();
                obj.OriginPlace = $('#txt-pickupMulti').val();
                obj.DestinationPlace = $('#txt-destinationMulti').val();
                obj.DepartureDate = ConvertMMddyyyy($('#txtddateMulti').val());
                obj.DepartureTime = $('#txt-depTimeMulti').val();
                obj.DepFlightInfo = $('#txt-infoMulti').val();
                obj.DepFlightNumber = $('#txt-numberMulti').val();
                obj.DepReason = $('#txt-reasonMulti').val();
                elists.push(obj);

                flag = 0;
                var counter = 2;
           
                $('#addContainers div').each(function () {
                    $('#TextBoxDiv' + counter).each(function () {
                        var obj2 = new Object();

                        if ($('#textboxorigin' + counter).val() == "") {
                            flag = 1;
                            alert('Please enter Origin Place');
                            $('#textboxorigin' + counter).focus();
                            return false;
                        }
                        else if ($('#textboxdest' + counter).val() == "") {
                            flag = 1;
                            alert('Please enter Destination place');
                            $('#textboxdest' + counter).focus();
                            return false;
                        }
                        else if ($('#txttraveldt' + counter).val() == "") {
                            flag = 1;
                            alert('Please select travel date.');
                            $('#txttraveldt' + counter).focus();
                            return false;
                        }
                        else if ($('#txttraveltym' + counter).val() == "") {
                            flag = 1;
                            alert('Please select travel time.');
                            $('#txttraveltym' + counter).focus();
                            return false;
                        }
                        else if ($('#textboxflightinfo' + counter).val() == "") {
                            flag = 1;
                            alert('Please enter flight info.');
                            $('#textboxflightinfo' + counter).focus();
                            return false;
                        }
                        
                        else {
                            flag = 0;
                            obj2.OriginPlace = $('#textboxorigin' + counter).val();
                            obj2.DestinationPlace = $('#textboxdest' + counter).val();
                            obj2.DepartureDate = ConvertMMddyyyy($('#txttraveldt' + counter).val());
                            obj2.DepartureTime = $('#txttraveltym' + counter).val();
                            obj2.DepFlightInfo = $('#textboxflightinfo' + counter).val();
                            obj2.DepFlightNumber = $('#textboxflightnumber' + counter).val();
                            obj2.DepReason = $('#txt-reasonMulti').val();
                            elists.push(obj2);
                        }
                    });
                    counter++;
                });


                //save master data for multicity
                var objMaster = new Object();
                objMaster.ReasonForTravel = $('#txt-reasonMulti').val();
                objMaster.Passengers = $("#adult_Dropdown :selected").text();
                objMaster.BookingStatus = "Open";
                if ($('#checkbox-cab').is(':checked')) {
                    objMaster.IsCabRequired = true;
                } else { objMaster.IsCabRequired = false; }
                if ($('#checkbox-hotel').is(':checked')) {
                    objMaster.IsHotelRequired = true;
                } else { objMaster.IsHotelRequired = false; }
                objMaster.Address = $('#txt-address').val();
                objMaster.City = $('#txt-city').val();
                objMaster.State = $('#txt-state').val();
                objMaster.EmailId = $('#txt-email').val();
                objMaster.Phno = $('#txt-mob').val();
                masterList.push(objMaster);


                //Save passenger information for multicity
                var objP = new Object();
                objP.FirstName = $('#txt-firstName').val();
                objP.LastName = $('#txt-lastName').val();
                if ($('#ddlbookingFor_roundtrip :selected').text() == "Mr.")
                { objP.Gender = "M" }
                else { objP.Gender = "F" };
                plists.push(objP);

                var flag2 = 0;
                var count = 0;
                $('#AddNewPassenger div div').each(function () {
                    var objP2 = new Object();

                    $('#newrow' + count).each(function () {
                        if ($('#textboxfirstN' + count).val() == "") {
                            flag2 = 1;
                            alert('Please enter First Name');
                            $(this).find("input")[0].focus();
                            return false;
                        }
                        else if ($('#textboxlast' + count).val() == "") {
                            flag2 = 1;
                            alert('Please enter Last Name');
                            $(this).find("input")[1].focus();
                            return false;
                        }
                        else {
                            flag2 = 0;
                            if ($('#title' + count + ' :selected').text() == "Mr.") {
                                objP2.Gender = "M"
                            }
                            else {
                                objP2.Gender = "F"
                            };
                            objP2.FirstName = $('#textboxfirstN' + count).val();
                            objP2.LastName = $('#textboxlast' + count).val();
                            plists.push(objP2);
                        }
                    });
                    count++;
                });

                //Save hotel information for multicity
                if ($('#checkbox-hotel').is(':checked')) {

                    var objH = new Object();
                    objH.City = $('#txt-city2').val();
                    objH.CheckInDate =ConvertMMddyyyy($('#txtddateC1').val());
                    objH.CheckOutDate =ConvertMMddyyyy($('#txtddateC2').val());
                    objH.CheckinTime = $('#txt-in').val();
                    objH.CheckoutTime = $('#txt-out').val();
                    objH.Entitlement = $('#entitlement :selected').text();
                    hlists.push(objH);
                }

                //Save cab information for multicity
                if ($('#checkbox-cab').is(':checked')) {
                    var objC = new Object();
                    objC.PickupLoc = $('#txt-loc1').val();
                    objC.PickupTime = $('#txt-time1').val();
                    objC.DropLoc = $('#txt-loc2').val();
                    objC.DropTime = $('#txt-time2').val();
                    clists.push(objC);
                }

                var flagEmail = 0;
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                var emailaddress = $("#txt-email").val();
                if (!emailReg.test(emailaddress)) {
                    $("#errtxt-email").html("Invalid Email").show();
                    flagEmail = 1;
                }

                if (flag == 0 && flag2==0 && flagEmail==0) {
                    $.ajax({
                        url: '../OAT/OALStoreDetails',
                        type: "POST",
                        processData: false,
                        data: JSON.stringify({ elist: elists, plist: plists, hlist: hlists, clist: clists, mlist: masterList }),
                        //data: elist,
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (response) {
                            if (response != null || response != '') {
                                window.location.href = "../Oat/oatbkresponse";
                            }
                        },
                        error: function (er) {
                        }
                    });
                }
             //   location.reload();
            }
        }
    });
});

//Add flights tabs by plus '+' button click in multicity
$(document).ready(function () {
    var counter = 2;
    $("#addButtonnew").click(function () {
        if (counter > 10) {
            alert("Only 10 tabs are allowed");
            return false;
        }
        var newTextBoxDiv = $(document.createElement('div'))
             .attr("id", 'TextBoxDiv' + counter);
        newTextBoxDiv.after().html('<br><div style="border:solid 1px #c2c2c2; padding:10px; border-radius:10px;">' +
            
            '<div class="row"><div class="col-md-3"><label style="color:grey;font-size:14px; font-weight:400;">From* </label>' +
            '<input  style="width: 90%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text" name="textbox' + counter +
             '" id="textboxorigin' + counter + '" value="" maxlength="30" ></div>' +
            '<div class="col-md-3"><label style="color:grey;font-size:14px; font-weight:400;">To* </label><br>' +
            '<input style="width: 90%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text" name="textbox' + counter +
             '" id="textboxdest' + counter + '" value="" maxlength="30" ></div>' +
            '<div class="col-md-3"><label style="color:grey;font-size:14px; font-weight:400;">Departure Date* </label>' +
            '<input style="width: 90%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text"  name="textbox' + counter +
             '" id="txttraveldt' + counter + '" value="" ></div>' +
            '<div class="col-md-3"><label style="color:grey;font-size:14px; font-weight:400;">Departure Time </label>' +
            '<input style="width: 90%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text"  name="textbox' + counter +
             '" id="txttraveltym' + counter + '" value="" ></div>' +
            '</div>   ' +
             '<div class="row"><div class="col-md-3"><label style="color:black; font-weight:bold;font-size:14px; ">Departure Flight Information* </label>' +
             '<input  style="width: 90%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text" name="textbox' + counter +
             '" id="textboxflightinfo' + counter + '" value="" maxlength="30" ></div>' +
            '<div class="col-md-3"><label style="color:grey;font-size:14px; font-weight:400;">Flight Number </label>' +
            '<input style="width: 90%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text" name="textbox' + counter +
             '" id="textboxflightnumber' + counter + '" value="" maxlength="12" ></div>' +
            '</div>   ' +
            '</div>'
           );

        newTextBoxDiv.appendTo("#addContainers");

        $(function () {
            $('#txttraveldt' + counter).datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
            $('#txttraveltym' + counter).wickedpicker({
                twentyFour: false, title:
                   'Select Time', showSeconds: false
            });
        });

        $(function () {
            $('#txttraveldt' + counter).keypress(function (event) {
                event.preventDefault();
                return false;
            });
        });

        counter++;

    });


    //Remove flights tabs by click on minus '-' button in multicity
    $("#removeButton").click(function () {
        if (counter == 2) {
            alert("No more tabs to remove");
            return false;
        }
        counter--;
        $("#TextBoxDiv" + counter).remove();
    });
    $("#getButtonValue").click(function () {
        var msg = '';
        for (i = 1; i < counter; i++) {
            msg += "\n Textbox #" + i + " : " + $('#textbox' + i).val();
        }
        alert(msg);
    });
});

//<------Calender and date controls textbox editing disable----------->
$(document).ready(function () {
    
    $(function () {
        $("#txt-out").keydown(function (e) {
            if (e.which === 8 || e.keycode === 8 || e.which === 46 || e.keycode === 46) {
                e.preventDefault();
                return false;
            }
        });
    });
    $(function () {
        $("#txt-in").keydown(function (e) {
            if (e.which === 8 || e.keycode === 8 || e.which === 46 || e.keycode === 46) {
                e.preventDefault();
                return false;
            }
        });
    });
    $(function () {
        $("#txt-time1").keydown(function (e) {
            if (e.which === 8 || e.keycode === 8 || e.which === 46 || e.keycode === 46) {
                e.preventDefault();
                return false;
            }
        });
    });
    $(function () {
        $("#txt-picktime2").keydown(function (e) {
            if (e.which === 8 || e.keycode === 8 || e.which === 46 || e.keycode === 46) {
                e.preventDefault();
                return false;
            }
        });
    });
    $(function () {
        $("#txt-time2").keydown(function (e) {
            if (e.which === 8 || e.keycode === 8 || e.which === 46 || e.keycode === 46) {
                e.preventDefault();
                return false;
            }
        });
    });
    $(function () {
        $("#txt-timeDrop").keydown(function (e) {
            if (e.which === 8 || e.keycode === 8 || e.which === 46 || e.keycode === 46) {
                e.preventDefault();
                return false;
            }
        });
    });



    $(function () {
        $("#txtddate").keypress(function (event) {
            event.preventDefault();
            return false;
        });
    });
    $(function () {
        $("#txtddate2").keypress(function (event) {
            event.preventDefault();
            return false;
        });
    });
    $(function () {
        $("#txtddateMulti").keypress(function (event) {
            event.preventDefault();
            return false;
        });
    });
    $(function () {
        $("#txtddateC1").keypress(function (event) {
            event.preventDefault();
            return false;
        });
    });
    $(function () {
        $("#txtddateC2").keypress(function (event) {
            event.preventDefault();
            return false;
        });
    });
});


//<------If validations are correct, hide error messages----------->
$(document).ready(function () {
$(function () {
    $("#txt-pick2").keyup(function () {
        $("#errtxt-pick2").html("").show();
    });
});
$(function () {
    $("#txt-locationDrop").keyup(function () {
        $("#errtxt-locationDrop").html("").show();
    });
});
$(function () {
    $("#txt-loc1").keyup(function () {
        $("#errtxt-loc1").html("").show();
    });
});
$(function () {
    $("#txt-loc2").keyup(function () {
        $("#errtxt-loc2").html("").show();
    });
});
$(function () {
    $("#txt-firstName").keyup(function () {
        $("#errtxt-firstName").html("").show();
    });
});
$(function () {
    $("#txt-lastName").keyup(function () {
        $("#errtxt-lastName").html("").show();
    });
});
$(function () {
    $("#txt-email").keyup(function () {
        $("#errtxt-email").html("").show();
    });
});
$(function () {
    $("#txt-mob").keyup(function () {
        $("#errmsg").html("").show();
    });
});
$(function () {
    $("#txt-city").keyup(function () {
        $("#errtxt-city").html("").show();
    });
});
$(function () {
    $("#txt-state").focus(function () {
        $("#errtxt-state").html("").show();
    });
});
$(function () {
    $("#txt-pickup").keyup(function () {
        $("#errtxt-pickup").html("").show();
    });
});
$(function () {
    $("#txt-destination").keyup(function () {
        $("#errtxt-destination").html("").show();
    });
});
$(function () {
    $("#txtddate").focus(function () {
        $("#errtxtddate").html("").show();
    });
});
$(function () {
    $("#txt-pickupMulti").keyup(function () {
        $("#errtxt-pickupMulti").html("").show();
    });
});
$(function () {
    $("#txt-destinationMulti").keyup(function () {
        $("#errtxt-destinationMulti").html("").show();
    });
});
$(function () {
    $("#txtddateMulti").focus(function () {
        $("#errtxtddateMulti").html("").show();
    });
});
$(function () {
    $("#txtddate2").focus(function () {
        $("#errtxtddate2").html("").show();
    });
});
$(function () {
    $("#txt-info").keyup(function () {
        $("#errtxt-info").html("").show();
    });
});

$(function () {
    $("#txt-infoMulti").keyup(function () {
        $("#errtxt-infoMulti").html("").show();
    });
});

$(function () {
    $("#txt-info2").keyup(function () {
        $("#errtxt-info2").html("").show();
    });
});

$(function () {
    $("#txt-city2").keyup(function () {
        $("#errtxt-city2").html("").show();
    });
});
$(function () {
    $("#txtddateC1").focus(function () {
        $("#errtxtddateC1").prop("hidden", true);
    });
});
$(function () {
    $("#txtddateC2").focus(function () {
        $("#errtxtddateC2").prop("hidden", true);
    });
});
$(function () {
    $("#txt-reason").keyup(function () {
        $("#errtxt-reason").html("").show();
    });
});
$(function () {
    $("#txt-reasonMulti").keyup(function () {
        $("#errtxt-reasonMulti").html("").show();
    });
});
$(function () {
    $("#txt-address").keyup(function () {
        $("#errtxt-address").html("").show();
    });
});
});

$(document).ready(function () {
    $("#txt-email").blur(function () {
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txt-email").val();
        if (!emailReg.test(emailaddress)) {
            $("#errtxt-email").html("Invalid Email").show();
            flagEmail = 1;
        } else
            $("#errtxt-email").html("").show();
    });

    
});
//<-----------End of validation functions----------->

// State list dropdown
function GetStateList() {
    $.ajax({
        type: "POST",
        url: "../OAT/GetStates",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (data) {
            var statelist = [];
            jstate = JSON.parse(data);
            for (var prop in jstate) {
                statelist.push(jstate[prop]);
            }
            $("#txt-state").autocomplete({
                source: statelist,
                minLength: 0,
                autofocus:true
            }).focus(function () {
                $(this).data("autocomplete").search($(this).val());
            });
            $("#txt-state").focus();
        },
        failure: function (response) {
            alert(response);
        },
        error: function (xhr, status, exception) {
            var err = eval("(" + xhr.responseText + ")");
            alert(err.Message);
        }
    });
}


//Change the passenger textboxes according to selection of adult dropdown
$(document).ready(function () {
    
    $("#adult_Dropdown").change(function () {
        var i = 0;
        $('#AddNewPassenger').empty(); // *reset the div on change*
        while (i < parseInt($(this).val())-1) {
            $('#AddNewPassenger').append('<div class="container" style="margin-top:20px;">' +            
            '<div class="row"><div id="newrow'+i+'"><div class="col-md-2"><label style="color:grey;font-size:14px; font-weight:400;">Title* </label>' +
            '<select style="color:black;width:90%; height:40px; padding-left:5px; text-indent:5px;' +
            'color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4;' +
        'font-family:Arial; border-radius:3px;" class="bookingtyp" id="title' + i + '">' +
                               ' <option>Ms.</option>' +
                               ' <option>Mr.</option>' +
                            '</select></div>' +
            '<div class="col-md-4"><label style="color:grey;padding-left:5px;font-size:14px; font-weight:400;">First Name* </label>' +
            '<input  style="width: 91%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text" name="textboxFir' + i +
             '" id="textboxfirstN' + i + '" value="" maxlength="30"  ></div>' +
            '<div class="col-md-4"><label style="color:grey;font-size:14px; font-weight:400;">Last Name* </label>' +
            '<input style="width: 90%; height:40px; padding-left:5px; text-indent:5px; color:#000000; font-size:15px;font-weight: 400; border:solid 1px #e4e4e4; font-family:Arial; border-radius:3px;" type="text"  name="textboxLas' + i +
             '" id="textboxlast' + i + '" value="" maxlength="30"  ></div>' +
            '</div></div>' +             
            '</div>')
            i++;
        }
    })
    

    
    //$("#txt-depTime").keydown(function (e) {
    //        if (e.which == 9 || e.keycode == 9) {
    //            e.preventDefault();
    //            $("#txt-reason").focus();
    //            return false;
    //        }
    //});

    //$("#txt-depTime").keyup(function (e) {
    //        if (e.which == 9 || e.keycode == 9) {
    //            e.preventDefault();
    //            $("#txt-reason").focus();
    //            return false;
    //        }
    //    });
   
});


