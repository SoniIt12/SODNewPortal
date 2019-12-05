var app = angular.module('UserTrHistory', ['ui.bootstrap', 'ngSanitize']);

//Main Grid Filter
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


//Page level function : Fetch Data
app.controller("TrHistory", function ($scope, $http, $timeout) {
    function loaddata() {
        var fdate = "01/01/1900";
        var tdate = "01/01/1900";
        $http({
            method: "GET",
            url: "../SodApprover/GetEmployeeBookingHistory_ByEmpCode?EmpId=0&prm=1&IsVendorBooking=false&fdate=" + fdate + "&tdate=" + tdate,
        }).then(function mySucces(response) {
            $scope.trlist = response.data;
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
            $scope.totalItems = $scope.trlist.length;
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });

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
        $scope.changeFlight = function (TravelRequestId) {
            //$http({
            //    method: "GET",
            //    url: "../ChangeFlight/ChangeFlight?trId=" + TravelRequestId
            //}).then(function mySucces(response) {
            window.location.href = '../ChangeFlight/ChangeFlight?trId=' + TravelRequestId;
            //function mySucces(response) {
            //    //$scope.myWelcome = response.statusText;
            //});
        };
        //For displaying employee status
        $scope.showstatus = function (TravelRequestId) {
            $http({
                method: "GET",
                url: "../SodApprover/GetEmployeeBookingStatus?reqId=" + TravelRequestId
            }).then(function mySucces(response) {
                var val = '';
                var s = response.data.toString().split('.');
                for (var i = 0; i < s.length; i++) {
                    if (i == 0)
                        val = s[i] + ' \n';
                    else
                        val = val + s[i] + '\n';
                }
                $scope.bstatus = val;
            }, function myError(response) {
                //$scope.myWelcome = response.statusText;
            });
        };
        //declare variable
        var range = [];
        var pscount = 0;
        var mealdata;
        var flightData;

        //show popup model data
        $scope.viewdetail = function (TravelRequestId) {
            $http({
                method: "GET",
                url: "../SodApprover/GetBookingInfo?trId=" + TravelRequestId
            }).then(function mySucces(response) {
                $scope.arrbooking = response.data["bookingInfo"];
                $scope.arrFlight = response.data["flightInfo"];
                $scope.arrpass = response.data["passInfo"];
                $scope.arrmeals = response.data["mealsInfo"];
                $scope.trequestId = TravelRequestId;
                $scope.arrCab = response.data["cabInfo"];
                $scope.arrHotel = response.data["hotelInfo"];

                $scope.pscount = response.data["bookingInfo"][0].Passengers;
                $scope.mealdata = response.data["mealsInfo"];
                $scope.flightData = response.data["flightInfo"];

                $scope.cabData = response.data["cabInfo"];
                $scope.hotelData = response.data["hotelInfo"];

                $scope.customerpaid = response.data["bookingInfo"][0].IsAmountPaidByTraveller == true ? " - Passenger Paid Booking" : "";
                allocateMeal($scope.pscount, $scope.arrpass, $scope.mealdata, $scope.flightData, $scope.arrbooking[0]);

            }, function myError(response) {
                //$scope.myWelcome = response.statusText;
            });
        };

        //---------Hotel Check-In Date----------------------------
        $scope.setCheckInDateCal = function (travelDate) {
            travelDate = ConvertMMddyyyy(travelDate);
            $("#txtCheckInDate").datepicker({
                numberOfMonths: 2,
                minDate: new Date(travelDate),
                size: 1,
                defaultDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
        }

        $scope.setCheckOutDateCal = function (travelDate) {
            travelDate = ConvertMMddyyyy(travelDate);
            $("#txtCheckOutDate").datepicker({
                numberOfMonths: 2,
                minDate: new Date(travelDate),
                size: 1,
                defaultDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
        }

        //Function for filling the hotel data in model pop up or providing fields for it.
        $scope.viewhoteldetail = function (TravelRequestId, sector) {
            $http({
                method: "GET",
                url: "../SodApprover/GetBookingInfo?trId=" + TravelRequestId
            }).then(function mySucces(response) {
                $scope.arrHotel = response.data["hotelInfo"];
                $scope.arrbooking = response.data["bookingInfo"];
                $scope.empCode = response.data["bookingInfo"][0].RequestedEmpCode;

                $scope.hotelData = response.data["hotelInfo"];
                $scope.flightList = response.data["flightInfo"];
                $scope.trequestId = TravelRequestId;
                //checking for the hotel booking data is present
                if ($scope.hotelData.length != 0) {
                    $scope.checkindate = $scope.ToJavaScriptDateMMM($scope.hotelData[0].CheckInDate);
                    $scope.checkoutdate = $scope.ToJavaScriptDateMMM($scope.hotelData[0].CheckOutDate);
                }
                else {
                    $scope.checkindate = "";
                    $scope.checkoutdate = "";
                }
                //var count = $scope.flightList.length;
                var count = 1
                if ($scope.arrbooking[0].TravelRequestTypeId == 3) {
                    count = $scope.flightList.length;
                }
                appendcity = "";
                for (var i = 0; i < count; i++) {
                    appendcity += "<option value = '" + $scope.flightList[i].DestinationPlace + " '>" + $scope.flightList[i].DestinationPlace + " </option>";
                }
                allocateHotel($scope.hotelData, $scope.arrbooking[0], $scope.checkindate, $scope.checkoutdate, count, sector);

                //To display city
                setTimeout(function () {
                    $('.select2').select2({
                        placeholder: 'Select City'
                    });

                    if ($scope.arrbooking[0].BookingFor != "Only Hotel") {
                        if ($scope.arrbooking[0].TravelRequestTypeId != 3) {
                            $('#txtCity0').val($scope.flightList[0].DestinationPlace + " ").trigger('change');
                            $("#txtCity0").prop("disabled", true);
                            $("#dvPlaceToVisit").css("height", "auto");
                        }
                        else {
                            $('#checkboxhotel0').prop("checked", true);
                            clickcheckboxhotel(0);
                            $("#dvPlaceToVisit").css("height", "470px");
                        }
                    }
                    //for (var i = 1; i < count; i++) {
                    //    if ($scope.arrbooking[0].BookingFor != "Only Hotel") {
                    //        $('#txtCity' + i).val($scope.flightList[i].DestinationPlace + " ").trigger('change');
                    //        $("#txtCity" + i).prop("disabled", true);
                    //    }
                    //}
                }, 100);

            }, function myError(response) {
                //$scope.myWelcome = response.statusText;
            });
        };
        //Function for filling the hotel data in model pop up or providing fields for it.
        $scope.viewcabdetail = function (TravelRequestId) {
            $http({
                method: "GET",
                url: "../SodApprover/GetBookingInfo?trId=" + TravelRequestId
            }).then(function mySucces(response) {
                $scope.arrbooking = response.data["bookingInfo"];
                $scope.trequestId = TravelRequestId;
                $scope.arrCab = response.data["cabInfo"];
                $scope.cabData = response.data["cabInfo"];
                allocateCab($scope.cabData, $scope.arrbooking[0]);
            }, function myError(response) {
                //$scope.myWelcome = response.statusText;
            });
        };
        //Convert date script date formt for display
        $scope.ToJavaScriptDate = function (value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
            //return (ConvertMMddyyyy(dt.toLocaleDateString()));
        };
        $scope.ToJavaScriptDateMMM = function (value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            const monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            return (dt.getDate()) + "-" + (monthName[dt.getMonth()]) + "-" + (dt.getFullYear());
            //return (ConvertMMddyyyy(dt.toLocaleDateString()));
        };
        $scope.editRow = function () {
            $("#txtHeader1").hide();
            $("#txtRow1").show();
            $("#hotelCancelRow1").hide();
        }
        //convert MM/dd/yyyy
        function ConvertMMddyyyy(dval) {
            var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
            return mval;
        }
    }

    $scope.ResendMailToHod = function (Trnid) {
        var result = confirm("Are you sure to send request to HOD ?");
        if (result) {
            $('.loader').show();
            $http({
                method: "GET",
                url: "../ResendMail/ResendMailToHod?trId=" + Trnid
            }).then(function mySucces(response) {
                if (response.data = "Success") {
                    alert("Mail has been re-sent successfully.");
                }
                else {
                    alert("Mail has not been re-sent successfully.");
                }
                $('.loader').hide();
            }, function myError(response) {
                alert("Some Error Occured");
                $('.loader').hide();
            });
        }
    }

    $scope.filter = function () {
        $timeout(function () {
            $scope.filterItems = $scope.filter.length;
        }, 1);
    };

    loaddata();
});

//For Reports
app.controller("UserReport", function ($scope, $http, $timeout) {
    $scope.filteredItemFooter = 0;
    function loaddata(prm) {
        $scope.trlist = [];
        $scope.tdlist = [];
        $scope.filteredItemFooter = 0;
        var fdate = ConvertMMddyyyy($("#txtfromdate").val());
        var tdate = ConvertMMddyyyy($("#txttodate").val())
        //var date = $("#txtfromdate").val() + "," + $("#txttodate").val() + "," + prm; //prod
        $http({
            method: "GET",
            url: "../SodApprover/GetEmployeeBookingHistory_ByEmpCode?EmpId=0&IsVendorBooking=false&fdate=" + fdate + "&tdate=" + tdate + "&prm=" + prm,
        }).then(function mySucces(response) {
            if (prm == 1) {
                $scope.trlistlst = true;
                $scope.trlist = response.data;
                $scope.currentPage = 1;
                $scope.entryLimit = 10;
                $scope.filteredItems = $scope.trlist.length;
                $scope.totalItems = $scope.trlist.length;
                $scope.filteredItemFooter = $scope.filteredItems;
            }
            else {
                $scope.trlistlst = false;
                $scope.tdlist = response.data;
                for (var i = 0; i < response.data.length; i++) {
                    $scope.tdlist[i].RequestDate = new Date(parseFloat((response.data[i].RequestDate).substring(6)));
                }
                $scope.currentPage = 1;
                $scope.entryLimit = 10;
                $scope.filteredItems = $scope.tdlist.length;
                $scope.totalItems = $scope.tdlist.length;
                $scope.filteredItemFooter = $scope.filteredItems;
            }
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
        $scope.setPage = function (pageNo) {
            $scope.currentPage = pageNo;
        };
        $scope.filter = function () {
            $timeout(function () {
                if ($scope.trlistlst == true)
                    $scope.filteredItems = $scope.filtered.length;
                if ($scope.trlistlst == false)
                    $scope.filteredItems = $scope.filtered1.length;
            }, 1);
        };
        $scope.filter1 = function () {
            $timeout(function () {
                $scope.filteredItems = $scope.filtered1.length;
            }, 1);
        };
        $scope.sort_by = function (predicate) {
            $scope.predicate = predicate;
            $scope.reverse = !$scope.reverse;
        };
    }
    $scope.getdatewiseList = function () {
        var isuser = $('#rdouser').is(':checked');
        var ishod = $('#rdoapprover').is(':checked');
        if (isuser) { loaddata(1); }
        if (ishod) { loaddata(2); }
    }
    $scope.hideTbl = function () {        
        $scope.tdlist = null;
        $scope.trlist = null;
        $scope.currentPage = null;
        $scope.entryLimit = null;
        $scope.filteredItems = null;
        $scope.totalItems = null;
        $scope.filteredItemFooter = 0;
    };
    //Down load the Search data
    $scope.downloadexcel = function () {
        var fdate = ConvertMMddyyyy($("#txtfromdate").val());
        var tdate = ConvertMMddyyyy($("#txttodate").val())
        var result = confirm("Are you sure to download?");
        if (result) {
            window.location = "../SodApprover/ExportVendorData?EmpId=0&IsVendorBooking=false&fdate=" + fdate + "&tdate=" + tdate;
        }
    };
    //convert MM/dd/yyyy
    function ConvertMMddyyyy(dval) {
        var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
        return mval;
    }

    //show popup model data
    $scope.viewdetail = function (TravelRequestId) {
        $http({
            method: "GET",
            url: "../SodApprover/GetBookingInfo?trId=" + TravelRequestId
        }).then(function mySucces(response) {
            $scope.arrbooking = response.data["bookingInfo"];
            $scope.arrFlight = response.data["flightInfo"];
            $scope.arrpass = response.data["passInfo"];
            $scope.arrmeals = response.data["mealsInfo"];
            $scope.trequestId = TravelRequestId;
            $scope.arrCab = response.data["cabInfo"];
            $scope.arrHotel = response.data["hotelInfo"];
            $scope.pscount = response.data["bookingInfo"][0].Passengers;
            $scope.mealdata = response.data["mealsInfo"];
            $scope.flightData = response.data["flightInfo"];
            $scope.cabData = response.data["cabInfo"];
            $scope.hotelData = response.data["hotelInfo"];
            $scope.customerpaid = response.data["bookingInfo"][0].IsAmountPaidByTraveller == true ? " - Passenger Paid Booking" : "";
            allocateMeal($scope.pscount, $scope.arrpass, $scope.mealdata, $scope.flightData, $scope.arrbooking[0]);
        }, function myError(response) {
            //$scope.myWelcome = response.statusText;
        });
    };


});

//Convert Time into 24 hrs format
function ConvertTime(time) {
    var AMPM = time.split(' ')[3];
    var hrs = parseInt(time.split(' ')[0]);
    var minutes = parseInt(time.split(' ')[2]);
    if (AMPM == "PM") {
        if (hrs != '12')
            hrs = hrs * 1 + 12;
    }
    else if (AMPM == "AM" && hrs == '12')
        hrs -= 12;
    else
        hrs = hrs;

    return hrs * 60 + minutes;

}


//Hotel Integration-----------------------------------------------------------------
function SubmitHotelData(trid, count) {
    var c = 0;
    var timeflag = true;
    for (var i = 0; i < count; i++) {
        var city;
        var CurrentDate, CheckInDate, CheckOutDate;
        city = document.getElementById("txtCity" + i).value;

        if ($("#checkboxhotel" + i).is(':checked')) {

            if (city.trim() == null || city.trim() == "") {
                alert("Please mention city code for hotel booking");
                $("#txtCity" + i).focus();
                return false;
            }
            $.ajax({
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                type: 'POST',
                url: '../EmployeeBookingDetail/CityCodeCheck',
                data: JSON.stringify({ citycode: city }),

                success: function (response) {
                    if (response == "NotExist") {
                        alert('City Code does not exist. Please enter a valid City Code.');
                        $("#txtCity" + i).focus();
                        return;
                    }
                },
                error: function (response) {
                    // alert(response);
                }
            });

            c = c + 1;
            today = new Date();
            //Local Server
            CurrentDate = today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear(); // current date in mm/dd/yyyy format.
            CurrentDate = Date.parse(ConvertMMddyyyy(CurrentDate).toString());//Date.parse(CurrentDate).toString();//for prod
            CheckInDate = Date.parse(ConvertMMddyyyy(document.getElementById("txtCheckInDate" + i).value));
            CheckOutDate = Date.parse(ConvertMMddyyyy(document.getElementById("txtCheckOutDate" + i).value));

            //Prod Server
            //CurrentDate = today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear();
            //CurrentDate = Date.parse(CurrentDate).toString();//ConvertMMddyyyy(Date.parse(CurrentDate).toString());
            //CheckInDate = Date.parse(document.getElementById("txtCheckInDate" + i).value);
            //CheckOutDate = Date.parse(document.getElementById("txtCheckOutDate" + i).value);
            if (document.getElementById("txtCheckInDate" + i).value.trim() == "" || document.getElementById("txtCheckInDate" + i).value.trim() == null) {
                alert("Please provide Check-In Date");
                $("#txtCheckInDate" + i).focus();
                return false;
            }
            if (document.getElementById("txtCheckOutDate" + i).value.trim() == "" || document.getElementById("txtCheckOutDate" + i).value.trim() == null) {
                alert("Please provide Check-Out Date");
                $("#txtCheckOutDate" + i).focus();
                return false;
            }
            if (document.getElementById("txtCheckintime" + i).value.trim() == "" || document.getElementById("txtCheckintime" + i).value.trim() == null) {
                alert("Please provide Check-In time");
                $("#txtCheckintime" + i).focus();
                return false;
            }
            if (document.getElementById("txtCheckouttime" + i).value.trim() == "" || document.getElementById("txtCheckouttime" + i).value.trim() == null) {
                alert("Please provide Check-Out time");
                $("#txtCheckouttime" + i).focus();
                return false;
            }
            if ($("#txtCheckInDate" + i).val() == $("#txtCheckOutDate" + i).val()) {
                var checkIntime = parseFloat(ConvertTime($('#txtCheckintime' + i).val()));
                var CheckoutTime = parseFloat(ConvertTime($('#txtCheckouttime' + i).val()));
                if (checkIntime == CheckoutTime) {
                    alert('For Same Check-In date and Check-Out date, Check-In time and Check-Out time should be different .');
                    $('#txtCheckintime' + i).focus();
                    timeflag = false;
                }
                if (checkIntime > CheckoutTime) {
                    alert('For Same Check-In date and Check-Out date,Check-Out time should be greater than Check-In time.');
                    $('#txtCheckouttime' + i).focus();
                    timeflag = false;
                }
            }
            if (!timeflag) return false;
            //var validateDate1 = $("#txtCheckInDate" + i).val();
            //if (!isValidDateHotel(validateDate1)) {
            //    alert("Invalid date format! Enter Check-In Date in dd/MM/yyyy format. Example: 14/09/2018");
            //    $("#txtCheckInDate" + i).focus();
            //    return;
            //}
            //var validateDate2 = $("#txtCheckOutDate" + i).val();
            //if (!isValidDateHotel(validateDate2)) {
            //    alert("Invalid date format! Enter Check-Out Date in dd/MM/yyyy format. Example: 14/09/2018");
            //    $("#txtCheckOutDate" + i).focus();
            //    return;
            //}
        }
    }

    var result = confirm("Are you sure to send hotel request to travel desk ?");
    if (result) {
        //Creating data object for the data being submitted.
        hotelDetailLst = postsodhotelmasterlst(trid, c);

        //Callling controller method data being submitted.
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: '../EmployeeBookingDetail/SubmitHotelInfo',
            data: JSON.stringify({ hotelDetailList: hotelDetailLst }),
            beforeSend: function () {
                $('.loader').show();
            },
            complete: function () {
                $('.loader').hide();
            },
            success: function (data) {
                alert("Data submitted successfully.")
                location.reload();
            },
            error: function () {
                alert(data);
            }
        });
    }
};

function postsodhotelmasterlst(travelRequestId, count) {
    var hotelDetailsList = new Array();
    for (var i = 0; i < count; i++) {
        var objhd = new Object();
        objhd.TravelRequestId = travelRequestId;
        objhd.HotelReferenceID = "SH" + travelRequestId;
        objhd.City = $('#txtCity' + i).val();
        objhd.EmployeeCode = $('#hddnEmpCode').val();

        //for local
        objhd.CheckInDate = ConvertMMddyyyy(document.getElementById("txtCheckInDate" + i).value);
        objhd.CheckOutDate = ConvertMMddyyyy(document.getElementById("txtCheckOutDate" + i).value);

        //for prod server
        //objhd.CheckInDate = document.getElementById("txtCheckInDate" + i).value;
        //objhd.CheckOutDate = document.getElementById("txtCheckOutDate" + i).value;

        if ($("#entitlement" + i + " :selected").text() == "Single") {
            objhd.NoOfGuest = "1";
        }
        else {
            objhd.NoOfGuest = "2";
        }
        objhd.CheckinTime = $('#txtCheckintime' + i).val();
        objhd.CheckoutTime = $('#txtCheckouttime' + i).val();
        objhd.EntryDate = new Date();
        objhd.HotelCode = "NA";
        objhd.HotelName = "NA";
        objhd.HotelAddress = "NA";
        objhd.HotelPhoneNo = "NA";
        objhd.IsAllocated = "0";
        objhd.Remarks_Status = "NA";
        if ($('#airtransport' + i + ' :selected').text() == "Yes") {
            objhd.AirportTransport = true;
            if ($('input[name="radio ' + i + '" ]:checked').val() == 'cabyes') {
                objhd.IsCabRequiredAsPerETA = true;
            } else {
                objhd.IsCabRequiredAsPerETA = false;
                objhd.CabPickupTime = $('#txtCabPickupTime' + i).val();
            }
        }
        else {
            objhd.AirportTransport = false;
        }
        objhd.IsCabRequiredAsPerETA = true;
        objhd.HotelRequestId = i + 1;
        hotelDetailsList.push(objhd);
    }
    return hotelDetailsList;
}
//-------------------------------------------------------------

function ConvertMMddyyyy(dval) {
    var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
    return mval;
}

function isValidDateHotel(sText) {
    var reDate = /(?:0[1-9]|[12][0-9]|3[01])\/(?:0[1-9]|1[0-2])\/(?:19|20\d{2})/;
    return reDate.test(sText);
}

function ToJavaScriptDateMMM(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    //const monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
    //    "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    // return (dt.getDate()) + "-" + (monthName[dt.getMonth()]) + "-" + (dt.getFullYear());
    return (ConvertMMddyyyy(dt.toLocaleDateString()));
    const monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    return (dt.getDate()) + "-" + (monthName[dt.getMonth()]) + "-" + (dt.getFullYear());
    //return (ConvertMMddyyyy(dt.toLocaleDateString()));
};


function toggleAirTransport(counter) {
    if ($("#airtransport" + counter + " option:selected").text() == "Yes") {
        $("#cabTimingText" + counter).prop("hidden", true);
        $("#CabRequiredDiv" + counter).prop("hidden", false);

    } else {
        $("#cabTimingText" + counter).prop("hidden", true);
        $("#CabRequiredDiv" + counter).prop("hidden", true);
    }
}

function changeCabResponse(counter) {
    if ($('input[name="radio' + counter + '"]:checked').val() == 'cabyes') {
        $("#cabTimingText" + counter).prop("hidden", true);
        $("#txtCabPickupTime" + counter).prop("hidden", true);

    }
    else if ($('input[name="radio' + counter + '"]:checked').val() == 'cabno') {
        $("#cabTimingText" + counter).prop("hidden", false);
        $("#txtCabPickupTime" + counter).prop("hidden", false);
    }
}

function allocateHotel(hotelData, bookinginfo, checkInDate, checkOutDate, count, sector) {
    $("#tblHotelDet").empty();
    var trhotel = "";
    if (bookinginfo.SodBookingTypeId == 1) {
        if (hotelData.length == count) {
            //hiding the div for hotel data submission and showing div with filled hotel details.
            trhotel += '<tr class="popheadcolor" style="font-weight:bold;">';
            trhotel += '<th>City</th><th>Check-In Date</th><th>Check-Out Date</th><th>Status</th><th>Hotel Details</th><th>Remark</th>';
            trhotel += '</tr>';
            $('#tblHotelDet').append(trhotel);
            for (var i = 0; i < hotelData.length; i++) {
                trHotel = "";
                trhotel = "<tr id='txtHeader" + i + "'>";
                hotelData[i].IsAllocated = (hotelData[i].IsAllocated == '1') ? "Allocated" : "Not Allocated";
                hotelData[i].HotelConfirmationNo = (hotelData[i].HotelConfirmationNo == null) ? "NA" : hotelData[i].HotelConfirmationNo;
                trhotel += "<td>" + hotelData[i].City + "</td><td>" + ToJavaScriptDateMMM(hotelData[i].CheckInDate.toString()) + "</td><td>"
                    + ToJavaScriptDateMMM(hotelData[i].CheckOutDate) + "</td>";
                trhotel += "<td>" + hotelData[i].IsAllocated + "</td>";
                trhotel += "<td>" + "Hotel Confirmation No.: " + hotelData[i].HotelConfirmationNo + "<br />" +
                    "Name: " + hotelData[i].HotelName + "<br />" +
                    "Address: " + hotelData[i].HotelAddress + "<br />" +
                    "Phone: " + hotelData[i].HotelPhoneNo + "<br />" +
                    "</td>";
                trhotel += "<td>" + hotelData[i].Remarks_Status + "</td>";
                trhotel += "</tr>"


                trhotel += "<tr hidden='hidden' id='txtRow" + i + "'>";
                hotelData[i].IsAllocated = (hotelData[i].IsAllocated == '1') ? "Allocated" : "Not Allocated";
                hotelData[i].HotelConfirmationNo = (hotelData[i].HotelConfirmationNo == null) ? "NA" : hotelData[i].HotelConfirmationNo;
                trhotel += "<td>" + hotelData[i].City + "</td><td>" +
                    "<input value='" + ToJavaScriptDateMMM(hotelData[i].CheckInDate.toString()) + "' type='text' id='txtchindate" + i + "'  class='inputremark form-control fh_inputtxt2 ' style='cursor: pointer;'>" + "</td><td>"
                    + "<input type='text' id='txtchoutdate" + i + "' value='" + ToJavaScriptDateMMM(hotelData[i].CheckOutDate.toString()) + "'  class='inputremark form-control fh_inputtxt2 ' style='cursor: pointer;'>" + "</td>";
                trhotel += "<td>" + hotelData[i].IsAllocated + "</td>";
                trhotel += "<td>" + "Hotel Confirmation No.: " + hotelData[i].HotelConfirmationNo + "<br />" +
                    "Name: " + hotelData[i].HotelName + "<br />" +
                    "Address: " + hotelData[i].HotelAddress + "<br />" +
                    "Phone: " + hotelData[i].HotelPhoneNo + "<br />" +
                    "</td>";
                trhotel += "<td>" + hotelData[i].Remarks_Status + "</td>";
                trhotel += "</tr>"


                trhotel += "<tr id='hotelCancelRow" + i + "'>";
                trhotel += "<td colspan='3'>" + "Hotel Cancellation: " + "</td>";
                if (hotelData[i].usercancellation == "Cancelled by User") {
                    trhotel += "<td colspan='3'>" + "Cancelled" + "&nbsp;&nbsp;<button type='button' class='btn btn-warning btn-sm' id='UndocancellationButton' onclick='UndoCancelledRequest(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + ")'>Undo Cancellation</button>" + "</td>";
                } else if (hotelData[i].usercancellation == "Cancelled") {
                    trhotel += "<td colspan='3'>" + "Cancelled by Traveldesk" + "&nbsp;&nbsp;<button type='button' class='btn btn-warning btn-sm' id='UndocancellationButton' onclick='UndoCancelledRequest(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + ")'>Undo Cancellation</button>" + "</td>";
                }
                else {
                    trhotel += "<td colspan='2'>" + "<input id='txtCancel" + i + "' type='text' placeholder='Reason for Cancellation' class='form-control input-sm' />" + "</td>";
                    trhotel += "<td colspan='1'>" + "<button type='button' class='btn btn-warning btn-sm' id='cancellationButton' onclick='cancelRequest(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + "," + i + ")'>Cancel Hotel</button>" + "</td>";

                }
                trhotel += "</tr>"

                trhotel += "<tr >";
                if (hotelData[i].usercancellation == "Cancelled by User" || hotelData[i].usercancellation == "Cancelled") {
                    trhotel += "<td style='text-align:center;' colspan='6' hidden id='editbtntd" + i + "'>" + "<input type='button' value='Edit' class='btn btn-warning btn-sm' onclick='editRow(" + i + ");' style='background-color: #EE1D23; border-color: #EE1D23; width:70px;' />" + "</td>";
                }
                else {
                    trhotel += "<td style='text-align:center;' colspan='6' id='editbtntd" + i + "'>" + "<input type='button' value='Edit' class='btn btn-warning btn-sm' onclick='editRow(" + i + ");' style='background-color: #EE1D23; border-color: #EE1D23; width:70px;' />" + "</td>";
                }
                trhotel += "<td style='text-align:center;' colspan='6' hidden='hidden' id='updatebtntd" + i + "'>" + "<input type='button' value='Update' class='btn btn-warning btn-sm' onclick='updateRow(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + "," + i + ");' style='background-color: #EE1D23; border-color: #EE1D23;' />" + "</td>";
                trhotel += "</tr>"

                $('#tblHotelDet').append(trhotel);
                $(function () {
                    $('#txtchindate' + i).datepicker({
                        numberOfMonths: 2,
                        defaultDate: new Date(),
                        minDate: new Date(),
                        maxDate: "+10M +00D",
                        dateFormat: "dd/mm/yy"
                    });
                });
                $(function () {
                    $('#txtchoutdate' + i).datepicker({
                        numberOfMonths: 2,
                        defaultDate: new Date(),
                        minDate: new Date(),
                        maxDate: "+10M +00D",
                        dateFormat: "dd/mm/yy"
                    });
                });

            }
            if (bookinginfo.BookingStatus == "Rejected") {
                $("#cancellationButton").prop("disabled", true);
            }
            $("#dvPlaceToVisit").empty();
            $("#dvHotelBookingDataFill").hide();
            $("#dvHotelBookingDataShow").show();


        } else {
            //showing the div for hotel data submission and hiding div for filled hotel details.
            $("#dvPlaceToVisit").empty();
            var trH = "";
            var trSub = "";
            if (bookinginfo.TravelRequestTypeId != 3) {
                var trHfix = "<div class='row'>" +
                    "<div class='col-md-12' style='text-align:left ;display:none;'><div><input type='checkbox' onclick='return false;' checked id='checkboxhotel" + 0 + "'/><span><b>Hotel Required</b></span></div></div>" +
                    "</div><br>" +
                    "<div id='hoteldivdetails" + 0 + "'>" +
                    "<div class='row'>" +
                    "<div class='col-md-4'><div ><span>City Code</span></div><div><input type='text' id='txtCity" + 0 + "' class='form-control' style='width:100%;height:35px;'/></div></div>" +
                    "<div class='col-md-4'><div><span>Check-In Date</span></div><div> <input id='txtCheckInDate" + 0 + "' type='text' placeholder='dd/mm/yyyy' class='fh_inputtxt2 form-control  ' /><i class='img_i_indt' onclick='document.getElementById('txtCheckInDate').focus();'></i></div></div>" +
                    "<div class='col-md-4'><div><span>Check-Out Date</span></div><div> <input id='txtCheckOutDate" + 0 + "' type='text' placeholder='dd/mm/yyyy' class='fh_inputtxt2 form-control  ' /><i class='img_i_odt' onclick='document.getElementById('txtCheckOutDate').focus();'></i></div></div>" +
                    "</div>" +
                    "<br />" +
                    "<div class='row'>" +
                    "<div class='col-md-4'><div><span>Check-In Time</span></div><div><input id='txtCheckintime" + 0 + "' type='text' class='timepicker extratextcontrol_height' /></div></div>" +
                    "<div class='col-md-4'><div><span>Check-Out Time</span></div><div> <input id='txtCheckouttime" + 0 + "' type='text' class='timepicker extratextcontrol_height' /></div></div>" +
                    "<div class='col-md-4'><div><span>Entitlement</span></div><div> <select class='form-control ' id='entitlement" + 0 + "'> <option>Single</option> <option>Sharing</option></select></div></div>" +
                    "</div>" +
                    "<br />" +
                    "<div class='row'>" +
                    "<div class='col-md-4'><div><span>Airport Transport Required</span></div><div><select class='form-control' onchange='toggleAirTransport(0);' id='airtransport" + 0 + "'> <option>No</option> <option>Yes</option></select></div></div>" +
                    "<div class='col-md-4' id ='CabRequiredDiv" + 0 + "' hidden = 'hidden'><div style='margin-top: 24px; margin- bottom: 10px;'><span>Require Cab as per ETA? </span>&nbsp;&nbsp;<input type='radio' name='radio" + 0 + "' value='cabyes' id='textboxcabyes" + 0 + "' checked = 'checked' onchange = 'changeCabResponse(" + 0 + ");' />" +
                    " <span>Yes</span> &nbsp;&nbsp; <input type='radio' name='radio" + 0 + "' value='cabno' id='textboxcabno" + 0 + "' onchange = 'changeCabResponse(" + 0 + ");' /> <span>No</span></div></div>" +
                    "<div class='col-md-4' id='cabTimingText" + 0 + "' hidden='hidden'><div><span>If Yes, suggest Cab pickup timing: </span></div><div><input id='txtCabPickupTime" + 0 + "' type='text' class='timepicker extratextcontrol_height hasWickedpicker'/></div></div>" +
                    "</div><br>" +
                    "<div class='row'></div>" +
                    "</div>";
                $('#dvPlaceToVisit').append(trHfix);

                $(function () {
                    $('#txtCheckInDate0').datepicker({
                        numberOfMonths: 2,
                        defaultDate: new Date(),
                        minDate: new Date(),
                        maxDate: "+10M +00D",
                        dateFormat: "dd/mm/yy"
                    });
                });
                $(function () {
                    $('#txtCheckOutDate0').datepicker({
                        numberOfMonths: 2,
                        defaultDate: new Date(),
                        minDate: new Date(),
                        maxDate: "+10M +00D",
                        dateFormat: "dd/mm/yy"
                    });
                });
                $(document).ready(function () {
                    $('.timepicker').wickedpicker({
                        twentyFour: false, title:
                            'Select Time', showSeconds: false
                    });
                });
            }
            else {
                for (var i = 0; i < count; i++) {
                    trH = "";
                    if (!hotelData[i]) {
                        trH += "<div class='row'>" +
                            "<div class='col-md-12' style='text-align:left'><div><input type='checkbox' onchange='clickcheckboxhotel(" + i + ")' id='checkboxhotel" + i + "'/><span><b>Add More Hotel (If required)</b></span></div></div>" +
                            "</div>" +
                            "<div hidden id='hoteldivdetails" + i + "'>" +
                            "<br><div class='row'>" +
                            "<div class='col-md-4'><div><span>City Code</span></div><div><select id='txtCity" + i + "' class='form-control select2 input-sm' style='width:100%'><option value=''> </option>" + appendcity + "</select></div></div>" +
                            "<div class='col-md-4'><div><span>Check-In Date</span></div><div> <input id='txtCheckInDate" + i + "' type='text' placeholder='dd/mm/yyyy' class='form-control input-sm fh_inputtxt2  ' /><i class='img_i_indt' onclick='document.getElementById('txtCheckInDate').focus();'></i></div></div>" +
                            "<div class='col-md-4'><div><span>Check-Out Date</span></div><div> <input id='txtCheckOutDate" + i + "' type='text' placeholder='dd/mm/yyyy' class='form-control input-sm fh_inputtxt2  ' /><i class='img_i_odt' onclick='document.getElementById('txtCheckOutDate').focus();'></i></div></div>" +
                            "</div>" +
                            "<br />" +
                            "<div class='row'>" +
                            "<div class='col-md-4'><div><span>Check-In Time</span></div><div><input id='txtCheckintime" + i + "' type='text' class='timepicker extratextcontrol_height' /></div></div>" +
                            "<div class='col-md-4'><div><span>Check-Out Time</span></div><div> <input id='txtCheckouttime" + i + "' type='text' class='timepicker extratextcontrol_height' /></div></div>" +
                            "<div class='col-md-4'><div><span>Entitlement</span></div><div> <select class='form-control input-sm' id='entitlement" + i + "'> <option>Single</option> <option>Sharing</option></select></div></div>" +
                            "</div>" +
                            "<br />" +
                            "<div class='row'>" +
                            "<div class='col-md-4'><div><span>Airport Transport Required</span></div><div><select class='form-control' onchange='toggleAirTransport(" + i + ");' id='airtransport" + i + "'> <option>No</option> <option>Yes</option></select></div></div>" +
                            "<div class='col-md-4' id ='CabRequiredDiv" + i + "' hidden = 'hidden'><div style='margin-top: 24px; margin- bottom: 10px;'><span>Require Cab as per ETA? </span>&nbsp;&nbsp;<input type='radio' name='radio" + i + "' value='cabyes' id='textboxcabyes" + i + "' checked = 'checked' onchange = 'changeCabResponse(" + i + ");' />" +
                            " <span>Yes</span> &nbsp;&nbsp;<input type='radio' name='radio" + i + "' value='cabno' id='textboxcabno" + i + "' onchange = 'changeCabResponse(" + i + ");' /> <span>No</span></div></div>" +
                            "<div class='col-md-4' id='cabTimingText" + i + "' hidden='hidden'><div><span>If Yes, suggest Cab pickup timing: </span></div><div><input id='txtCabPickupTime" + i + "' type='text' class='timepicker extratextcontrol_height hasWickedpicker'/></div></div>" +
                            "</div><br>" +
                            "</div>" +
                            "<hr style='border: 1px solid lightgrey;'><div></div>";
                        $('#dvPlaceToVisit').append(trH);
                        $(function () {
                            $('#txtCheckInDate' + i).datepicker({
                                numberOfMonths: 2,
                                defaultDate: new Date(),
                                minDate: new Date(),
                                maxDate: "+10M +00D",
                                dateFormat: "dd/mm/yy"
                            });
                        });
                        $(function () {
                            $('#txtCheckOutDate' + i).datepicker({
                                numberOfMonths: 2,
                                defaultDate: new Date(),
                                minDate: new Date(),
                                maxDate: "+10M +00D",
                                dateFormat: "dd/mm/yy"
                            });
                        });
                        $(document).ready(function () {
                            $('.timepicker').wickedpicker({
                                twentyFour: false, title:
                                    'Select Time', showSeconds: false
                            });
                        });
                    }
                    else {
                        trHotel = "";
                        trhotel += "<div><div><b>Hotel Information(s)</b></div><table class='flighdtl table table-striped table-bordered'>";
                        trhotel += "<tr class='popheadcolor' style='font-weight:bold;'>";
                        trhotel += "<th>City</th><th>Check-In Date</th><th>Check-Out Date</th><th>Status</th><th>Hotel Details</th><th>Remark</th>";
                        trhotel += "</tr>";
                        trhotel += "<tr id='txtHeader" + i + "'>";
                        hotelData[i].IsAllocated = (hotelData[i].IsAllocated == '1') ? "Allocated" : "Not Allocated";
                        hotelData[i].HotelConfirmationNo = (hotelData[i].HotelConfirmationNo == null) ? "NA" : hotelData[i].HotelConfirmationNo;
                        trhotel += "<td>" + hotelData[i].City + "</td><td>" + ToJavaScriptDateMMM(hotelData[i].CheckInDate.toString()) + "</td><td>"
                            + ToJavaScriptDateMMM(hotelData[i].CheckOutDate) + "</td>";
                        trhotel += "<td>" + hotelData[i].IsAllocated + "</td>";
                        trhotel += "<td>" + "Hotel Confirmation No.: " + hotelData[i].HotelConfirmationNo + "<br />" +
                            "Name: " + hotelData[i].HotelName + "<br />" +
                            "Address: " + hotelData[i].HotelAddress + "<br />" +
                            "Phone: " + hotelData[i].HotelPhoneNo + "<br />" +
                            "</td>";
                        trhotel += "<td>" + hotelData[i].Remarks_Status + "</td>";
                        trhotel += "</tr>"


                        trhotel += "<tr hidden='hidden' id='txtRow" + i + "'>";
                        hotelData[i].IsAllocated = (hotelData[i].IsAllocated == '1') ? "Allocated" : "Not Allocated";
                        hotelData[i].HotelConfirmationNo = (hotelData[i].HotelConfirmationNo == null) ? "NA" : hotelData[i].HotelConfirmationNo;
                        trhotel += "<td>" + hotelData[i].City + "</td><td>" +
                            "<input value='" + ToJavaScriptDateMMM(hotelData[i].CheckInDate.toString()) + "' type='text' id='txtchindate" + i + "'  class='inputremark form-control fh_inputtxt2 ' style='cursor: pointer;'>" + "</td><td>"
                            + "<input type='text' id='txtchoutdate" + i + "' value='" + ToJavaScriptDateMMM(hotelData[i].CheckOutDate.toString()) + "'  class='inputremark form-control fh_inputtxt2 ' style='cursor: pointer;'>" + "</td>";
                        trhotel += "<td>" + hotelData[i].IsAllocated + "</td>";
                        trhotel += "<td>" + "Hotel Confirmation No.: " + hotelData[i].HotelConfirmationNo + "<br />" +
                            "Name: " + hotelData[i].HotelName + "<br />" +
                            "Address: " + hotelData[i].HotelAddress + "<br />" +
                            "Phone: " + hotelData[i].HotelPhoneNo + "<br />" +
                            "</td>";
                        trhotel += "<td>" + hotelData[i].Remarks_Status + "</td>";
                        trhotel += "</tr>"


                        trhotel += "<tr id='hotelCancelRow" + i + "'>";
                        trhotel += "<td colspan='3'>" + "Hotel Cancellation: " + "</td>";
                        if (hotelData[i].usercancellation == "Cancelled by User") {
                            trhotel += "<td colspan='3'>" + "Cancelled" + "&nbsp;&nbsp;<button type='button' class='btn btn-warning btn-sm' id='UndocancellationButton' onclick='UndoCancelledRequest(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + ")'>Undo Cancellation</button>" + "</td>";
                        } else if (hotelData[i].usercancellation == "Cancelled") {
                            trhotel += "<td colspan='3'>" + "Cancelled by Traveldesk" + "&nbsp;&nbsp;<button type='button' class='btn btn-warning btn-sm' id='UndocancellationButton' onclick='UndoCancelledRequest(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + ")'>Undo Cancellation</button>" + "</td>";
                        }
                        else {
                            trhotel += "<td colspan='2'>" + "<input id='txtCancel" + i + "' type='text' placeholder='Reason for Cancellation' class='form-control input-sm' />" + "</td>";
                            trhotel += "<td colspan='1'>" + "<button type='button' class='btn btn-warning btn-sm' id='cancellationButton' onclick='cancelRequest(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + "," + i + ")'>Cancel Hotel</button>" + "</td>";

                        }
                        trhotel += "</tr>"

                        trhotel += "<tr >";
                        if (hotelData[i].usercancellation == "Cancelled by User" || hotelData[i].usercancellation == "Cancelled") {
                            trhotel += "<td style='text-align:center;' colspan='6' hidden id='editbtntd" + i + "'>" + "<input type='button' value='Edit' class='btn btn-warning btn-sm' onclick='editRow(" + i + ");' style='background-color: #EE1D23; border-color: #EE1D23; width:70px;' />" + "</td>";
                        }
                        else {
                            trhotel += "<td style='text-align:center;' colspan='6' id='editbtntd" + i + "'>" + "<input type='button' value='Edit' class='btn btn-warning btn-sm' onclick='editRow(" + i + ");' style='background-color: #EE1D23; border-color: #EE1D23; width:70px;' />" + "</td>";
                        }
                        trhotel += "<td style='text-align:center;' colspan='6' hidden='hidden' id='updatebtntd" + i + "'>" + "<input type='button' value='Update' class='btn btn-warning btn-sm' onclick='updateRow(" + hotelData[i].TravelRequestId + "," + hotelData[i].HotelRequestId + "," + i + ");' style='background-color: #EE1D23; border-color: #EE1D23;' />" + "</td>";
                        trhotel += "</tr></table></div>"

                        $('#dvPlaceToVisit').append(trhotel);
                        $(function () {
                            $('#txtchindate' + i).datepicker({
                                numberOfMonths: 2,
                                defaultDate: new Date(),
                                minDate: new Date(),
                                maxDate: "+10M +00D",
                                dateFormat: "dd/mm/yy"
                            });
                        });
                        $(function () {
                            $('#txtchoutdate' + i).datepicker({
                                numberOfMonths: 2,
                                defaultDate: new Date(),
                                minDate: new Date(),
                                maxDate: "+10M +00D",
                                dateFormat: "dd/mm/yy"
                            });
                        });


                        if (bookinginfo.BookingStatus == "Rejected") {
                            $("#cancellationButton").prop("disabled", true);
                        }
                    }
                }
            }
            $('#SubmitBtn').empty();;
            trSub += "<div class='row'><div class='col-md-12'><div  style=' text-align:center;'>" +
                "<input class='btn btn-danger' type='button' id='fltHotelSubmit' onclick='SubmitHotelData(" + bookinginfo.TravelRequestId + "," + count + ")' value='Submit'  />" +
                "<button type='button' data-dismiss='modal' class='btn btn-default pull-right'>Close</button></div > " +
                "</div>";
            $('#SubmitBtn').append(trSub);
            //if (bookinginfo.BookingFor != "Only Hotel") {
            //    $('#txtCity' + i).html(sector.split('-')[1]);
            //    $('#txtCity0').val(sector.split('-')[1]);
            //}


            $("#dvHotelBookingDataFill").show();
            $("#dvHotelBookingDataShow").hide();
        }
    } else {
        //hiding div for filled hotel details.
        $("#dvHotelBookingDataFill").hide();
        $("#dvHotelBookingDataShow").show();

    }
}

