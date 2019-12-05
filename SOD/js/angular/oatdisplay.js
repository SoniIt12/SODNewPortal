//OAT Module :Booking Linst Info
$(document).ready(function () {
    setTimeout(function () {
        $('.citySelect').select2({
            placeholder: 'Select City'
        });
    }, 1000);
})

var app = angular.module('OatViewDetails', []);
app.controller("OatView", function ($scope, $http, $timeout) {
    $scope.choiceBookingFor = ["Self", "Others (On Behalf of Employee / Non-Employee)", "Roistering(Flight ops / In Flight)", "SPOC"];
    $scope.choiceBookingType = ["Domestic", "International"];
    $scope.fltType = 'One Way';
    // $scope.SpocType = 'behalf Of Hod';
    $scope.noOfPax = 1;
    $scope.noOfHotelDiv = 1;
    $scope.noOfPassanger = 1;
    $scope.PassengerDetails = [
        {
            "IsEmployee": true,
            "IsFlightRequired": "",
            "IsHotelRequired": ""
        }];
    $scope.FlightAndHotel = false;
    $scope.HotelTab = false;
    $scope.Flighttab = false;
    $scope.personalTab = true;
    $scope.employeeinfo = false;
    $scope.submitPersonalDetails = false;
    $scope.submitPersonalOtherDetails = false;
    $scope.submitPersonalInfo = false;
    $scope.submitFlightInfo = false;
    $scope.submitHotelInfo = false;
    $scope.editableInput = true;
    $scope.isemployeeData = false;
    $scope.sameFlightAsAbove = false;
    $scope.ISflight = false;
    $('#FlightTab').addClass("disabled");
    $('#hotelTab').addClass("disabled");
    $("#noOfPAx").prop("readonly", true);
    $scope.paxType = [];
    $scope.IOTAList = [];
    var countHotel = 0;
    var CountFlight = 0;
    var countFlightplushotel = 0;
    $scope.HotelDetails = [];
    $scope.FlightDetails = [];
    var EmpArray = [];
    var flag = true;

    // $scope.isSameDetail = false; 



    //increment passenger info div as per the no of passanger
    $scope.getNumber = function (sign) {
        if (sign == "plus") {
            $scope.noOfPassanger = parseInt($('#noOfPAx').val()) + 1;
            $scope.PassengerDetails.push({
                "IsEmployee": true,
            });
        }
        else if (sign == '') {
            $scope.noOfPassanger = $scope.noOfPassanger
        }
        else if (sign == "minus") {
            $scope.noOfPassanger = parseInt($('#noOfPAx').val()) - 1;
            $scope.PassengerDetails.pop({});
        }
        else {
            $scope.noOfPassanger = 1;
        }
        return new Array($scope.noOfPassanger);
    }

    //passanger no btn disabled after filling the passanger info
    $scope.getBookingFor = function () {
        if ($scope.selectedType == 'Self') {
            $('#noOfPAx').val(1);
            $("#minusBtn").attr("disabled", true);
            $("#plusBtn").attr("disabled", true);
            $("#noOfPAx").prop("readonly", true);
            $scope.getNumber('self');
        }
        else {
            $("#minusBtn").attr("disabled", false);
            $("#plusBtn").attr("disabled", false);
            $("#noOfPAx").prop("readonly", false);
        }
        $scope.SpocType = ($scope.selectedType == 'SPOC') ? 'behalfOfHod' : 'Pilot';
    }

    //get details of employee who is making booking
    $scope.getUserDetails = function () {
        $(".loader").show();
        $http({
            method: "GET",
            url: "../OAT/getUserDetail"
        }).then(function mySucces(response) {
            $scope.empDetails = response.data;
            if ($scope.empDetails[0].Gender == "F") {
                $scope.empDetails[0].Gender = "Ms.";
            }
            else $scope.empDetails[0].Gender = "Mr.";
            $scope.empDetails[0].FirstName = response.data[0].EmpName.split(" ")[0].trim();
            $scope.empDetails[0].LastName = response.data[0].EmpName.split(" ")[1].trim();
            $(".loader").hide();
        }, function myError(response) {
            alert(response);
            $(".loader").hide();
        });
    }

    //get details of employee who is making booking
    $scope.getPaxDetails = function () {
        $(".loader").show();
        $http({
            method: "GET",
            url: "../OAT/getPaxType"
        }).then(function mySucces(response) {
            $(".loader").hide();
            $scope.paxType = response.data;
        }, function myError(response) {
            alert(response);
            $(".loader").hide();
        });
    }

    $scope.getOATBookingRight = function () {
        $(".loader").show();
        $http({
            method: "GET",
            url: "../OAT/getOATBookingRight"
        }).then(function mySucces(response) {
            $(".loader").hide();
            $scope.OATBookingRight = response.data;
            if ($scope.OATBookingRight.length == 0)
                $scope.choiceBookingFor.splice(2, 2);
            else if ($scope.OATBookingRight[0].IsRoisteringRight && $scope.OATBookingRight[0].IsSpocRight)
                $scope.choiceBookingFor;
            else if ($scope.OATBookingRight[0].IsSpocRight)
                $scope.choiceBookingFor.splice(3, 1);
            else if ($scope.OATBookingRight[0].IsRoisteringRight)
                $scope.choiceBookingFor.splice(2, 1);

        }, function myError(response) {
            alert(response);
            $(".loader").hide();
        });
    }
    $scope.checkduplicacyOfEMployee = function (empcode, index) {
        if (empcode == undefined) {
            return false;
        }
        for (var i = 0; i < $scope.PassengerDetails.length - 1; i++) {
            if ($scope.PassengerDetails[i].EmployeeCode == empcode && i != index) {
                return true;
            }
        }
    }

    //get employee data according to employee id
    $scope.FetchEmployeeData = function (empcode, index) {
        if ($scope.checkduplicacyOfEMployee(empcode, index)) {
            $("#duplicateEmp" + index).show();
            $("#empCode" + index).focus();
            return false;
        }
        else {
            $("#duplicateEmp" + index).hide();
        }
        if ($scope.PassengerDetails[index].IsEmployee) {
            if (empcode == undefined) {
                $("#empCode" + index).focus();
                return false;
            }
            $(".loader").show();
            $http({
                method: "POST",
                url: "../OAT/FetchEmployeeData",
                data: JSON.stringify({ empcode: empcode }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                var employeeIndex = "isemployeeData" + index;
                var empEmailIndex = "isEmpEmail" + index;
                var empPhoneIndex = "isEmpMobile" + index;
                var empLNameIndex = "isEmpLName" + index;
                $(".loader").hide();
                if (response.data.length > 0) {
                    //EmpArray.push(empcode);                   
                    $scope[employeeIndex] = true;
                    $scope.PassengerDetails[index].LastName = response.data[0].LastName;
                    $scope.PassengerDetails[index].Designation = response.data[0].DesignationName;
                    $scope.PassengerDetails[index].Department = response.data[0].DepartmentName;
                    $scope.PassengerDetails[index].PhoneNo = response.data[0].Phone;
                    $scope.PassengerDetails[index].EmailId = (response.data[0].Email).toLowerCase();
                    $scope.PassengerDetails[index].FirstName = response.data[0].FirstName;
                    if (response.data[0].Gender == "M") {
                        $scope.PassengerDetails[index].Gender = "Mr.";
                    }
                    else {
                        $scope.PassengerDetails[index].Gender = "Ms.";
                    }
                    $scope.PassengerDetails[index].PhoneNo == "" ? $scope[empPhoneIndex] = false : $scope[empPhoneIndex] = true;
                    $scope.PassengerDetails[index].EmailId == "" ? $scope[empEmailIndex] = false : $scope[empEmailIndex] = true;
                    $scope.PassengerDetails[index].LastName == "" ? $scope[empLNameIndex] = false : $scope[empLNameIndex] = true;
                    $("#notvalidEmp" + index).hide();
                    //$scope.IsEmp[index] = true;
                    $scope.submitPersonalOtherDetails = false;
                }
                else {
                    $scope[employeeIndex] = false;
                    //$scope.IsEmp[index] = false;
                    $("#notvalidEmp" + index).show();
                    $scope.PassengerDetails[index].LastName = "";
                    $scope.PassengerDetails[index].Designation = "";
                    $scope.PassengerDetails[index].Department = "";
                    $scope.PassengerDetails[index].PhoneNo = "";
                    $scope.PassengerDetails[index].EmailId = "";
                    $scope.PassengerDetails[index].FirstName = "";
                    $scope.PassengerDetails[index].Gender = "";
                    $scope.PassengerDetails[index].EmployeeCode = empcode;
                    $("#empCode" + index).focus();
                    $(".loader").hide();
                    return false;

                }
            }, function myError(response) {
                alert(response);
                $(".loader").hide();
            });
        }
        else {
            //EmpArray.push(empcode);
            $("#notvalidEmp" + index).hide();
            $scope.isemployeeData = false;
            $scope.PassengerDetails[index].EmployeeCode = empcode;
            $scope.PassengerDetails[index].LastName = "";
            $scope.PassengerDetails[index].Designation = "";
            $scope.PassengerDetails[index].Department = "";
            $scope.PassengerDetails[index].PhoneNo = "";
            $scope.PassengerDetails[index].EmailId = "";
            $scope.PassengerDetails[index].FirstName = "";
            $scope.PassengerDetails[index].Gender = "";
        }
    }

    //to filter iotalist
    $scope.addOrRemoveHotel = function (index, passengerlst, type) {
        if (type == "Flight") {
            if (passengerlst.IsFlightRequired) {
                $scope.FlightDetails.push({
                    "Issamedetail": false,
                    "IsInternational": $scope.bookingType == "Domestic" ? false : true,
                    "multicity": []
                });
                CountFlight = $scope.FlightDetails.length - 1;
                $scope.FlightDetails[CountFlight].EmpCode = $scope.selectedType == 'Self' ? $scope.empDetails[0].EmpCode : passengerlst.EmployeeCode;
                $scope.FlightDetails[CountFlight].EmpName = $scope.selectedType == 'Self' ? $scope.empDetails[0].FirstName + " " + $scope.empDetails[0].LastName : passengerlst.FirstName + " " + passengerlst.LastName;
                $scope.FlightDetails[CountFlight].Gender = $scope.selectedType == 'Self' ? $scope.empDetails[0].Gender : passengerlst.Gender;
                setTimeout(function () {
                    $scope.otaList("select-", "to", CountFlight);
                    $scope.otaList("select-", "from", CountFlight);
                    $scope.otaList("select-return", "to", CountFlight);
                    $scope.otaList("select-return", "from", CountFlight);                  
                    $('.timepicker').wickedpicker({
                        twentyFour: false, title:
                            'Select Time', showSeconds: false
                    });
                }, 200);               
                //CountFlight++;
            }
            else {
                $scope.FlightDetails.pop();
            }
        }
        if (type == "Hotel") {
            if (passengerlst.IsHotelRequired) {
                $scope.HotelDetails.push({
                    "Issamedetail": false,
                    "IsInternational": $scope.bookingType == "Domestic" ? false : true,
                    "multicity": [],
                    "multycityList": []
                });
                countHotel = $scope.HotelDetails.length - 1;
                $scope.HotelDetails[countHotel].EmpCode = $scope.selectedType == 'Self' ? $scope.empDetails[0].EmpCode : passengerlst.EmployeeCode;
                $scope.HotelDetails[countHotel].EmpName = $scope.selectedType == 'Self' ? $scope.empDetails[0].FirstName + " " + $scope.empDetails[0].LastName : passengerlst.FirstName + " " + passengerlst.LastName;
                $scope.HotelDetails[countHotel].Gender = $scope.selectedType == 'Self' ? $scope.empDetails[0].Gender : passengerlst.Gender;
                setTimeout(function () {
                    $scope.otaList("select-", "hotel", countHotel);
                    $('.timepicker').wickedpicker({
                        twentyFour: false, title:
                            'Select Time', showSeconds: false
                    });
                }, 200);
               
                countHotel++;
            }
            else {
                $scope.HotelDetails.pop();
            }
        }
        if (!$scope.personalTab) {
            if (($scope.FlightDetails.length == 0) && ($scope.HotelDetails.length == 0)) {
                $('#hotelTab').addClass("disabled");
                $('#FlightTab').addClass("disabled");
                $scope.personalTab = true;
                $scope.Flighttab = false;
                $scope.HotelTab = false;
                $scope.FlightAndHotel = false;
            }
            else if (($scope.FlightDetails.length > 0) && ($scope.HotelDetails.length > 0)) {
                $('.nav-tabs a[href="#tab_default_2"]').tab('show');
                $scope.FlightAndHotel = true;
                //$scope.personalTab = $scope.personalTab ;
                $scope.Flighttab = false;
                $scope.HotelTab = false;
                $('#FlightTab').removeClass("disabled");
            }
            else if ($scope.FlightDetails.length > 0) {
                $('.nav-tabs a[href="#tab_default_2"]').tab('show');
                $scope.Flighttab = true;
                //$scope.personalTab = false;
                $scope.FlightAndHotel = false;
                $scope.HotelTab = false;
                $('#FlightTab').removeClass("disabled");
                $('#hotelTab').addClass("disabled");
            }
            else {
                $('.nav-tabs a[href="#tab_default_3"]').tab('show');
                $scope.HotelTab = true;
                //$scope.personalTab = false;
                $scope.FlightAndHotel = false;
                $scope.Flighttab = false;
                $('#hotelTab').removeClass("disabled");
                $('#FlightTab').addClass("disabled");
            }
        }
    }

    // submit passanger details and generate hotel and flight div according to passanger requirement
    $scope.OtherPassangerDetails = function (passangerList) {
        $(".loader").show();
        for (var i = 0; i < passangerList.length; i++) {
            // $scope.PassengerDetails[0].Gender = $scope.passangerList[i].Title == "F" ? "Ms." : "Mr.";
            if (!passangerList[i].IsFlightRequired && !passangerList[i].IsHotelRequired) {
                alert("Please select flight required or hotel required for each employee.");
                $(".loader").hide();
                return false;
            }
        }
        if ($scope.othersPersonalForm.$valid) {
            var ch = 0;
            var cf = 0;
            for (var i = 0; i < passangerList.length; i++) {
                if (passangerList[i].IsFlightRequired) {
                    $scope.FlightDetails[cf].EmpCode = $scope.selectedType == 'Self' ? $scope.empDetails[0].EmpCode : passangerList[i].EmployeeCode;
                    $scope.FlightDetails[cf].EmpName = $scope.selectedType == 'Self' ? $scope.empDetails[0].FirstName + " " + $scope.empDetails[0].LastName : passangerList[i].FirstName + " " + passangerList[i].LastName;
                    $scope.FlightDetails[cf].Gender = $scope.selectedType == 'Self' ? $scope.empDetails[0].Gender : passangerList[i].Gender;
                    $scope.otaList("select-", "to", cf);
                    $scope.otaList("select-", "from", cf);
                    $scope.otaList("select-return", "to", cf);
                    $scope.otaList("select-return", "from", cf);
                    $('.timepicker').wickedpicker({
                        twentyFour: false, title:
                            'Select Time', showSeconds: false
                    });
                    cf++;
                }
                if (passangerList[i].IsHotelRequired) {
                    $scope.HotelDetails[ch].EmpCode = $scope.selectedType == 'Self' ? $scope.empDetails[0].EmpCode : passangerList[i].EmployeeCode;
                    $scope.HotelDetails[ch].EmpName = $scope.selectedType == 'Self' ? $scope.empDetails[0].FirstName + " " + $scope.empDetails[0].LastName : passangerList[i].FirstName + " " + passangerList[i].LastName;
                    $scope.HotelDetails[ch].Gender = $scope.selectedType == 'Self' ? $scope.empDetails[0].Gender : passangerList[i].Gender;
                    $scope.otaList("select-", "hotel", ch);
                    $('.timepicker').wickedpicker({
                        twentyFour: false, title:
                            'Select Time', showSeconds: false
                    });
                    ch++;
                }
                $(".loader").hide();
            }

            $scope.submitPersonalDetails = false;
            $scope.submitPersonalOtherDetails = false
            if (($scope.FlightDetails.length > 0) && ($scope.HotelDetails.length > 0)) {
                $('.nav-tabs a[href="#tab_default_2"]').tab('show');
                $scope.FlightAndHotel = true;
                $scope.personalTab = false;
                $('#FlightTab').removeClass("disabled");
            }
            else if ($scope.FlightDetails.length > 0) {
                $('.nav-tabs a[href="#tab_default_2"]').tab('show');
                $scope.Flighttab = true;
                $scope.personalTab = false;
                $('#FlightTab').removeClass("disabled");
            }
            else {
                $('.nav-tabs a[href="#tab_default_3"]').tab('show');
                $scope.HotelTab = true;
                $scope.personalTab = false;
                $('#hotelTab').removeClass("disabled");
            }
            $("#minusBtn").attr("disabled", true);
            $("#plusBtn").attr("disabled", true);
            $("#noOfPAx").prop("readonly", true);
            //$("#bookingFor").prop("readonly", true);
            $scope.editableInput = false;
        }
        else {
            if ($scope.selectedType == 'Self') {
                $scope.submitPersonalDetails = true;
            }
            else {
                $scope.submitPersonalOtherDetails = true;
            }
            $(".loader").hide();
            return false;
        }
    }

    $scope.changeFltType = function (type) {
        $scope.fltType = type;
        setTimeout(function () {
            for (var i = 0; i <= $scope.flightDetails.length; i++) {
                $scope.otaList("select-", "to", i);
                $scope.otaList("select-", "from", i);
                $scope.otaList("select-return", "to", i);
                $scope.otaList("select-return", "from", i);
                $('.timepicker').wickedpicker({
                    twentyFour: false, title:
                        'Select Time', showSeconds: false
                });
                $('.roundtimepicker').wickedpicker({
                    twentyFour: false, title:
                        'Select Time', showSeconds: false
                });

            }
        }, 100);

    }

    $scope.changespocType = function (type) {
        $scope.SpocType = type;
    }

    //To bind selectize dropdown
    $scope.otaList = function (id, position, index) {
        var Id = id + position + index;
        $('#' + Id).selectize({
            valueField: 'SectorCode',
            labelField: 'SectorName',
            searchField: ['SectorCode', 'SectorName'],
            options: {},
            create: false,
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
                var dd = "";
                if (position == "to") {
                    dd = $('#' + id + "from" + index).val();
                }
                
                var isDomestic = $scope.bookingType == "Domestic" ? "true" : "false";
                $http({
                    method: "POST",
                    url: "../OAT/FetchIOTAList",
                    data: JSON.stringify({ IOTACode: query + "," + dd +  ","+ isDomestic  }),
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

    //submit all details as required and generate the request
    $scope.SubmitAllDetails = function (lstDetails) {
        $(".loader").show();
        var passengerDetail = {};
        $scope.OatAlreadyBookedFlightLst = [];
        //var sodbookingLst = new Array();
        //var sodflightLst = new Array();
        var hotelDetailLst = new Array();
        if ($scope.selectedType == 'Self') {
            $scope.PassengerDetails[0].LastName = $scope.empDetails[0].LastName
            $scope.PassengerDetails[0].Designation = $scope.empDetails[0].Designation;
            $scope.PassengerDetails[0].Department = $scope.empDetails[0].Department;
            $scope.PassengerDetails[0].PhoneNo = $scope.empDetails[0].Phone;
            $scope.PassengerDetails[0].EmailId = $scope.empDetails[0].Email;
            $scope.PassengerDetails[0].FirstName = $scope.empDetails[0].FirstName;
            $scope.PassengerDetails[0].Base = $scope.empDetails[0].Base;
            $scope.PassengerDetails[0].EmployeeCode = $scope.empDetails[0].EmpCode;
            $scope.PassengerDetails[0].Gender = $scope.empDetails[0].Gender;
        }
        if (($scope.flightDetails.length > 0 && $scope.HotelDetails.length > 0) || ($scope.flightDetails.length == 0 && $scope.HotelDetails.length > 0)) {
            if ($scope.HotelDetailsForm.$valid) {
                $scope.HotelDetails = lstDetails;
                $scope.submitHotelInfo = false;
                //var sodbookingLst = new Array();
                //var sodflightLst = new Array();
                var hotelDetailLst = new Array();
                //sodbookingLst = $scope.postsodbookingLst();
                //sodflightLst = $scope.postsodFlightLst();
                hotelDetailLst = $scope.postsodhotelmasterlst();
            }
            else {
                $scope.submitHotelInfo = true;
                $(".loader").hide();
                return false;
            }
        }
        else if ($scope.flightDetailsForm.$valid) {

            for (var j = 0; j <= lstDetails.length - 1; j++) {
                if (lstDetails[j].multicity.length != 0) {
                    var date1 = lstDetails[j].DepartureDate;
                    var newArr = [];
                    newArr.push(date1);
                    for (var i = 0; i < lstDetails[j].multicity.length; i++) {
                        newArr.push(lstDetails[j].multicity[i].DepartureDate);
                    }
                    var sortArr = newArr.sort();
                    for (var i = 0; i < newArr.length - 1; i++) {
                        if (sortArr[i] == sortArr[i + 1]) {
                            alert("you cannot travel on same date.")
                            return false;
                        }
                    }
                }
            }
            $scope.submitFlightInfo = false;
            $scope.flightDetails = lstDetails;
        }
        else {
            $scope.submitFlightInfo = true;
            $(".loader").hide();
            return false;
        }
        $scope.passengerDetail = {
            personalInfo: $scope.postEmployeeInfoLst(),
            passangerInfo: $scope.PassengerDetails,
            flightInfo: $scope.postFlightDetailslst(),
            flightType: $scope.fltType,
            hotelDetailList: hotelDetailLst,
        }

        if (flag = false) {
            return false;
        }
        else {
            $http({
                method: "POST",
                url: "../OAT/CheckAlreadyBookData",
                data: JSON.stringify($scope.passengerDetail),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $(".loader").hide();
                if (response.data != "") {
                    $scope.OatAlreadyBookedFlightLst = response.data;
                    $scope.currentPage = 1;
                    $scope.entryLimit = 10;
                    $scope.filteredItems = $scope.OatAlreadyBookedFlightLst.length;
                    $("#myModal").modal('show');
                }
                else {
                    $scope.submit();
                }

            }, function myError(response) {
                $(".loader").hide();
                window.location.reload();
            });
        }
    }
    $scope.submit = function () {
        $http({
            method: "POST",
            url: "../OAT/submitDataForBooking",
            data: JSON.stringify($scope.passengerDetail),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $(".loader").hide();
            var url = '../HotelOnly/BookingResponse';
            window.location.href = url;
        }, function myError(response) {
            alert(response);
            $(".loader").hide();
            window.location.reload();
        });
    }
    // after flight info submit open the hotel div
    $scope.flightDetails = function (flightinfo) {
        if ($scope.flightDetailsForm.$valid) {
            //for (var j = 0; j <= flightinfo.length - 1; j++) {
            //    var DepartureTime1 = $scope.ConvertTimeto24(flightinfo[j].DepartureT1);
            //    var DepartureTime2 = flightinfo[j].DepartureT2 == undefined ? "" : $scope.ConvertTimeto24(flightinfo[j].DepartureT2);
            //    //if (DepartureTime2 != "") {
            //    //    if ((DepartureTime2 - DepartureTime1) == 0) {
            //    //        if ((DepartureTime2 - DepartureTime1) < 90) {
            //    //            alert("Departure Flight Time From and Departure Flight Time To should be 90 min difference.");
            //    //            flag = false;
            //    //            return
            //    //        }
            //    //    }
            //    //}
            //}

            if ($scope.fltType == "Multi Trip") {
                for (var j = 0; j <= flightinfo.length - 1; j++) {
                    //var DepartureTime1 = $scope.ConvertTimeto24(flightinfo[j].DepartureT1);
                    //var DepartureTime2 = flightinfo[j].DepartureT2 == undefined ? "" : $scope.ConvertTimeto24(flightinfo[j].DepartureT2);
                    //if (DepartureTime2 != "") {
                    //    if ((DepartureTime2 - DepartureTime1) == 0) {
                    //        if ((DepartureTime2 - DepartureTime1) < 90) {
                    //            alert("Departure Flight Time From and Departure Flight Time To should be 90 min difference.");
                    //            flag = false;
                    //            return
                    //        }
                    //    }
                    //}
                    if (flightinfo[j].multicity.length != 0) {
                        var date1 = flightinfo[j].DepartureDate;
                        var newArr = [];
                        newArr.push(date1);
                        for (var i = 0; i < flightinfo[j].multicity.length; i++) {
                            newArr.push(flightinfo[j].multicity[i].DepartureDate);
                        }
                        var sortArr = newArr.sort();
                        for (var i = 0; i < newArr.length - 1; i++) {
                            if (sortArr[i] == sortArr[i + 1]) {
                                alert("you cannot travel on same date.")
                                return false;
                            }
                        }
                    }
                }
            }
            if ($scope.fltType == "Round Trip") {
                for (var i = 0; i < flightinfo.length; i++) {
                    var deptDate = new Date($scope.Convertyyyymmdd(flightinfo[i].DepartureDate));
                    var retDeptDate = new Date($scope.Convertyyyymmdd(flightinfo[i].returnDepartureDate));
                    if (retDeptDate < deptDate) {
                        alert("Return Date should not less than Departure Date.");
                        return false;
                    }
                   // var ReturnTime1 = $scope.ConvertTimeto24(flightinfo[i].ReturnT1);
                    //var ReturnTime2 = $scope.FlightDetails[i].ReturnT2 == undefined ? "" : $scope.ConvertTimeto24(flightinfo[i].ReturnT2);
                    //if (ReturnTime2 != "") {
                    //    if ((ReturnTime2 - ReturnTime1) == 0) {
                    //        if ((ReturnTime2 - ReturnTime1) < 90) {
                    //            alert("Return Flight Time From and Return Flight Time To should be 90 min difference.");
                    //            flag = false;
                    //            return
                    //        }
                    //    }
                    //}
                }
            }
            $scope.submitFlightInfo = false;
            $scope.ISflight = true;
            $("#tab_default_2 :input").prop("disabled", true);
            $('#tab_default_2 select').prop('disabled', true);
            $('.FlightSelectize').css('cursor', 'none');
            //$scope.flightDetails = $scope.flightDetails == undefined? flightinfo;
            for (var i = 0; i < flightinfo.length; i++) {
                for (var j = 0; j < $scope.HotelDetails.length; j++) {
                    if (flightinfo[i].EmpCode == $scope.HotelDetails[j].EmpCode) {
                        if (flightinfo[i].multicity.length != 0 && flightinfo[i].multicity.length != undefined) {
                            $scope.HotelDetails[j].multycityList = [];
                            $scope.HotelDetails[j].multycityList.push({
                                "DestinationPlace": flightinfo[i].DestinationPlace,
                                "checkinDate": flightinfo[i].DepartureDate
                            })
                            for (var k = 0; k < flightinfo[i].multicity.length; k++) {
                                $scope.HotelDetails[j].multycityList.push({
                                    "DestinationPlace": flightinfo[i].multicity[k].DestinationPlace,
                                    "checkinDate": flightinfo[i].multicity[k].DepartureDate
                                })
                            }
                        }
                        else if ($scope.fltType == "Round Trip") {
                            $scope.HotelDetails[i].checkoutDate = flightinfo[i].returnDepartureDate;
                            $scope.HotelDetails[j].city = flightinfo[i].DestinationPlace;
                            $scope.HotelDetails[j].checkinDate = flightinfo[i].DepartureDate;
                            //$timeout(function () {
                            //    $("#MessageDateTo").datepicker("option", "minDate", $scope.messagesForm.dateFrom);
                            //    $("#MessageDateFrom").datepicker("option", "maxDate", $scope.messagesForm.dateTo);
                            //})                           
                        }
                        else {
                            $scope.HotelDetails[j].city = flightinfo[i].DestinationPlace;
                            $scope.HotelDetails[j].checkinDate = flightinfo[i].DepartureDate;
                        }
                    }
                }
            }
            $('.nav-tabs a[href="#tab_default_3"]').tab('show');
            $scope.HotelTab = true;
            $scope.FlightAndHotel = false;
            $('#hotelTab').removeClass("disabled");
        }
        else {
            $scope.submitFlightInfo = true;

            return false;
        }
    }

    $scope.selectdate = function (city, index) {
        for (var i = 0; i < $scope.HotelDetails.length; i++) {
            for (var j = 0; j < $scope.HotelDetails[i].multycityList.length; j++) {
                if (city == $scope.HotelDetails[i].multycityList[j].DestinationPlace) {
                    $scope.HotelDetails[i].multycityList[j].checkindate = $scope.HotelDetails[i].multycityList[j].checkinDate;
                    alert($scope.HotelDetails[i].multycityList[j].checkindate);
                }
            }
        }
        // $("#date" + index).val($scope.HotelDetails[i].multycityList[j].checkindate);	
    }

    //To add new div in Flight and Hotel
    $scope.addnewDiv = function (id, type) {
        if (type == "hotel") {
            if ($scope.HotelDetails[id].multicity.length == $scope.HotelDetails[id].multycityList.length - 1) {
                alert("No more Hotel is required for this passenger.");
                return false;
            }
            $scope.HotelDetails[id].multicity.push({
                "Issamedetail": false,
                "IsInternational": $scope.bookingType == "Domestic" ? false : true,
            });
            setTimeout(function () {
                $('.timepickermult').wickedpicker({
                    twentyFour: false, title:
                        'Select Time', showSeconds: false
                });
            }, 100);
        }
        else {
            if ($scope.FlightDetails[id].multicity.length == 4) {
                alert("One passenger should be allowed only for 5 Flight Booking.");
                return false;
            }
            $scope.FlightDetails[id].multicity.push({
                "FlightType": $scope.fltType,
                "IsInternational": $scope.bookingType == "Domestic" ? false : true,
                "EmpCode": $scope.FlightDetails[id].EmpCode,
                "EmpName": $scope.FlightDetails[id].EmpName,
                "Gender": $scope.FlightDetails[id].Gender
            });

            setTimeout(function () {
                var length = $scope.FlightDetails[id].multicity.length - 1;
                var index = "" + id + length;
                // var 
                $scope.otaList("select-mult", "to", index);
                $scope.otaList("select-mult", "from", index);

                $('.timepickermult').wickedpicker({
                    twentyFour: false, title:
                        'Select Time', showSeconds: false
                });
            }, 100);

        }
    }

    $scope.removeDiv = function (id, type) {
        if (type == "hotel") {
            if ($scope.HotelDetails[id].multicity.length == 0) {
                alert("No more records to remove");
                return false;
            }
            else {
                $scope.HotelDetails[id].multicity.pop();
            }
        }
        else {
            if ($scope.FlightDetails[id].multicity.length == 0) {
                alert("No more records to remove");
                return false;
            }
            else {
                $scope.FlightDetails[id].multicity.pop();
            }
        }
    }

    // fill details as above required
    $scope.FillDetailsAsAbove = function (list, index, type) {
        debugger;
        var temflight = new Array();
        var temflightMulticity = new Array();
        temflight.push(list[index]);
        temflightMulticity.push(list[index].multicity);
        if (!list[index].Issamedetail) {
            var multicityObj = $.extend({}, list[index].multicity, list[index - 1].multicity);
            var newObject = $.extend({}, list[index], list[index - 1]);
            newObject.EmpCode = temflight[0].EmpCode;
            newObject.EmpName = temflight[0].EmpName;
            newObject.Issamedetail = temflight[0].Issamedetail;
            if (type == "flight") {
                // $scope.sameFlightAsAbove = true;
                //$scope.flightDetails[index].selectize = true;
                $scope.FlightDetails[index] = newObject;
                $scope.FlightDetails[index].multicity = multicityObj;
            }
            else {
                newObject.city = temflight[0].city;
                newObject.multycityList = temflight[0].multycityList;
                newObject.checkinDate = temflight[0].checkinDate;
                //multicityObj[0].checkinDate = temflightMulticity.checkinDate;
                //multicityObj.checkinDate = {};
                $scope.HotelDetails[index] = newObject;
                $scope.HotelDetails[index].multicity = multicityObj;
                //$scope.flightDetails[index].selectize = false;
            }

        } else {
            if (type == "flight") {

                $scope.FlightDetails[index] = {
                    "Issamedetail": false,
                    "multicity": [],//temflightMulticity,
                    "EmpCode": temflight[0].EmpCode,
                    "EmpName": temflight[0].EmpName
                };
                setTimeout(function () {
                    $scope.otaList("select-", "to", index);
                    $scope.otaList("select-", "from", index);
                    $scope.otaList("select-return", "from", index);
                    $scope.otaList("select-return", "to", index);
                }, 100);
            }
            else {
                $scope.HotelDetails[index] = {
                    "Issamedetail": false,
                    "EmpCode": temflight[0].EmpCode,
                    "EmpName": temflight[0].EmpName,
                    "multycityList": temflight[0].multycityList,
                    "multicity": []
                };
                setTimeout(function () {
                    $scope.otaList("select-", "hotel", countHotel);
                }, 100);
            }
        }
    }
    // details  of the enployee who is generating the request
    $scope.postEmployeeInfoLst = function () {
        debugger;
        var postEmployeeInfoLst = new Array();
        var objPd = new Object();
        objPd.RequestDate = new Date();
        objPd.Gender = $scope.empDetails[0].Gender;
        objPd.RequestedEmpId = $scope.empDetails[0].EmpId;
        objPd.RequestedEmpCode = $scope.empDetails[0].EmpCode;
        objPd.RequestedEmpName = $scope.empDetails[0].EmpName;
        objPd.RequestedEmpDesignation = $scope.empDetails[0].Designation;
        objPd.RequestedEmpDept = $scope.empDetails[0].Department;
        objPd.EmailId = $scope.empDetails[0].Email;
        objPd.PhoneNo = $scope.empDetails[0].Phone;
        objPd.PaxNo = $scope.noOfPassanger;
        objPd.BookingForOnBehalfof = ($scope.selectedType == "SPOC" || $scope.selectedType == "Roistering(Flight ops / In Flight)") ? $scope.SpocType : "NA";
        objPd.bookingFor = $scope.selectedType,
            objPd.BookingType = $scope.bookingType;
        objPd.Base = $scope.empDetails.Base;
        if ($scope.FlightDetails.length != 0) {
            objPd.ReasonForTravel = $scope.FlightDetails[0].ReasonForTravel;
        }
        else {
            objPd.ReasonForTravel = $scope.HotelDetails[0].reasonOfHotel;
        }
        postEmployeeInfoLst.push(objPd);
        return postEmployeeInfoLst;
    }
    
    // details of flight info as per trip FOR OAT table	
    $scope.postFlightDetailslst = function () {
        var postFlightInfolst = new Array();
        for (var i = 0; i < $scope.FlightDetails.length; i++) {
            //$scope.FlightDetails[i].DepartureDate = $scope.Convertyyyymmdd($scope.FlightDetails[i].DepartureDate);	
            $scope.FlightDetails[i].MulticityLength = $scope.FlightDetails[i].multicity.length;
            $scope.FlightDetails[i].FlightType = $scope.fltType;
            var DepartureTime1 = $scope.ConvertTimeto24($scope.FlightDetails[i].DepartureT1);
            var DepartureTime2 = $scope.FlightDetails[i].DepartureT2 == undefined ? "" : $scope.ConvertTimeto24($scope.FlightDetails[i].DepartureT2);
            //if ((DepartureTime2 - DepartureTime1) == 0) {	
            //    if ((DepartureTime2 - DepartureTime1) < 90) {	
            //        alert("Departure Fight Time From and Departure Flight Time To should be 90 min difference.");	
            //        return false;	
            //    }	
            //}	
            postFlightInfolst.push($scope.FlightDetails[i]);
            var lastindex = postFlightInfolst.length - 1;
            if (DepartureTime2 == "") {
                $scope.FlightDetails[i].DepartureTime = $scope.FlightDetails[i].DepartureT1 + " - " + $scope.FlightDetails[i].DepartureT1;
            }
            else {
                $scope.FlightDetails[i].DepartureTime = $scope.ConvertTime($scope.FlightDetails[i].DepartureT1) + " - " + $scope.ConvertTime($scope.FlightDetails[i].DepartureT2);
            }
            postFlightInfolst[lastindex].DepartureDate = $scope.Convertyyyymmdd(postFlightInfolst[lastindex].DepartureDate);
            if ($scope.FlightDetails[i].multicity.length > 0) {
                for (var j = 0; j < $scope.FlightDetails[i].multicity.length; j++) {

                    //var objFlt = new Object();	

                    var DepartureTime1 = $scope.ConvertTimeto24($scope.FlightDetails[i].multicity[j].DepartureT1);
                    var DepartureTime2 = $scope.FlightDetails[i].DepartureT2 == undefined ? "" : $scope.ConvertTimeto24($scope.FlightDetails[i].DepartureT2);
                    if (DepartureTime2 == "") {
                        $scope.FlightDetails[i].multicity[j].DepartureTime = $scope.FlightDetails[i].multicity[j].DepartureT1 + " - " + $scope.FlightDetails[i].multicity[j].DepartureT1;
                    }
                    else {
                        $scope.FlightDetails[i].multicity[j].DepartureTime = $scope.ConvertTime($scope.FlightDetails[i].multicity[j].DepartureT1) + " - " + $scope.ConvertTime($scope.FlightDetails[i].multicity[j].DepartureT2);
                    }
                    postFlightInfolst.push($scope.FlightDetails[i].multicity[j]);

                    postFlightInfolst[lastindex + j + 1].DepartureDate = $scope.Convertyyyymmdd(postFlightInfolst[lastindex + j + 1].DepartureDate);
                    // postFlightInfolst[lastindex + j + 1].DepartureTime = $scope.ConvertTime($scope.FlightDetails[i].DepartureT1) + " - " + $scope.ConvertTime($scope.FlightDetails[i].DepartureT2);	
                    //postFlightInfolst.push(objFlt);	
                }
            }
            if ($scope.fltType == "Round Trip") {
                var deptDate = new Date($scope.Convertyyyymmdd($scope.FlightDetails[i].DepartureDate));
                var retDeptDate = new Date($scope.Convertyyyymmdd($scope.FlightDetails[i].returnDepartureDate));
                if (retDeptDate < deptDate) {
                    return false;
                }
                var ReturnTime1 = $scope.ConvertTimeto24($scope.FlightDetails[i].ReturnT1);
                var ReturnTime2 = $scope.FlightDetails[i].ReturnT2 == undefined ? "" : $scope.ConvertTimeto24($scope.FlightDetails[i].ReturnT2);
                //if ((ReturnTime2 - ReturnTime1) == 0) {	
                //    if ((ReturnTime2 - ReturnTime1) < 90) {	
                //        alert("Return Flight Time From and Return Flight Time To should be 90 min difference.");	
                //        flag = false;	
                //        return	
                //    }	
                //}	
                var objFl = new Object();

                objFl.FlightType = $scope.fltType;
                objFl.OriginPlace = $scope.FlightDetails[i].DestinationPlace;
                objFl.DestinationPlace = $scope.FlightDetails[i].OriginPlace;
                objFl.DepartureDate = $scope.Convertyyyymmdd($scope.FlightDetails[i].returnDepartureDate)
                //objFl.DepartureTime = $scope.FlightDetails[i].returnDepartureTime;	
                if (ReturnTime2 == "") {
                    objFl.DepartureTime = $scope.FlightDetails[i].ReturnT1 + " - " + $scope.FlightDetails[i].ReturnT1;
                }
                else {
                    objFl.DepartureTime = $scope.ConvertTime($scope.FlightDetails[i].ReturnT1) + " - " + $scope.ConvertTime($scope.FlightDetails[i].ReturnT2);
                }
                objFl.AirCraftName = $scope.FlightDetails[i].returnAircraftName;
                objFl.FlightNumber = $scope.FlightDetails[i].returnFlightNo;
                objFl.ReasonForTravel = $scope.FlightDetails[i].ReasonForTravel;
                objFl.IsInternational = $scope.FlightDetails[i].IsInternational;
                objFl.EmpCode = $scope.FlightDetails[i].EmpCode;
                objFl.EmpName = $scope.FlightDetails[i].EmpName;
                objFl.Gender = $scope.FlightDetails[i].Gender;
                postFlightInfolst.push(objFl);
            }
        }
        return postFlightInfolst;
    }

   
    // for travel request hotel details modal
    $scope.postsodhotelmasterlst = function () {
        flag = true;
        var hotelDetailsList = new Array();
        for (var i = 0; i < $scope.HotelDetails.length; i++) {
            var checkInDate = new Date($scope.Convertyyyymmdd($scope.HotelDetails[i].checkinDate));
            var checkOutDate = new Date($scope.Convertyyyymmdd($scope.HotelDetails[i].checkoutDate));
            //if (CheckOutDate < checkInDate) {
            //    return false;              
            //}
            if ((checkOutDate - checkInDate) == 0) {
                var checkINTime = $scope.ConvertTimeto24($scope.HotelDetails[i].checkInTime);
                var checkOutTime = $scope.ConvertTimeto24($scope.HotelDetails[i].checkOutTime);
                if (checkOutTime < checkINTime) {
                    alert("Check-out Time should be less than Check-in Time.");
                    flag = false;
                    return;
                }
                else if ((checkOutTime - checkINTime) < 90) {
                    alert("Check-in Time and Check-out Time should be 90 min difference.");
                    flag = false;
                    return;
                }
               
            }
            var objhd = new Object();
            objhd.EmpCode = $scope.HotelDetails[i].EmpCode;
            objhd.HotelCity = $scope.HotelDetails[i].city.toUpperCase();
            objhd.CheckInDate = $scope.Convertyyyymmdd($scope.HotelDetails[i].checkinDate);
            objhd.CheckOutDate = $scope.Convertyyyymmdd($scope.HotelDetails[i].checkoutDate);
            objhd.CheckinTime = $scope.HotelDetails[i].checkInTime;
            objhd.CheckoutTime = $scope.HotelDetails[i].checkOutTime;
            objhd.AgencyCode = "NA";
            objhd.TravelDate = "NA";
            objhd.FlightNo = "NA";
            objhd.Sector = "NA - " + $scope.HotelDetails[i].city.toUpperCase();
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
            hotelDetailsList.push(objhd);
            if ($scope.HotelDetails[i].multicity.length > 0) {
                for (var j = 0; j < $scope.HotelDetails[i].multicity.length; j++) {
                    var objhd = new Object();
                    var checkInDate = new Date($scope.Convertyyyymmdd($scope.HotelDetails[i].multicity[j].checkinDate));
                    var CheckOutDate = new Date($scope.Convertyyyymmdd($scope.HotelDetails[i].multicity[j].checkoutDate));
                    //if (CheckOutDate < checkInDate) {                        
                    //    return false;
                    //}
                    if (CheckOutDate == checkInDate) {
                        var checkINTime = $scope.ConvertTimeto24($scope.HotelDetails[i].multicity[j].checkInTime);
                        var checkOutTime = $scope.ConvertTimeto24($scope.HotelDetails[i].multicity[j].checkOutTime);
                        if (checkOutTime < checkINTime) {
                            alert("Check-out Time should be less than Check-in Time.");
                            flag = false;
                            return;
                        }
                        else if ((checkOutTime - checkINTime) < 90) {
                            alert("Check-in Time and Check-out Time should be 90 min difference.");
                            flag = false;
                            return;
                        }

                    }
                    objhd.EmpCode = $scope.HotelDetails[i].EmpCode;
                    objhd.HotelCity = $scope.HotelDetails[i].multicity[j].city.toUpperCase();
                    objhd.CheckInDate = $scope.Convertyyyymmdd($scope.HotelDetails[i].multicity[j].checkinDate);
                    objhd.CheckOutDate = $scope.Convertyyyymmdd($scope.HotelDetails[i].multicity[j].checkoutDate);
                    objhd.CheckinTime = $scope.HotelDetails[i].multicity[j].checkInTime;
                    objhd.CheckoutTime = $scope.HotelDetails[i].multicity[j].checkOutTime;
                    objhd.AgencyCode = "NA";
                    objhd.TravelDate = "NA";
                    objhd.FlightNo = "NA";
                    objhd.Sector = "NA - " + $scope.HotelDetails[i].multicity[j].city.toUpperCase();
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
                    hotelDetailsList.push(objhd);
                }
            }
        }
        return hotelDetailsList;
    }

    $scope.ConvertTimeto24 = function (time) {
       
        var hh = parseInt(time.split(' ')[0]);
        if (time.split(' ')[3] == "PM") {
             hh = parseInt(time.split(' ')[0]) + 12;
        }
        var mm = parseInt(time.split(' ')[2]);
        var Totaltime = hh * 60 + mm;
        return Totaltime;
    }
    $scope.ConvertTime = function (time) {
        var h = time.split(':')[0].trim();
        var mm = time.split(':')[1].trim();
        if (h.length != 2) {
            var hh = "0" + h;
        }
        else {

            var hh = h;
        }
        var totaltime = hh + ":" + mm;
        return totaltime;
    }

    //convert MM/dd/yyyy
    $scope.Convertyyyymmdd = function (dval) {
        var mval = (dval.split('/')[2] + '-' + dval.split('/')[1] + '-' + dval.split('/')[0]);
        return mval;
    };

    //Convertyyyymmdd
    $scope.getAngularDate = function (dval) {
        return new Date(parseInt(dval.substring(6, 19)));
    }
    $scope.getUserDetails();
    $scope.getPaxDetails();
    $scope.getOATBookingRight();

    $(function ($) {
        var options = {
            minimum: 1,
            maximize: 10,
            onChange: valChanged,
            onMinimum: function (e) {
                //console.log('reached minimum: ' + e)
            },
            onMaximize: function (e) {
                //console.log('reached maximize' + e)
            }
        }
        $('#handleCounter').handleCounter(options)
        $("#plusBtn").attr("disabled", true);
    })
    function valChanged(d) {
        //            console.log(d)
    }

    $scope.goNextTab = function (index) {
        for (var i = 0; i <= $scope.noOfPassanger; i++) {
            if ($('#Isflight' + index).prop('checked') == true) {
            }
        }
    }
})
app.directive("datepicker", function ($parse) {
    function link(scope, element, attrs) {
        // CALL THE "datepicker()" METHOD USING THE "element" OBJECT.

        element.datepicker({
            dateFormat: "dd/mm/yy",
            changeYear: true,
            changeMonth: true,
            defaultDate: new Date(),
            minDate: new Date()
        });
        if (attrs.minDate != null && attrs.minDate != "") {
            scope.$watch($parse(attrs.minDate), function (newval) {
                element.datepicker("option", "minDate", newval);
            });
        }
        if (attrs.maxDate != null && attrs.maxDate != "") {
            scope.$watch($parse(attrs.maxDate), function (newval) {
                if (attrs.maxDate == attrs.minDate) {
                    newval = (newval.split('/')[1] + '/' + newval.split('/')[0] + '/' + newval.split('/')[2]);
                    var newDate = new Date(newval);
                    newDate.setDate(newDate.getDate() + 5);
                    element.datepicker("option", "maxDate", newDate);
                } else {
                    //newDate = attrs.maxDate;
                    element.datepicker("option", "maxDate", newval);
                }
            });
        }
    }

    return {
        require: 'ngModel',
        link: link
    };
});
app.directive("inputDisabled", function () {
    return function (scope, element, attrs) {
        scope.$watch(attrs.inputDisabled, function (val) {
            if (val)
                element.removeAttr("disabled");
            else
                element.attr("disabled", "disabled");
        });
    }
});
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






