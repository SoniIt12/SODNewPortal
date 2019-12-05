var app = angular.module('OatViewDetails', ['ui.bootstrap', 'ngSanitize']);
app.filter('startFrom', function () {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    }
});
app.controller("OatTravelDetails", function ($scope, $http, $timeout) {
    $scope.HotelDetail = {};
    $scope.flightInfo = {};
    $scope.forms = {};
    $scope.tableRowExpanded = false;
    $scope.tableRowIndexExpandedCurr = "";
    $scope.tableRowIndexExpandedPrev = "";
    $scope.storeIdExpanded = "";
    $scope.cancelledBooking = false;
    $scope.submitflightDetail = false;
    $scope.submithotelDetail = false;

    $scope.getangularDate = function (dval) {
        return new Date(parseInt(dval.substring(6, 19)));
    }

    $scope.dayDataCollapseFn = function () {
        $scope.dayDataCollapse = [];
        for (var i = 0; i < $scope.OatMasterList.length; i += 1) {
            $scope.dayDataCollapse.push(false);
        }
    };

    $scope.selectTableRow = function (index, oatReqId, trreqId, bookingfor) {
        if (typeof $scope.dayDataCollapse === 'undefined') {
            $scope.dayDataCollapseFn();
        }
        $('.loader').show();
        $http({
            method: "POST",
            url: "../Oat/GetSelectedOatList",
            data: JSON.stringify({ oatReqId: oatReqId, trreqId: trreqId, bookingfor: bookingfor }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('.loader').hide();
            $scope.selectedOatList = response.data;
            $scope.oatreqId = $scope.selectedOatList.length == 0 ? 0 : response.data[0].OATTravelRequestId;
            $scope.edit == false;
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
        if ($scope.tableRowExpanded === false && $scope.tableRowIndexExpandedCurr === "" && $scope.storeIdExpanded === "") {
            $scope.tableRowIndexExpandedPrev = "";
            $scope.tableRowExpanded = true;
            $scope.tableRowIndexExpandedCurr = index;
            $scope.storeIdExpanded = oatReqId;
            $scope.dayDataCollapse[index] = true;
        } else if ($scope.tableRowExpanded === true) {
            if ($scope.tableRowIndexExpandedCurr === index && $scope.storeIdExpanded === oatReqId) {
                $scope.tableRowExpanded = false;
                $scope.tableRowIndexExpandedCurr = "";
                $scope.storeIdExpanded = "";
                $scope.dayDataCollapse[index] = false;
            } else {
                $scope.tableRowIndexExpandedPrev = $scope.tableRowIndexExpandedCurr;
                $scope.tableRowIndexExpandedCurr = index;
                $scope.storeIdExpanded = oatReqId;
                $scope.dayDataCollapse[$scope.tableRowIndexExpandedPrev] = false;
                $scope.dayDataCollapse[$scope.tableRowIndexExpandedCurr] = true;
            }
        }

    };

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };
    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered;
        }, 1);
    };
    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };

    $scope.getMasterOAtList = function () {
        $('.loader').show();
        $http({
            method: "GET",
            url: "../Oat/GetMasterOatList",
        }).then(function mySucces(response) {
            $scope.OatMasterList = response.data;
            $scope.currentPage = 1;
            $scope.entryLimit = 10;
            $scope.filteredItems = $scope.OatMasterList;
            $scope.totalItems = $scope.OatMasterList.length;
            $('.loader').hide();
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }

    //Function for filling the hotel data in model pop up or providing fields for it.
    $scope.viewhoteldetail = function (TravelRequestId, sector) {
        $('.loader').hide();
        $http({
            method: "POST",
            url: "../Oat/GetBookingHotelInfo?trId=" + TravelRequestId
        }).then(function mySucces(response) {
            $scope.hotelData = response.data;
            $scope.trn = $scope.hotelData.length == 0 ? 0 : response.data[0].TravelRequestId;
            if ($scope.hotelData.length != 0) {
                var length = $scope.hotelData[0].CheckInDate.length;
                $scope.hotelData[0].CheckInDate = ($scope.hotelData[0].CheckInDate).substring(6, (length - 2));
                $scope.hotelData[0].CheckOutDate = ($scope.hotelData[0].CheckOutDate).substring(6, (length - 2));
            }
            else {
                $scope.hotelData[0].CheckInDate = "";
                $scope.hotelData[0].CheckOutDate = "";
            }
            $('.loader').hide();
        }, function myError(response) {
            $('.loader').hide();
            //$scope.myWelcome = response.statusText;
        });
    };
    $scope.viewFlightDetails = function (index, oatreqID, ID) {
        $('.loader').show();
        $http({
            method: "POST",
            url: "../Oat/GetFlightOatListasPasID",
            data: JSON.stringify({ 'oatReqID': oatreqID, 'ID': ID }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $scope.FlightDetail = response.data;
            if ($scope.FlightDetail.length != 0) {
                var length = $scope.FlightDetail[0].DepartureDate.length;
                $scope.FlightDetail[0].DepartureDate = ($scope.FlightDetail[0].DepartureDate).substring(6, (length - 2));
            }
            $scope.oatreqId = oatreqID;
            $('.loader').hide();
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }
    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        dataAsFormData.append('RejectionSrc', data);
        return dataAsFormData;
    };
    $scope.hideShowCancelDocumentDiv = function (type) {
        $scope.cancelType = type;
        $scope.openCancelDocumentDiv == 'false' ? $scope.openCancelDocumentDiv = 'true' : $scope.openCancelDocumentDiv = 'false';
    }

    $scope.cancellationRequestToITh = function (PassengerID, originPlace, DestinationPlace) {
        var PassengerID = $("#passengerId").text();
        var cancelSrc = $('#cancellationSrc')[0].files[0];
        var cancelReason = $("#CancelReason").val();
        if (cancelReason == "") {
            alert("Please provide reason for the cancellation. ");
            $("#CancelReason").focus();
            return false;
        }
        var type = $scope.cancelType;
        if (type != "c") {
            if (cancelSrc == undefined) {
                alert("Please select any proof for reason.");
                return;
            }
            if (cancelSrc.size > 1048576) {
                alert("File size should be less than 1 MB.");
                return;
            }
            if ((!cancelSrc.type.includes("image")) && (!cancelSrc.type.includes("pdf"))) {
                alert("File should be Image or PDF.");
                return;
            }
        }
        var bookingDetail = PassengerID + "," + originPlace + "," + DestinationPlace + "," + "By OAT Desk" + "," + $scope.oatreqId;
        //var bookingDetail = PassengerID + "," + originPlace + "," + DestinationPlace + "," + "By User";
        var result = confirm("Are you sure to cancel this request ?");
        if (result) {
            $('.loader').show();
            $http({
                method: "POST",
                url: "../OatDesk/cancellationRequestToIth?cancelReason=" + cancelReason + "&BookingDetail=" + bookingDetail + "&cancelType=" + type,
                data: getModelAsFormData(cancelSrc),
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined, 'Cache-Control': 'no-cache , no-store' }
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert(JSON.parse(response.data));
                window.location.reload();
            }, function myError(response) {
                $('.loader').hide();
                $scope.myWelcome = response.statusText;
            });
        }
    }
    $scope.editRow = function (i) {
        $scope.edit = true;
        setTimeout(function () {
            $('#edtchindate').datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
            $('#edtchOutdate').datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
            $('#fDeptDate').datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });

        }, 200);
    }

    $scope.updateFlightDetail = function (fltDetail) {
        var result = confirm("Are you sure to update flight detail ?");
        var DeptDate = $('#fDeptDate').val();
        if (result) {
           // fltDetail.DepartureDate = $scope.ConvertMMddyyyy(new Date(parseInt(DeptDate)).toLocaleDateString());
            fltDetail.DepartureDate = $scope.ConvertMMddyyyy(DeptDate);
            $http({
                method: "POST",
                url: "../Oat/updateFlightDetail",
                data: JSON.stringify({ 'Detail': fltDetail }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {                
                alert(JSON.parse(response.data));
                $('.loader').hide();
                window.location.reload();
            }, function myError(response) {
                $('.loader').hide();
                $scope.myWelcome = response.statusText;
            });
        }        
    }

    //$("#txtRow" + i).show();
    //$("#editbtntd").hide();
    //$("#updatebtntd").show();
    //$("#hotelCancelRow" + i).hide();


    $scope.sendHoldRequest = function () {
        var ReqID = $scope.oatreqId;
        var holdBy = "by OAT User";
        var result = confirm("Are you sure to hold this request ?");
        if (result) {
            $('.loader').show();
            $http({
                method: "POST",
                url: "../OatDesk/sendHoldRequestToIth",
                data: JSON.stringify({ 'oatReqId': ReqID, 'holdBy': holdBy }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert(JSON.parse(response.data));
                window.location.reload();
            }, function myError(response) {
                $('.loader').hide();
                $scope.myWelcome = response.statusText;
            });
        }
    }

    $scope.undoHoldRequest = function () {
        var ReqID = $scope.oatreqId;
        var holdBy = "by OAT User";
        var result = confirm("Are you sure to unhold this request ?");
        if (result) {
            $('.loader').show();
            $http({
                method: "POST",
                url: "../OatDesk/undoHoldRequest",
                data: JSON.stringify({ 'oatReqId': ReqID, 'holdBy': holdBy }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert(JSON.parse(response.data));
                window.location.reload();
            }, function myError(response) {
                $('.loader').hide();
                $scope.myWelcome = response.statusText;
            });
        }
    }

    //convert MM/dd/yyyy
    $scope.ConvertMMddyyyy = function (dval) {
        var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
        return mval;
    };

    $scope.updateRow = function (newRow) {
        var result = confirm("Are you sure to update hotel list ?");
        var check_inDate = $('#edtchindate').val();
        var check_OutDate = $('#edtchOutdate').val();
        if (result) {
            var oldEcode = newRow.EmpCode;
            var upDatedrow = new Array();
            var obj = new Object();
            obj.Sector = newRow.Sector;
            var checkInDate = "";
            var checkInDate = "";
            if (check_inDate.length > 10) {
                checkInDate = new Date(parseInt(check_inDate));
                obj.CheckInDate = new Date(parseInt(check_inDate)).toLocaleDateString();
                // obj.CheckInDate = $scope.ConvertMMddyyyy(new Date(parseInt(newRow.CheckInDate)).toLocaleDateString());
            }
            else {
                checkInDate = new Date($scope.ConvertMMddyyyy(check_inDate));
                //obj.CheckInDate = newRow.CheckInDate; // for server
                obj.CheckInDate = $scope.ConvertMMddyyyy(check_inDate);
            }

            if (check_OutDate.length > 10) {
                checkOutDate = new Date(parseInt(check_OutDate));
                if (checkOutDate < checkInDate) {
                    alert('Check-Out Date should be greater or equal to Check-In Date ');
                    return;
                }
                obj.CheckOutDate = new Date(parseInt(check_OutDate)).toLocaleDateString();
                //obj.CheckOutDate = $scope.ConvertMMddyyyy(new Date(parseInt(newRow.CheckOutDate)).toLocaleDateString());
            }
            else {
                checkOutDate = new Date($scope.ConvertMMddyyyy(check_OutDate));
                if (checkOutDate < new Date($scope.ConvertMMddyyyy(check_inDate))) {
                    alert('Check-Out Date should be greater or equal to Check-In Date ');
                    return;
                }
                // obj.CheckOutDate = newRow.CheckOutDate; // for server
                obj.CheckOutDate = $scope.ConvertMMddyyyy(check_OutDate);
            }

            obj.EmpCode = newRow.EmpCode;
            obj.TrnId = newRow.TravelRequestId;
            upDatedrow.push(obj);
            var trid = newRow.TravelRequestId;
            $('.loader').show();
            $http({
                method: "POST",
                url: "../bulk/UpdateBulkDetail",
                data: JSON.stringify({ oldEcode, upDatedrow: upDatedrow, trid }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                if (response.data[0] == "0") {
                    alert('EmpCode is not valid.');
                }
                else {
                    alert('Data is updated successfully.');
                }
                $('.loader').hide();
                location.reload();
            }, function myError(response) {
                $('.loader').show();
                alert('Error: Invalid request processing...');
            });
        }
    }
    $scope.viewIthAttachedSrc = function (TrnId, ID, criteria) {
        $('.loader').show();
        $http({
            method: "POST",
            url: "../OatDesk/viewIthAttachedSrc",
            data: JSON.stringify({ 'ID': ID, 'criteria': criteria }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            var url = '../OatDesk/ViewAttachment';
            window.open(url, '_blank');
            $('.loader').hide();
        });
    }

    $scope.bookingCancel = function (hoteldetail) {
        //if ($scope.trdlist[id - 1].IsBookingcancelled) {
        //    alert("This booking is already cancelled");
        //    return false;
        //}
        if (hoteldetail.reasonForCancellaton == undefined) {
            alert("Please enter the reason for cancellation.");
            return false;
        }
        var result = confirm("Are you sure to cancel this Booking Request ?");
        $('.loader').show();
        if (result) {
            var empcode = hoteldetail.EmpCode;
            var ReasonForCancellation = hoteldetail.reasonForCancellaton;
            var trid = hoteldetail.TravelRequestId;
            $http({
                method: "POST",
                url: "../bulk/CancelBookingRequest",
                data: JSON.stringify({ empcode, trid, ReasonForCancellation }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                // console.log(response.data);
                if (response.data[0] != 0) {
                    // $scope.cancelledBooking = true;
                    //$("#cancelledBooking" + id).prop("hidden", false);
                    // $scope.trdlist[id - 1].IsBookingcancelled = true;
                    $('.loader').hide();
                    alert('This booking is Successfully cancelled');
                    $scope.cancelledBooking = true;
                    //loaddata();
                }

            }, function myError(response) {
                $('.loader').show();
                alert('Error: Invalid request processing...');
            });
        }
        else {
            $('.loader').hide();
        }
    }



    //submit Detail as required and generate the request of Flight or Hotel
    $scope.SubmitAllDetails = function (OatrequestNo, type) {

        $scope.hotelDetail = [];
        $scope.flightInfo = [];
        if (type != 'flight') {
            var objhd = new Object();
            if ($('#bookchindate').val() == "") {
                alert("Please enter your check-in date.")
                return false;
            }
            if ($('#bookchOutdate').val() == "") {
                alert("Please enter your check-in date.")
                return false;
            }
            if (new Date($scope.ConvertMMddyyyy($('#bookchindate').val())) > new Date($scope.ConvertMMddyyyy($('#bookchOutdate').val()))) {
                alert("Check-out date should be less than check-in date.")
                return false;
            }
            objhd.HotelCity = $('#BookHotelCityCode').val();
            objhd.CheckInDate = $scope.ConvertMMddyyyy($('#bookchindate').val());
            objhd.CheckOutDate = $scope.ConvertMMddyyyy($('#bookchOutdate').val());
            objhd.CheckinTime = $('#ch-InTimePicker').val();
            objhd.CheckoutTime = $('#ch-OutTimePicker').val();
            objhd.AgencyCode = "NA";
            objhd.TravelDate = "NA";
            objhd.FlightNo = "NA";
            objhd.Sector = "NA - " + ($('#BookHotelCityCode').val()).toUpperCase();
            objhd.Purpose = "NA";
            objhd.Meal = "NA";
            objhd.Beverage = "NA";
            objhd.PNR = "";
            objhd.BookingType = "Standby";
            objhd.IsHotelRequired = true;
            if ($('#airtransport :selected').text() == "Yes") {
                objhd.AirportTransport = true;
                //objhd.IsCabRequiredAsPerETA = false;
                //objhd.CabPickupTime = $('#txtCabPickupTime').val();
            }
            else {
                objhd.AirportTransport = false;
                //objhd.IsCabRequiredAsPerETA = true;
            }
            objhd.HotelRequestId = 1;
            $scope.hotelDetail.push(objhd);
        }
        else {
            if (($('#BookFlightTo').val() == '') || ($('#BookFlightFrom').val() == '') || ($('#bookDepartureDate').val() == '') || ($('#bookdeptTime').val() == '') || ($('#BookAircraftName').val() == '') || ($('#bookFlightNo').val() == '') || ($('#bookReasonOfTrv').val() == '')) {
                $scope.submitflightDetail = true;
                return false;
            }
            var objfd = new Object();
            objfd.PassengerID = $scope.passengerId
            objfd.OATTravelRequestId = OatrequestNo
            objfd.FlightType = "One-Way";
            objfd.OriginPlace = $('#BookFlightTo').val();
            objfd.DestinationPlace = $('#BookFlightFrom').val();
            objfd.DepartureDate = $scope.ConvertMMddyyyy($('#bookDepartureDate').val());
            objfd.DepartureTime = $('#bookdeptTime').val();
            objfd.AirCraftName = $('#BookAircraftName').val();
            objfd.FlightNumber = $('#bookFlightNo').val();
            objfd.ReasonForTravel = $('#bookReasonOfTrv').val();
            $scope.flightInfo.push(objfd);

        }

        $(".loader").show();
        $http({
            method: "POST",
            url: "../OAT/submitBookingFromHistory",
            data: JSON.stringify({ flightInfo: $scope.flightInfo, hotelDetailList: $scope.hotelDetail, passID: $scope.passengerId, oatReqNo: OatrequestNo }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $(".loader").hide();
            alert(response.data);
            window.location.reload;
        }, function myError(response) {
            alert(response);
            $(".loader").hide();
            window.location.reload();
        });
    }


    $scope.OpenBookHotelDiv = function (passId) {
        $scope.passengerId = passId;
        setTimeout(function () {
            $('#bookchindate').datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
            $('#bookchOutdate').datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy"
            });
            $('#ch-InTimePicker').wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false
            });
            $('#ch-OutTimePicker').wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false
            });

            $scope.getCityDetail();
            setTimeout(function () {
                $('.bookhotelselect').select2({
                    placeholder: 'Select City'
                });
            }, 1000);


        }, 200);
    }

    $scope.OpenBookFlightDiv = function (passId) {
        $scope.passengerId = passId;
        setTimeout(function () {
            $('#bookDepartureDate').datepicker({
                numberOfMonths: 2,
                defaultDate: new Date(),
                minDate: new Date(),
                maxDate: "+10M +00D",
                dateFormat: "dd/mm/yy",
                onSelect: function (date) {
                    $scope.bookDepartureDate = date;
                    $scope.$apply();
                    //angular.element($('#bookDepartureDate')).triggerHandler('input');                 
                }
            });

            $scope.otaList('BookFlightFrom');
            $scope.otaList('BookFlightTo');

        }, 200);
    }
    //To bind selectize dropdown
    $scope.otaList = function (id) {
        var Id = id;
        $('#' + Id).selectize({
            valueField: 'SectorCode',
            labelField: 'SectorName',
            searchField: ['SectorCode', 'SectorName'],
            options: {},
            create: true,
            render: {
                option: function (item, escape) {
                    var actors = [];
                    for (var i = 0, n = item.length; i < n; i++) {
                        actors.push('<span>' + escape(item[i].SectorCode) + "-" + escape(item[i].SectorName) + '</span>');
                    }
                    return '<div>' +
                        '<span class="title">' +
                        '</span>' +
                        '<span class="description">' + escape(item.SectorCode) + "-" + escape(item.SectorName) + '</span>' +
                        '<span class="actors">' + (actors.length ? 'Starring ' + actors.join(', ') : '') + '</span>' +
                        '</div>';
                }
            },
            load: function (query, callback) {
                if (!query.length) return callback();
                $http({
                    method: "POST",
                    url: "../OAT/FetchIOTAList",
                    data: JSON.stringify({ IOTACode: query }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    callback(response.data);


                }, function myError(response) {
                    alert(response);
                });
            }
        });
    }

    var appenddata1 = "<option value =''></option>";
    $scope.getCityDetail = function () {
        appenddata1 = ""
        $.ajax({
            method: "GET",
            url: "../city/GetCityListData",
            success: function (data1) {
                for (var i = 0; i < data1.length; i++) {
                    appenddata1 += "<option value = '" + data1[i].CityCode + " '>" + data1[i].CityCode + "-" + data1[i].CityName + " </option>";

                }
                $("#BookHotelCityCode").html(appenddata1);
            },
            error: function (response) {
            }
        });
    };
    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        dataAsFormData.append('RejectionSrc', data);
        //for (var i = 0; data.response.length; i++)
        //    angular.forEach(data, function (value, key) {

        //    });
        return dataAsFormData;
    };




    $scope.getMasterOAtList();

});
app.directive("datepicker", function () {
    function link(scope, element, attrs) {
        // CALL THE "datepicker()" METHOD USING THE "element" OBJECT.
        element.datepicker({
            dateFormat: "dd/mm/yy",
            changeYear: true,
            changeMonth: true,
            defaultDate: +0,
            minDate: new Date()
        });
    }

    return {
        require: 'ngModel',
        link: link
    };
});
app.directive('formattedDate', function (dateFilter) {
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