function clickcheckboxhotel(i) {
    if ($("#checkboxhotel" + i).is(':checked')) {
        $("#hoteldivdetails" + i).show();
    } else {
        $("#hoteldivdetails" + i).hide();
    }
}

function editRow(i) {
    $("#txtHeader" + i).hide();
    $("#txtRow" + i).show();
    $("#editbtntd" + i).hide();
    $("#updatebtntd" + i).show();
    $("#hotelCancelRow" + i).hide();
}

function isValidDate(sText) {
    var reDate = /^(([1-9])|([0][1-9])|([1-2][0-9])|([3][0-1]))\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\-\d{4}$/;
    return reDate.test(sText);
}

function updateRow(trid, hid, i) {
    //var checkin = $("#txtchindate" + i).val();
    //var checkout = $("#txtchoutdate" + i).val();
    ////for server
    //var checkin = $("#txtchindate" + i).val();
    //var checkout = $("#txtchoutdate" + i).val();

    //    //for local server
    var checkin = ConvertMMddyyyy($("#txtchindate" + i).val());
    var checkout = ConvertMMddyyyy($("#txtchoutdate" + i).val());


    if (checkin.length <= 0) {
        alert("Please Enter Check-In Date.");
        return;
    }
    if (checkout.length <= 0) {
        alert("Please Enter Check-Out Date.");
        return;
    }
    if (Date.parse(checkin) > Date.parse(checkout)) {
        alert("Check-Out date cannot be before Check-In date");
        $("#txtchoutdate" + i).focus();
        return false;
    }
    //var validateDate1 = $("#txtchindate" + i).val();
    //if (!isValidDateHotel(validateDate1)) {
    //    alert("Invalid date format! Enter Check-In Date in dd/MM/yyyy format. Example: 14/09/2018");
    //    $("#txtCheckInDate" + i).focus();
    //    return;
    //}
    //var validateDate2 = $("#txtchoutdate" + i).val();
    //if (!isValidDateHotel(validateDate2)) {
    //    alert("Invalid date format! Enter Check-Out Date in dd/MM/yyyy format. Example: 14/09/2018");
    //    $("#txtCheckOutDate" + i).focus();
    //    return;
    //}

    var result = confirm("Are you sure to update Check-In Date and Check-Out Date ?");
    if (result) {
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: '../EmployeeBookingDetail/updateHotelDates',
            data: JSON.stringify({ TravelRequestId: trid, hotelid: hid, checkin: checkin, checkout: checkout }),
            beforeSend: function () {
                $('.loader').show();
            },
            complete: function () {
                $('.loader').hide();
            },
            success: function (response) {
                if (response == "Done") {
                    alert("The Check-in and Check-Out Dates have been updated successfully.");
                    location.reload();
                }
                else {
                    alert(response);
                    location.reload();
                }
            },
            error: function (response) {
                alert(response);
                location.reload();
            }
        });
    }
}
//$scope.getList = function () {
//    var isdt = $('#rdodate').is(':checked');
//    var isapprver = $('#rdoapprover').is(':checked');
//    if (isdt) { getdata(1); }
//    if (isapprver) { getdata(2); }

