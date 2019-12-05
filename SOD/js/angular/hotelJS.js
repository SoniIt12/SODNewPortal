//Hotel Desk Module :Hotel Booking Linst Info
var app = angular.module('TransHistory', ['ui.bootstrap', 'ngSanitize']);

app.filter('startFrom', function () {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    }
});

//New Line Filter
app.filter('nlfilter', function () {
    return function (text) {
        text = String(text).trim();
        return (text.length > 0 ? '<p>' + text.replace(/[\r\n]+/g, '</p><p>') + '</p>' : null);
    }
});


//Get Employee Booking History
app.controller("TransController", function ($scope, $http, $timeout, $interval) {
   
    function loaddata() {
        $('.loader').show();
        $http({
            method: "GET",
            url: "../trns/GetEmpHotelBookingInfo?type=SOD"
        }).then(function mySucces(response) {
            $('.loader').hide();
            $scope.trlist = response.data.bookingList;//fill hotel booking details data
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
            $scope.totalItems = $scope.trlist.length;
            $scope.deptlist = response.data.deptList;//fill department list data
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    };

    $scope.counter = 0;
    var increaseCounter = function () {
        $scope.counter = $scope.counter + 1;
        $http({
            method: "GET",
            url: "../trns/GetEmpHotelBookingInfo?type=SOD"
        }).then(function mySucces(response) {
            $scope.trlist = response.data.bookingList;//fill hotel booking details data
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
            $scope.totalItems = $scope.trlist.length;
            $scope.deptlist = response.data.deptList;//fill department list data

        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
        $("#tbldetail").addClass("table-striped");
        $("#tbldetail tr:nth-child(3) td:nth-child(22)").hide();
        $("#tbldetail tr:nth-child(3) td:nth-child(21)").show();

        $("#tbldetail tr:nth-child(2) td:nth-child(22)").hide();
        $("#tbldetail tr:nth-child(2) td:nth-child(21)").show();
        $("#chkheader").show();
    }

    function loaddataRefresh() {
        var promise = $interval(increaseCounter, 300000);
    }

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };
    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered.length;
            $scope.filteredItemsOat = $scope.filtered.length;
        }, 1);
    };

    $scope.filterdept = function (val) {
        $timeout(function () {
            $scope.search = $scope.searchDept.dept_name;
            $scope.filteredItems = $scope.searchDept.dept_name.length;
        }, 1);
    };

    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };

    $scope.showstatus = function (TravelRequestId, HotelRequestId) {
        //    $http({
        //        method: "GET",
        //        url: "../trns/GetHotelStatus?reqId=" + TravelRequestId + "|" + HotelRequestId
        //    }).then(function mySucces(response) {
        //        var val = '';
        //        var s = response.data.toString().split('.');
        //        for (var i = 0; i < s.length; i++) {
        //            if (i == 0)
        //                val = s[i] + ' \n';
        //            else
        //                val = val + s[i] + '\n';
        //        }
        //        $scope.bstatus = val;
        //    }, function myError(response) {
        //        //$scope.myWelcome = response.statusText;
        //    });
    };
    //Check All
    $scope.checkAll = function () {
        if ($scope.selectedAll) {
            $scope.selectedAll = true;
        } else {
            $scope.selectedAll = false;
        }
        angular.forEach($scope.trlist, function (list) {
            list.Selected = $scope.selectedAll;
            list.Selection = $scope.selectionAll;
        });
    };

    $scope.GetApproverIds = function () {
        $('#txtcodeE').val("");
        $('#txthaddressE').val("");
        $('#txthphoneE').val("");
        $('#txthemailE').val("");
        $('#txtotheremailE').val("");
        $('#txtremarksE').val("");

        $('#txtcodeEx').val("");
        $('#txthaddressEx').val("");
        $('#txthphoneEx').val("");
        $('#txthemailEx').val("");
        $('#txtotheremailEx').val("");
        $('#txtremarksEx').val("");
       
        var counter = 0;
        var i = 0;
        var sector = "";
        var checklists = new Array();
        var slist = $('#rdoSod').is(':checked') == true ? $("#tbldetail input[type=checkbox]") : $("#tbldetailEx input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[10].innerText;
                    var obj = new Object();
                    obj.Sector = eid.split('-')[1].trim();
                    sector = eid.split('-')[1].trim();
                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });

        if ($('#rdoExceptional').is(':checked') == true) {            
            var layout_select_html = hotelFullData; //save original dropdown list
            var layout_select_htmlNew = layout_select_html;
            $('#ddlCustomersEx').html(layout_select_htmlNew); //set original dropdown list back
            if (sector == "DEL") {
                $('#ddlCustomersEx').children('option').each(function () { //loop through options
                    if (!$(this).text().endsWith("DEL") && !$(this).text().endsWith("GGN") && !$(this).text().endsWith("Please select")) {
                        $(this).remove();
                    }
                });
            }
            else {               
                $('#ddlCustomersEx').children('option').each(function () { //loop through options
                    if (!$(this).text().endsWith(sector) && !$(this).text().endsWith("Please select")) { //do your conditional and if it should not be in the dropdown list
                        $(this).remove(); //remove option from list
                    }
                });
            }
        }
        else if ($('#rdoSod').is(':checked') == true) {
            var layout_select_html = hotelFullData;//save original dropdown list
            var layout_select_htmlnew = layout_select_html 
            $('#ddlCustomers').html(layout_select_htmlnew); //set original dropdown list back
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
        }

        $http({
            method: "GET",
            url: "../trns/GetApproverIds"
        }).then(function mySucces(response) {

            $("#SpanApprover1").text(response.data[0].Designation);
            $("#SpanApprover2").text(response.data[1].Designation);
            //$("#SpanApprover3").text(response.data[2].EmpName);
            $("#EmailApprover1").text(response.data[0].EmailId);
            $("#EmailApprover2").text(response.data[1].EmailId);
            //$("#EmailApprover3").text(response.data[2].EmailId);

            $("#SpanApproverOat1").text(response.data[0].Designation);
            $("#SpanApproverOat2").text(response.data[1].Designation);
            //$("#SpanApproverOat3").text(response.data[2].EmpName);
            $("#EmailApproverOat1").text(response.data[0].EmailId);
            $("#EmailApproverOat2").text(response.data[1].EmailId);
            //$("#EmailApproverOat3").text(response.data[2].EmailId);

            $("#SpanApprover1Ex").text(response.data[0].Designation); //edited by soni
            $("#SpanApprover2Ex").text(response.data[1].Designation); //edited by soni
            //$("#SpanApprover3").text(response.data[2].EmpName);
            $("#EmailApprover1Ex").text(response.data[0].EmailId); //edited by soni
            $("#EmailApprover2Ex").text(response.data[1].EmailId); //edited by soni
        }, function myError(response) {
        });
    };

    //show popup model data
    $scope.viewdetail = function (TravelRequestId) {
        $http({
            method: "GET",
            url: "../trns/GetBookingandHotelInfo?trId=" + TravelRequestId
        }).then(function mySucces(response) {
            $scope.arrbooking = response.data["hotelinfo"];

            $scope.arrFlight = response.data["flightInfo"];
            $scope.arrpass = response.data["passInfo"];
            $scope.arrpassod = response.data["bookingInfo"];
            $scope.trequestId = TravelRequestId;
            $scope.flightData = response.data["flightInfo"];
            hoteldatafill();

        }, function myError(response) {
        });
    };

    $scope.viewdetailoat = function (TravelRequestId) {
        $http({
            method: "GET",
            url: "../trns/GetBookingandHotelInfo?trId=" + TravelRequestId
        }).then(function mySucces(response) {
            $scope.arrbookingOat = response.data["hotelinfoOat"];
            $scope.txtremarksOat = response.data["hotelinfoOat"][0].Remarks_Status;
            $scope.txtcodeOat = response.data["hotelinfoOat"][0].HotelCode;
            $scope.txthnameOat = response.data["hotelinfoOat"][0].HotelName;
            $scope.txthaddressOat = response.data["hotelinfoOat"][0].HotelAddress;
            $scope.txthphoneOat = response.data["hotelinfoOat"][0].HotelPhoneNo;
            $scope.txthemailOat = response.data["hotelinfoOat"][0].PrimaryEmail;
            $scope.txtotheremailOat = response.data["hotelinfoOat"][0].SecondaryEmail;

            if ((response.data["hotelinfoOat"][0].HotelStatus) == "Pending from Hotel" ||
                (response.data["hotelinfoOat"][0].HotelStatus) == "Approved by Hotel") {
                $("#txthnameOat").prop("hidden", false);
                $("#ddlCustomersOat").prop("hidden", true);
            } else {
                $("#txthnameOat").prop("hidden", true);
                $("#ddlCustomersOat").prop("hidden", false);
            }

            $scope.arrFlightOat = response.data["flightInfoOat"];
            $scope.arrpassOat = response.data["passInfoOat"];
            $scope.arrpassodOat = response.data["bookingInfoOat"];
            $scope.trequestIdOat = TravelRequestId;
            $scope.flightDataOat = response.data["flightInfoOat"];
            hoteldatafillOat();
        }, function myError(response) {
        });
    };

    //get hotel details popup for sod
    $scope.viewdetailHotel = function (TravelRequestId, HotelRequestId) {
        $('#ResponseTextApproval').hide();
        $http({
            method: "GET",
            url: "../trns/GetHotelDetailbyTrID?trId=" + TravelRequestId + "|" + HotelRequestId
        }).then(function mySucces(response) {
            $scope.arrbookinghotelTrid = response.data["hotelinfobyTrid"];
            $scope.hotelData = response.data["hotelData"];
            $scope.sharedUserdetails = response.data["sharedUserdetails"];
            $scope.sharedtravID = response.data["sharedtravID"];
            $scope.sharedhId = response.data["sharedhId"];
            $scope.trequestIdhotel = TravelRequestId;
            $scope.rejectedData = response.data["rejectedData"];
            $scope.cancelledData = response.data["cancelledData"];

        }, function myError(response) {
        });
    };

    //get hotel details popup for oat
    $scope.viewdetailHotelOat = function (TravelRequestId) {
        $('#ResponseTextApprovalOat').hide();
        $http({
            method: "GET",
            url: "../trns/GetHotelDetailbyTrIDOat?trId=" + TravelRequestId
        }).then(function mySucces(response) {
            $scope.arrbookinghotelTridOat = response.data["hotelinfobyTridOat"];
            $scope.hotelDataOat = response.data["hotelDataOat"];
            $scope.sharedUserdetailsOat = response.data["sharedUserdetailsOat"];
            $scope.sharedtravIDOat = response.data["sharedtravIDOat"];
            $scope.trequestIdhotelOat = TravelRequestId;

        }, function myError(response) {
        });
    };

    // view detail for hod approval status
    $scope.viewdetailHODApproval = function (TravelRequestId, HotelRequestId) {
        $http({
            method: "GET",
            url: "../trns/getdetailHODApproval?trId=" + TravelRequestId + "|" + HotelRequestId
        }).then(function mySucces(response) {
            $scope.approvalinfo = response.data;

        }, function myError(response) {
        });
    };

    $scope.viewdetailHODApprovalOat = function (TravelRequestId, HotelRequestId) {
        $http({
            method: "GET",
            url: "../trns/getdetailHODApprovalOat?trId=" + TravelRequestId + "|" + HotelRequestId
        }).then(function mySucces(response) {
            $scope.approvalinfoOat = response.data;

        }, function myError(response) {
        });
    };

    $('#txtcode').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txthname').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txthaddress').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txthphone').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txtremarks').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txtcodeOat').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txthnameOat').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txthaddressOat').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txthphoneOat').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });
    $('#txtremarksOat').keydown(function (e) {
        if (e.keyCode == 220 || e.which == 220) {
            e.preventDefault();
        }
    });


    //Approved Hotel info
    $scope.apphotelinfo = function (TravelRequestId, HotelRequestId) {
        var result = confirm("Are you sure to send confirmation mail to user ?");
        if (result) {
            $('.loader').show();
            var elists = new Array();
            var obj = new Object();
            obj.TravelRequestId = TravelRequestId;
            obj.HotelRequestId = HotelRequestId;
            elists.push(obj);

            $http({
                method: "POST",
                url: "../trns/AppHotelRequest",
                data: JSON.stringify({ elist: elists }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Email notification has been sent successfully.');
                $('#btnsubmit').hide();
                $('#ResponseMail').hide();
                $('#ResponseTextApproval').show();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }


    //Reject hotel Info
    $scope.rejecthotelinfo = function (TravelRequestId) {
        if ($('#txtremarksE').val() == 'NA' || $('#txtremarksE').val() == '' || $('#txtremarksE').val() == null) {
            alert('Please enter rejection remarks.');
            $("#txtremarksE").focus();
            return false;
        }
        var result = confirm("Are you sure to reject this hotel request ?");
        if (result) {
            $scope.rejectloading = true;
            var remarks = $('#txtremarksE').val();
            var req = TravelRequestId + '|' + remarks;
            $http({
                method: "POST",
                url: "../trns/RejectHotelRequest?req=" + req
            }).then(function mySucces(response) {
                alert('Hotel Request has been rejected successfully.');
                location.reload();
                $scope.rejectloading = false;
                $scope.rejtext = "Rejected";
                $('#btnsubmit').hide();
                $scope.emailNotification();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    }


    // approve oat request
    $scope.apphotelinfoOat = function (TravelRequestId) {
        var result = confirm("Are you sure to send confirmation mail to user ?");
        if (result) {
            var elists = new Array();
            var obj = new Object();
            obj.TravelRequestId = TravelRequestId;
            elists.push(obj);

            $http({
                method: "POST",
                url: "../trns/AppHotelRequestOat",
                data: JSON.stringify({ elist: elists }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                alert('Email notification has been sent successfully.');
                $('#btnsubmitOat').hide();
                $('#ResponseMailOat').hide();
                $('#ResponseTextApprovalOat').show();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });

        }
    }


    // reject oat request
    $scope.rejecthotelinfoOat = function (TravelRequestId) {
        if ($('#txtremarksOat').val() == 'NA' || $('#txtremarksOat').val() == '' || $('#txtremarksOat').val() == null) {
            alert('Please enter rejection remarks.');
            $("#txtremarksOat").focus();
            return false;
        }
        var result = confirm("Are you sure to reject this hotel request ?");
        if (result) {
            $scope.rejectloadingOat = true;
            var remarks = $('#txtremarksOat').val();
            var req = TravelRequestId + '|' + remarks;
            $http({
                method: "POST",
                url: "../trns/RejectHotelRequestOat?req=" + req
            }).then(function mySucces(response) {
                alert('Hotel Request has been rejected successfully.');
                location.reload();
                $scope.rejectloadingOat = false;
                $scope.rejtextOat = "Rejected";
                $('#btnsubmitOat').hide();
                $scope.emailNotification();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    }

    //send request to hotel
    $scope.requestToHotel = function () {
        var counter = 0;
        var i = 0;
        var checklists = new Array();
        var slist = $("#tbldetail input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[1].innerText;
                    var hid = $(this).closest('tr').find('td')[2].innerText;
                    var gender = $(this).closest('tr').find('td')[3].innerText;
                    var sector = $(this).closest('tr').find('td')[10].innerText;
                    var obj = new Object();
                    obj.TravelRequestId = eid.split('-')[1];
                    obj.HotelRequestId = hid;
                    obj.Gender = gender;
                    obj.Sector = sector.split('-')[1].trim();
                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });

        if (i == 0) {
            alert('Please select at least 1 employee to send request to hotel.');
            return;
        }
        if (i > 2) {
            alert('Error: Cannot share more than 2 users together.');
            return;
        }
        if (i == 2) {
            var gender1 = checklists[0].Gender;
            var gender2 = checklists[1].Gender;
            if (gender1 != gender2) {
                alert('Sharing cannot be done for opposite gender.')
                return;
            }
            if (checklists[0].Sector != checklists[1].Sector) {
                alert('Sector must be same for both selected Employee.')
                return;
            }
        }

        if ($('#ddlCustomers :selected').text().toString() == "Please select") {
            alert('Please select hotel name.');
            return false;
        }


        if ($('#txtcodeE').val() == 'NA' || $('#txtcodeE').val() == '' || $('#txtcodeE').val() == null ||
            $('#txthnameE').val() == 'NA' || $('#txthnameE').val() == '' || $('#txthnameE').val() == null ||
            $('#txthaddressE').val() == 'NA' || $('#txthaddressE').val() == '' || $('#txthaddressE').val() == null ||
            $('#txthphoneE').val() == 'NA' || $('#txthphoneE').val() == '' || $('#txthphoneE').val() == null ||
            $('#txthemailE').val() == 'NA' || $('#txthemailE').val() == '' || $('#txthemailE').val() == null) {
            alert('Please Submit all records.');
            $("#txtcodeE").focus();
            return false;
        }
        if ($('#txtRqstSubE').val() == 'NA' || $('#txtRqstSubE').val() == '' || $('#txtRqstSubE').val() == null) {
            alert('Please enter name for request submitted by.');
            $("#txtRqstSubE").focus();
            return false;
        }
        if ($('#txtremarksE').val() == 'NA' || $('#txtremarksE').val() == '' || $('#txtremarksE').val() == null) {
            alert('Please Enter Remarks.');
            $("#txtremarksE").focus();
            return false;
        }


        var name = "";
        var type = "";
        var phoneNo = "";
        var taxIncluded = true;
        var hotelprice = "";
        var occupancy = "";
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
            $scope.hotelprice = hotelprice;
        }
        else {
            name = $('#txthnameE').val();
            type = "Non-Contractual";
            if ($("#chktax").is(':checked')) {
                taxIncluded = true;
            } else {
                taxIncluded = false;
            }
            if (i == 1) {
                occupancy = "Single";
            } else {
                occupancy = "Double";
            }
        }
        phoneNo = $('#txthphoneE').val();

        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txthemailE").val();
        if (!emailReg.test(emailaddress)) {
            alert("Invalid Primary Email Id");
            $("#txthemailE").focus();
            return false;
        }

        if (name == "Please select") {
            alert('Please select hotel name.');
            return false;
        }

        if (i == 1) {
            var travelrqstid = checklists[0].TravelRequestId;
            var hotelrqstid = checklists[0].HotelRequestId;
            $http({
                method: "POST",
                url: "../trns/FindSimilarTravelData",
                data: JSON.stringify({ TravelRequestId: travelrqstid, hotelname: name, hotelrqstid: hotelrqstid }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                //if (JSON.parse(response.data) == "NotExist") {
                if (response.data.length == 0) {
                    var result = confirm("Are you sure to send the request to hotel?");
                    if (result) {
                        $('.loader').show();
                        $scope.printloading = true;

                        var elists = new Array();
                        for (var i = 0; i < checklists.length; i++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[i].TravelRequestId;
                            obj.HotelRequestId = checklists[i].HotelRequestId;
                            obj.HotelCode = $('#txtcodeE').val();
                            obj.HotelName = name;
                            obj.HotelAddress = $('#txthaddressE').val();
                            obj.HotelPhone = phoneNo;
                            obj.HotelType = type;
                            obj.PrimaryEmail = $('#txthemailE').val();
                            obj.SecondaryEmail = $('#txtotheremailE').val();
                            obj.Remarks = $('#txtremarksE').val();
                            obj.SubmittedBy = $('#txtRqstSubE').val();
                            obj.IsTaxIncluded = taxIncluded;
                            obj.HotelCurrencyCode = $('#txthCurrency1').text();
                            elists.push(obj);
                        }

                        var sodOat = "SOD";
                        $('#btnHotelRequestE').prop("disabled", true);

                        $http({
                            method: "POST",
                            url: "../trns/SendHotelRequest",
                            data: JSON.stringify({ elist: elists, hotelprice: hotelprice, occupancy: occupancy, sodOat: sodOat }),
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8'
                        }).then(function mySucces(response) {
                            $('.loader').hide();
                            alert('Request has been successfully sent to Hotel- ' + name + '.');
                            $('#btnHotelRequestE').prop("disabled", false);
                            location.reload();
                        }, function myError(response) {
                            $('.loader').hide();
                            alert('Error: Invalid request processing...');
                            $('#btnHotelRequestE').prop("disabled", false);
                        });
                    }
                }
                else {
                    //var existingTrid = JSON.parse(response.data).split('|')[0];
                    // var exishotelid = JSON.parse(response.data).split('|')[1];   
                    var elists = new Array();
                    for (var i = 0; i < checklists.length; i++) {
                        var obj = new Object();
                        obj.TravelRequestId = checklists[i].TravelRequestId;
                        obj.HotelRequestId = checklists[i].HotelRequestId;
                        obj.HotelCode = $('#txtcodeE').val();
                        obj.HotelName = name;
                        obj.HotelAddress = $('#txthaddressE').val();
                        obj.HotelPhone = phoneNo;
                        obj.HotelType = type;
                        obj.PrimaryEmail = $('#txthemailE').val();
                        obj.SecondaryEmail = $('#txtotheremailE').val();
                        obj.Remarks = $('#txtremarksE').val();
                        obj.SubmittedBy = $('#txtRqstSubE').val();
                        obj.IsTaxIncluded = taxIncluded;
                        obj.HotelCurrencyCode = $('#txthCurrency1').text();
                        elists.push(obj);
                    }
                    $scope.elists = elists;
                    var sodOat = "SOD";
                    var clubresponse = confirm('An Employee with the same City, Check-In Date & Hotel ' + name + ' has been allocated. Do you want to club?');
                    if (clubresponse) {
                        $scope.clubbingSelectionDiv = true;
                        $scope.hotelBookingDiv = false;
                        $scope.sendToHotelButton = true
                        $scope.listForClubbling = response.data;
                        //loadTableDataSorted(travelrqstid, hotelrqstid, existingTrid, exishotelid);
                        //setTimeout(function () { $scope.highlightRows() }, 1000);
                    } else {
                        var result = confirm("Are you sure to send the request to hotel?");
                        if (result) {
                            $('.loader').show();
                            $scope.printloading = true;

                            var elists = new Array();
                            for (var i = 0; i < checklists.length; i++) {
                                var obj = new Object();
                                obj.TravelRequestId = checklists[i].TravelRequestId;
                                obj.HotelRequestId = checklists[i].HotelRequestId;
                                obj.HotelCode = $('#txtcodeE').val();
                                obj.HotelName = name;
                                obj.HotelAddress = $('#txthaddressE').val();
                                obj.HotelPhone = phoneNo;
                                obj.HotelType = type;
                                obj.PrimaryEmail = $('#txthemailE').val();
                                obj.SecondaryEmail = $('#txtotheremailE').val();
                                obj.Remarks = $('#txtremarksE').val();
                                obj.SubmittedBy = $('#txtRqstSubE').val();
                                obj.IsTaxIncluded = taxIncluded;
                                obj.HotelCurrencyCode = $('#txthCurrency1').text();
                                elists.push(obj);
                            }
                            var sodOat = "SOD";
                            $('#btnHotelRequestE').prop("disabled", true);
                            $http({
                                method: "POST",
                                url: "../trns/SendHotelRequest",
                                data: JSON.stringify({ elist: elists, hotelprice: hotelprice, occupancy: occupancy, sodOat: sodOat }),
                                dataType: 'json',
                                contentType: 'application/json; charset=utf-8'
                            }).then(function mySucces(response) {
                                $('.loader').hide();
                                alert('Request has been successfully sent to Hotel- ' + name + '.');
                                $('#btnHotelRequestE').prop("disabled", false);
                                location.reload();
                            }, function myError(response) {
                                $('.loader').hide();
                                alert('Error: Invalid request processing...');
                                $('#btnHotelRequestE').prop("disabled", false);
                            });
                        }
                    }
                }
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');

            });

        } else {
            var result = confirm("Are you sure to send the request to hotel?");
            if (result) {
                $('.loader').show();
                $scope.printloading = true;

                var elists = new Array();
                for (var i = 0; i < checklists.length; i++) {
                    var obj = new Object();
                    obj.TravelRequestId = checklists[i].TravelRequestId;
                    obj.HotelRequestId = checklists[i].HotelRequestId;
                    obj.HotelCode = $('#txtcodeE').val();
                    obj.HotelName = name;
                    obj.HotelAddress = $('#txthaddressE').val();
                    obj.HotelPhone = phoneNo;
                    obj.HotelType = type;
                    obj.PrimaryEmail = $('#txthemailE').val();
                    obj.SecondaryEmail = $('#txtotheremailE').val();
                    obj.Remarks = $('#txtremarksE').val();
                    obj.SubmittedBy = $('#txtRqstSubE').val();
                    obj.IsTaxIncluded = taxIncluded;
                    obj.HotelCurrencyCode = $('#txthCurrency1').text();
                    elists.push(obj);
                }

                var sodOat = "SOD";
                $('#btnHotelRequestE').prop("disabled", true);
                $http({
                    method: "POST",
                    url: "../trns/SendHotelRequest",
                    data: JSON.stringify({ elist: elists, hotelprice: hotelprice, occupancy: occupancy, sodOat: sodOat }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    $('.loader').hide();
                    alert('Request has been successfully sent to Hotel- ' + name + '.');
                    $('#btnHotelRequestE').prop("disabled", false);
                    location.reload();
                }, function myError(response) {
                    $('.loader').hide();
                    alert('Error: Invalid request processing...');
                    $('#btnHotelRequestE').prop("disabled", false);
                });
            }
        }

    }

    $scope.requestToHotelOat = function (TravelRequestId) {
        var counter = 0;
        var i = 0;
        var checklists = new Array();
        var slist = $("#tbloatdetail input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[1].innerText;
                    var obj = new Object();
                    obj.TravelRequestId = eid.split('-')[1];
                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });

        if (i == 0) {
            alert('Please select at least 1 employee to send request to hotel.');
            return;
        }
        if (i > 2) {
            alert('Error: Cannot share more than 2 users together.');
            return;
        }

        if ($('#txtcodeOat').val() == 'NA' || $('#txtcodeOat').val() == '' || $('#txtcodeOat').val() == null ||
            $('#txthnameOat').val() == 'NA' || $('#txthnameOat').val() == '' || $('#txthnameOat').val() == null ||
            $('#txthaddressOat').val() == 'NA' || $('#txthaddressOat').val() == '' || $('#txthaddressOat').val() == null ||
            $('#txthphoneOat').val() == 'NA' || $('#txthphoneOat').val() == '' || $('#txthphoneOat').val() == null ||
            $('#txtremarksOat').val() == 'NA' || $('#txtremarksOat').val() == '' || $('#txtremarksOat').val() == null ||
            $('#txthemailOat').val() == 'NA' || $('#txthemailOat').val() == '' || $('#txthemailOat').val() == null) {
            alert('Please enter all the details.');
            $("#txtcodeOat").focus();
            return false;
        }
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txthemailOat").val();
        if (!emailReg.test(emailaddress)) {
            alert("Invalid Primary Email Id");
            $("#txthemailOat").focus();
            return false;
        }
        var result = confirm("Are you sure to send the request to hotel?");
        if (result) {
            $('.loader').show();
            $scope.printloading = true;
            var name = "";
            var type = "";
            var phoneNo = "";
            var taxIncluded = true;
            var hotelprice = "";
            var occupancy = "";
            if ($('input[name="radioHotelOat"]:checked').val() == 'rdoExistingOat') {
                name = $('#ddlCustomersOat :selected').text().split('-')[0].toString();
                type = "Contractual";
                if ($('#txtTaxOat').val() == '1') {
                    taxIncluded = true;
                } else {
                    taxIncluded = false;
                }
                if (i == 1) {
                    occupancy = "Single";
                    if ($('input[name="radiomealOat"]:checked').val() == 'meal1') {
                        hotelprice = $('#singlemealpriceOat').text();
                    } else if ($('input[name="radiomealOat"]:checked').val() == 'meal2') {
                        hotelprice = $('#singlemealprice2Oat').text();
                    } else if ($('input[name="radiomealOat"]:checked').val() == 'meal3') {
                        hotelprice = $('#singlemealprice3Oat').text();
                    }
                } else {
                    occupancy = "Double";
                    if ($('input[name="radiomealOat"]:checked').val() == 'meal1') {
                        hotelprice = $('#doublemealpriceOat').text();
                    } else if ($('input[name="radiomealOat"]:checked').val() == 'meal2') {
                        hotelprice = $('#doublemealprice2Oat').text();
                    } else if ($('input[name="radiomealOat"]:checked').val() == 'meal3') {
                        hotelprice = $('#doublemealprice3Oat').text();
                    }
                }
            }
            else {
                name = $('#txthnameOat').val();
                type = "Non-Contractual";
                if ($("#chktaxOat").is(':checked')) {
                    taxIncluded = true;
                } else {
                    taxIncluded = false;
                }
                if (i == 1) {
                    occupancy = "Single";
                } else {
                    occupancy = "Double";
                }
            }
            phoneNo = $('#txthphoneOat').val();

            var elists = new Array();
            for (var i = 0; i < checklists.length; i++) {
                var obj = new Object();
                obj.TravelRequestId = checklists[i].TravelRequestId;
                obj.HotelCode = $('#txtcodeOat').val();
                obj.HotelName = name;
                obj.HotelAddress = $('#txthaddressOat').val();
                obj.HotelPhone = phoneNo;
                obj.HotelType = type;
                obj.PrimaryEmail = $('#txthemailOat').val();
                obj.SecondaryEmail = $('#txtotheremailOat').val();
                obj.Remarks = $('#txtremarksOat').val();
                obj.IsTaxIncluded = taxIncluded;
                elists.push(obj);
            }

            var sodOat = "OAT";
            $http({
                method: "POST",
                url: "../trns/SendHotelRequest",
                data: JSON.stringify({ elist: elists, hotelprice: hotelprice, occupancy: occupancy, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Request has been successfully sent to Hotel- ' + name + '.');
                location.reload();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }

    // send request to hotel for exceptional cases

    $scope.requestToHotelEx = function () {
        var counter = 0;
        var i = 0;
        var checklists = new Array();
        var slist = $("#tbldetailEx input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[1].innerText;
                    var hid = $(this).closest('tr').find('td')[2].innerText;
                    var gender = $(this).closest('tr').find('td')[3].innerText;
                    var sector = $(this).closest('tr').find('td')[10].innerText;

                    var obj = new Object();
                    obj.TravelRequestId = eid.split('-')[1];
                    obj.HotelRequestId = hid;
                    obj.Gender = gender;
                    obj.Sector = sector.split('-')[1].trim();
                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });
        if (i == 0) {
            alert('Please select at least 1 employee to send request to hotel.');
            return;
        }
        if (i > 2) {
            alert('Error: Cannot share more than 2 users together.');
            return;
        }
        if (i == 2) {
            var gender1 = checklists[0].Gender;
            var gender2 = checklists[1].Gender;
            if (gender1 != gender2) {
                alert('Sharing cannot be done for opposite gender.')
                return;
            }
            if (checklists[0].Sector != checklists[1].Sector) {
                alert('Sector must be same for both selected Employee.')
                return;
            }
        }
        if ($('#txtcodeEx').val() == 'NA' || $('#txtcodeEx').val() == '' || $('#txtcodeEx').val() == null ||
            $('#txthnameEx').val() == 'NA' || $('#txthnameEx').val() == '' || $('#txthnameEx').val() == null ||
            $('#txthaddressEx').val() == 'NA' || $('#txthaddressEx').val() == '' || $('#txthaddressEx').val() == null ||
            $('#txthphoneEx').val() == 'NA' || $('#txthphoneEx').val() == '' || $('#txthphoneEx').val() == null ||
            $('#txthemailEx').val() == 'NA' || $('#txthemailEx').val() == '' || $('#txthemailEx').val() == null) {
            alert('Please enter all the details.');
            $("#txtcodeEx").focus();
            return false;
        }
        if ($('#txtRqstSubEx').val() == 'NA' || $('#txtRqstSubEx').val() == '' || $('#txtRqstSubEx').val() == null) {
            alert('Please enter name for request submitted by.');
            $("#txtRqstSubEx").focus();
            return false;
        }
        if ($('#txtremarksEx').val() == 'NA' || $('#txtremarksEx').val() == '' || $('#txtremarksEx').val() == null) {
            alert('Please enter remarks.');
            $("#txtremarksEx").focus();
            return false;
        }

        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txthemailEx").val();
        if (!emailReg.test(emailaddress)) {
            alert("Invalid Primary Email Id");
            $("#txthemailEx").focus();
            return false;
        }

        var name = "";
        var type = "";
        var phoneNo = "";
        var hotelprice = "";
        var occupancy = "";
        if ($('input[name="radioHotelEx"]:checked').val() == 'rdoExisting') {
            name = $('#ddlCustomersEx :selected').text().split('-')[0].toString();
            type = "Contractual";
            if (i == 1) {
                occupancy = "Single";
                if ($('input[name="radiomealEx"]:checked').val() == 'meal1') {
                    hotelprice = $('#singlemealpriceEx').text();
                } else if ($('input[name="radiomealEx"]:checked').val() == 'meal2') {
                    hotelprice = $('#singlemealprice2Ex').text();
                } else if ($('input[name="radiomealEx"]:checked').val() == 'meal3') {
                    hotelprice = $('#singlemealprice3Ex').text();
                }
            } else {
                occupancy = "Double";
                if ($('input[name="radiomealEx"]:checked').val() == 'meal1') {
                    hotelprice = $('#doublemealpriceEx').text();
                } else if ($('input[name="radiomealEx"]:checked').val() == 'meal2') {
                    hotelprice = $('#doublemealprice2Ex').text();
                } else if ($('input[name="radiomealEx"]:checked').val() == 'meal3') {
                    hotelprice = $('#doublemealprice3Ex').text();
                }
            }
            $scope.hotelprice = hotelprice;
        }
        else {
            name = $('#txthnameEx').val();
            type = "Non-Contractual";
            if (i == 1) {
                occupancy = "Single";
            } else {
                occupancy = "Double";
            }
        }
        phoneNo = $('#txthphoneEx').val();
        if (i == 1) {
            var travelrqstid = checklists[0].TravelRequestId;
            var hotelrqstid = checklists[0].HotelRequestId;
            $http({
                method: "POST",
                url: "../trns/FindSimilarTravelData",
                data: JSON.stringify({ TravelRequestId: travelrqstid, hotelname: name, hotelrqstid: hotelrqstid }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
               // if (JSON.parse(response.data) == "NotExist") {
                if (response.data.length == 0) {
                    ;                  
                    var result = confirm("Are you sure to send the request to hotel?");
                    if (result) {
                        $('.loader').show();
                        $scope.printloading = true;
                        var elists = new Array();
                        for (var i = 0; i < checklists.length; i++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[i].TravelRequestId;
                            obj.HotelRequestId = checklists[i].HotelRequestId; 
                            obj.HotelCode = $('#txtcodeEx').val();
                            obj.HotelName = name;
                            obj.HotelAddress = $('#txthaddressEx').val();
                            obj.HotelPhone = phoneNo;
                            obj.HotelType = type;
                            obj.PrimaryEmail = $('#txthemailEx').val();
                            obj.SecondaryEmail = $('#txtotheremailEx').val();
                            obj.Remarks = $('#txtremarksEx').val();
                            obj.SubmittedBy = $('#txtRqstSubEx').val();
                            obj.HotelCurrencyCode = $('#txthCurrency1Ex').text();
                            elists.push(obj);
                        }
                        var sodOat = "SOD";
                        $('#btnHotelRequestExceptionEx').prop("disabled", true);
                        $http({
                            method: "POST",
                            url: "../trns/SendHotelRequest",
                            data: JSON.stringify({ elist: elists, hotelprice: hotelprice, occupancy: occupancy, sodOat: sodOat }),
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8'
                        }).then(function mySucces(response) {
                            $('.loader').hide();
                            alert('Request has been successfully sent to Hotel- ' + name + '.');
                            $('#btnHotelRequestExceptionEx').prop("disabled", false);
                            location.reload();
                        }, function myError(response) {
                            $('.loader').hide();
                            alert('Error: Invalid request processing...');
                            $('#btnHotelRequestExceptionEx').prop("disabled", false);
                        });
                    }
                }
                else {

                    //var existingTrid = JSON.parse(response.data).split('|')[0];
                    // var exishotelid = JSON.parse(response.data).split('|')[1];   
                    var elists = new Array();
                    for (var i = 0; i < checklists.length; i++) {
                        var obj = new Object();
                        obj.TravelRequestId = checklists[i].TravelRequestId;
                        obj.HotelRequestId = checklists[i].HotelRequestId;
                        obj.HotelCode = $('#txtcodeEx').val();
                        obj.HotelName = name;
                        obj.HotelAddress = $('#txthaddressEx').val();
                        obj.HotelPhone = phoneNo;
                        obj.HotelType = type;
                        obj.PrimaryEmail = $('#txthemailEx').val();
                        obj.SecondaryEmail = $('#txtotheremailEx').val();
                        obj.Remarks = $('#txtremarksEx').val();
                        obj.SubmittedBy = $('#txtRqstSubEx').val();
                        obj.HotelCurrencyCode = $('#txthCurrency1Ex').text();
                        elists.push(obj);
                    }
                    $scope.elists = elists;
                    var sodOat = "SOD";
                    var clubresponse = confirm('An Employee with the same City, Check-In Date & Hotel ' + name + ' has been allocated. Do you want to club?');
                    if (clubresponse) {
                        $scope.clubbingSelectionDiv = true;
                        $scope.hotelBookingDiv = false;
                        $scope.sendToHotelButton = true
                        $scope.listForClubbling = response.data;
                        //loadTableDataSortedEx(travelrqstid, hotelrqstid, existingTrid, exishotelid);
                        //setTimeout(function () { $scope.highlightRowsEx() }, 1000);
                    } else {
                        var result = confirm("Are you sure to send the request to hotel?");
                        if (result) {
                            $('.loader').show();
                            $scope.printloading = true;
                            var elists = new Array();
                            for (var i = 0; i < checklists.length; i++) {
                                var obj = new Object();
                                obj.TravelRequestId = checklists[i].TravelRequestId;
                                obj.HotelRequestId = checklists[i].HotelRequestId;
                                obj.HotelCode = $('#txtcodeEx').val();
                                obj.HotelName = name;
                                obj.HotelAddress = $('#txthaddressEx').val();
                                obj.HotelPhone = phoneNo;
                                obj.HotelType = type;
                                obj.PrimaryEmail = $('#txthemailEx').val();
                                obj.SecondaryEmail = $('#txtotheremailEx').val();
                                obj.Remarks = $('#txtremarksEx').val();
                                obj.SubmittedBy = $('#txtRqstSubEx').val();
                                obj.HotelCurrencyCode = $('#txthCurrency1Ex').text();
                                elists.push(obj);
                            }
                            var sodOat = "SOD";
                            $('#btnHotelRequestExceptionEx').prop("disabled", true);
                            $http({
                                method: "POST",
                                url: "../trns/SendHotelRequest",
                                data: JSON.stringify({ elist: elists, hotelprice: hotelprice, occupancy: occupancy, sodOat: sodOat }),
                                dataType: 'json',
                                contentType: 'application/json; charset=utf-8'
                            }).then(function mySucces(response) {
                                $('.loader').hide();
                                alert('Request has been successfully sent to Hotel- ' + name + '.');
                                $('#btnHotelRequestExceptionEx').prop("disabled", false);
                                location.reload();
                            }, function myError(response) {
                                $('.loader').hide();
                                alert('Error: Invalid request processing...');
                                $('#btnHotelRequestExceptionEx').prop("disabled", false);
                            });
                        }
                    }
                }
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
        else {
            var result = confirm("Are you sure to send the request to hotel?");
            if (result) {
                $('.loader').show();
                $scope.printloading = true;

                var elists = new Array();
                for (var i = 0; i < checklists.length; i++) {
                    var obj = new Object();
                    obj.TravelRequestId = checklists[i].TravelRequestId;
                    obj.HotelRequestId = checklists[i].HotelRequestId;
                    obj.HotelCode = $('#txtcodeEx').val();
                    obj.HotelName = name;
                    obj.HotelAddress = $('#txthaddressEx').val();
                    obj.HotelPhone = phoneNo;
                    obj.HotelType = type;
                    obj.PrimaryEmail = $('#txthemailEx').val();
                    obj.SecondaryEmail = $('#txtotheremailEx').val();
                    obj.Remarks = $('#txtremarksEx').val();
                    obj.SubmittedBy = $('#txtRqstSubEx').val();
                    obj.HotelCurrencyCode = $('#txthCurrency1Ex').text();
                    elists.push(obj);
                }

                var sodOat = "SOD";
                $('#btnHotelRequestExceptionEx').prop("disabled", true);
                $http({
                    method: "POST",
                    url: "../trns/SendHotelRequest",
                    data: JSON.stringify({ elist: elists, hotelprice: hotelprice, occupancy: occupancy, sodOat: sodOat }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    $('.loader').hide();
                    alert('Request has been successfully sent to Hotel- ' + name + '.');
                    $('#btnHotelRequestExceptionEx').prop("disabled", false);
                    location.reload();
                }, function myError(response) {
                    $('.loader').hide();
                    alert('Error: Invalid request processing...');
                    $('#btnHotelRequestExceptionEx').prop("disabled", false);
                });
            }
        }

    }

    // send request to approvers
    $scope.sendFinancialApprovalRequest = function () {
        var sodOat = "SOD";
        var counter = 0;
        var i = 0;
        var checklists = new Array();
        var slist = $("#tbldetail input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[1].innerText;
                    var hid = $(this).closest('tr').find('td')[2].innerText;
                    var gender = $(this).closest('tr').find('td')[3].innerText;
                    var obj = new Object();
                    obj.TravelRequestId = eid.split('-')[1];
                    obj.HotelRequestId = hid;
                    obj.Gender = gender;
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
        if (i > 2) {
            alert('Error: Cannot share more than 2 users together.');
            return false;
        }
        if (i == 2) {
            var gender1 = checklists[0].Gender;
            var gender2 = checklists[1].Gender;
            if (gender1 == gender2) {

            } else {
                alert('Sharing cannot be done for opposite gender.')
                return;
            }
        }

        if ($('#txthnameE').val() == 'NA' || $('#txthnameE').val() == '' || $('#txthnameE').val() == null) {
            alert('Please enter hotel name.');
            $("#txthnameE").focus();
            return false;
        }

        if ($('#txtcodeE').val() == 'NA' || $('#txtcodeE').val() == '' || $('#txtcodeE').val() == null) {
            alert('Please enter hotel code.');
            $("#txtcodeE").focus();
            return false;
        }
        if ($('#txthaddressE').val() == 'NA' || $('#txthaddressE').val() == '' || $('#txthaddressE').val() == null) {
            alert('Please enter hotel address.');
            $("#txthaddressE").focus();
            return false;
        }
        if ($('#txthphoneE').val() == 'NA' || $('#txthphoneE').val() == '' || $('#txthphoneE').val() == null) {
            alert('Please enter hotel phone no..');
            $("#txthphoneE").focus();
            return false;
        }
        if ($('#txthemailE').val() == 'NA' || $('#txthemailE').val() == '' || $('#txthemailE').val() == null) {
            alert('Please enter primary email id.');
            $("#txtcodeE").focus();
            return false;
        }
        if ($('#txtRqstSubE').val() == 'NA' || $('#txtRqstSubE').val() == '' || $('#txtRqstSubE').val() == null) {
            alert('Please enter name for request submitted by');
            $("#txtRqstSubE").focus();
            return false;
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
            $("#txtairporttransferEx").focus();
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
            alert('Please enter buffet time details.');
            $("#txtlaundry").focus();
            return false;
        }
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txthemailE").val();
        if (!emailReg.test(emailaddress)) {
            alert("Invalid Primary Email Id");
            $("#txthemailE").focus();
            return false;
        }
        var otherEmail = $('#txtotheremailE').val();
        if (otherEmail.length > 0) {
            var Smail = $('#txtotheremailE').val().split(',');
            for (var i = 0; i < Smail.length; i++) {
                if (!emailReg.test(Smail[i].trim())) {
                    alert('Please enter valid other email.');
                    $('#txtotheremailE').focus();
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
        $scope.hodemail1 = hodemail1;
        $scope.hodemail2 = hodemail2;
        var hname = $('#txthnameE').val();
        if (i == 1) {
            var travelrqstid = checklists[0].TravelRequestId;
            var hotelrqstid = checklists[0].HotelRequestId;
            $http({
                method: "POST",
                url: "../trns/FindSimilarTravelData",
                data: JSON.stringify({ TravelRequestId: travelrqstid, hotelname: hname, hotelrqstid: hotelrqstid }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                //if (JSON.parse(response.data) == "NotExist") {
                    if (response.data.length == 0) {
                    var result = confirm("Are you sure to send the request to HOD?");
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
                            name = $('#txthnameE').val();
                            type = "Non-Contractual";
                        }
                        phoneNo = $('#txthphoneE').val();
                        var taxIncluded = true;
                        if ($("#chktax").is(':checked')) {
                            taxIncluded = true;
                        } else {
                            taxIncluded = false;
                        }
                        var elists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelCode = $('#txtcodeE').val();
                            obj.HotelName = name;
                            obj.HotelAddress = $('#txthaddressE').val();
                            obj.HotelPhone = phoneNo;
                            obj.HotelType = type;
                            obj.PrimaryEmail = $('#txthemailE').val();
                            obj.SecondaryEmail = $('#txtotheremailE').val();
                            obj.Remarks = $('#txtremarksE').val();
                            obj.SubmittedBy = $('#txtRqstSubE').val();
                            obj.HotelPrice = $('#txtprice').val();
                            obj.IsTaxIncluded = taxIncluded;
                            obj.HotelCurrencyCode = ($('#selecttxtCurrency1').val() == "") || ($('#selecttxtCurrency1').val() == "Null") ? "INR" : $('#selecttxtCurrency1').val();
                            elists.push(obj);
                        }
                        var inclists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelName = name;
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
                            url: "../trns/sendFinancialApprovalRequest",
                            data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
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
                else {
                    //var existingTrid = JSON.parse(response.data).split('|')[0];
                    // var exishotelid = JSON.parse(response.data).split('|')[1];
                    var clubresponse = confirm('An Employee with the same City, Check-In Date & Hotel ' + hname + ' has been allocated. Do you want to club?');
                    if (clubresponse) {
                        //loadTableDataSorted(travelrqstid, hotelrqstid, existingTrid, exishotelid);
                        //setTimeout(function () { $scope.highlightRows() }, 2000);
                        // $('.loader').show();
                        $scope.clubbingSelectionDiv = true;
                        $scope.hotelBookingDiv = false;
                        $scope.sendToFinancialButton = true;
                        $scope.listForClubbling = response.data;
                        var name = "";
                        var type = "";
                        var phoneNo = "";
                        if ($('input[name="radioHotel"]:checked').val() == 'rdoExisting') {
                            name = $('#ddlCustomers :selected').text().split('-')[0].toString();
                            type = "Contractual";
                        }
                        else {
                            name = $('#txthnameE').val();
                            type = "Non-Contractual";
                        }
                        phoneNo = $('#txthphoneE').val();
                        var taxIncluded = true;
                        if ($("#chktax").is(':checked')) {
                            taxIncluded = true;
                        } else {
                            taxIncluded = false;
                        }
                        var elists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelCode = $('#txtcodeE').val();
                            obj.HotelName = name;
                            obj.HotelAddress = $('#txthaddressE').val();
                            obj.HotelPhone = phoneNo;
                            obj.HotelType = type;
                            obj.PrimaryEmail = $('#txthemailE').val();
                            obj.SecondaryEmail = $('#txtotheremailE').val();
                            obj.Remarks = $('#txtremarksE').val();
                            obj.SubmittedBy = $('#txtRqstSubE').val();
                            obj.HotelPrice = $('#txtprice').val();
                            obj.IsTaxIncluded = taxIncluded;
                            obj.HotelCurrencyCode = ($('#selecttxtCurrency1').val() == "") || ($('#selecttxtCurrency1').val() == "Null") ? "INR" : $('#selecttxtCurrency1').val();
                            elists.push(obj);
                        }
                        $scope.elists = elists;
                        var inclists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelName = name;
                            obj.Accomodation = $('#txtaccomodation').val();
                            obj.Food = $('#txtfood').val();
                            obj.AirportTransfers = $('#txtairporttransfer').val();
                            obj.RoomService = $('#txtroom').val();
                            obj.BuffetTime = $('#txtbuffet').val();
                            obj.Laundry = $('#txtlaundry').val();
                            inclists.push(obj);
                        }
                        $scope.inclists = inclists;
                        $('#btnFinancialApproval').prop("disabled", true)
                    }
                    else {
                        var result = confirm("Are you sure to send the request to HOD?");
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
                                name = $('#txthnameE').val();
                                type = "Non-Contractual";
                            }
                            phoneNo = $('#txthphoneE').val();
                            var taxIncluded = true;
                            if ($("#chktax").is(':checked')) {
                                taxIncluded = true;
                            } else {
                                taxIncluded = false;
                            }
                            var elists = new Array();
                            for (var j = 0; j < checklists.length; j++) {
                                var obj = new Object();
                                obj.TravelRequestId = checklists[j].TravelRequestId;
                                obj.HotelRequestId = checklists[j].HotelRequestId;
                                obj.HotelCode = $('#txtcodeE').val();
                                obj.HotelName = name;
                                obj.HotelAddress = $('#txthaddressE').val();
                                obj.HotelPhone = phoneNo;
                                obj.HotelType = type;
                                obj.PrimaryEmail = $('#txthemailE').val();
                                obj.SecondaryEmail = $('#txtotheremailE').val();
                                obj.Remarks = $('#txtremarksE').val();
                                obj.SubmittedBy = $('#txtRqstSubE').val();
                                obj.HotelPrice = $('#txtprice').val();
                                obj.IsTaxIncluded = taxIncluded;
                                obj.HotelCurrencyCode = ($('#selecttxtCurrency1').val() == "") || ($('#selecttxtCurrency1').val() == "Null") ? "INR" : $('#selecttxtCurrency1').val();
                                elists.push(obj);
                            }
                            var inclists = new Array();
                            for (var j = 0; j < checklists.length; j++) {
                                var obj = new Object();
                                obj.TravelRequestId = checklists[j].TravelRequestId;
                                obj.HotelRequestId = checklists[j].HotelRequestId;
                                obj.HotelName = name;
                                obj.Accomodation = $('#txtaccomodation').val();
                                obj.Food = $('#txtfood').val();
                                obj.AirportTransfers = $('#txtairporttransfer').val();
                                obj.RoomService = $('#txtroom').val();
                                obj.BuffetTime = $('#txtbuffet').val();
                                obj.Laundry = $('#txtlaundry').val();
                                inclists.push(obj);
                            }
                            $('#btnFinancialApproval').prop("disabled", true)
                            $http({
                                method: "POST",
                                url: "../trns/sendFinancialApprovalRequest",
                                data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
                                dataType: 'json',
                                contentType: 'application/json; charset=utf-8'
                            }).then(function mySucces(response) {
                                $('.loader').hide();
                                alert('Email notification has been sent successfully.');
                                $('#btnFinancialApproval').prop("disabled", false)
                                location.reload();
                            }, function myError(response) {
                                $('.loader').hide();
                                alert('Error: Invalid request processing...');
                                $('#btnFinancialApproval').prop("disabled", false)
                            })
                        }
                    }
                }
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');

            });
        }
        else {
            var result = confirm("Are you sure to send the request to HOD?");
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
                    name = $('#txthnameE').val();
                    type = "Non-Contractual";

                }
                phoneNo = $('#txthphoneE').val();
                var taxIncluded = true;
                if ($("#chktax").is(':checked')) {
                    taxIncluded = true;
                } else {
                    taxIncluded = false;
                }
                var elists = new Array();
                for (var j = 0; j < checklists.length; j++) {
                    var obj = new Object();
                    obj.TravelRequestId = checklists[j].TravelRequestId;
                    obj.HotelRequestId = checklists[j].HotelRequestId;
                    obj.HotelCode = $('#txtcodeE').val();
                    obj.HotelName = name;
                    obj.HotelAddress = $('#txthaddressE').val();
                    obj.HotelPhone = phoneNo;
                    obj.HotelType = type;
                    obj.PrimaryEmail = $('#txthemailE').val();
                    obj.SecondaryEmail = $('#txtotheremailE').val();
                    obj.Remarks = $('#txtremarksE').val();
                    obj.SubmittedBy = $('#txtRqstSubE').val();
                    obj.HotelPrice = $('#txtprice').val();
                    obj.IsTaxIncluded = taxIncluded;
                    obj.HotelCurrencyCode = ($('#selecttxtCurrency1').val() == "") || ($('#selecttxtCurrency1').val() == "Null") ? "INR" : $('#selecttxtCurrency1').val();
                    elists.push(obj);
                }
                var inclists = new Array();
                for (var j = 0; j < checklists.length; j++) {
                    var obj = new Object();
                    obj.TravelRequestId = checklists[j].TravelRequestId;
                    obj.HotelRequestId = checklists[j].HotelRequestId;
                    obj.HotelName = name;
                    obj.Accomodation = $('#txtaccomodation').val();
                    obj.Food = $('#txtfood').val();
                    obj.AirportTransfers = $('#txtairporttransfer').val();
                    obj.RoomService = $('#txtroom').val();
                    obj.BuffetTime = $('#txtbuffet').val();
                    obj.Laundry = $('#txtlaundry').val();
                    inclists.push(obj);
                }
                $('#btnFinancialApproval').prop("disabled", true)
                $http({
                    method: "POST",
                    url: "../trns/sendFinancialApprovalRequest",
                    data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    $('.loader').hide();
                    alert('Email notification has been sent successfully.');
                    $('#btnFinancialApproval').prop("disabled", false)
                    location.reload();
                }, function myError(response) {
                    $('.loader').hide();
                    alert('Error: Invalid request processing...');
                    $('#btnFinancialApproval').prop("disabled", false)
                })
            }
        }
    }

    //Date 03-10-2018
    $scope.sendFinancialApprovalRequestEx = function () {
        var sodOat = "SOD";
        var counter = 0;
        var i = 0;
        var checklists = new Array();
        var slist = $("#tbldetailEx input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[1].innerText;
                    var hid = $(this).closest('tr').find('td')[2].innerText;
                    var gender = $(this).closest('tr').find('td')[3].innerText;
                    var obj = new Object();
                    obj.TravelRequestId = eid.split('-')[1];
                    obj.HotelRequestId = hid;
                    obj.Gender = gender;
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
        if (i > 2) {
            alert('Error: Cannot share more than 2 users together.');
            return false;
        }
        if (i == 2) {
            var gender1 = checklists[0].Gender;
            var gender2 = checklists[1].Gender;
            if (gender1 == gender2) {

            } else {
                alert('Sharing cannot be done for opposite gender.')
                return;
            }
        }
        if ($('#txthnameEx').val() == 'NA' || $('#txthnameEx').val() == '' || $('#txthnameEx').val() == null) {
            alert('Please enter hotel name.');
            $("#txthnameEx").focus();
            return false;
        }
        if ($('#txtcodeEx').val() == 'NA' || $('#txtcodeEx').val() == '' || $('#txtcodeEx').val() == null) {
            alert('Please enter hotel code.');
            $("#txtcodeEx").focus();
            return false;
        }
        if ($('#txthaddressEx').val() == 'NA' || $('#txthaddressEx').val() == '' || $('#txthaddressEx').val() == null) {
            alert('Please enter hotel address.');
            $("#txthaddressEx").focus();
            return false;
        }
        if ($('#txthphoneEx').val() == 'NA' || $('#txthphoneEx').val() == '' || $('#txthphoneEx').val() == null) {
            alert('Please enter hotel phone no..');
            $("#txthphoneEx").focus();
            return false;
        }
        if ($('#txthemailEx').val() == 'NA' || $('#txthemailEx').val() == '' || $('#txthemailEx').val() == null) {
            alert('Please enter primary email id.');
            $("#txthemailEx").focus();
            return false;
        }
        if ($('#txtRqstSubEx').val() == 'NA' || $('#txtRqstSubEx').val() == '' || $('#txtRqstSubEx').val() == null) {
            alert('Please enter name for request submitted by');
            $("#txtRqstSubEx").focus();
            return false;
        }
        if ($('#txtpriceEx').val() == 'NA' || $('#txtpriceEx').val() == '' || $('#txtpriceEx').val() == null) {
            alert('Please enter hotel price.');
            $("#txtprice").focus();
            return false;
        }
        if ($('#txtaccomodationEx').val() == 'NA' || $('#txtaccomodationEx').val() == '' || $('#txtaccomodationEx').val() == null) {
            alert('Please enter accomodation details.');
            $("#txtaccomodationEx").focus();
            return false;
        }
        if ($('#txtfoodEx').val() == 'NA' || $('#txtfoodEx').val() == '' || $('#txtfoodEx').val() == null) {
            alert('Please enter your meal plan.');
            $("#txtfoodEx").focus();
            return false;
        }
        if ($('#txtairporttransferEx').val() == 'NA' || $('#txtairporttransferEx').val() == '' || $('#txtairporttransferEx').val() == null) {
            alert('Please enter airport transport detail.');
            $("#txtairporttransferEx").focus();
            return false;
        }
        if ($('#txtroomEx').val() == 'NA' || $('#txtroomEx').val() == '' || $('#txtroomEx').val() == null) {
            alert('Please enter room service details.');
            $("#txtroomEx").focus();
            return false;
        }
        if ($('#txtbuffetEx').val() == 'NA' || $('#txtbuffetEx').val() == '' || $('#txtbuffetEx').val() == null) {
            alert('Please enter buffet time details.');
            $("#txtbuffetEx").focus();
            return false;
        }
        if ($('#txtlaundryEx').val() == 'NA' || $('#txtlaundryEx').val() == '' || $('#txtlaundryEx').val() == null) {
            alert('Please enter Laundary details.');
            $("#txtlaundryEx").focus();
            return false;
        }
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txthemailEx").val();
        if (!emailReg.test(emailaddress)) {
            alert("Invalid Primary Email Id");
            $("#txthemailEx").focus();
            return false;
        }
        var otherEmail = $('#txtotheremailEx').val();
        if (otherEmail.length > 0) {
            var Smail = $('#txtotheremailEx').val().split(',');
            for (var i = 0; i < Smail.length; i++) {
                if (!emailReg.test(Smail[i].trim())) {
                    alert('Please enter valid other email.');
                    $('#txtotheremailEx').focus();
                    return false;
                }
            }
        }

        var hodemail1 = "";
        var hodemail2 = "";
        var priceval = $("#txtpriceEx").val();

        if ($('input[name="typeHotelEx"]:checked').val() == 'DomesticCurr') {
            if (priceval <= 7000) {
                hodemail1 = $("#EmailApprover1Ex").text();
            }
            else {
                if (($("#chkapprover1Ex").prop("checked") == false) || ($("#chkapprover2Ex").prop("checked") == false)) {
                    alert('Approval required from both approvers for price above 7000');
                    return false;
                }
                hodemail1 = $("#EmailApprover1Ex").text();
                hodemail2 = $("#EmailApprover2Ex").text();
            }
        }
        else {
            if ($('#selecttxtCurrencyEx').val() == "Null") {
                alert('Please select Currency Code.');
                $('#selecttxtCurrencyEx').focus();
                return false;
            }
            if (($("#chkapprover1Ex").prop("checked") == false) || ($("#chkapprover2Ex").prop("checked") == false)) {
                alert('Approval required from both approvers for International Booking');
                return false;
            }
            hodemail1 = $("#EmailApprover1Ex").text();
            hodemail2 = $("#EmailApprover2Ex").text();
            
        }
        $scope.hodemail1 = hodemail1;
        $scope.hodemail2 = hodemail2;

        var hname = $('#txthnameEx').val();
        if (i == 1) {
            var travelrqstid = checklists[0].TravelRequestId;
            var hotelrqstid = checklists[0].HotelRequestId;
            $http({
                method: "POST",
                url: "../trns/FindSimilarTravelData",
                data: JSON.stringify({ TravelRequestId: travelrqstid, hotelname: hname, hotelrqstid: hotelrqstid }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                //if (JSON.parse(response.data) == "NotExist") {
                if (response.data.length  == 0) {
                    var result = confirm("Are you sure to send the request to HOD?");
                    if (result) {
                        $('.loader').show();
                        var name = "";
                        var type = "";
                        var phoneNo = "";
                        if ($('input[name="radioHotelEx"]:checked').val() == 'rdoExisting') {
                            name = $('#ddlCustomersEx :selected').text().split('-')[0].toString();
                            type = "Contractual";
                        }
                        else {
                            name = $('#txthnameEx').val();
                            type = "Non-Contractual";
                        }
                        phoneNo = $('#txthphoneEx').val();
                        var taxIncluded = true;
                        if ($("#chktaxEx").is(':checked')) {
                            taxIncluded = true;
                        } else {
                            taxIncluded = false;
                        }
                        var elists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelCode = $('#txtcodeEx').val();
                            obj.HotelName = name;
                            obj.HotelAddress = $('#txthaddressEx').val();
                            obj.HotelPhone = phoneNo;
                            obj.HotelType = type;
                            obj.PrimaryEmail = $('#txthemailEx').val();
                            obj.SecondaryEmail = $('#txtotheremailEx').val();
                            obj.Remarks = $('#txtremarksEx').val();
                            obj.SubmittedBy = $('#txtRqstSubEx').val();
                            obj.HotelPrice = $('#txtpriceEx').val();
                            obj.IsTaxIncluded = taxIncluded;
                            obj.HotelCurrencyCode = (($('#selecttxtCurrencyEx').val() == "") || ($('#selecttxtCurrencyEx').val() == "Null")) ? "INR" : $('#selecttxtCurrencyEx').val();
                            elists.push(obj);
                        }

                        var inclists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelName = name;
                            obj.Accomodation = $('#txtaccomodationEx').val();
                            obj.Food = $('#txtfoodEx').val();
                            obj.AirportTransfers = $('#txtairporttransferEx').val();
                            obj.RoomService = $('#txtroomEx').val();
                            obj.BuffetTime = $('#txtbuffetEx').val();
                            obj.Laundry = $('#txtlaundryEx').val();
                            inclists.push(obj);
                        }
                        $('#btnFinancialApproval').prop("disabled", true)
                        $http({
                            method: "POST",
                            url: "../trns/sendFinancialApprovalRequest",
                            data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8'
                        }).then(function mySucces(response) {
                            $('.loader').hide();
                            alert('Email notification has been sent successfully.');
                            $('#btnFinancialApproval').prop("disabled", false)
                            location.reload();
                        }, function myError(response) {
                            $('.loader').hide();
                            alert('Error: Invalid request processing...');
                            $('#btnFinancialApproval').prop("disabled", false)
                        })
                    }
                } else {
                    // var existingTrid = JSON.parse(response.data).split('|')[0];
                    //var exishotelid = JSON.parse(response.data).split('|')[1];
                    var clubresponse = confirm('An Employee with the same City, Check-In Date & Hotel ' + hname + ' has been allocated. Do you want to club?');
                    if (clubresponse) {
                        // loadTableDataSortedEx(travelrqstid, hotelrqstid, existingTrid, exishotelid);
                        // setTimeout(function () { $scope.highlightRowsEx() }, 2000);
                        $scope.clubbingSelectionDiv = true;
                        $scope.hotelBookingDiv = false;
                        $scope.sendToFinancialButton = true;
                        $scope.listForClubbling = response.data;
                        var name = "";
                        var type = "";
                        var phoneNo = "";
                        if ($('input[name="radioHotelEx"]:checked').val() == 'rdoExisting') {
                            name = $('#ddlCustomersEx :selected').text().split('-')[0].toString();
                            type = "Contractual";
                        }
                        else {
                            name = $('#txthnameEx').val();
                            type = "Non-Contractual";
                        }
                        phoneNo = $('#txthphoneEx').val();
                        var taxIncluded = true;
                        if ($("#chktaxEx").is(':checked')) {
                            taxIncluded = true;
                        } else {
                            taxIncluded = false;
                        }
                        var elists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelCode = $('#txtcodeEx').val();
                            obj.HotelName = name;
                            obj.HotelAddress = $('#txthaddressEx').val();
                            obj.HotelPhone = phoneNo;
                            obj.HotelType = type;
                            obj.PrimaryEmail = $('#txthemailEx').val();
                            obj.SecondaryEmail = $('#txtotheremailEx').val();
                            obj.Remarks = $('#txtremarksEx').val();
                            obj.SubmittedBy = $('#txtRqstSubEx').val();
                            obj.HotelPrice = $('#txtpriceEx').val();
                            obj.IsTaxIncluded = taxIncluded;
                            obj.HotelCurrencyCode = ($('#selecttxtCurrencyEx').val() == "") || ($('#selecttxtCurrencyEx').val() == "Null") ? "INR" : $('#selecttxtCurrencyEx').val();
                            elists.push(obj);
                        }
                        
                        $scope.elists = elists;
                        var inclists = new Array();
                        for (var j = 0; j < checklists.length; j++) {
                            var obj = new Object();
                            obj.TravelRequestId = checklists[j].TravelRequestId;
                            obj.HotelRequestId = checklists[j].HotelRequestId;
                            obj.HotelName = name;
                            obj.Accomodation = $('#txtaccomodationEx').val();
                            obj.Food = $('#txtfoodEx').val();
                            obj.AirportTransfers = $('#txtairporttransferEx').val();
                            obj.RoomService = $('#txtroomEx').val();
                            obj.BuffetTime = $('#txtbuffetEx').val();
                            obj.Laundry = $('#txtlaundryEx').val();
                            inclists.push(obj);
                        }
                        $scope.inclists = inclists;
                        $('#btnFinancialApproval').prop("disabled", true)
                    } else {
                        var result = confirm("Are you sure to send the request to HOD?");
                        if (result) {
                            $('.loader').show();
                            var name = "";
                            var type = "";
                            var phoneNo = "";
                            if ($('input[name="radioHotelEx"]:checked').val() == 'rdoExisting') {
                                name = $('#ddlCustomersEx :selected').text().split('-')[0].toString();
                                type = "Contractual";
                            }
                            else {
                                name = $('#txthnameEx').val();
                                type = "Non-Contractual";
                            }
                            phoneNo = $('#txthphoneEx').val();
                            var taxIncluded = true;
                            if ($("#chktaxEx").is(':checked')) {
                                taxIncluded = true;
                            } else {
                                taxIncluded = false;
                            }
                            var elists = new Array();
                            for (var j = 0; j < checklists.length; j++) {
                                var obj = new Object();
                                obj.TravelRequestId = checklists[j].TravelRequestId;
                                obj.HotelRequestId = checklists[j].HotelRequestId;
                                obj.HotelCode = $('#txtcodeEx').val();
                                obj.HotelName = name;
                                obj.HotelAddress = $('#txthaddressEx').val();
                                obj.HotelPhone = phoneNo;
                                obj.HotelType = type;
                                obj.PrimaryEmail = $('#txthemailEx').val();
                                obj.SecondaryEmail = $('#txtotheremailEx').val();
                                obj.Remarks = $('#txtremarksEx').val();
                                obj.SubmittedBy = $('#txtRqstSubEx').val();
                                obj.HotelPrice = $('#txtpriceEx').val();
                                obj.IsTaxIncluded = taxIncluded;
                                obj.HotelCurrencyCode = ($('#selecttxtCurrencyEx').val() == "") || ($('#selecttxtCurrencyEx').val() == "Null") ? "INR" : $('#selecttxtCurrencyEx').val();
                                elists.push(obj);
                            }

                            var inclists = new Array();
                            for (var j = 0; j < checklists.length; j++) {
                                var obj = new Object();
                                obj.TravelRequestId = checklists[j].TravelRequestId;
                                obj.HotelRequestId = checklists[j].HotelRequestId;
                                obj.HotelName = name;
                                obj.Accomodation = $('#txtaccomodationEx').val();
                                obj.Food = $('#txtfoodEx').val();
                                obj.AirportTransfers = $('#txtairporttransferEx').val();
                                obj.RoomService = $('#txtroomEx').val();
                                obj.BuffetTime = $('#txtbuffetEx').val();
                                obj.Laundry = $('#txtlaundryEx').val();
                                inclists.push(obj);
                            }
                            $('#btnFinancialApproval').prop("disabled", true)
                            $http({
                                method: "POST",
                                url: "../trns/sendFinancialApprovalRequest",
                                data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
                                dataType: 'json',
                                contentType: 'application/json; charset=utf-8'
                            }).then(function mySucces(response) {
                                $('.loader').hide();
                                alert('Email notification has been sent successfully.');
                                $('#btnFinancialApproval').prop("disabled", false)
                                location.reload();
                            }, function myError(response) {
                                $('.loader').hide();
                                alert('Error: Invalid request processing...');
                                $('#btnFinancialApproval').prop("disabled", false)
                            })
                        }
                    }
                }
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');

            });
        }
        else {
            var result = confirm("Are you sure to send the request to HOD?");
            if (result) {
                $('.loader').show();
                var name = "";
                var type = "";
                var phoneNo = "";
                if ($('input[name="radioHotelEx"]:checked').val() == 'rdoExisting') {
                    name = $('#ddlCustomersEx :selected').text().split('-')[0].toString();
                    type = "Contractual";
                }
                else {
                    name = $('#txthnameEx').val();
                    type = "Non-Contractual";

                }
                phoneNo = $('#txthphoneEx').val();
                var taxIncluded = true;
                if ($("#chktaxEx").is(':checked')) {
                    taxIncluded = true;
                } else {
                    taxIncluded = false;
                }
                var elists = new Array();
                for (var j = 0; j < checklists.length; j++) {
                    var obj = new Object();
                    obj.TravelRequestId = checklists[j].TravelRequestId;
                    obj.HotelRequestId = checklists[j].HotelRequestId;
                    obj.HotelCode = $('#txtcodeEx').val();
                    obj.HotelName = name;
                    obj.HotelAddress = $('#txthaddressEx').val();
                    obj.HotelPhone = phoneNo;
                    obj.HotelType = type;
                    obj.PrimaryEmail = $('#txthemailEx').val();
                    obj.SecondaryEmail = $('#txtotheremailEx').val();
                    obj.Remarks = $('#txtremarksEx').val();
                    obj.SubmittedBy = $('#txtRqstSubEx').val();
                    obj.HotelPrice = $('#txtpriceEx').val();
                    obj.IsTaxIncluded = taxIncluded;
                    obj.HotelCurrencyCode = ($('#selecttxtCurrencyEx').val() == "") || ($('#selecttxtCurrencyEx').val() == "Null") ? "INR" : $('#selecttxtCurrencyEx').val();
                    elists.push(obj);

                }
                var inclists = new Array();
                for (var j = 0; j < checklists.length; j++) {
                    var obj = new Object();
                    obj.TravelRequestId = checklists[j].TravelRequestId;
                    obj.HotelRequestId = checklists[j].HotelRequestId;
                    obj.HotelName = name;
                    obj.Accomodation = $('#txtaccomodationEx').val();
                    obj.Food = $('#txtfoodEx').val();
                    obj.AirportTransfers = $('#txtairporttransferEx').val();
                    obj.RoomService = $('#txtroomEx').val();
                    obj.BuffetTime = $('#txtbuffetEx').val();
                    obj.Laundry = $('#txtlaundryEx').val();
                    inclists.push(obj);
                }
                $('#btnFinancialApproval').prop("disabled", true)
                $http({
                    method: "POST",
                    url: "../trns/sendFinancialApprovalRequest",
                    data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    $('.loader').hide();
                    alert('Email notification has been sent successfully.');
                    $('#btnFinancialApproval').prop("disabled", false)
                    location.reload();
                }, function myError(response) {
                    $('.loader').hide();
                    alert('Error: Invalid request processing...');
                    $('#btnFinancialApproval').prop("disabled", false)
                })
            }
        }
    }

    // send request to approvers
    $scope.sendFinancialApprovalRequestOat = function () {
        var sodOat = "OAT";
        var hodemail1 = "";
        var hodemail2 = "";
        var priceval = $("#txtprice").val();
        if (priceval <= 7000) {
            hodemail1 = $("#EmailApproverOat1").text();
        }
        else {
            if (($("#chkapproverOat1").prop("checked") == false) || ($("#chkapproverOat2").prop("checked") == false)) {
                alert('Approval required from both approvers for price above 7000');
                return false;
            }
            hodemail1 = $("#EmailApproverOat1").text();
            hodemail2 = $("#EmailApproverOat2").text();           
        }

        var counter = 0;
        var i = 0;
        var checklists = new Array();
        var slist = $("#tbloatdetail input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[1].innerText;
                    var obj = new Object();
                    obj.TravelRequestId = eid.split('-')[1];
                    checklists.push(obj);
                    i++;
                }
            }
            counter++;
        });

        if (i == 0) {
            alert('Please select at least 1 employee to send request to hotel.');
            return;
        }
        if (i > 2) {
            alert('Error: Cannot share more than 2 users together.');
            return;
        }

        if ($('#txtcodeOat').val() == 'NA' || $('#txtcodeOat').val() == '' || $('#txtcodeOat').val() == null ||
            $('#txthnameOat').val() == 'NA' || $('#txthnameOat').val() == '' || $('#txthnameOat').val() == null ||
            $('#txthaddressOat').val() == 'NA' || $('#txthaddressOat').val() == '' || $('#txthaddressOat').val() == null ||
            $('#txthphoneOat').val() == 'NA' || $('#txthphoneOat').val() == '' || $('#txthphoneOat').val() == null ||
            $('#txtpriceOat').val() == 'NA' || $('#txtpriceOat').val() == '' || $('#txtpriceOat').val() == null ||
            $('#txthemailOat').val() == 'NA' || $('#txthemailOat').val() == '' || $('#txthemailOat').val() == null) {
            alert('Please enter all the details.');
            $("#txtcodeOat").focus();
            return false;
        }
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var emailaddress = $("#txthemailOat").val();
        if (!emailReg.test(emailaddress)) {
            alert("Invalid Primary Email Id");
            $("#txthemailOat").focus();
            return false;
        }
        var result = confirm("Are you sure to send the request to hotel?");
        if (result) {
            $('.loader').show();
            $scope.printloading = true;
            var name = "";
            var type = "";
            var phoneNo = "";
            if ($('input[name="radioHotelOat"]:checked').val() == 'rdoExistingOat') {
                name = $('#ddlCustomersOat :selected').text().split('-')[0].toString();
                type = "Contractual";
            }
            else {
                name = $('#txthnameOat').val();
                type = "Non-Contractual";
            }
            phoneNo = $('#txthphoneOat').val();

            var elists = new Array();
            for (var i = 0; i < checklists.length; i++) {
                var obj = new Object();
                obj.TravelRequestId = checklists[i].TravelRequestId;
                obj.HotelCode = $('#txtcodeOat').val();
                obj.HotelName = name;
                obj.HotelAddress = $('#txthaddressOat').val();
                obj.HotelPhone = phoneNo;
                obj.HotelType = type;
                obj.PrimaryEmail = $('#txthemailOat').val();
                obj.SecondaryEmail = $('#txtotheremailOat').val();
                obj.Remarks = $('#txtremarksOat').val();
                obj.HotelPrice = $('#txtpriceOat').val();
                elists.push(obj);
            }
            var inclists = new Array();
            for (var j = 0; j < checklists.length; j++) {
                var obj = new Object();
                obj.TravelRequestId = checklists[j].TravelRequestId;
                obj.HotelName = name;
                obj.Accomodation = $('#txtaccomodationOat').val();
                obj.Food = $('#txtfoodOat').val();
                obj.AirportTransfers = $('#txtairporttransferOat').val();
                obj.RoomService = $('#txtroomOat').val();
                obj.BuffetTime = $('#txtbuffetOat').val();
                obj.Laundry = $('#txtlaundryOat').val();
                inclists.push(obj);
            }
            $scope.hodemail1 = hodemail1;
            $scope.hodemail2 = hodemail2;
            $http({
                method: "POST",
                url: "../trns/sendFinancialApprovalRequest",
                data: JSON.stringify({ elist: elists, hodemail1: hodemail1, hodemail2: hodemail2, inclist: inclists, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Email notification has been sent successfully.');
                location.reload();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            })
        }
    };
    //Resend reminder mail to hotel
    $scope.ResendRequestToHotel = function (TravelRequestId, sharedtravID, HotelRequestId, sharedhId) {
        var elists = new Array();
        var obj = new Object();
        obj.TravelRequestId = TravelRequestId;
        obj.HotelRequestId = HotelRequestId;
        elists.push(obj);

        if (sharedtravID.toString.length > 0) {
            var obj2 = new Object();
            obj2.TravelRequestId = sharedtravID;
            obj2.HotelRequestId = sharedhId;
            elists.push(obj2);
        }
        var result = confirm("Are you sure to resend request to hotel ?");
        if (result) {
            var sodOat = "SOD";
            $('.loader').show();
            $http({
                method: "POST",
                url: "../trns/ResendHotelRequest",
                data: JSON.stringify({ elist: elists, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',

            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Request has been successfully resent to Hotel');
                $('#ResponseTextResend').show();
                $('#btnResendMail').hide();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }
    //Resend reminder mail to hotel oat
    $scope.ResendRequestToHotelOat = function (TravelRequestId, sharedtravID) {
        var elists = new Array();
        var obj = new Object();
        obj.TravelRequestId = TravelRequestId;
        elists.push(obj);

        var obj2 = new Object();
        obj2.TravelRequestId = sharedtravID;
        elists.push(obj2);

        var result = confirm("Are you sure to resend request to hotel ?");
        if (result) {
            $('.loader').show();
            var sodOat = "OAT";
            $http({
                method: "POST",
                url: "../trns/ResendHotelRequest",
                data: JSON.stringify({ elist: elists, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Request has been successfully resent to Hotel');
                $('#ResponseTextResendOat').show();
                $('#btnResendMailOat').hide();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }

    //request to hotel by button after approval
    $scope.RequestToHotlAfterApproval = function (TravelRequestId, HotelRequestId) {
        var elists = new Array();
        var obj = new Object();
        obj.TravelRequestId = TravelRequestId;
        obj.HotelRequestId = HotelRequestId;
        elists.push(obj);

        var result = confirm("Are you sure to send request to hotel ?");
        if (result) {
            $('.loader').show();
            var sodOat = "SOD";
            $http({
                method: "POST",
                url: "../trns/RequestToHotlAfterApproval",
                data: JSON.stringify({ elist: elists, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Request has been successfully sent to Hotel.');
                $('#btnRqstHotelApp').hide();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }

    $scope.RequestToHotlAfterApprovalOat = function (TravelRequestId) {
        var elists = new Array();
        var obj = new Object();
        obj.TravelRequestId = TravelRequestId;
        elists.push(obj);

        var result = confirm("Are you sure to send request to hotel ?");
        if (result) {
            $('.loader').show();
            var sodOat = "OAT";
            $http({
                method: "POST",
                url: "../trns/RequestToHotlAfterApproval",
                data: JSON.stringify({ elist: elists, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Request has been successfully sent to Hotel.');
                $('#btnRqstHotelAppOat').hide();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }

    // Resend request to financial approver for non-contractual hotels
    $scope.ResendApproverRequest = function (TravelRequestId, HotelRequestId) {
        var result = confirm("Are you sure to resend requests to Approver(s) ?");
        if (result) {
            $('.loader').show();
            var sodOat = "SOD";
            $http({
                method: "POST",
                url: "../trns/ResendApproverRequest",
                data: JSON.stringify({ TravelRequestId: TravelRequestId, HotelRequestId: HotelRequestId, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Request has been successfully resent to respected Approver(s).');

            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }

    $scope.ResendApproverRequestOat = function (TravelRequestId) {
        var result = confirm("Are you sure to resend requests to Approver(s) ?");
        if (result) {
            $('.loader').show();
            var sodOat = "OAT";
            $http({
                method: "POST",
                url: "../trns/ResendApproverRequest",
                data: JSON.stringify({ TravelRequestId: TravelRequestId, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Request has been successfully resent to respected Approver(s).');

            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }
    //Send responsive mail to hotel for receiving checkin-checkout time
    $scope.SendResponsiveMail = function (TravelRequestId, sharedtravID, HotelRequestId, sharedhId) {
        var elists = new Array();
        var obj = new Object();
        obj.TravelRequestId = TravelRequestId;
        obj.HotelRequestId = HotelRequestId;
        elists.push(obj);

        if (sharedtravID.toString.length > 0) {
            var obj2 = new Object();
            obj2.TravelRequestId = sharedtravID;
            obj2.HotelRequestId = sharedhId;
            elists.push(obj2);
        }
        var result = confirm("Are you sure to send request to Hotel to receive Check-in & Check-out time ?");
        if (result) {
            $('.loader').show();
            var sodOat = "SOD";
            $http({
                method: "POST",
                url: "../trns/ResponsiveMailtoHotel",
                data: JSON.stringify({ elist: elists, sodOat: sodOat }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert('Responsive mail has been successfully sent to Hotel');
                $('#ResponsiveMailResend').show();
                $('#btnResponsiveMail').hide();
            }, function myError(response) {
                $('.loader').hide();
                alert('Error: Invalid request processing...');
            });
        }
    }
    //Send Email Notification 
    $scope.emailNotification = function () {
        $.ajax({
            url: "../trns/sendEmailNotification",
            success: function (result) {
            }
        });
    }
    $scope.ToJavaScriptDate = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        const monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        return (dt.getDate()) + "-" + (monthName[dt.getMonth()]) + "-" + (dt.getFullYear());
    };

    $scope.ToJavaScriptDateTime = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        const monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        return (dt.getDate()) + "-" + (monthName[dt.getMonth()]) + "-" + (dt.getFullYear()) +
            "  " + (dt.getHours()) + ":" + (dt.getMinutes()) + ":" + (dt.getSeconds());
    };
    $scope.hotelCancellation = function (travelRequestId, HotelRequestId) {
        var result = confirm("Are you sure to send Cancellation request to hotel?");
        if (result) {
            $.ajax({
                type: "POST",
                url: "../trns/hotelCancellationRequest?travelRqstId=" + travelRequestId + "|" + HotelRequestId + "|SOD",
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $('.loader').show();
                },
                complete: function () {
                    $('.loader').hide();
                },
                success: function (response) {
                    if (response == 'Failed') {
                        alert('Cancellation is not possible as user already had a stay in the hotel.');
                    } else {
                        alert('Cancellation Request has been successfully sent to Hotel');
                        $('#cancelButton').hide();
                        //$('#ResponseCancel').show();
                        $('#ResponseText').show();
                    }
                }
            });
        }
    };


    $scope.getCancelled = false;
    $scope.hotelCancellationUser = function (travelRequestId, HotelRequestId,type) {
        $('#ResponseTextApproval').hide();
        $http({
            method: "GET",
            url: "../trns/GetHotelDetailbyTrID?trId=" + travelRequestId + "|" + HotelRequestId
        }).then(function mySucces(response) {
            $scope.arrbookinghotelTrid = response.data["hotelinfobyTrid"];
            $scope.hotelData = response.data["hotelData"];
            $scope.sharedUserdetails = response.data["sharedUserdetails"];
            $scope.sharedtravID = response.data["sharedtravID"];
            $scope.sharedhId = response.data["sharedhId"];
            $scope.trequestIdhotel = travelRequestId;
            $scope.rejectedData = response.data["rejectedData"];
            $scope.cancelledData = response.data["cancelledData"];
            if (($scope.arrbookinghotelTrid.length == 0 && $scope.rejectedData.length == 0 && $scope.cancelledData.length == 0) || type == 2) {
                $scope.getCancelled = false;
                var result = confirm("Are you sure to permanently Cancel the request on behalf of user?");
                if (result) {
                    $.ajax({
                        type: "POST",
                        url: "../trns/hotelCancellationRequestUser?travelRqstId=" + travelRequestId + "|" + HotelRequestId + "|SOD",
                        data: {},
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        beforeSend: function () {
                            $('.loader').show();
                        },
                        complete: function () {
                            $('.loader').hide();
                        },
                        success: function (response) {
                            if (response == 'Failed') {
                                alert('Cancellation is not possible as user already had a stay in the hotel.');
                            } else {
                                alert('Request no. ' + travelRequestId + ' has been cancelled successfully.');
                                $('#cancelButtonUser').hide();
                                //$('#ResponseCancel').show();
                                $('#ResponseTextUser').show();
                                location.reload();
                            }
                        }
                    });
                }
            }
            else {
                $scope.getCancelled = true;
                $("#viewdetailModalHotel").modal();
            }
        }, function myError(response) {
        });
    };


    $scope.hotelCancellationOat = function (travelRequestId) {
        $.ajax({
            type: "POST",
            url: "../trns/hotelCancellationRequest?travelRqstId=" + travelRequestId + "|OAT",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                alert('Cancellation Request has been successfully sent to Hotel');
                $('#cancelButtonOat').hide();
                $('#ResponseCancelOat').show();
            }
        });
    }


    function loadDataOAT() {
        //alert('hello');
        $.ajax({
            type: "GET",
            url: "../trns/GetEmpHotelBookingInfo?type=OAT",
            async: false,
        }).then(function mySucces(response1) {
            $scope.troatlist = response1.bookingoatList;
            $scope.currentPageOat = 1;
            $scope.entryLimitOat = 10;
            $scope.filteredItemsOat = angular.copy(response1.bookingoatList.length);
            $scope.totalItemsOAt = angular.copy(response1.bookingoatList.length);
            $scope.deptlistOat = response1.deptListOat;
            //alert(response1.bookingoatList[0].FlightNo);
        });
    };

    //commented by soni
    //function loadTableDataSorted(travelrqstid, hotelrqstid, existingTrid, exishotelid) {
    //    $('#viewdetailModalBookHotel').modal('toggle');
    //    $scope.trlist = null;
    //    $scope.currentPage = null;
    //    $scope.entryLimit = null;
    //    $scope.filteredItems = null;
    //    $scope.totalItems = null;
    //    $scope.deptlist = null;
    //    $http({
    //        method: "POST",
    //        url: "../trns/GetUserDoubleOccupancyTraveldesk",
    //        data: JSON.stringify({ newTrid: travelrqstid, existingTrid: existingTrid, hotelrqstid: hotelrqstid, exishotelid: exishotelid }),
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8'
    //    }).then(function mySucces(response) {
    //        $scope.trlist = response.data.bookingList;//fill hotel booking details data
    //        $scope.trlist = response.data.bookingList;//fill hotel booking details data
    //        $scope.currentPage = 1; //current page
    //        $scope.entryLimit = 10; //max no of items to display in a page
    //        $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
    //        $scope.totalItems = $scope.trlist.length;
    //        $scope.deptlist = response.data.deptList;//fill department list data
    //        if (response.data.bookingList[1].HotelConfirmationNo != "") {
    //            $('#ddlCustomers').children('option').each(function () { //only selected hotellist on the option
    //                if (!$(this).text().startsWith(response.data.bookingList[1].HotelName) && !$(this).text().startsWith("Please select")) { //do your conditional and if it should not be in the dropdown list
    //                    $(this).remove(); //remove option from list
    //                }
    //            });
    //        }
    //    }, function myError(response) {
    //        $scope.myWelcome = response.statusText;
    //    });

    //};

    $scope.sendhotelrequestwithClubbing = function (lst, index) {
      
        var travelrqstid = $scope.elists[0].TravelRequestId;
        var hotelrqstid = $scope.elists[0].HotelRequestId;
        var existingTrid = lst[index].TravelRequestId;
        var exishotelid = lst[index].HotelRequestId;
       

        var obj = new Object();
        obj.TravelRequestId = existingTrid //$scope.clubbedEmpDetail.TravelRequestId;
        obj.HotelRequestId = exishotelid//$scope.clubbedEmpDetail.HotelRequestId;
        obj.HotelCode = $scope.elists[0].HotelCode;
        obj.HotelName = $scope.elists[0].HotelName;
        obj.HotelAddress = $scope.elists[0].HotelAddress;
        obj.HotelPhone = $scope.elists[0].HotelPhone;
        obj.HotelType = $scope.elists[0].HotelType;
        obj.PrimaryEmail = $scope.elists[0].PrimaryEmail;
        obj.SecondaryEmail = $scope.elists[0].SecondaryEmail;
        obj.Remarks = $scope.elists[0].Remarks;
        obj.SubmittedBy = $scope.elists[0].SubmittedBy;
        obj.HotelCurrencyCode = $scope.elists[0].HotelCurrencyCode;
        $scope.elists.push(obj);

        var sodOat = "SOD";
        $http({
            method: "POST",
            url: "../trns/SendHotelRequest",
            data: JSON.stringify({ elist: $scope.elists, hotelprice: $scope.hotelprice, occupancy: "Double", sodOat: sodOat }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('.loader').hide();
            alert('Request has been successfully sent to Hotel- ' + name + '.');
            $('#btnHotelRequestExceptionEx').prop("disabled", false);
            location.reload();
        }, function myError(response) {
            $('.loader').hide();
            alert('Error: Invalid request processing...');
            location.reload();
            $('#btnHotelRequestExceptionEx').prop("disabled", false);
        });

    };


    $scope.sendFinanciahotelrequestwithClubbing = function (lst, index) {
          
        var existingTrid = lst[index].TravelRequestId;
        var exishotelid = lst[index].HotelRequestId;

        var obj = new Object();
        obj.TravelRequestId = existingTrid //$scope.clubbedEmpDetail.TravelRequestId;
        obj.HotelRequestId = exishotelid//$scope.clubbedEmpDetail.HotelRequestId;
        obj.HotelCode = $scope.elists[0].HotelCode;
        obj.HotelName = $scope.elists[0].HotelName;
        obj.HotelAddress = $scope.elists[0].HotelAddress;
        obj.HotelPhone = $scope.elists[0].HotelPhone;
        obj.HotelType = $scope.elists[0].HotelType;
        obj.PrimaryEmail = $scope.elists[0].PrimaryEmail;
        obj.SecondaryEmail = $scope.elists[0].SecondaryEmail;
        obj.Remarks = $scope.elists[0].Remarks;
        obj.SubmittedBy = $scope.elists[0].SubmittedBy;
        obj.HotelCurrencyCode = $scope.elists[0].HotelCurrencyCode;
        $scope.elists.push(obj);

        var objs = new Object();
        objs.TravelRequestId = existingTrid;
        objs.HotelRequestId = exishotelid;
        objs.HotelName = $scope.inclists[0].HotelName;
        objs.Accomodation = $scope.inclists[0].Accomodation;
        objs.Food = $scope.inclists[0].Food;
        objs.AirportTransfers = $scope.inclists[0].AirportTransfers;
        objs.RoomService = $scope.inclists[0].RoomService;
        objs.BuffetTime = $scope.inclists[0].BuffetTime;
        objs.Laundry = $scope.inclists[0].Laundry;
        $scope.inclists.push(objs);

        var sodOat = "SOD";
        $http({
            method: "POST",
            url: "../trns/sendFinancialApprovalRequest",
            data: JSON.stringify({ elist: $scope.elists, hodemail1: $scope.hodemail1, hodemail2: $scope.hodemail2, inclist: $scope.inclists, sodOat: sodOat }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('.loader').hide();
            alert('Email notification has been sent successfully.');
            $('#btnFinancialApproval').prop("disabled", false)
            location.reload();
        }, function myError(response) {
            $('.loader').hide();
            location.reload();
            alert('Error: Invalid request processing...');
            $('#btnFinancialApproval').prop("disabled", false)
        })

    };

    /* commented by sony */
    //function loadTableDataSortedEx(travelrqstid, hotelrqstid, existingTrid, exishotelid) {
    //    $('#viewdetailModalBookHotelEx').modal('toggle');
    //    $scope.trlistEx = null;
    //    $scope.currentPageEx = null;
    //    $scope.entryLimitEx = null;
    //    $scope.filteredItemsEx = null;
    //    $scope.totalItemsEx = null;
    //    $scope.deptlistEx = null;
    //    $http({
    //        method: "POST",
    //        url: "../trns/GetUserDoubleOccupancyTraveldesk",
    //        data: JSON.stringify({ newTrid: travelrqstid, existingTrid: existingTrid, hotelrqstid: hotelrqstid, exishotelid: exishotelid }),
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8'
    //    }).then(function mySucces(response) {
    //        $scope.trlistEx = response.data.bookingList;//fill hotel booking details data
    //        $scope.currentPageEx = 1; //current page
    //        $scope.entryLimitEx = 10; //max no of items to display in a page
    //        $scope.filteredItemsEx = $scope.trlistEx.length; //Initially for no filter  
    //        $scope.totalItemsEx = $scope.trlistEx.length;
    //        $scope.deptlistEx = response.data.deptList;//fill department list data 
    //        if (response.data.bookingList[1].HotelConfirmationNo != "") {
    //            $('#ddlCustomersEx').children('option').each(function () { //only selected hotellist on the option
    //                if (!$(this).text().startsWith(response.data.bookingList[1].HotelName) && !$(this).text().startsWith("Please select")) { //do your conditional and if it should not be in the dropdown list
    //                    $(this).remove(); //remove option from list
    //                }
    //            });
    //        }
    //    }, function myError(response) {
    //        $scope.myWelcome = response.statusText;
    //    });

    //};


    function loadDataExceptional() {
        $('.loader').show();
        $.ajax({
            type: "POST",
            url: "../trns/GetHotelInfoExceptional",
            // data: JSON.stringify({ trid: textEx }), //Not in use
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
        }).then(function mySucces(response) {
            $('.loader').hide();
            $("#exceptionaldiv").prop("hidden", false);
            $scope.trlistEx = response.hotelExList;
            $scope.currentPageEx = 1; //current page
            $scope.entryLimitEx = 10; //max no of items to display in a page
            $scope.filteredItemsEx = $scope.trlistEx.length; //Initially for no filter  
            $scope.totalItemsEx = $scope.trlistEx.length;
        });
    };

    function loadDataTime() {
        $('.loader').show();
        $.ajax({
            type: "GET",
            url: "../trns/GetDetailsCheckTimeConflict",
            async: false,
        }).then(function mySucces(response1) {
            $('.loader').hide();
            $scope.trlistTime = response1.bookingTimeList;
            $scope.currentPageTime = 1;
            $scope.entryLimitTime = 10;
            $scope.filteredItemsTime = angular.copy(response1.bookingTimeList.length);
            $scope.totalItemsTime = angular.copy(response1.bookingTimeList.length);
        });
    };


    $scope.Sod_OatChange = function (rdtype) {
        if (rdtype == "sodhotel") {
            $("#soddiv").prop("hidden", false);
            $("#oatdiv").prop("hidden", true);
            $("#exceptionaltxtdiv").prop("hidden", true);
            $("#exceptionaldiv").prop("hidden", true);
            $("#timediv").prop("hidden", true);
        }
        else if (rdtype == "oathotel") {
            loadDataOAT();
            hoteldatafillOat();
            $("#soddiv").prop("hidden", true);
            $("#oatdiv").prop("hidden", false);
            $("#exceptionaltxtdiv").prop("hidden", true);
            $("#exceptionaldiv").prop("hidden", true);
            $("#timediv").prop("hidden", true);
        }
        else if (rdtype == "exceptional") {
            loadDataExceptional();
            hoteldatafillEx();
            $("#soddiv").prop("hidden", true);
            $("#oatdiv").prop("hidden", true);
            $("#exceptionaltxtdiv").prop("hidden", true);
            $("#exceptionaldiv").prop("hidden", false);
            $("#timediv").prop("hidden", true);
        }
        else if (rdtype == "checktime") {
            loadDataTime();
            $("#soddiv").prop("hidden", true);
            $("#oatdiv").prop("hidden", true);
            $("#exceptionaltxtdiv").prop("hidden", true);
            $("#exceptionaldiv").prop("hidden", true);
            $("#timediv").prop("hidden", false);
        }
    }

    //show exceptional cases div
    $scope.ShowExceptionalCases = function () {
        loadDataExceptional();
        hoteldatafillEx();
    }

    $scope.highlightRows = function () {

        $('#tbldetail').removeClass("table-striped");
        $("#tbldetail tr:nth-child(2)").css("background-color", "#C6DEFF");
        $("#tbldetail tr:nth-child(3)").css("background-color", "#C6DEFF");

        $("#tbldetail tr:nth-child(3) td:nth-child(22)").show();
        $("#tbldetail tr:nth-child(3) td:nth-child(21)").hide();

        $("#tbldetail tr:nth-child(2) td:nth-child(22)").show();
        $("#tbldetail tr:nth-child(2) td:nth-child(21)").hide();
        $("#chkheader").hide();
    }

    $scope.highlightRowsEx = function () {

        $('#tbldetailEx').removeClass("table-striped");
        $("#tbldetailEx tr:nth-child(2)").css("background-color", "#C6DEFF");
        $("#tbldetailEx tr:nth-child(3)").css("background-color", "#C6DEFF");

        $("#tbldetailEx tr:nth-child(3) td:nth-child(22)").show();
        $("#tbldetailEx tr:nth-child(3) td:nth-child(21)").hide();

        $("#tbldetailEx tr:nth-child(2) td:nth-child(22)").show();
        $("#tbldetailEx tr:nth-child(2) td:nth-child(21)").hide();
        $("#chkheaderEx").hide();
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

    
    $scope.hotelBookingDiv = true;
    $scope.clubbingSelectionDiv = false;
    $scope.btnsubmit = true;
    $scope.btnsubmitResend = true;
    $scope.btnsubmitResend2 = true;
    $scope.btnreject = true;
    $scope.btnsubmitOat = true;
    $scope.btnrejectOat = true;
    $scope.btnsubmitOatResend = true;
    $scope.btnsubmitOatResend2 = true;
    $scope.aaptext = "Approve";
    $scope.rejtext = "Reject";
    $scope.aaptextOat = "Approve";
    $scope.rejtextOat = "Reject";
    $scope.aaptextHotel = "Request to Hotel";
    $scope.aaptextHotelOat = "Request to Hotel";
    $scope.sendToFinancialButton = false;
    $scope.sendToHotelButton = false; 
    loaddata();
    hoteldatafill();
    loaddataRefresh();
  
});
var hotelFullData = ""; 
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
            hotelFullData = $("#ddlCustomers").html();
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};



function hoteldatafillEx() {
    var ddlCustomers = $("#ddlCustomersEx");    
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
            hotelFullData = $("#ddlCustomersEx").html();
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
            $('#txtcodeE').val(response.hoteldetails[0].HotelCode);
            $('#txthnameE').val(response.hoteldetails[0].HotelName);
            $('#txthaddressE').val(response.hoteldetails[0].Address);
            $('#txthphoneE').val(response.hoteldetails[0].Phone);
            $('#txthemailE').val(response.hoteldetails[0].PrimaryEmail);
            $('#txtotheremailE').val(response.hoteldetails[0].SecondaryEmail);
            $('#txtTax').val(response.hoteldetails[0].IsTaxIncluded);
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

function hotelfilldropdownOat() {
    var name = $('#ddlCustomersOat :selected').text();
    $.ajax({
        type: "POST",
        url: "../trns/hotelfilldropdown?name=" + name,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //$scope.hoteldetailsfill = response.data["hoteldetails"];
            $('#txtcodeOat').val(response.hoteldetails[0].HotelCode);
            $('#txthnameOat').val(response.hoteldetails[0].HotelName);
            $('#txthaddressOat').val(response.hoteldetails[0].Address);
            $('#txthphoneOat').val(response.hoteldetails[0].Phone);
            $('#txthemailOat').val(response.hoteldetails[0].PrimaryEmail);
            $('#txtotheremailOat').val(response.hoteldetails[0].SecondaryEmail);
            $('#txtTaxOat').val(response.hoteldetails[0].IsTaxIncluded);
        }
    });

}

function hotelfilldropdownEx() {
    var name = $('#ddlCustomersEx :selected').text();
    $.ajax({
        type: "POST",
        url: "../trns/hotelfilldropdown?name=" + name,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $('#txtcodeEx').val(response.hoteldetails[0].HotelCode);
            $('#txthnameEx').val(response.hoteldetails[0].HotelName);
            $('#txthaddressEx').val(response.hoteldetails[0].Address);
            $('#txthphoneEx').val(response.hoteldetails[0].Phone);
            $('#txthemailEx').val(response.hoteldetails[0].PrimaryEmail);
            $('#txtotheremailEx').val(response.hoteldetails[0].SecondaryEmail);
            $('#txtTaxEx').val(response.hoteldetails[0].IsTaxIncluded);

            var pricecount = response.pricedetails.length;
            if (pricecount == '1') {
                $('#mealpricediv1Ex').show();
                $('#mealpricediv2Ex').hide();
                $('#mealpricediv3Ex').hide();
                $("#singlemealpriceEx").text(response.pricedetails[0].SinglePrice);
                $("#doublemealpriceEx").text(response.pricedetails[0].DoublePrice);
                if (response.pricedetails[0].TCId == "1") {
                    $("#mealtypeEx").text("APAI");
                } else if (response.pricedetails[0].TCId == "2") {
                    $("#mealtypeEx").text("MAP");
                } else {
                    $("#mealtypeEx").text("CPAI");
                }
                $('#txthCurrency1Ex').text(response.pricedetails[0].HotelCurrencyCode == null ? 'INR' : response.pricedetails[0].HotelCurrencyCode);
            } else if (pricecount == '2') {
                $('#mealpricediv1Ex').show();
                $('#mealpricediv2Ex').show();
                $('#mealpricediv3Ex').hide();
                $("#singlemealpriceEx").text(response.pricedetails[0].SinglePrice);
                $("#doublemealpriceEx").text(response.pricedetails[0].DoublePrice);
                if (response.pricedetails[0].TCId == "1") {
                    $("#mealtypeEx").text("APAI");
                } else if (response.pricedetails[0].TCId == "2") {
                    $("#mealtypeEx").text("MAP");
                } else if (response.pricedetails[0].TCId == "3") {
                    $("#mealtypeEx").text("CPAI");
                }
                $('#txthCurrency1Ex').text(response.pricedetails[0].HotelCurrencyCode == null ? 'INR' : response.pricedetails[0].HotelCurrencyCode);
                $("#singlemealprice2Ex").text(response.pricedetails[1].SinglePrice);
                $("#doublemealprice2Ex").text(response.pricedetails[1].DoublePrice);
                if (response.pricedetails[1].TCId == "1") {
                    $("#mealtype2Ex").text("APAI");
                } else if (response.pricedetails[1].TCId == "2") {
                    $("#mealtype2Ex").text("MAP");
                } else if (response.pricedetails[1].TCId == "3") {
                    $("#mealtype2Ex").text("CPAI");
                }
                $('#txthCurrency2Ex').text(response.pricedetails[1].HotelCurrencyCode == null ? 'INR' : response.pricedetails[1].HotelCurrencyCode);
            } else if (pricecount == '3') {
                $('#mealpricediv1Ex').show();
                $('#mealpricediv2Ex').show();
                $('#mealpricediv3Ex').show();
                $("#singlemealpriceEx").text(response.pricedetails[0].SinglePrice);
                $("#doublemealpriceEx").text(response.pricedetails[0].DoublePrice);
                if (response.pricedetails[0].TCId == "1") {
                    $("#mealtypeEx").text("APAI");
                } else if (response.pricedetails[0].TCId == "2") {
                    $("#mealtypeEx").text("MAP");
                } else if (response.pricedetails[0].TCId == "3") {
                    $("#mealtypeEx").text("CPAI");
                }
                $('#txthCurrency1Ex').text(response.pricedetails[0].HotelCurrencyCode == null ? 'INR' : response.pricedetails[0].HotelCurrencyCode);

                $("#singlemealprice2Ex").text(response.pricedetails[1].SinglePrice);
                $("#doublemealprice2Ex").text(response.pricedetails[1].DoublePrice);
                if (response.pricedetails[1].TCId == "1") {
                    $("#mealtype2Ex").text("APAI");
                } else if (response.pricedetails[1].TCId == "2") {
                    $("#mealtype2Ex").text("MAP");
                } else if (response.pricedetails[1].TCId == "3") {
                    $("#mealtype2Ex").text("CPAI");
                }
                $('#txthCurrency2').text(response.pricedetails[1].HotelCurrencyCode == null ? 'INR' : response.pricedetails[1].HotelCurrencyCode);

                $("#singlemealprice3Ex").text(response.pricedetails[2].SinglePrice);
                $("#doublemealprice3Ex").text(response.pricedetails[2].DoublePrice);
                if (response.pricedetails[2].TCId == "1") {
                    $("#mealtype3Ex").text("APAI");
                } else if (response.pricedetails[2].TCId == "2") {
                    $("#mealtype3Ex").text("MAP");
                } else if (response.pricedetails[2].TCId == "3") {
                    $("#mealtype3Ex").text("CPAI");
                }
                $('#txthCurrency3').text(response.pricedetails[2].HotelCurrencyCode == null ? 'INR' : response.pricedetails[2].HotelCurrencyCode);
            }
        }
    });
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

//Uniq Filter
app.filter('unique', function () {
    return function (collection, keyname) {
        var output = [],
            keys = []
        found = [];

        if (!keyname) {
            angular.forEach(collection, function (row) {
                var is_found = false;
                angular.forEach(found, function (foundRow) {
                    if (foundRow == row) {
                        is_found = true;
                    }
                });
                if (is_found) { return; }
                found.push(row);
                output.push(row);

            });
        }
        else {
            angular.forEach(collection, function (row) {
                var item = row[keyname];
                if (item === null || item === undefined) return;
                if (keys.indexOf(item) === -1) {
                    keys.push(item);
                    output.push(row);
                }
            });
        }
        return output;
    };
});

