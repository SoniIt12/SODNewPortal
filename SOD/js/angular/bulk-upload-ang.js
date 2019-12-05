//Get Bulk Booking Master Data
//********Angular Code *********************************************************************
var app = angular.module('appblk', ['ui.bootstrap.modal']);
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

//Bulk Upload Controller
app.controller("bulkCtrl", function ($scope, $http, $timeout, $modal, $filter) {
    function toggleStart($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $timeout(function () {
            vm.isStartOpen = !vm.isStartOpen;
        });
    }
    $scope.allAppOrRejected = false;
    $scope.validEmployee = false;

    function loaddata(prm) {
        $scope.trlist = [];
        $scope.trlists = [];
        var date = ConvertMMddyyyy($("#txtfromdate").val()) + "," + ConvertMMddyyyy($("#txttodate").val()) + "," + prm;//dev
        //var date = $("#txtfromdate").val() + "," + $("#txttodate").val() + "," + prm; //prod
        $http({
            method: "GET",
            url: "../bulk/GetMasterBulkList?prm=" + date + "&BookingType=bulk"
        }).then(function mySucces(response) {
            if (prm == 1) {
                $scope.trlist = response.data;
                $scope.currentPage = 1;
                $scope.entryLimit = 10;
                $scope.filteredItems = $scope.trlist.length;
                $scope.totalItems = $scope.trlist.length;
            }
            else if (prm == 2) {
                $scope.trlists = response.data;
                $scope.currentPage = 1;
                $scope.entryLimit = 10;
                $scope.filteredItems = $scope.trlists.length;
                $scope.totalItems = $scope.trlists.length;
            }
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

        //Convert date script date formt for display
        $scope.ToJavaScriptDate = function (value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
        };

        //convert MM/dd/yyyy
        $scope.ConvertMMddyyyy = function (dval) {
            var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
            return mval;
        };
    }
    $scope.clearfields = function (dlist, index) {
        if (dlist == 0) {
            $('#addChkIndt').val('');
            $('#addChkOutdt').val('');
            $('#addChkInTime' + index).val('');
            $('#addChkOutTime' + index).val('');
            $('#addAirTrans' + index).val['false'];
        }
    }

    var counter = 1;
    $('#removeButton').hide();
    $scope.addnewDiv = function () {
        $scope.NoOfDiv = [];
        for (var i = 0; i < counter; i++) {
            $scope.NoOfDiv.push({ 'ddlhotelreq': 1 });
            $('#removeButton').show();
        }

        counter++;
        bappright($("#hdnbbr").val());
        $("#btnsendRqtoHOD").prop("hidden", false);
    }
    $scope.removeDiv = function () {
        if (counter == 1) {
            alert("No more records to remove"); l
        }
        if (counter == 2) {
            $('#removeButton').show();
            //$("#btnsendRqtoHOD").prop("hidden", true);
            //$("#btnGeneratePNR").prop("hidden", true);
            $('#btnGeneratePNR').hide();
            $('#btnsendRqtoHOD').hide();
            $('#removeButton').hide();

        }
        counter--;
        $scope.NoOfDiv.splice(counter - 1, 1);
    }


    //View Popup
    $scope.viewdetail = function (TRId, flieStatus) {
        $('input:radio[name=typeDetails][value=bookingDetail]').click();
        $("#blkBookingDetails").prop("hidden", false);
        $("#blkHotelReport").prop("hidden", true);
        $("#btnsendRqtoHOD").hide();
        $scope.NoOfDiv = "";
        counter = 1;
        $http({
            method: "GET",
            url: "../bulk/GetDetailsBulkList?TrId=" + TRId
        }).then(function mySucces(response) {
            $scope.trdlist = response.data;
            for (var i = 0; i < response.data.length; i++) {
                length = response.data[i].CheckInDate.length - 2;
                $scope.trdlist[i].CheckInDate = (response.data[i].CheckInDate).substring(6, length);
                $scope.trdlist[i].CheckOutDate = (response.data[i].CheckOutDate).substring(6, length);
                $scope.trdlist[i].IsBookingcancelled = response.data[i].IsBookingcancelled;
                $scope.trdlist[i].FileSatus = flieStatus;
            }
            $scope.trn = TRId;
        }, function myError(response) {
            alert(response);
        });
    };

    $scope.getangularDate = function (dval) {
        var length = dval.length - 2;
        return new Date(parseInt(dval.substring(6, length)));
    }

    //view popup for hotel Report
    $scope.viewdetailbkList = function (TRId) {
        $("#blkBookingDetails").prop("hidden", true);
        $("#blkHotelReport").prop("hidden", false);
        $http({
            method: "GET",
            url: "../billing/GetHotelBulkBillingList?prm=" + TRId
        }).then(function mySucces(response) {
            $scope.trlistb = response.data;
            $scope.trId = TRId;
            // console.log(TRId);
        }, function myError(response) {
        });
    }

    $scope.downloadexcelBlist = function (prm) {
        window.location = '../billing/ExportBulkListFromTsv?prm=' + prm;
    };


    $scope.IsVisible = false;
    $scope.Visible = true;
    $scope.EditDetailList = function (id) {
        var hotelrequired = $("#ishHtelReq_" + id).val();
        $("#edit_employee_" + id).prop("hidden", false);
        $("#noEdit_EmpCode_" + id).prop("hidden", true);
        $("#btnupdate_" + id).prop("hidden", false);
        $("#updateCancel_" + id).prop("hidden", false);
        $("#editBtn_" + id).prop("hidden", true);
        $('#edit_employee_' + id).addClass('poptextEdit');
        if (hotelrequired == "Yes") {
            $("#edit_checkinDate_" + id).prop("hidden", false);
            $("#noEdit_checkinDate_" + id).prop("hidden", true);
            $("#edit_checkoutDate_" + id).prop("hidden", false);
            $("#noEdit_checkoutDate_" + id).prop("hidden", true);
            $('#edit_checkinDate_' + id).addClass('poptextEdit');
            $('#edit_checkoutDate_' + id).addClass('poptextEdit');
        }
        // $("#btnupdate_" + id).addClass("disabled");
        //setTimeout(function () {
        //    $('#edit_employee_' + id).addClass('poptextEdit');           
        //}, 100)
    }
    $scope.fetchdetails = function (id, trn) {
        $('#edit_employee_' + id).addClass('poptextEdit');
        var newECode = $("#edtEmpcode_" + id).val();
        var trid = trn;
        if (newECode.length <= 0) {
            alert("Employee Code must be Required");
            return false;
        }
        if (newECode.length >= 6) {
            var result = confirm("Are you sure to change employee code ?");
            if (result) {
                $http({
                    method: "POST",
                    url: "../bulk/fetchdetails",
                    data: JSON.stringify({ newECode: newECode, trid: trid }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    // console.log(response);
                    if (response.data.length > 0) {
                        if (response.data == "Exist") {
                            alert('This Employee code is already in the booking list.');

                            //$("#edtEmpcode_" + id).focus();
                            return false;
                        }
                        else {
                            $scope.trdlist[id - 1].FirstName = response.data[0].FirstName;
                            $scope.trdlist[id - 1].LastName = response.data[0].LastName;
                            if (response.data[0].Gender == "M") {
                                $scope.trdlist[id - 1].Title = "Mr";
                            }
                            else {
                                $scope.trdlist[id - 1].Title = "Ms";
                                $scope.trdlist[id - 1].MobileNo = response.data[0].Phone;
                            }
                            //return $scope.disableLink = false; 

                            $("#btnupdate_" + id).removeClass("disabled");
                            $("#edit_employee_" + id).removeClass("poptextEdit");

                        }
                    }
                    else {
                        alert('EmpCode is not valid.');

                        $("#btnupdate_" + id).addClass("disabled");
                    }


                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                });
            }
        }
        else {
            alert('Invalid Employee code');
            $("#btnupdate_" + id).addClass("disabled");
        }
    }
    $scope.fetchdetails_edit = function (id, trn) {
        var newECode = $("#addEmpCode" + id).val();
        var trid = trn;
        var Title = '';
        if (newECode.length <= 0) {
            alert("Employee Code Should be Required");
            $scope.validEmployee = false;
            return;
        }
        if (newECode.length >= 6) {
            var result = confirm("Are you sure to add the employee code :-" + newECode);
            if (result) {
                $http({
                    method: "POST",
                    url: "../bulk/fetchdetails",
                    data: JSON.stringify({ newECode, trid }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    // console.log(response);
                    if (response.data.length > 0) {
                        if (response.data == "Exist") {
                            alert('This Employee code is already in the booking list.');
                            //$("#addEmpCode" + id).focus();
                            $scope.validEmployee = false;
                            return false;
                        }
                        else {
                            var FirstName = response.data[0].FirstName;
                            var LastName = response.data[0].LastName;
                            if (response.data[0].Gender == "M") {
                                Title = "Mr";
                            }
                            else {
                                Title = "Ms";
                            }
                            var Fullname = Title + ' ' + response.data[0].FirstName + ' ' + response.data[0].LastName;
                            $scope.validEmployee = true;
                            $('#addEmpName' + id).val(Fullname);
                        }
                    }
                    else {
                        $scope.validEmployee = false;
                        alert('EmpCode is not valid.');
                    }
                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                });
            }
        }
        else {
            alert('Invalid Employee code');
        }
    }

    // $scope.cancelledBooking = false;
    $scope.bookingCancel = function (newRow, trid, id) {
        if ($scope.trdlist[id - 1].IsBookingcancelled) {
            alert("This booking is already cancelled");
            return false;
        }
        var result = confirm("Are you sure to cancel this Booking Request ?");
        if (result) {
            var empcode = newRow.EmpCode;
            var ReasonForCancellation = "cancel by Spoc";
            $http({
                method: "POST",
                url: "../bulk/CancelBookingRequest",
                data: JSON.stringify({ empcode, trid, ReasonForCancellation }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                // console.log(response.data);
                if (response.data[0] != 0) {
                    //$scope.cancelledBooking = true;
                    //$("#cancelledBooking" + id).prop("hidden", false);
                    $scope.trdlist[id - 1].IsBookingcancelled = true;
                    alert('this booking is cancelled');
                    loaddata();
                }

            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    }

    $scope.updateRow = function (newRow, trId, id) {
        var result = confirm("Are you sure to update hotel list ?");
        if (result) {
            var oldEcode = newRow.EmpCode;
            var upDatedrow = new Array();
            var obj = new Object();
            obj.EmpCode = $("#edtEmpcode_" + id).val();
            obj.Sector = newRow.Sector;
            var travelDate = new Date(ConvertMMddyyyy(newRow.CheckInDate));
            var hotelrequired = $("#ishHtelReq_" + id).val();
            var checkInDate = "";
            var a = "";
            var checkDate = "";
            if (hotelrequired == "Yes") {
                if (newRow.CheckInDate.length > 10) {
                    checkInDate = new Date(parseInt(newRow.CheckInDate));
                    if (checkInDate < travelDate) {
                        alert('Check-In Date should be greater or equal to Travel Date ');
                        return false;
                    }
                    //obj.CheckInDate = ConvertMMddyyyy(new Date(parseInt(newRow.CheckInDate)).toLocaleDateString());
                    obj.CheckInDate = new Date(parseInt(newRow.CheckInDate)).toLocaleDateString();
                }
                else {
                    checkInDate = new Date(ConvertMMddyyyy(newRow.CheckInDate));
                    if (checkInDate < travelDate) {
                        alert('Check-In Date should be greater or equal to Travel Date ');
                        return false;
                    }
                    //obj.CheckInDate = newRow.CheckInDate;
                    obj.CheckInDate = ConvertMMddyyyy(newRow.CheckInDate);
                }

                if (newRow.CheckOutDate.length > 10) {
                    checkOutDate = new Date(parseInt(newRow.CheckOutDate));
                    if (checkOutDate < checkInDate) {
                        alert('Check-Out Date should be greater or equal to Check-In Date ');
                        return false;
                    }
                    //obj.CheckOutDate = ConvertMMddyyyy(new Date(parseInt(newRow.CheckOutDate)).toLocaleDateString());
                    obj.CheckOutDate = new Date(parseInt(newRow.CheckOutDate)).toLocaleDateString();
                }
                else {
                    checkOutDate = new Date(ConvertMMddyyyy(newRow.CheckOutDate));
                    if (checkOutDate < checkInDate) {
                        alert('Check-Out Date should be greater or equal to Check-In Date ');
                        return false;
                    }
                    obj.CheckOutDate = ConvertMMddyyyy(newRow.CheckOutDate);
                }
                checkDate = new Date(checkInDate);
                checkDate = checkDate.setDate(checkDate.getDate() + 30);
                //checkDate = checkDate.setDate(checkInDate.getDate() + 30);
                var a = new Date(parseInt(checkDate));
                if (checkOutDate > a) {
                    alert("Check-Out Date should be one month of check-in date");
                    return false;
                }
                //if (newRow.CheckOutDate.length <= 10) {
                //    checkOutDate = new Date(ConvertMMddyyyy(newRow.CheckOutDate));
                //    checkdate = new Date(ConvertMMddyyyy(newRow.CheckInDate));

                //    obj.CheckOutDate = ConvertMMddyyyy(newRow.CheckOutDate);
                //}
                //else {

                //    checkOutDate = new Date(parseInt(newRow.CheckOutDate));
                //    checkDate = new Date(parseInt(newRow.CheckInDate));
                //    checkDate = checkDate.setDate(checkDate.getDate() + 30);
                //    var a = new Date(parseInt(checkDate));
                //    if (checkOutDate > a) {
                //        alert("Check-Out Date should be one month of check-in date");
                //        return false;
                //    }
                //    obj.CheckOutDate = new Date(parseInt(newRow.CheckOutDate)).toLocaleDateString();
                //}
            }
            obj.FirstName = newRow.FirstName;
            obj.LastName = newRow.LastName;
            obj.CheckinTime = newRow.CheckinTime;
            obj.FlightNo = newRow.FlightNo;
            obj.TrnId = trId;
            upDatedrow.push(obj);
            var trid = trId;

            $http({
                method: "POST",
                url: "../bulk/UpdateBulkDetail",
                data: JSON.stringify({ oldEcode, upDatedrow: upDatedrow, trid }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                if (response.data[0] == "0") {
                    alert('You have not changed any data.');
                }
                else if (response.data[0] == "-1") {
                    alert('EmpCode is not valid.');
                }
                else {
                    alert('Data is updated successfully.');
                    $("#edit_employee_" + id).prop("hidden", true);
                    $("#noEdit_EmpCode_" + id).prop("hidden", false);
                    $("#btnupdate_" + id).prop("hidden", true);
                    $("#editBtn_" + id).prop("hidden", false);
                    $("#updateCancel_" + id).prop("hidden", true);
                    $("#edit_checkinDate_" + id).prop("hidden", true);
                    $("#noEdit_checkinDate_" + id).prop("hidden", false);
                    $("#edit_checkoutDate_" + id).prop("hidden", true);
                    $("#noEdit_checkoutDate_" + id).prop("hidden", false);
                }

                loaddata();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    };

    $scope.cancelUpdate = function (newRow, trId, id) {
        $("#edit_employee_" + id).prop("hidden", true);
        $("#noEdit_EmpCode_" + id).prop("hidden", false);
        $("#edit_checkinDate_" + id).prop("hidden", true);
        $("#noEdit_checkinDate_" + id).prop("hidden", false);
        $("#edit_checkoutDate_" + id).prop("hidden", true);
        $("#noEdit_checkoutDate_" + id).prop("hidden", false);
        $("#btnupdate_" + id).prop("hidden", true);
        $("#updateCancel_" + id).prop("hidden", true);
        $("#editBtn_" + id).prop("hidden", false);
        $('#edit_employee_' + id).removeClass('poptextEdit');
        $('#edit_checkinDate_' + id).removeClass('poptextEdit');
        $('#edit_checkoutDate_' + id).removeClass('poptextEdit');

        // $("#btnupdate_" + id).removeClass("disabled");
    }
    $scope.GetHotelDetails = function (ecode, trn) {
        $http({
            method: "POST",
            url: "../bulk/GetHotelDetail",
            data: JSON.stringify({ EmpCode: ecode, TravelRequestId: trn }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $scope.Hdetails = response.data[0];
        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });

    }

    $scope.sendReqtoHOD_edit = function () {

        var blist = new Array();
        var blistdetails = new Array();
        var counter = 1;
        var i = 0;
        var slist = $("#tblbookinginfo input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var obj = new Object();
                    obj.EmpCode = $(this).closest('tr').find('td input')[0].value;;
                    obj.PNRStatus = "";
                    obj.BTrId = $('#trn').val();
                    blist.push(obj);

                    var obj1 = new Object();
                    obj1.EmpCode = $(this).closest('tr').find('td input')[0].value;
                    //obj1.EmpName = $(this).closest('tr').find('td input')[2].value;;
                    obj1.TravelDate = ConvertMMddyyyy($(this).closest('tr').find('td input')[2].value);
                    obj1.Sector = $(this).closest('tr').find('td input')[3].value;
                    obj1.FlightNo = $(this).closest('tr').find('td input')[4].value;
                    obj1.PNR = '';
                    obj1.Meal = $(this).closest('tr').find('td select')[0].value;
                    obj1.Beverage = $(this).closest('tr').find('td select')[1].value;
                    obj1.IsHotelRequired = $(this).closest('tr').find('td select')[2].value == 1 ? "true" : "false";
                    obj1.CheckInDate = obj1.IsHotelRequired == "false" ? "01/01/1990" : ConvertMMddyyyy($(this).closest('tr').find('td input')[6].value);//$(this).closest('tr').find('td input')[6].value;//pro// 
                    obj1.CheckOutDate = obj1.IsHotelRequired == "false" ? "01/01/1990" : ConvertMMddyyyy($(this).closest('tr').find('td input')[7].value);//$(this).closest('tr').find('td input')[7].value;//pro//
                    obj1.CheckinTime = $(this).closest('tr').find('td input')[8].value;
                    obj1.CheckoutTime = $(this).closest('tr').find('td input')[9].value;
                    obj1.AirportTransport = $(this).closest('tr').find('td select')[3].value;
                    obj1.BookingType = $(this).closest('tr').find('td select')[4].value;
                    obj1.AgencyCode = $(this).closest('tr').find('td input')[10].value;
                    obj1.TrnId = $('#trn').val();
                    obj1.HotelCity = $(this).closest('tr').find('td input')[3].value.substring('4').trim();
                    obj1.PNRStatus = "";
                    blistdetails.push(obj1);
                    i++;
                }
            }
            counter++;
        });

        if (i == 0) {
            alert('Please check the row.');
            return;
        }
        for (var i = 0; i < blistdetails.length; i++) {
            if (blistdetails[i].EmpCode == "") {
                alert("Please Enter the Employee Code");
                return;
            }
            if (blistdetails[i].TravelDate == "") {
                alert("Please Enter the Travel Date");
                return;
            }
            if (blistdetails[i].Sector == "") {
                alert("Please Enter sector");
            }
            if (blistdetails[i].IsHotelRequired != 'false') {
                if (blistdetails[i].CheckInDate == "" || blistdetails[i].CheckInDate == "undefined//undefined") {
                    alert("Please Enter the Check-In Date");
                    return;
                }
            }
            if (blistdetails[i].IsHotelRequired != 'false') {
                if ((blistdetails[i].CheckOutDate == "" || blistdetails[i].CheckOutDate == "undefined//undefined")) {
                    alert("Please Enter the Check-Out Date");
                    return;
                }
            }
            if (blistdetails[i].FlightNo == "") {
                alert("Please Enter the Flight No");
                return;
            }
            if (blistdetails[i].IsHotelRequired != 'false') {
                if (new Date((blistdetails[i].CheckInDate)) < new Date((blistdetails[i].TravelDate))) {
                    alert('Check-In Date should be greater or equal to Travel Date ');
                    return;
                }
            }
            if (blistdetails[i].IsHotelRequired != 'false') {
                if (new Date((blistdetails[i].CheckOutDate)) < new Date((blistdetails[i].CheckInDate))) {
                    alert('Check-Out Date should be greater or equal to Check-In Date ');
                    return;
                }
            }
            if (blistdetails[i].IsHotelRequired != 'false') {
                var checkdate = new Date(blistdetails[i].CheckInDate);
                checkdate = checkdate.setDate(checkdate.getDate() + 30);
                var a = new Date(checkdate);
                if (new Date(blistdetails[i].CheckOutDate) > a) {
                    alert('Check-Out Date should be less than 30 days to Check-In Date ');
                    return;
                }
            }
            if (blistdetails[i].IsHotelRequired != 'false') {
                if (new Date(blistdetails[i].CheckoutTime) == "") {
                    alert('please enter check-In Time');
                    return;
                }
            }
            if (blistdetails[i].IsHotelRequired != 'false') {
                if (blistdetails[i].CheckinTime == "") {
                    alert('please enter check-In Time');
                    return;
                }
            }
        }

        var blist = JSON.stringify({ blist: blist, DetalsList: blistdetails });
        $scope.btnDisable = true;
        $http({
            method: "POST",
            url: '../bulk/sendReqtoHOD_edit',
            processData: false,
            data: blist,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
        }).then(function mySucces(response) {
            if (response != null || response != '') {
                $('.loader').delay(100).hide(2000);
                alert("Record has been saved successfully & request has been sent to respected Hod For approval");
                $('#viewdetailModal').hide();
                // $("#viewdetailModal").modal("hide")
                $scope.viewdetail($scope.trn);
                $('#viewdetailModal').show();
                //setTimeout(function () {
                //    $("#viewdetailModal").modal("show");

                //}, 200)
                for (var i = 0; i < blistdetails.length - 1; i++) {
                    $scope.removeDiv();
                }
                $scope.btnDisable = false;
            }

            else {
                $("#loaderMsg")[0].innerHTML = "SOD Bulk Booking Request has been sent already ..";
                $('.loader').delay(1000).hide(2000);
                $scope.btnDisable = false;
                return;
            }
        }, function myError(response) {
            alert('OOPs! Something Went Wrong');
            $scope.btnDisable = false;

        });
    }

    $scope.GeneratePnrRoleHod = function () {
        var blist = new Array();
        var blistdetails = new Array();
        var counter = 1;
        var i = 0;
        var slist = $("#tblbookinginfo input[type=checkbox]");
        slist.each(function () {
            if (counter > 0) {
                if ($(this).is(':checked')) {
                    var obj = new Object();
                    obj.EmpCode = $(this).closest('tr').find('td input')[0].value;;
                    obj.PNRStatus = "";
                    obj.BTrId = $('#trn').val();
                    blist.push(obj);

                    var obj1 = new Object();
                    obj1.EmpCode = $(this).closest('tr').find('td input')[0].value;
                    //obj1.EmpName = $(this).closest('tr').find('td input')[2].value;;
                    obj1.TravelDate = $(this).closest('tr').find('td input')[2].value;
                    obj1.Sector = $(this).closest('tr').find('td input')[3].value;
                    obj1.FlightNo = $(this).closest('tr').find('td input')[4].value;
                    obj1.PNR = '';
                    obj1.Meal = $(this).closest('tr').find('td select')[0].value;
                    obj1.Beverage = $(this).closest('tr').find('td select')[1].value;
                    obj1.IsHotelRequired = $(this).closest('tr').find('td select')[2].value;
                    obj1.CheckInDate = $(this).closest('tr').find('td input')[6].value;//ConvertMMddyyyy($(this).closest('tr').find('td input')[6].value);
                    obj1.CheckOutDate = $(this).closest('tr').find('td input')[7].value;//ConvertMMddyyyy($(this).closest('tr').find('td input')[7].value);
                    obj1.CheckinTime = $(this).closest('tr').find('td input')[8].value;
                    obj1.CheckoutTime = $(this).closest('tr').find('td input')[9].value;
                    obj1.AirportTransport = $(this).closest('tr').find('td select')[3].value;
                    obj1.BookingType = $(this).closest('tr').find('td select')[4].value;
                    obj1.AgencyCode = $(this).closest('tr').find('td input')[10].value;
                    obj1.TrnId = $('#trn').val();
                    obj1.HotelCity = $(this).closest('tr').find('td input')[3].value.substring('4').trim();
                    obj1.PNRStatus = "";
                    blistdetails.push(obj1);
                    i++;
                }
            }
            counter++;
        });

        if (i == 0) {
            alert('Please check the row.');
            return;
        }
        for (var i = 0; i < blistdetails.length; i++) {
            if (blistdetails[i].EmpCode == "") {
                alert("Please Enter the Employee Code");
                return;
            }
            if (blistdetails[i].TravelDate == "") {
                alert("Please Enter the Travel Date");
                return;
            }
            if (blistdetails[i].Sector == "") {
                alert("Please Enter sector");
            }
            if (blistdetails[i].CheckInDate == "" || blistdetails[i].CheckInDate == "undefined//undefined") {
                alert("Please Enter the Check-In Date");
                return;
            }
            if (blistdetails[i].CheckOutDate == "" || blistdetails[i].CheckOutDate == "undefined//undefined") {
                alert("Please Enter the Check-Out Date");
                return;
            }
            if (blistdetails[i].FlightNo == "") {
                alert("Please Enter the Flight No");
                return;
            }
            if (new Date(blistdetails[i].CheckInDate) < new Date(ConvertMMddyyyy(blistdetails[i].TravelDate))) {
                alert('Check-In Date should be greater or equal to Travel Date ');
                return;
            }
            if (new Date(blistdetails[i].CheckOutDate) < new Date(blistdetails[i].CheckInDate)) {
                alert('Check-Out Date should be greater or equal to Check-In Date ');
                return;
            }
            if (new Date(blistdetails[i].addChkOutTime) == "") {
                alert('please enter check-In Time');
                return;
            }
            if (blistdetails[i].addChkInTime == "") {
                alert('please enter check-In Time');
                return;
            }
        }
        var blist = JSON.stringify({ blist: blist, DetalsList: blistdetails });
        $http({
            method: "POST",
            url: '../bulk/GeneratePnrRoleHod',
            processData: false,
            data: blist,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
        }).then(function mySucces(response) {
            if (response.data.length > 0) {
                $('.loader').delay(100).hide(2000);
                var status = true;
                $.each(response.data, function (i, item) {
                    $.each(item, function (key, val) {
                        if (item.PNRStatus.substring(0, 6) == 'ERR001')
                            status = false;
                    });
                });
                if (!status) {
                    alert("Error: PNR Generation has failed.");
                }
                else {
                    alert("PNR has been genrated succesfully");
                }
                $('#viewdetailModal').hide();
                // $("#viewdetailModal").modal("hide")
                $scope.viewdetail($scope.trn);
                $('#viewdetailModal').show();
                for (var i = 0; i < blistdetails.length; i++) {
                    $scope.removeDiv();
                }
                $scope.btnDisable = false;
            }
            else {
                $("#loaderMsg")[0].innerHTML = "SOD Bulk Booking Request has been sent already ..";
                $('.loader').delay(1000).hide(2000);
                $scope.btnDisable = false;
                return false;
            }
        }, function myError(response) {
            alert('OOPs! Something Went Wrong');
            $scope.btnDisable = false;
            return false;

        });
    }

    //download transaction excel details
    $scope.downloadTrans = function (TRId) {
        //alert('Hi' + TRId);
        window.location = "../bulk/ExportexcelTrans?TrId=" + TRId;
    };

    //download PNR wise excel details
    $scope.downloadPNRwise = function (prm) {
        if ($('#rdodate').is(':checked')) prm = 1;
        if ($('#rdopnr').is(':checked')) prm = 2;

        //var data = ConvertMMddyyyy($("#txtfromdate").val()) + "," + ConvertMMddyyyy($("#txttodate").val()) + "," + prm;
        //Prod
        var data = $("#txtfromdate").val() + "," + $("#txttodate").val() + "," + prm;
        window.location = "../bulk/ExportexcelPNRwise?prm=" + data;
    };
    //Hideshow Table
    $scope.hideTbl = function () {
        $scope.trlists = null;
        $scope.trlist = null;
        $scope.currentPage = null;
        $scope.entryLimit = null;
        $scope.filteredItems = null;
        $scope.totalItems = null;
    };

    //Load Data Function
    $scope.getmasterbulkList = function () {
        var isdt = $('#rdodate').is(':checked');
        var ispnr = $('#rdopnr').is(':checked');
        if (isdt) { loaddata(1); }
        if (ispnr) { loaddata(2); }

    }

})
app.directive("datepicker", function () {

    function link(scope, element, attrs) {
        element.datepicker({
            dateFormat: "dd/mm/yy",
            changeYear: true,
            changeMonth: true,
            defaultDate: +0
        });
    }
    return {
        require: 'ngModel',
        link: link
    };
});
app.directive('formattedDate', function (dateFilter) {
    //return {
    //    require: 'ngModel',
    //    scope: {
    //        format: "="
    //    },
    //    link: function (scope, element, attrs, ngModelController) {
    //        ngModelController.$parsers.push(function (data) {
    //            //convert data from view format to model format
    //            return dateFilter(data, scope.format); //converted
    //        });

    //        ngModelController.$formatters.push(function (data) {
    //            //convert data from model format to view format
    //            return dateFilter(data, scope.format); //converted
    //        });
    //    }
    //}
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {

            var dateFormat = attrs['date'] || 'yyyy/MM/dd';

            ctrl.$formatters.unshift(function (modelValue) {
                return dateFilter(modelValue, dateFormat);
            });
        }
    };
});


//Bulk Upload Approval for HOD
app.controller("bulkappCtrl", function ($scope, $http, $timeout, $filter) {

    function loaddata() {
        $http({
            method: "GET",
            url: "../bulk/GetBulkApprovalMasterList"
        }).then(function mySucces(response) {
            $scope.applist = response.data;
            $scope.currentPage = 1;
            $scope.entryLimit = 10;
            //$scope.totalItems = $scope.applist;
            $scope.filteredItems = $scope.applist.length;
            $scope.totalItems = $scope.applist.length;

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

        //Convert date script date formt for display
        $scope.ToJavaScriptDate = function (value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
        };

        //convert MM/dd/yyyy
        $scope.ConvertMMddyyyy = function (dval) {
            var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
            return mval;
        };

        //View Bulk Booking List for HOD Approval
        $scope.viewHODAppList = function (TRId) {
            $http({
                method: "GET",
                url: "../bulk/GetDetailsBulkList?TrId=" + TRId
            }).then(function mySucces(response) {
                $scope.trdlist = response.data;
                for (var i = 0; i < $scope.trdlist.length; i++) {
                    if (($scope.trdlist[i].BookingType == 'Standby' || $scope.trdlist[i].BookingType == 'standby') && ($scope.trdlist[i].PNR_Status == 0)) {
                        $scope.allAppOrRejected = true;
                    }
                }
                $scope.trn = TRId;
            }, function myError(response) {
                alert(response);
            });
        }


        //Check All
        $scope.checkAll = function () {
            if (!$scope.selectedAll) {
                $scope.selectedAll = true;
            } else {
                $scope.selectedAll = false;
            }
            angular.forEach($scope.trdlist, function (dlist) {
                dlist.Selected = $scope.selectedAll;
            });
        };

        //Reject Bulk Booking List by  HOD : Not Selective Option
        $scope.rejectAppList = function (TRId) {
            var s = confirm("Are you sure to rejecte the bulk booking request ? ")
            if (s) {
                $http({
                    method: "GET",
                    url: "../bulk/RejectBulkBookingNotSelective?trId=" + TRId
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

        $scope.AcceptAppList = function (TRId) {
            var ok = confirm('Are you sure to approve this request ?');
            if (ok) {
                $http({
                    method: "GET",
                    url: '../bulk/ApproveToGeneratePNR?trid=' + TRId,
                }).then(function mySucces(response) {
                    if (response.data.length == 0) {
                        alert("This request is successfully approved.");
                        location.reload();
                        //updatePNRSelectiveStatus(response);
                        loaddata();//suggested change Rejected Status--for the current row --get current index of the row
                    }
                }, function myError(response) {
                    alert(response);
                });
            }
        }
    }
    loaddata();
});

//Send Rejection Email Notification to User :Non Selective
sendRejectionEmailNotification = function () {
    $.ajax({
        url: "../bulk/sendEmailNotification",
        success: function (result) {
        }
    });
}


//close popup and refresh page
function closeRefresh() {
    location.reload();
}

function xRefresh() {
    location.reload();
}

//reject PnrList
function RejectAppSelective() {
    var counter = 0;
    var i = 0;
    var elists = new Array();
    var slist = $("#tblbookinginfo input[type=checkbox]");
    slist.each(function () {
        if (counter > 0) {
            if ($(this).is(':checked')) {
                var obj = new Object();
                obj.BTrId = $(this).closest('tr').find('td')[1].innerText;
                obj.AddNo = $(this).closest('tr').find('td')[2].innerText;
                obj.EmpCode = $(this).closest('tr').find('td')[3].innerText;
                elists.push(obj);
                i++;
            }
        }
        counter++;
    });
    var BTrId = $('#hdTrid').val();

    if (i == 0) {
        alert('Please check the row.');
        return;
    }
    var vlist = JSON.stringify({ elist: elists, travelReqId: BTrId });
    $.ajax({
        url: '../bulk/RejectBulkBookingSelective',
        type: "Post",
        processData: false,
        data: vlist,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        complete: function () {
            $("#divloaderMsg")[0].innerHTML = $("#divloaderMsg")[0].innerHTML + " PNR generation process has been completed....";
            $('#popupLoader').delay(100).hide(5000);
            $('#viewdetailModal').delay(200).hide(5000);
            closeRefresh();
        },
        success: function (response) {
            if (response != null || response != '') {
                updatePNRSelectiveStatus(response);
                sendRejectionEmailNotification();
                $("#loaderMsg")[0].innerHTML = " PNR generation process has been done.";
            }
            else {
                $("#loaderMsg")[0].innerHTML = "PNR has been already generated ...";
                $('#popupLoader').delay(100).hide(5000);
                return;
            }
        },
        error: function (er) {
            $("#divloaderMsg")[0].innerHTML = "";
            $("#divloaderMsg")[0].innerHTML = "Error :PNR Generation has failed....";
            $('#popupLoader').delay(100).hide(5000);
        }
    });
}
//Generate PNR selective option function
function generatePNRSelective() {
    var counter = 0;
    var i = 0;
    var elists = new Array();
    var slist = $("#tblbookinginfo input[type=checkbox]");
    slist.each(function () {
        if (counter > 0) {
            if ($(this).is(':checked')) {
                var obj = new Object();
                obj.BTrId = $(this).closest('tr').find('td')[1].innerText;
                obj.AddNo = $(this).closest('tr').find('td')[2].innerText;
                obj.EmpCode = $(this).closest('tr').find('td')[3].innerText;
                elists.push(obj);
                i++;
            }
        }
        counter++;
    });
    var BTrId = $('#hdTrid').val();

    if (i == 0) {
        alert('Please check the row.');
        return;
    }
    var vlist = JSON.stringify({ elist: elists, travelReqId: BTrId });
    $.ajax({
        url: '../bulk/ApproveToGeneratePNR_Selective',
        type: "POST",
        processData: false,
        data: vlist,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            $("#divloaderMsg")[0].innerHTML = "";
            $("#divloaderMsg")[0].innerHTML = "Generating PNR....";
            $('#popupLoader').show();
        },
        complete: function () {
            $("#divloaderMsg")[0].innerHTML = $("#divloaderMsg")[0].innerHTML + " PNR generation process has been completed....";
            $('#popupLoader').delay(100).hide(5000);
            $('#viewdetailModal').delay(200).hide(5000);
            closeRefresh();
        },
        success: function (response) {
            if (response != null || response != '') {
                updatePNRSelectiveStatus(response);
                $("#loaderMsg")[0].innerHTML = " PNR generation process has been done.";
            }
            else {
                $("#loaderMsg")[0].innerHTML = "PNR has been already generated ...";
                $('#popupLoader').delay(100).hide(5000);
                return;
            }
        },
        error: function (er) {
            $("#divloaderMsg")[0].innerHTML = "";
            $("#divloaderMsg")[0].innerHTML = "Error :PNR Generation has failed....";
            $('#popupLoader').delay(100).hide(5000);
        }
    });
}

//For Not Selective Case
function generatePNR() {
    var ok = confirm('Are you sure to approve this request ?');
    if (ok) {
        $.ajax({
            url: '../bulk/ApproveToGeneratePNR?trid=' + $("#hdTrid").val(),
            type: "GET",
            processData: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                $("#divloaderMsg")[0].innerHTML = "";
                $("#divloaderMsg")[0].innerHTML = "Generating PNR....";
                $('#popupLoader').show();
            },
            complete: function () {
                $("#divloaderMsg")[0].innerHTML = $("#divloaderMsg")[0].innerHTML + " PNR generation process has been completed....";
                $('#popupLoader').delay(100).hide(5000);
                $('#viewdetailModal').delay(200).hide(5000);
                closeRefresh();
            },
            success: function (response) {
                if (response != null || response != '') {
                    updatePNRStatus(response);
                    $("#loaderMsg")[0].innerHTML = " PNR generation process has been done.";
                }
                else {
                    $("#loaderMsg")[0].innerHTML = "PNR has been already generated ...";
                    $('#popupLoader').delay(100).hide(5000);
                    return;
                }
            },
            error: function (er) {
                $("#divloaderMsg")[0].innerHTML = "";
                $("#divloaderMsg")[0].innerHTML = "Error :PNR Generation has failed....";
                $('#popupLoader').delay(100).hide(5000);
            }
        });
    }
}
//Update PNR Status
function updatePNRStatus(response) {
    $('#chkheader').attr('disabled', 'disabled');
    for (var i = 0; i < response.length; i++) {
        var counter = 0;
        $('#tblbookinginfo tr').each(function () {
            if (counter > 0) {
                var eid = $(this).closest('tr').find('td')[1].innerText;
                var chkbox = $(this).closest('tr').find('input:checkbox').eq(0);
                // if (chkbox) {
                if (eid == response[i].EmpCode && response[i].PNRStatus.split('|')[0].trim() != "ERR001") {
                    var th = $(this);
                    th.find("#spnr").replaceWith(response[i].PNRStatus.split('|')[0] + " " + "<img  src='../img/right.png' style='margin-bottom:4px;margin-left:4px;width:15px;height:15px;' alt='PNR has been generated successfully.' />");
                    $(this).closest('tr').find('input:checkbox').attr('disabled', 'disabled');
                    return;
                }
                else if (eid == response[i].EmpCode && response[i].PNRStatus.split('|')[0].trim() == "ERR001") {
                    var th = $(this);
                    th.find("#spnr").replaceWith(response[i].PNRStatus.split('|')[0] + " " + "<img  src='../img/rejected1.png' style='margin-bottom:4px;margin-left:4px;width:15px;height:15px;' alt='PNR ERROR' />");
                    $(this).closest('tr').find('input:checkbox').attr('disabled', 'disabled');
                    return;
                }
                //}
            }
            counter++;
        });
    }
}

function updatePNRSelectiveStatus(response) {
    var counter = 0;
    var slist = $("#tblbookinginfo input[type=checkbox]");
    slist.each(function () {
        if (counter > 0) {
            for (var i = 0; i < response.length; i++) {
                if ($(this).is(':checked')) {
                    var eid = $(this).closest('tr').find('td')[1].innerText;

                    // if (chkbox) {
                    if (eid == response[i].EmpCode && response[i].PNRStatus.split('|')[0].trim() != "ERR001") {
                        $(this).closest('tr').find('input:checkbox').replaceWith("Approved" + " " + "<img  src='../img/right.png' style='margin-bottom:4px;margin-left:4px;width:15px;height:15px;' alt='PNR has been generated successfully.' />");
                        return;
                    }
                    else if (eid == response[i].EmpCode && response[i].PNRStatus.split('|')[0].trim() == "ERR001") {
                        $(this).closest('tr').find('input:checkbox').replaceWith("Approved" + " " + "<img  src='../img/rejected1.png' style='margin-bottom:4px;margin-left:4px;width:15px;height:15px;' alt='PNR has been generated successfully.' />");
                        return;
                    }
                }
            }
        }
        counter++;
    });
}

function changeDateFormat(inputDate) {  // expects Y-m-d
    var splitDate = inputDate.split('/');
    if (splitDate.count == 0) {
        return null;
    }
    return splitDate[2] + '/' + splitDate[1] + '/' + splitDate[0];
}

function saveRow() {
    var blist = new Array();
    var blistdetails = new Array();
    var counter = 0;
    var i = 0;
    var slist = $("#tblbookinginfo input[type=checkbox]");
    slist.each(function () {
        if (counter > 0) {
            if ($(this).is(':checked')) {
                var obj = new Object();
                obj.EmpCode = $('#addEmpCode').val();
                obj.PNRStatus = "";
                obj.BTrId = $('#trn').val();
                blists.push(obj);
                i++;
            }
        }
        counter++;
    });

    if (i == 0) {
        alert('Please check the row.');
        return;
    }
    var obj1 = new Object();
    obj1.EmpCode = $('#addEmpCode').val();
    obj1.EmpName = $('#addEmpName').val();
    obj1.TravelDate = changeDateFormat($('#addTrvdate').val());
    obj1.Sector = $('#addsector').val();
    obj1.FlightNo = $('#addflightNo').val();
    obj1.PNR = $('#addPnr').val();
    obj1.Meal = $('#addmeal').children("option:selected").val();
    obj1.Beverage = $('#addBvrg').children("option:selected").val();
    obj1.IsHotelRequired = $('#addhtlreq').children("option:selected").val();
    obj1.CheckInDate = $('#addChkindt').val();
    obj1.CheckOutDate = $('#addChkOutdt').val();
    obj1.BookingType = $('#addBookType').children("option:selected").val();
    obj1.BTrId = $('#trn').val();
    obj1.PNRStatus = "";
    blistdetails.push(obj1);
    var blist = JSON.stringify({ DetalsList: blistdetails });
    $.ajax({
        url: '../bulk/saveBulk_newRow',
        type: 'POST',
        processData: false,
        data: blist,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            alert("Data is saved successfully");
        },
        error: function (er) {
            alert("Data is Not Saved something went wrong");
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