//}

function cancelRequest(trid, hid, i) {
    var reason = $('#txtCancel' + i).val();
    if (reason.length < 1) { alert("Enter Cancellation reason"); return; }
    var result = confirm("Are you sure to cancel this hotel request ?");
    if (result) {
        var tridNew = trid + "|" + hid + "|" + reason + "|SOD";
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: '../EmployeeBookingDetail/cancelHotelRequest',
            data: JSON.stringify({ TravelRequestId: tridNew }),
            beforeSend: function () {
                $('.loader').show();
            },
            complete: function () {
                $('.loader').hide();
            },
            success: function (response) {
                if (response == "Rejected") {
                    alert("The request cannot be cancelled as it is already rejected by the TravelDesk");
                    location.reload();
                }
                else if (response == "Failed") {
                    alert("The request cannot be cancelled as Checkin has been already done.");
                    location.reload();
                }
                else if (response == "CancellationInitiated") {
                    alert("Cancellation process has been initiated successfully.");
                    $('#cancellationButton').hide();
                    location.reload();
                }
                else {
                    alert(response);
                    location.reload();
                }
            },
            error: function (response) {
                //alert(response);
                location.reload();
            }
        });
    }
}

getCityDetail();
var appenddata1 = "";
var appendcity = "";
function getCityDetail() {
    $.ajax({
        method: "GET",
        url: "../city/GetCityListData",
        success: function (data1) {
            for (var i = 0; i < data1.length; i++) {
                appenddata1 += "<option value = '" + data1[i].CityCode + " '>" + data1[i].CityCode + "-" + data1[i].CityName + " </option>";
            }
        },
        error: function (response) {
        }
    });
};

