//Get Bulk Booking Hotel Data
//********Angular Code *********************************************************************
var app = angular.module('appblk', ['ui.bootstrap', 'dndLists']);
//Start Form
app.filter('startFrom', function () {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    }
});


//Bulk Upload for hotel
app.controller("bulkappCtrl", function ($scope, $http, $timeout, $filter, $interval) {
    //$scope.GetApproverIds =
    function GetApproverIds() {
        $('#txtcode').val("");
        $('#txthaddress').val("");
        $('#txthphone').val("");
        $('#txthemail').val("");
        $('#txtotheremail').val("");
        $('#txtremarks').val("");
        var counter = 0;
        var i = 0;
        var sector = "";
        var checklists = new Array();
        var slist = $("#tblbookinginfo input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[10].innerText;
                    var obj = new Object();
                    obj.Sector = eid.split('-')[1];
                    sector = eid.split('-')[1];
                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });

        $http({
            method: "GET",
            url: "../trns/GetApproverIds"
        }).then(function mySucces(response) {
            $("#SpanApprover1").text(response.data[0].Designation);
            $("#SpanApprover2").text(response.data[1].Designation);
            $("#EmailApprover1").text(response.data[0].EmailId);
            $("#EmailApprover2").text(response.data[1].EmailId);
        }, function myError(response) {
        });
    };
    $scope.applist = [];
    function loaddata() {
        var conName = (window.location.pathname).split('/')[1].trim().toString();
        $('.loader').show();
        $http({
            method: "POST",
            url: "../bulk/GetBulkHotelList",
            data: JSON.stringify({ 'type': conName }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'                
        }).then(function mySucces(response) {
           $('.loader').hide();
            $scope.applist = response.data;
            $scope.currentPage = 1;
            $scope.entryLimit = 10; 
            //$scope.totalItems = $scope.applist;
            $scope.filteredItems = $scope.applist.length;
            $scope.totalItems = $scope.applist.length;

        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    };

    $scope.counter = 0;
    var increaseCounter = function () {
        $scope.counter = $scope.counter + 1;
        loaddata();
        //$http({
        //    method: "GET",
        //    url: "../bulk/GetBulkHotelList"
        //}).then(function mySucces(response) {
        //    $scope.applist = response.data;
        //    $scope.currentPage = 1;
        //    $scope.entryLimit = 10;
        //    //$scope.totalItems = $scope.applist;
        //    $scope.filteredItems = $scope.applist.length;
        //    $scope.totalItems = $scope.applist.length;

        //}, function myError(response) {
        //    $scope.myWelcome = response.statusText;
        //});
    }

    function loaddataRefresh() {
        var promise = $interval(increaseCounter, 60000);
    }

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered.length;
        }, 1);
    };

    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };

    //Convert date script date formt for display
    $scope.ToJavaScriptDate = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
    };

    //Convert date script date formt for display dd/mmm/yyy/hh/mm
    $scope.ToJavaScriptDateTime = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        const monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        return (dt.getDate()) + "-" + (monthName[dt.getMonth()]) + "-" + (dt.getFullYear()) +
            "  " + (dt.getHours()) + ":" + (dt.getMinutes()) + ":" + (dt.getSeconds());
    };

    //convert MM/dd/yyyy
    $scope.ConvertMMddyyyy = function (dval) {
        var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
        return mval;
    };

    //View Bulk Booking List for HOD Approval
    $scope.viewPopupList = function (TRId) {
        hoteldatafill();
        $scope.NoncontractualButton = "false";
        $http({
            method: "GET",
            url: "../bulk/GetHotelListPopup?TrId=" + TRId
        }).then(function mySucces(response) {
            $scope.trdlist = response.data;
            $scope.trn = TRId;
            var arr = new Array();
            for (var i = 0; i < $scope.trdlist.length; i++) {
                arr.push(i + 1);
                if (response.data[i].HotelType.trim() == "Non-Contractual") {
                    $scope.NoncontractualButton = "true";
                }
            }
            $scope.trdCountList = arr;

        }, function myError(response) {
            alert(response);
        });
    }

    //View traveldesk hotel status
    $scope.Viewstatushotel = function (TRId) {
        $http({
            method: "GET",
            url: "../bulk/GetViewstatushotel?TrId=" + TRId
        }).then(function mySucces(response) {
            if (response == "pending") {
                return 1;
            } else {
                return 0;
            }
        }, function myError(response) {
            alert(response);
        });
    }


    //send mail to all users with same club id- approved by traveldesk
    $scope.apphotelinfo = function (empcode, TravelRequestId) {
        var result = confirm("Are you sure to send confirmation mail to user(s) ?");
        if (result) {
            $http({
                method: "POST",
                url: "../bulk/AppHotelRequest",
                data: JSON.stringify({ empcode: empcode, TravelRequestId: TravelRequestId }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                alert('Email notifications have been sent successfully.');
                $scope.viewPopupList(TravelRequestId);

            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    }

    //resend mail to particular user
    $scope.AppHotelRequestResendMail = function (empcode, TravelRequestId) {
        var result = confirm("Are you sure to send confirmation mail to user ?");
        if (result) {
            $http({
                method: "POST",
                url: "../bulk/AppHotelRequestResend",
                data: JSON.stringify({ empcode: empcode, TravelRequestId: TravelRequestId }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                alert('Email notification has been sent successfully.');
                $scope.viewPopupList(TravelRequestId);

            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    }

    //Check All
    $scope.checkAll = function () {
        if ($scope.selectedAll) {
            $scope.selectedAll = true;
        } else {
            $scope.selectedAll = false;
        }
        angular.forEach($scope.trdlist, function (dlist) {
            dlist.Selected = $scope.selectedAll;
        });
    };

    //clear textboxes and checkboxes on popup close
    $scope.closeRefresh = function () {
        $("#ddlCustomers").prop('selectedIndex', 0);
        $('#txtcode').val("");
        $('#txthname').val("");
        $('#txthaddress').val("");
        $('#txthphone').val("");
        $('#txthemail').val("");
        $('#txtotheremail').val("");
        $('#txtremarks').val("");
        $scope.selectedAll = false;
    }

    //Reject Bulk Booking List by traveldesk : Not Selective Option
    $scope.rejectAppList = function (TRId) {
        var s = confirm("Are you sure to rejecte the bulk booking request ? ")
        if (s) {
            $http({
                method: "GET",
                url: "../bulk/RejectHotelBulkBookingNotSelective?trId=" + TRId
            }).then(function mySucces(response) {
                if (response != null) {
                    alert(response.data);
                    location.reload();
                    sendRejectionEmailNotification();
                    loaddata();//suggested change Rejected Status--for the current row --get current index of the row
                }
            }, function myError(response) {
                alert(response);
            });
        }
    }

    $scope.GetFilterHotel = function () {
        var counter = 0;
        var i = 0;
        var sector = "";
        var checklists = new Array();
        var slist = $("#tblbookinginfo input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[7].innerText;
                    var obj = new Object();
                    obj.Sector = eid.split('-')[1];
                    sector = eid.split('-')[1].trim();
                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });

        var layout_select_html = $('#ddlCustomers').html(); //save original dropdown list
        $('#ddlCustomers').html(layout_select_html); //set original dropdown list back
        if (sector == "DEL") {
            $('#ddlCustomers').children('option').each(function () { //loop through options
                if (!$(this).text().endsWith("DEL") && !$(this).text().endsWith("GGN") && !$(this).text().endsWith("Please select")) {
                    $(this).remove();
                }
            });
        }
        else {
            $('#ddlCustomers').children('option').each(function () { //loop through options
                if (!$(this).text().endsWith(sector) && !$(this).text().endsWith("Please select")) { //do your conditional and if it should not be in the dropdown list
                    $(this).remove(); //remove option from list
                }
            });
        }
    };

    $scope.validateSector = function (id) {
        var flag = false;
        var i = 1;
        var sectorList = new Array();
        var slist = $("#tblbookinginfo input[type=checkbox]:checked");
        if (slist.length > 1) {
            slist.each(function () {
                if ($(this).is(':checked')) {
                    var sec = new Object();
                    sec.sector = $(this).closest('tr').find('td')[7].innerText;
                    var val = sec.sector;
                    var sec = val.split("-").pop();
                    sectorList.push(sec.trim());
                    for (var i = 1; i < sectorList.length; i++) {
                        if (sectorList[i] != sectorList[i - 1]) {
                            $("#chkVal_" + id).prop("checked", false);
                            if (!flag) {
                                alert("Different sectors request can not be clubbed for the same hotel.");
                                flag = true;
                            }

                        }
                    }
                }
            });
        }
        $scope.GetFilterHotel();
    }

    //Send selective requests to hotel
    $scope.sendhotelRequestSelective = function (reqID) {
        //Hotel Validation
        if ($('#txtcode').val() == '' || $('#txtcode').val() == null ||
            $('#txthaddress').val() == '' || $('#txthaddress').val() == null ||
            $('#txthphone').val() == '' || $('#txthphone').val() == null ||
            $('#txthemail').val() == '' || $('#txthemail').val() == null) {
            alert('Please complete hotel details.');
            return;
        }
        if ($('#txtrqstSubm').val() == '' || $('#txtrqstSubm').val() == null) {
            alert('Please enter name for request submitted by.');
            $('#txtrqstSubm').focus();
            return;
        }
        if ($('#txtremarks').val() == '' || $('#txtremarks').val() == null) {
            alert('Please enter hotel approval remarks.');
            $('#txtremarks').focus();
            return;
        }

        var counter = 0;
        var i = 0;
        var elists = new Array();
        var slist = $("#tblbookinginfo input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var obj = new Object();
                    obj.BReqId = $(this).closest('tr').find('#hdnBrId')[0].value;
                    obj.EmpCode = $(this).closest('tr').find('td')[1].innerText;
                    obj.sharingId = $(this).closest('tr').find('#shareIdDrop').val();
                    obj.gender = $(this).closest('tr').find('td')[3].innerText;
                    obj.HotelCity = $(this).closest('tr').find('td')[11].innerText;
                    var stringcheckinDate = $(this).closest('tr').find('td')[12].innerText;
                    var stringcheckoutDate = $(this).closest('tr').find('td')[13].innerText;
                    obj.CheckInDate = new Date(stringcheckinDate.split('/')[2], (stringcheckinDate.split('/')[1]) - 1, stringcheckinDate.split('/')[0]);
                    obj.CheckOutDate = new Date(stringcheckoutDate.split('/')[2], (stringcheckoutDate.split('/')[1]) - 1, stringcheckoutDate.split('/')[0]);;
                    obj.CheckinTime = $(this).closest('tr').find('td')[14].innerText;
                    //obj.CheckoutTime = $(this).closest('tr').find('td')[13].innerText;
                    if ($(this).closest('tr').find('td')[15].innerText == "Yes") {
                        obj.AirportTransport = true;
                    } else {
                        obj.AirportTransport = false;
                    }
                    if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
                        name = $('#ddlCustomers :selected').text().split('-')[0].toString();
                        type = "Contractual";
                        if ($('#txtTax').val() == '1') {
                            taxIncluded = true;
                        } else {
                            taxIncluded = false;
                        }
                        if (i == 1) {
                            occupancy = "Single";
                            if ($('input[name="radiomeal"]:checked').val() == 'meal1') {
                                hotelprice = $('#singlemealprice').text();
                            } else if ($('input[name="radiomeal"]:checked').val() == 'meal2') {
                                hotelprice = $('#singlemealprice2').text();
                            } else if ($('input[name="radiomeal"]:checked').val() == 'meal3') {
                                hotelprice = $('#singlemealprice3').text();
                            }
                        } else {
                            occupancy = "Double";
                            if ($('input[name="radiomeal"]:checked').val() == 'meal1') {
                                hotelprice = $('#doublemealprice').text();
                            } else if ($('input[name="radiomeal"]:checked').val() == 'meal2') {
                                hotelprice = $('#doublemealprice2').text();
                            } else if ($('input[name="radiomeal"]:checked').val() == 'meal3') {
                                hotelprice = $('#doublemealprice3').text();
                            }
                        }
                    }
                    obj.hotelprice = hotelprice;
                    elists.push(obj);
                    i++;
                }
            }
            counter++;
        });
        if (i == 0) {
            alert('Please select at least 1 employee to send request to hotel.');
            return;
        }

        //validation for gender and check-in time
        for (var item = 0; item < elists.length; item++) {
            //debugger;
            var id = elists[item].sharingId;
            var genderid = elists[item].gender;
            var checkinTime = convertTime12to24(elists[item].CheckinTime);
            var datetimeCheckin = new Date();
            datetimeCheckin.setHours(checkinTime.split(':')[0], checkinTime.split(':')[1], 0, 0);

            var count = 0;
            for (var i = item; i < elists.length; i++) {
                var shareid = elists[i].sharingId;
                var genderShare = elists[i].gender;
                if (id == shareid) {
                    count++;
                    if (genderid == genderShare) {

                    } else {
                       // alert('Sharing id cannot be same for opposite gender.');
                       // return;
                    }
                }
            }
            if (count > 2) {
               // alert('Sharing id cannot be same for more than 2 people.');
                //return;
            }
        }

        var hlists = new Array();
        for (var i = 0; i < elists.length; i++) {
            var objhotel = new Object();
            //obj.TravelRequestId = TravelRequestId;
            if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
                name = $('#ddlCustomers :selected').text();
                type = "Contractual";
            }
            else {
                name = $('#txthname').val();
                type = "Non-Contractual";
            }
            if (name == "Please select") {
                alert("Please select hotel name");
                return;
            }
            objhotel.TravelRequestID = reqID;
            objhotel.EmployeeCode = elists[i].EmpCode;
            objhotel.sharingId = elists[i].sharingId;
            objhotel.City = elists[i].HotelCity;
            objhotel.CheckInDate = elists[i].CheckInDate;
            objhotel.CheckOutDate = elists[i].CheckOutDate;
            objhotel.CheckinTime = elists[i].CheckinTime;
            objhotel.CheckoutTime = elists[i].CheckoutTime;
            objhotel.AirportTransport = elists[i].AirportTransport;
            objhotel.hotelprice = elists[i].hotelprice;

            //objhotel.clubId = "CH" + elists[0].EmpCode + reqID;
            objhotel.HotelStatus = "Pending from Hotel";
            objhotel.HotelCode = $('#txtcode').val();
            objhotel.HotelName = name.split('-')[0].trim();
            objhotel.HotelAddress = $('#txthaddress').val();
            objhotel.HotelPhoneNo = $('#txthphone').val();
            objhotel.PrimaryEmail = $('#txthemail').val();
            objhotel.SecondaryEmail = $('#txtotheremail').val();
            objhotel.Remarks_Status = $('#txtremarks').val();
            objhotel.HotelType = type;
            objhotel.BReqId = elists[i].BReqId;
            objhotel.HotelCurrencyCode = $('#txthCurrency1').text() == "" ? "INR" : $('#txthCurrency1').text();
            objhotel.SubmittedBy = $('#txtrqstSubm').val();
            hlists.push(objhotel);
        }

        var jsonlist = JSON.stringify({ elist: elists, hlist: hlists });
        $('#btnRequestHotel').prop("disabled", true);
             $('.loader').show();
        $.ajax({
            url: '../bulk/SendBulkRequestToHotel?trid=' + $("#hdTrid").val(),
            type: "POST",
            processData: false,
            data: jsonlist,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response == "Save failed") {
                    alert(response);
                    $('.loader').hide();
                    $('#btnRequestHotel').prop("disabled", false);
                }
                else {
                    //updatePNRStatus(response);
                    alert('Request successfully sent to hotel ' + name);
                    $('.loader').hide();
                    $('#btnRequestHotel').prop("disabled", false);
                    $scope.viewPopupList(reqID);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert("Status: " + textStatus); alert("Error: " + errorThrown);
                alert('Error: Invalid request processing...');
                $('#btnRequestHotel').prop("disabled", false);
                $('.loader').hide();
            }  
        });
    }

    //resend request to hotel
    $scope.ResendRequestToHotel = function (reqID, clubId, hotelname, PrimaryEmail, secondaryEmail, HotelCity) {
        var jsonlist = JSON.stringify({ travelreqId: reqID, clubId: clubId, HotelName: hotelname, PrimaryEmail: PrimaryEmail, sec_mail: secondaryEmail, HotelCity: HotelCity });
        $('.loader').show();
        $.ajax({
            url: '../bulk/ResendRequestToHotel',
            type: "POST",
            processData: false,
            data: jsonlist,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response == "Save failed") {
                    alert(response);
                    $('.loader').hide();
                }
                else {
                    //updatePNRStatus(response);
                    alert('Request successfully sent to hotel ' + name);
                    $('.loader').hide();
                    $scope.viewPopupList(reqID);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert("Status: " + textStatus); alert("Error: " + errorThrown);
                alert('Error: Invalid request processing...');
                $('.loader').hide();
            }  
            
                
        });
    }

    $scope.onDrop = function (srcList, srcIndex, targetList, targetIndex) {
        // Copy the item from source to target.
        targetList.splice(targetIndex, 0, srcList[srcIndex]);
        // Remove the item from the source, possibly correcting the index first.
        // We must do this immediately, otherwise ng-repeat complains about duplicates.
        if (srcList == targetList && targetIndex <= srcIndex) srcIndex++;
        srcList.splice(srcIndex, 1);
        // By returning true from dnd-drop we signalize we already inserted the item.
        return true;
    };

    // send request to approvers
    $scope.sendFinancialApprovalRequest = function () {
        var sodOat = "SOD";
        var counter = 0;
        var i = 0;
        var checklists = new Array();

        var slist = $("#tblbookinginfo input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var obj = new Object();
                    obj.BReqId = $(this).closest('tr').find('#hdnBrId')[0].value;
                    obj.EmpCode = $(this).closest('tr').find('td')[1].innerText;
                    obj.sharingId = $(this).closest('tr').find('#shareIdDrop').val();
                    obj.gender = $(this).closest('tr').find('td')[3].innerText;
                    obj.HotelCity = $(this).closest('tr').find('td')[11].innerText;
                    var stringcheckinDate = $(this).closest('tr').find('td')[12].innerText;
                    var stringcheckoutDate = $(this).closest('tr').find('td')[13].innerText;
                    obj.CheckInDate = new Date(stringcheckinDate.split('/')[2], (stringcheckinDate.split('/')[1]) - 1, stringcheckinDate.split('/')[0]);
                    obj.CheckOutDate = new Date(stringcheckoutDate.split('/')[2], (stringcheckoutDate.split('/')[1]) - 1, stringcheckoutDate.split('/')[0]);;
                    obj.CheckinTime = $(this).closest('tr').find('td')[14].innerText;
                    //obj.CheckoutTime = $(this).closest('tr').find('td')[13].innerText;
                    if ($(this).closest('tr').find('td')[15].innerText == "Yes") {
                        obj.AirportTransport = true;
                    } else {
                        obj.AirportTransport = false;
                    }
                    obj.TravelRequestId = $scope.trn;

                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });
        if (i == 0) {
            alert('Please select at least 1 employee to send request to HOD for approval.');
            return false;
        }

        //validation for gender and check-in time
        for (var item = 0; item < checklists.length; item++) {
            //debugger;
            var id = checklists[item].sharingId;
            var genderid = checklists[item].gender;
            var checkinTime = convertTime12to24(checklists[item].CheckinTime);
            var datetimeCheckin = new Date();
            datetimeCheckin.setHours(checkinTime.split(':')[0], checkinTime.split(':')[1], 0, 0);

            var count = 0;
            for (var i = item; i < checklists.length; i++) {
                var shareid = checklists[i].sharingId;
                var genderShare = checklists[i].gender;
                if (id == shareid) {
                    count++;
                    if (genderid != genderShare) {
                        alert('Sharing id cannot be same for opposite gender.');
                        return;
                    }
                }
            }
            if (count > 2) {
                alert('Sharing id cannot be same for more than 2 people.');
                return;
            }
        }

        if ($('#txthname').val() == 'NA' || $('#txthname').val() == '' || $('#txthname').val() == null) {
            alert('Please enter hotel name.');
            $("#txthname").focus();
            return false;
        }
        if ($('#txtcode').val() == 'NA' || $('#txtcode').val() == '' || $('#txtcode').val() == null) {
            alert('Please enter hotel code.');
            $("#txtcode").focus();
            return false;
        }
        if ($('#txthaddress').val() == 'NA' || $('#txthaddress').val() == '' || $('#txthaddress').val() == null) {
            alert('Please enter hotel address.');
            $("#txthaddress").focus();
            return false;
        }
        if ($('#txthphone').val() == 'NA' || $('#txthphone').val() == '' || $('#txthphone').val() == null) {
            alert('Please enter hotel phone.');
            $("#txthphone").focus();
            return false;
        }
        if ($('#txthemail').val() == 'NA' || $('#txthemail').val() == '' || $('#txthemail').val() == null) {
            alert('Please enter primary email id.');
            $("#txthemail").focus();
            return false;
        }
        if ($('#txtrqstSubm').val() == '' || $('#txtrqstSubm').val() == null) {
            alert('Please enter name for request submitted by.');
            $('#txtrqstSubm').focus();
            return;
        }
        if ($('#txtprice').val() == 'NA' || $('#txtprice').val() == '' || $('#txtprice').val() == null) {
            alert('Please enter hotel price.');
            $("#txtprice").focus();
            return false;
        }

        if ($('#txtaccomodation').val() == 'NA' || $('#txtaccomodation').val() == '' || $('#txtaccomodation').val() == null) {
            alert('Please enter accomodation details.');
            $("#txtaccomodation").focus();
            return false;
        }
        if ($('#txtfood').val() == 'NA' || $('#txtfood').val() == '' || $('#txtfood').val() == null) {
            alert('Please enter your meal plan.');
            $("#txtfood").focus();
            return false;
        }
        if ($('#txtairporttransfer').val() == 'NA' || $('#txtairporttransfer').val() == '' || $('#txtairporttransfer').val() == null) {
            alert('Please enter airport transport detail.');
            $("#txtairporttransfer").focus();
            return false;
        }
        if ($('#txtroom').val() == 'NA' || $('#txtroom').val() == '' || $('#txtroom').val() == null) {
            alert('Please enter room service details.');
            $("#txtroom").focus();
            return false;
        }
        if ($('#txtbuffet').val() == 'NA' || $('#txtbuffet').val() == '' || $('#txtbuffet').val() == null) {
            alert('Please enter buffet time details.');
            $("#txtbuffet").focus();
            return false;
        }
        if ($('#txtlaundry').val() == 'NA' || $('#txtlaundry').val() == '' || $('#txtlaundry').val() == null) {
            alert('Please enter laundary details.');
            $("#txtlaundry").focus();
            return false;
        }
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txthemail").val();
        if (!emailReg.test(emailaddress)) {
            alert("Invalid Primary Email Id");
            $("#txthemail").focus();
            return false;
        }
        var otherEmail = $('#txtotheremail').val();
        if (otherEmail.length > 0) {
            var Smail = $('#txtotheremail').val().split(',');
            for (var i = 0; i < Smail.length; i++) {
                if (!emailReg.test(Smail[i].trim())) {
                    alert('Please enter valid other email.');
                    $('#txtotheremail').focus();
                    return false;
                }
            }
        }
        var hodemail1 = "";
        var hodemail2 = "";
        var priceval = $("#txtprice").val();
        if ($('input[name="typeHotel"]:checked').val() == 'DomesticCurr') {
            if (priceval <= 7000) {
                hodemail1 = $("#EmailApprover1").text();
            }
            else {
                if (($("#chkapprover1").prop("checked") == false) || ($("#chkapprover2").prop("checked") == false)) {
                    alert('Approval required from both approvers for price above 7000');
                    return false;
                }
                hodemail1 = $("#EmailApprover1").text();
                hodemail2 = $("#EmailApprover2").text();
            }
        }
        else {
            if ($('#selecttxtCurrency1').val() == "Null") {
                alert('Please select Currency Code.');
                $('#selecttxtCurrency1').focus();
                return false;
            }

            if (($("#chkapprover1").prop("checked") == false) || ($("#chkapprover2").prop("checked") == false)) {
                alert('Approval required from both approvers for International Booking.');
                return false;
            }
            hodemail1 = $("#EmailApprover1").text();
            hodemail2 = $("#EmailApprover2").text();
        }
        var hname = $('#txthname').val();
        var result = confirm("Are you sure to send the request to Financial Approval(s) ?");
        if (result) {
            $('.loader').show();
            var name = "";
            var type = "";
            var phoneNo = "";
            if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
                name = $('#ddlCustomers :selected').text().split('-')[0].toString();
                type = "Contractual";
            }
            else {
                name = $('#txthname').val();
                type = "Non-Contractual";

            }
            phoneNo = $('#txthphone').val();
            var taxIncluded = true;
            if ($("#chktax").is(':checked')) {
                taxIncluded = true;
            } else {
                taxIncluded = false;
            }
            var hlists = new Array();
            for (var j = 0; j < checklists.length; j++) {
                var obj = new Object();
                if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
                    name = $('#ddlCustomers :selected').text();
                    type = "Contractual";
                }
                else {
                    name = $('#txthname').val();
                    type = "Non-Contractual";
                }
                obj.TravelRequestId = checklists[j].TravelRequestId;
                obj.EmployeeCode = checklists[j].EmpCode;
                obj.sharingId = checklists[j].sharingId;
                obj.City = checklists[j].HotelCity;
                obj.CheckInDate = checklists[j].CheckInDate;
                obj.CheckOutDate = checklists[j].CheckOutDate;
                obj.CheckinTime = checklists[j].CheckinTime;
                obj.CheckoutTime = checklists[j].CheckoutTime;
                obj.AirportTransport = checklists[j].AirportTransport;
                obj.HotelCode = $('#txtcode').val();
                obj.HotelName = name
                obj.HotelAddress = $('#txthaddress').val();
                obj.HodApprovalStatus = "0";
                obj.hotelprice = checklists[j].hotelprice;
                obj.HotelPhone = phoneNo
                obj.HotelType = type;
                obj.PrimaryEmail = $('#txthemail').val();
                obj.SecondaryEmail = $('#txtotheremail').val();
                obj.Remarks = $('#txtremarks').val();
                obj.HotelPrice = $('#txtprice').val();
                obj.HotelType = type;
                obj.BReqId = checklists[j].BReqId;
                obj.IsTaxIncluded = taxIncluded;
                obj.HotelCurrencyCode = $('#txthCurrency1').text() == "" ? "INR" : $('#txthCurrency1').text();
                obj.SubmittedBy = $('#txtrqstSubm').val();
                hlists.push(obj);
            }
            var inclists = new Array();
            for (var j = 0; j < checklists.length; j++) {
                var obj = new Object();
                obj.TravelRequestId = checklists[j].TravelRequestId;
                obj.HotelName = $('#txthname').val();
                obj.Accomodation = $('#txtaccomodation').val();
                obj.Food = $('#txtfood').val();
                obj.AirportTransfers = $('#txtairporttransfer').val();
                obj.RoomService = $('#txtroom').val();
                obj.BuffetTime = $('#txtbuffet').val();
                obj.Laundry = $('#txtlaundry').val();
                inclists.push(obj);
            }
            $('#btnFinancialApproval').prop("disabled", true);
            $http({
                method: "POST",
                url: "../bulk/sendFinancialApprovalRequest",
                data: JSON.stringify({ hlist: hlists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Email notification has been sent successfully.');
                $('#btnFinancialApproval').prop("disabled", false);
                location.reload();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
                $('#btnFinancialApproval').prop("disabled", false);
            })
        }
    }
    //if (i == 1) {
    //    var travelrqstid = checklists[0].TravelRequestId;
    //    var hotelrqstid = checklists[0].HotelRequestId;
    //    $http({
    //        method: "POST",
    //        url: "../trns/FindSimilarTravelData",
    //        data: JSON.stringify({ TravelRequestId: travelrqstid, hotelname: hname, hotelrqstid: hotelrqstid }),
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8'
    //    }).then(function mySucces(response) {
    //        if (JSON.parse(response.data) == "NotExist") {
    //            var result = confirm("Are you sure to send the request to HOD?");
    //            if (result) {
    //                $('.loader').show();
    //                var name = "";
    //                var type = "";
    //                var phoneNo = "";
    //                if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
    //                    name = $('#ddlCustomers :selected').text().split('-')[0].toString();
    //                    type = "Contractual";
    //                }
    //                else {
    //                    name = $('#txthnameE').val();
    //                    type = "Non-Contractual";
    //                }
    //                phoneNo = $('#txthphoneE').val();
    //                var taxIncluded = true;
    //                if ($("#chktax").is(':checked')) {
    //                    taxIncluded = true;
    //                } else {
    //                    taxIncluded = false;
    //                }
    //                var elists = new Array();
    //                for (var j = 0; j < checklists.length; j++) {
    //                    var obj = new Object();
    //                    obj.TravelRequestId = checklists[j].TravelRequestId;
    //                    obj.HotelRequestId = checklists[j].HotelRequestId;
    //                    obj.HotelCode = $('#txtcode').val();
    //                    obj.HotelName = name;
    //                    obj.HotelAddress = $('#txthaddressE').val();
    //                    obj.HotelPhone = phoneNo;
    //                    obj.HotelType = type;
    //                    obj.PrimaryEmail = $('#txthemailE').val();
    //                    obj.SecondaryEmail = $('#txtotheremailE').val();
    //                    obj.Remarks = $('#txtremarksE').val();
    //                    obj.HotelPrice = $('#txtprice').val();
    //                    obj.IsTaxIncluded = taxIncluded;
    //                    obj.HotelCurrencyCode = ($('#selecttxtCurrency1').val() == "") || ($('#selecttxtCurrency1').val() == "Null") ? "INR" : $('#selecttxtCurrency1').val();
    //                    elists.push(obj);
    //                }
    //                var inclists = new Array();
    //                for (var j = 0; j < checklists.length; j++) {
    //                    var obj = new Object();
    //                    obj.TravelRequestId = checklists[j].TravelRequestId;
    //                    obj.HotelRequestId = checklists[j].HotelRequestId;
    //                    obj.HotelName = name;
    //                    obj.Accomodation = $('#txtaccomodation').val();
    //                    obj.Food = $('#txtfood').val();
    //                    obj.AirportTransfers = $('#txtairporttransfer').val();
    //                    obj.RoomService = $('#txtroom').val();
    //                    obj.BuffetTime = $('#txtbuffet').val();
    //                    obj.Laundry = $('#txtlaundry').val();
    //                    inclists.push(obj);
    //                }
    //                $http({
    //                    method: "POST",
    //                    url: "../trns/sendFinancialApprovalRequest",
    //                    data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
    //                    dataType: 'json',
    //                    contentType: 'application/json; charset=utf-8'
    //                }).then(function mySucces(response) {
    //                    $('.loader').hide();
    //                    alert('Email notification has been sent successfully.');
    //                    location.reload();
    //                }, function myError(response) {
    //                    $('.loader').hide();
    //                    alert('Error: Invalid request processing...');
    //                })
    //            }
    //        }
    //        else {
    //            var existingTrid = JSON.parse(response.data).split('|')[0];
    //            var exishotelid = JSON.parse(response.data).split('|')[1];
    //            var clubresponse = confirm('An Employee with the same City, Check-In Date & Hotel ' + hname + ' has been allocated. Do you want to club?');
    //            if (clubresponse) {
    //                loadTableDataSorted(travelrqstid, hotelrqstid, existingTrid, exishotelid);
    //                setTimeout(function () { $scope.highlightRows() }, 2000);
    //            }
    //            else {
    //                var result = confirm("Are you sure to send the request to HOD?");
    //                if (result) {
    //                    $('.loader').show();
    //                    var name = "";
    //                    var type = "";
    //                    var phoneNo = "";
    //                    if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
    //                        name = $('#ddlCustomers :selected').text().split('-')[0].toString();
    //                        type = "Contractual";
    //                    }
    //                    else {
    //                        name = $('#txthnameE').val();
    //                        type = "Non-Contractual";
    //                    }
    //                    phoneNo = $('#txthphoneE').val();
    //                    var taxIncluded = true;
    //                    if ($("#chktax").is(':checked')) {
    //                        taxIncluded = true;
    //                    } else {
    //                        taxIncluded = false;
    //                    }
    //                    var elists = new Array();
    //                    for (var j = 0; j < checklists.length; j++) {
    //                        var obj = new Object();
    //                        obj.TravelRequestId = checklists[j].TravelRequestId;
    //                        obj.HotelRequestId = checklists[j].HotelRequestId;
    //                        obj.HotelCode = $('#txtcodeE').val();
    //                        obj.HotelName = name;
    //                        obj.HotelAddress = $('#txthaddressE').val();
    //                        obj.HotelPhone = phoneNo;
    //                        obj.HotelType = type;
    //                        obj.PrimaryEmail = $('#txthemailE').val();
    //                        obj.SecondaryEmail = $('#txtotheremailE').val();
    //                        obj.Remarks = $('#txtremarksE').val();
    //                        obj.HotelPrice = $('#txtprice').val();
    //                        obj.IsTaxIncluded = taxIncluded;
    //                        obj.HotelCurrencyCode = ($('#selecttxtCurrency1').val() == "") || ($('#selecttxtCurrency1').val() == "Null") ? "INR" : $('#selecttxtCurrency1').val();
    //                        elists.push(obj);
    //                    }
    //                    var inclists = new Array();
    //                    for (var j = 0; j < checklists.length; j++) {
    //                        var obj = new Object();
    //                        obj.TravelRequestId = checklists[j].TravelRequestId;
    //                        obj.HotelRequestId = checklists[j].HotelRequestId;
    //                        obj.HotelName = name;
    //                        obj.Accomodation = $('#txtaccomodation').val();
    //                        obj.Food = $('#txtfood').val();
    //                        obj.AirportTransfers = $('#txtairporttransfer').val();
    //                        obj.RoomService = $('#txtroom').val();
    //                        obj.BuffetTime = $('#txtbuffet').val();
    //                        obj.Laundry = $('#txtlaundry').val();
    //                        inclists.push(obj);
    //                    }

    //                    $http({
    //                        method: "POST",
    //                        url: "../trns/sendFinancialApprovalRequest",
    //                        data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
    //                        dataType: 'json',
    //                        contentType: 'application/json; charset=utf-8'
    //                    }).then(function mySucces(response) {
    //                        $('.loader').hide();
    //                        alert('Email notification has been sent successfully.');
    //                        location.reload();
    //                    }, function myError(response) {
    //                        $('.loader').hide();
    //                        alert('Error: Invalid request processing...');
    //                    })
    //                }
    //            }
    //        }
    //    }, function myError(response) {
    //        $('.loader').hide();
    //        alert('Error: Invalid request processing...');

    //    });
    //}
    //else {
    //    var result = confirm("Are you sure to send the request to HOD?");
    //    if (result) {
    //        $('.loader').show();
    //        var name = "";
    //        var type = "";
    //        var phoneNo = "";
    //        if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
    //            name = $('#ddlCustomers :selected').text().split('-')[0].toString();
    //            type = "Contractual";
    //        }
    //        else {
    //            name = $('#txthnameE').val();
    //            type = "Non-Contractual";

    //        }
    //        phoneNo = $('#txthphoneE').val();
    //        var taxIncluded = true;
    //        if ($("#chktax").is(':checked')) {
    //            taxIncluded = true;
    //        } else {
    //            taxIncluded = false;
    //        }
    //        var elists = new Array();
    //        for (var j = 0; j < checklists.length; j++) {
    //            var obj = new Object();
    //            obj.TravelRequestId = checklists[j].TravelRequestId;
    //            obj.HotelRequestId = checklists[j].HotelRequestId;
    //            obj.HotelCode = $('#txtcodeE').val();
    //            obj.HotelName = name;
    //            obj.HotelAddress = $('#txthaddressE').val();
    //            obj.HotelPhone = phoneNo;
    //            obj.HotelType = type;
    //            obj.PrimaryEmail = $('#txthemailE').val();
    //            obj.SecondaryEmail = $('#txtotheremailE').val();
    //            obj.Remarks = $('#txtremarksE').val();
    //            obj.HotelPrice = $('#txtprice').val();
    //            obj.IsTaxIncluded = taxIncluded;
    //            obj.HotelCurrencyCode = ($('#selecttxtCurrency1').val() == "") || ($('#selecttxtCurrency1').val() == "Null") ? "INR" : $('#selecttxtCurrency1').val();
    //            elists.push(obj);
    //        }
    //        var inclists = new Array();
    //        for (var j = 0; j < checklists.length; j++) {
    //            var obj = new Object();
    //            obj.TravelRequestId = checklists[j].TravelRequestId;
    //            obj.HotelRequestId = checklists[j].HotelRequestId;
    //            obj.HotelName = name;
    //            obj.Accomodation = $('#txtaccomodation').val();
    //            obj.Food = $('#txtfood').val();
    //            obj.AirportTransfers = $('#txtairporttransfer').val();
    //            obj.RoomService = $('#txtroom').val();
    //            obj.BuffetTime = $('#txtbuffet').val();
    //            obj.Laundry = $('#txtlaundry').val();
    //            inclists.push(obj);
    //        }
    //        $http({
    //            method: "POST",
    //            url: "../trns/sendFinancialApprovalRequest",
    //            data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
    //            dataType: 'json',
    //            contentType: 'application/json; charset=utf-8'
    //        }).then(function mySucces(response) {
    //            $('.loader').hide();
    //            alert('Email notification has been sent successfully.');
    //            location.reload();
    //        }, function myError(response) {
    //            $('.loader').hide();
    //            alert('Error: Invalid request processing...');
    //        })
    //    }
    //}
    //}

    // view detail for hod approval status
    $scope.viewdetailHODApproval = function (TravelRequestId, BReqId) {
        $http({
            method: "GET",
            url: "../bulk/getdetailHODApproval?trId=" + TravelRequestId + "|" + BReqId
        }).then(function mySucces(response) {
            $scope.approvalinfo = response.data;

        }, function myError(response) {
        });
    };

    $scope.ResendApproverRequest = function (TravelRequestId) {
        var result = confirm("Are you sure to resend requests to Approver(s) ?");
        if (result) {
            $('.loader').show();
            var sodOat = "SOD";
            $http({
                method: "POST",
                url: "../bulk/ResendApproverRequest",
                data: JSON.stringify({ TravelRequestId: TravelRequestId, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert(JSON.parse(response.data));

            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }


    //request to hotel by button after approval
    $scope.RequestToHotlAfterApproval = function (TravelRequestId) {

        var result = confirm("Are you sure to send request to hotel ?");
        if (result) {
            $('.loader').show();
            var sodOat = "SOD";
            $http({
                method: "POST",
                url: "../bulk/RequestToHotlAfterApproval",
                data: JSON.stringify({ TravelRequestId: TravelRequestId, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert(JSON.parse(response.data));
                $('#btnRqstHotelApp').hide();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }

    $scope.CurrencyList = function () {
        $http({
            method: "GET",
            url: "../InclusionMaster/GetCurrencyList"
        }).then(function mySucces(response) {
            $scope.CurrencyList = response.data;

        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    };
    $scope.CurrencyList();

    loaddata();
    hoteldatafill();
    loaddataRefresh();
    GetApproverIds();
});


//Send Rejection Email Notification to User :Non Selective
sendRejectionEmailNotification = function () {
    $.ajax({
        url: "../bulk/sendEmailNotification",
        success: function (result) {
        }
    });
}

function xRefresh() {
    location.reload();
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
function validatePhoneNumber(evt) {
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
        key == 46 || key == 45 ||
        //comma 
        key == 188) {
        // input is VALID
    }
    else {
        // input is INVALID
        e.returnValue = false;
        if (e.preventDefault) e.preventDefault();
    }
}
function preventAndOperator(evt) {
    var e = evt || window.event;
    var key = e.keyCode || e.which;

    if (key == 55 || key == 222 || key == 16) {
        // input is InVALID
        e.returnValue = false;
        if (e.preventDefault) e.preventDefault();
        alert("you enter invalid charater.");
    }
    else {
        //alert("else");
        // input is VALID       
    }
}


//fill hotel names in dropdown list from database
function hoteldatafill() {
    var ddlCustomers = $("#ddlCustomers");
    ddlCustomers.empty().append('<option selected="selected" value="0" disabled = "disabled">Loading.....</option>');
    $.ajax({
        type: "POST",
        url: "../trns/HotelListData",
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            ddlCustomers.empty().append('<option selected="selected" value="0">Please select</option>');
            $.each(response, function () {
                ddlCustomers.append($("<option></option>").val(this['Value']).html(this['Text']));
            });
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};


//fill hotel information of existing hotel by name on dropdown value change
function hotelfilldropdown() {
    var name = $('#ddlCustomers :selected').text();
    $.ajax({
        type: "POST",
        url: "../trns/hotelfilldropdown?name=" + name,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //$scope.hoteldetailsfill = response.data["hoteldetails"];
            $('#txtcode').val(response.hoteldetails[0].HotelCode);
            $('#txthname').val(response.hoteldetails[0].HotelName);
            $('#txthaddress').val(response.hoteldetails[0].Address);
            $('#txthphone').val(response.hoteldetails[0].Phone);
            $('#txthemail').val(response.hoteldetails[0].PrimaryEmail);
            $('#txtotheremail').val(response.hoteldetails[0].SecondaryEmail);
            var pricecount = response.pricedetails.length;
            if (pricecount == '1') {
                $('#mealpricediv1').show();
                $('#mealpricediv2').hide();
                $('#mealpricediv3').hide();
                $("#singlemealprice").text(response.pricedetails[0].SinglePrice);
                $("#doublemealprice").text(response.pricedetails[0].DoublePrice);
                if (response.pricedetails[0].TCId == "1") {
                    $("#mealtype").text("APAI");
                } else if (response.pricedetails[0].TCId == "2") {
                    $("#mealtype").text("MAP");
                } else {
                    $("#mealtype").text("CPAI");
                }
                $('#txthCurrency1').text(response.pricedetails[0].HotelCurrencyCode == null ? 'INR' : response.pricedetails[0].HotelCurrencyCode);
            } else if (pricecount == '2') {
                $('#mealpricediv1').show();
                $('#mealpricediv2').show();
                $('#mealpricediv3').hide();
                $("#singlemealprice").text(response.pricedetails[0].SinglePrice);
                $("#doublemealprice").text(response.pricedetails[0].DoublePrice);
                if (response.pricedetails[0].TCId == "1") {
                    $("#mealtype").text("APAI");
                } else if (response.pricedetails[0].TCId == "2") {
                    $("#mealtype").text("MAP");
                } else if (response.pricedetails[0].TCId == "3") {
                    $("#mealtype").text("CPAI");
                }
                $('#txthCurrency1').text(response.pricedetails[0].HotelCurrencyCode == null ? 'INR' : response.pricedetails[0].HotelCurrencyCode);
                $("#singlemealprice2").text(response.pricedetails[1].SinglePrice);
                $("#doublemealprice2").text(response.pricedetails[1].DoublePrice);
                if (response.pricedetails[1].TCId == "1") {
                    $("#mealtype2").text("APAI");
                } else if (response.pricedetails[1].TCId == "2") {
                    $("#mealtype2").text("MAP");
                } else if (response.pricedetails[1].TCId == "3") {
                    $("#mealtype2").text("CPAI");
                }
                $('#txthCurrency2').text(response.pricedetails[1].HotelCurrencyCode == null ? 'INR' : response.pricedetails[1].HotelCurrencyCode);
            } else if (pricecount == '3') {
                $('#mealpricediv1').show();
                $('#mealpricediv2').show();
                $('#mealpricediv3').show();
                $("#singlemealprice").text(response.pricedetails[0].SinglePrice);
                $("#doublemealprice").text(response.pricedetails[0].DoublePrice);
                if (response.pricedetails[0].TCId == "1") {
                    $("#mealtype").text("APAI");
                } else if (response.pricedetails[0].TCId == "2") {
                    $("#mealtype").text("MAP");
                } else if (response.pricedetails[0].TCId == "3") {
                    $("#mealtype").text("CPAI");
                }
                $('#txthCurrency1').text(response.pricedetails[0].HotelCurrencyCode == null ? 'INR' : response.pricedetails[0].HotelCurrencyCode);
                $("#singlemealprice2").text(response.pricedetails[1].SinglePrice);
                $("#doublemealprice2").text(response.pricedetails[1].DoublePrice);
                if (response.pricedetails[1].TCId == "1") {
                    $("#mealtype2").text("APAI");
                } else if (response.pricedetails[1].TCId == "2") {
                    $("#mealtype2").text("MAP");
                } else if (response.pricedetails[1].TCId == "3") {
                    $("#mealtype2").text("CPAI");
                }
                $('#txthCurrency2').text(response.pricedetails[1].HotelCurrencyCode == null ? 'INR' : response.pricedetails[1].HotelCurrencyCode);

                $("#singlemealprice3").text(response.pricedetails[2].SinglePrice);
                $("#doublemealprice3").text(response.pricedetails[2].DoublePrice);
                if (response.pricedetails[2].TCId == "1") {
                    $("#mealtype3").text("APAI");
                } else if (response.pricedetails[2].TCId == "2") {
                    $("#mealtype3").text("MAP");
                } else if (response.pricedetails[2].TCId == "3") {
                    $("#mealtype3").text("CPAI");
                }
                $('#txthCurrency3').text(response.pricedetails[2].HotelCurrencyCode == null ? 'INR' : response.pricedetails[2].HotelCurrencyCode);
            }
        }
    });
}


//convert string form of hh:mm AM/PM to 24 hour time
function convertTime12to24(time12h) {
    var modifier = time12h.split(' ')[1];
    var time = time12h.split(' ')[0];

    var hours = time.split(':')[0];

    if (hours === '12') {
        hours = '00';
    }

    if (modifier === 'PM') {
        hours = parseInt(hours, 10) + 12;
    }

    return hours + ':' + time.split(':')[1];
}

!function (e) {
    function n(e, n) { return "all" == n ? e : e.filter(function (e) { return -1 != n.toLowerCase().indexOf(e) }) } var a = "application/x-dnd", r = "application/json", t = "Text", d = ["move", "copy", "link"]
    e.directive("dndDraggable", ["$parse", "$timeout", function (e, i) {
        return function (l, f, c) {
            f.attr("draggable", "true"), c.dndDisableIf && l.$watch(c.dndDisableIf, function (e) { f.attr("draggable", !e) }), f.on("dragstart", function (s) {
                if (s = s.originalEvent || s, "false" == f.attr("draggable")) return !0
                o.isDragging = !0, o.itemType = c.dndType && l.$eval(c.dndType).toLowerCase(), o.dropEffect = "none", o.effectAllowed = c.dndEffectAllowed || d[0], s.dataTransfer.effectAllowed = o.effectAllowed
                var g = l.$eval(c.dndDraggable), u = a + (o.itemType ? "-" + o.itemType : "")
                try { s.dataTransfer.setData(u, angular.toJson(g)) } catch (p) {
                    var v = angular.toJson({ item: g, type: o.itemType })
                    try { s.dataTransfer.setData(r, v) } catch (p) {
                        var D = n(d, o.effectAllowed)
                        s.dataTransfer.effectAllowed = D[0], s.dataTransfer.setData(t, v)
                    }
                } if (f.addClass("dndDragging"), i(function () { f.addClass("dndDraggingSource") }, 0), s._dndHandle && s.dataTransfer.setDragImage && s.dataTransfer.setDragImage(f[0], 0, 0), e(c.dndDragstart)(l, { event: s }), c.dndCallback) {
                    var y = e(c.dndCallback)
                    o.callback = function (e) { return y(l, e || {}) }
                } s.stopPropagation()
            }), f.on("dragend", function (n) {
                n = n.originalEvent || n, l.$apply(function () {
                    var a = o.dropEffect, r = { copy: "dndCopied", link: "dndLinked", move: "dndMoved", none: "dndCanceled" }
                    e(c[r[a]])(l, { event: n }), e(c.dndDragend)(l, { event: n, dropEffect: a })
                }), o.isDragging = !1, o.callback = void 0, f.removeClass("dndDragging"), f.removeClass("dndDraggingSource"), n.stopPropagation(), i(function () { f.removeClass("dndDraggingSource") }, 0)
            }), f.on("click", function (n) { c.dndSelected && (n = n.originalEvent || n, l.$apply(function () { e(c.dndSelected)(l, { event: n }) }), n.stopPropagation()) }), f.on("selectstart", function () { this.dragDrop && this.dragDrop() })
        }
    }]), e.directive("dndList", ["$parse", function (e) {
        return function (i, l, f) {
            function c(e) {
                if (!e) return t
                for (var n = 0; n < e.length; n++) if (e[n] == t || e[n] == r || e[n].substr(0, a.length) == a) return e[n]
                return null
            } function s(e) { return o.isDragging ? o.itemType || void 0 : e == t || e == r ? null : e && e.substr(a.length + 1) || void 0 } function g(e) { return E.disabled ? !1 : E.externalSources || o.isDragging ? E.allowedTypes && null !== e ? e && -1 != E.allowedTypes.indexOf(e) : !0 : !1 } function u(e, a) {
                var r = d
                return a || (r = n(r, e.dataTransfer.effectAllowed)), o.isDragging && (r = n(r, o.effectAllowed)), f.dndEffectAllowed && (r = n(r, f.dndEffectAllowed)), r.length ? e.ctrlKey && -1 != r.indexOf("copy") ? "copy" : e.altKey && -1 != r.indexOf("link") ? "link" : r[0] : "none"
            } function p() { return T.remove(), l.removeClass("dndDragover"), !0 } function v(n, a, r, t, d, l) { return e(n)(i, { callback: o.callback, dropEffect: r, event: a, external: !o.isDragging, index: void 0 !== d ? d : D(), item: l || void 0, type: t }) } function D() { return Array.prototype.indexOf.call(m.children, h) } function y() {
                var e
                return angular.forEach(l.children(), function (n) {
                    var a = angular.element(n)
                    a.hasClass("dndPlaceholder") && (e = a)
                }), e || angular.element("<li class='dndPlaceholder'></li>")
            } var T = y()
            T.remove()
            var h = T[0], m = l[0], E = {}
            l.on("dragenter", function (e) {
                e = e.originalEvent || e
                var n = f.dndAllowedTypes && i.$eval(f.dndAllowedTypes)
                E = { allowedTypes: angular.isArray(n) && n.join("|").toLowerCase().split("|"), disabled: f.dndDisableIf && i.$eval(f.dndDisableIf), externalSources: f.dndExternalSources && i.$eval(f.dndExternalSources), horizontal: f.dndHorizontalList && i.$eval(f.dndHorizontalList) }
                var a = c(e.dataTransfer.types)
                return a && g(s(a)) ? void e.preventDefault() : !0
            }), l.on("dragover", function (e) {
                e = e.originalEvent || e
                var n = c(e.dataTransfer.types), a = s(n)
                if (!n || !g(a)) return !0
                if (h.parentNode != m && l.append(T), e.target != m) {
                    for (var r = e.target; r.parentNode != m && r.parentNode;) r = r.parentNode
                    if (r.parentNode == m && r != h) {
                        var d = r.getBoundingClientRect()
                        if (E.horizontal) var o = e.clientX < d.left + d.width / 2
                        else var o = e.clientY < d.top + d.height / 2
                        m.insertBefore(h, o ? r : r.nextSibling)
                    }
                } var i = n == t, D = u(e, i)
                return "none" == D ? p() : f.dndDragover && !v(f.dndDragover, e, D, a) ? p() : (e.preventDefault(), i || (e.dataTransfer.dropEffect = D), l.addClass("dndDragover"), e.stopPropagation(), !1)
            }), l.on("drop", function (e) {
                e = e.originalEvent || e
                var n = c(e.dataTransfer.types), a = s(n)
                if (!n || !g(a)) return !0
                e.preventDefault()
                try { var d = JSON.parse(e.dataTransfer.getData(n)) } catch (l) { return p() } if ((n == t || n == r) && (a = d.type || void 0, d = d.item, !g(a))) return p()
                var y = n == t, T = u(e, y)
                if ("none" == T) return p()
                var h = D()
                return f.dndDrop && (d = v(f.dndDrop, e, T, a, h, d), !d) ? p() : (o.dropEffect = T, o.dropEffect = T, y || (e.dataTransfer.dropEffect = T), d !== !0 && i.$apply(function () { i.$eval(f.dndList).splice(h, 0, d) }), v(f.dndInserted, e, T, a, h, d), p(), e.stopPropagation(), !1)
            }), l.on("dragleave", function (e) {
                e = e.originalEvent || e
                var n = document.elementFromPoint(e.clientX, e.clientY)
                m.contains(n) && !e._dndPhShown ? e._dndPhShown = !0 : p()
            })
        }
    }]), e.directive("dndNodrag", function () { return function (e, n, a) { n.attr("draggable", "true"), n.on("dragstart", function (e) { e = e.originalEvent || e, e._dndHandle || (e.dataTransfer.types && e.dataTransfer.types.length || e.preventDefault(), e.stopPropagation()) }), n.on("dragend", function (e) { e = e.originalEvent || e, e._dndHandle || e.stopPropagation() }) } }), e.directive("dndHandle", function () { return function (e, n, a) { n.attr("draggable", "true"), n.on("dragstart dragend", function (e) { e = e.originalEvent || e, e._dndHandle = !0 }) } })
    var o = {}
}(angular.module("dndLists", []));