function UndoCancelledRequest(trid, hid) {
    var result = confirm("Are you sure to undo the cancelled hotel request ?");
    if (result) {
        var tridNew = trid + "|" + hid + "|SOD";
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: '../EmployeeBookingDetail/UndoCancelledRequest',
            data: JSON.stringify({ TravelRequestId: tridNew }),
            beforeSend: function () {
                $('.loader').show();
            },
            complete: function () {
                $('.loader').hide();
            },
            success: function (response) {
                alert(response);
                location.reload();
            },
            error: function (response) {
                alert(response);
                location.reload();
            }
        });
    }
}

//Get Table length -- tblviewHotel
function allocateCab(cabData, bookinginfo) {
    $("#tblviewToCab").empty();
    $("#tblviewFromCab").empty();
    var trcab = "";
    if (bookinginfo.SodBookingTypeId == 1) {
        if (cabData.length > 0) {
            //hiding the div for cab data submission and showing div with filled cab details.

            //showing cab booking data
            trcab += '<tr class="popheadcolor" style="font-weight:bold;">';
            trcab += '<td>Pick up location</td><td>Pick Up Timing</td><td>Drop Location</td><td>Drop Timing</td><td>Status</td><td>Remarks</td>';
            trcab += '</tr>';
            $('#tblviewToCab').append(trcab);
            trcab = "";
            cabData[0].IsAllocated = (cabData[0].IsAllocated == '0') ? "Not Allocated" : "Allocated";
            trcab = "<tr>"
            trcab += "<td>" + cabData[0].OneWay_From + "</td><td>" + cabData[0].OneWay_From_Time + "</td><td>" + cabData[0].OneWay_To + "</td>";
            trcab += "<td>" + cabData[0].OneWay_To_Time + "</td>";
            trcab += "<td>" + cabData[0].IsAllocated + "</td>";
            trcab += "<td>" + cabData[0].Remarks_Status + "</td>";
            trcab += "</tr>";
            $('#tblviewToCab').append(trcab);

            trcab = "";
            //populating return journey cab
            trcab += '<tr class="popheadcolor" style="font-weight:bold;">';
            trcab += '<td>Pick up location</td><td>Pick Up Timing</td><td>Drop Location</td><td>Drop Timing</td><td>Status</td><td>Remarks</td>';
            trcab += '</tr>';

            $('#tblviewFromCab').append(trcab);
            trcab = "";
            // cabData[0].IsAllocated = (cabData[0].IsAllocated == '0') ? "Not Allocated" : "Allocated";
            trcab = "<tr>"
            trcab += "<td>" + cabData[0].Return_From + "</td><td>" + cabData[0].Return_From_Time + "</td><td>" + cabData[0].Return_To + "</td>";
            trcab += "<td>" + cabData[0].Return_To_Time + "</td>";
            trcab += "<td>" + cabData[0].IsAllocated + "</td>";
            trcab += "<td>" + cabData[0].Remarks_Status + "</td>";
            trcab += '</tr>';
            $('#tblviewFromCab').append(trcab);
            //trhotel = $('</tr>');
            //$('#tblviewHotel').append(trhotel);
            $("#dvCabBookingDataFill").hide();
            $("#dvCabBookingDataShow").show();
        }
        else {
            //showing the div for cab data submission and hiding div for filled cab details.
            $("#dvCabBookingDataFill").show();
            $("#dvCabBookingDataShow").hide();
        }
    }
}

//Get Table length
function allocateMeal(pscount, passinfo, mealdata, flightData, bookinginfo) {
    $("#tblviewPassInfo").empty();
    var trmeal = "";
    if (bookinginfo.SodBookingTypeId == 1) {
        trmeal = $("<tr class='popheadcolor' style='font-weight:bold;'/>");
        trmeal.append("<th>Traveler Name</th><th>Designation</th><th>Department</th>");
        $('#tblviewPassInfo').append(trmeal);
        trmeal = "";
        trmeal = $('<tr/>');
        trmeal.append("<td>" + bookinginfo.Title + " " + bookinginfo.RequestedEmpName + "</td>");
        trmeal.append("<td>" + bookinginfo.RequestedEmpDesignation + "</td>");
        trmeal.append("<td>" + bookinginfo.RequestedEmpDept + "</td>");
        $('#tblviewPassInfo').append(trmeal);
    }
    else if (bookinginfo.SodBookingTypeId == 2) {
        trmeal = $("<tr class='popheadcolor' style='font-weight:bold;'/>");
        trmeal.append("<th>Traveler Name</th>");
        for (var col = 0; col < flightData.length; col++) {
            trmeal.append("<th>" + flightData[col].OriginPlace + '-' + flightData[col].DestinationPlace + "</th>");
        }
        $('#tblviewPassInfo').append(trmeal);
        trmeal = "";
        var cols = $("#tblviewPassInfo").find("tr:first th");

        for (var i = 1; i <= pscount; i++) {
            trmeal = $('<tr/>');
            trmeal.append("<td>" + passinfo[i - 1].Title + ". " + passinfo[i - 1].TravelerFirstName + " " + passinfo[i - 1].TravelerLastName + "</td>");
            for (var t = 1; t < cols.length; t++) {
                for (var j = 0; j < mealdata.length; j++) {
                    if (cols[t].innerText == mealdata[j].Sector && mealdata[j].PassengerId == i) {
                        trmeal.append("<td>" + mealdata[j].MealType + "</td>");
                    }
                    else if (cols[t].innerText == mealdata[j].Sector && mealdata[j].PassengerId == 0) {
                        trmeal.append("<td>" + mealdata[j].MealType + "</td>");
                    }
                }
            }
            $('#tblviewPassInfo').append(trmeal);
        }
    }
}