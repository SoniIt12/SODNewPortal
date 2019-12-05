var app = angular.module('OatViewDetails', []);

app.controller("IthRensonse", function ($scope, $http, $timeout, $filter) {
    $scope.arrIthNewDetail = [];
    $scope.fileName = false;
    $scope.files = [];
    $scope.hodDiv = false;
    $scope.forms = {};
    $scope.getIthResponseByHod = [];
    $scope.selectedId = [];
    $scope.submitIthForm = false;
    $scope.fileSizeError = "false";
    $scope.successResponse = false;
    $scope.FinancialApprovalDiv = false;
    $scope.FinancialApprovalList = [];
    var types = "";
    

    $scope.getDetailIthResponse = function (oatreqId) {
        $('.loader').show();
        var name = window.location.href;
        var params = new URL(name).searchParams;
        var reqId = params.get('str');
        types = params.get('type');
        $scope.type = types;
        $scope.oatReqId = reqId.split(',')[0];
        var ithTransactionID = reqId.split(',')[1];
        $http({
            method: "POST",
            url: "../OatDesk/GetDetailForIthResponse",
            data: JSON.stringify({ 'oatReqId': $scope.oatReqId, 'ithTransactionID': ithTransactionID, 'type': types }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('.loader').hide();
            $scope.arrPassenger = response.data["PassengerDetail"];
            $scope.arrIthDetail = response.data["IthDetails"];
            $scope.arrSecDetail = response.data["SectorDetail"];
            $scope.allcancel = response.data["allcancel"];
            if (types == 'ORa') {
                $scope.arrIthNewDetail.push({ 'response': $scope.arrIthDetail });
                for (var n = 0; n < $scope.arrIthDetail.length; n++) {
                    var index = n + 1;
                    var idDeparture = "departureTimePicker" + index + "0";
                    var idArrival = "arrivalTimePicker" + index + "0";
                    var dept = $scope.arrIthDetail[n].DepartureTime.split(" ")[0] + " : " + $scope.arrIthDetail[n].DepartureTime.split(" ")[2];
                    $scope.timepickerRevertedData(idDeparture, dept);
                    var arr = $scope.arrIthDetail[n].ArrivalTime.split(" ")[0] + " : " + $scope.arrIthDetail[n].ArrivalTime.split(" ")[2];
                    $scope.timepickerRevertedData(idArrival, arr);
                }
            }
            else if (types == 'ua' || types == 'ha') {
                $scope.hodDiv = true;
                if (types == 'ua') {
                    $http({
                        method: "POST",
                        url: "../OatDesk/GetFinancialApproval",
                        data: JSON.stringify({ 'ReqId': $scope.oatReqId }),
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8'
                    }).then(function mySucces(response) {
                        if (response.data["IsAmountGreater"]) { $scope.FinancialApprovalDiv = true };
                        $scope.FinancialApprovalList = response.data["FinancialApproval"];
                        //$scope.successResponse = true;
                    }, function myError(response) {
                        $scope.myWelcome = response.statusText;
                    });
                    //var sdg = '';
                    //ng - init = "roomId = '@ViewBag.IsAmountGreater'";
                    ////ng - init="sdg = '@ViewBag.IsAmountGreater'";
                    ////var sdg = @ViewBag.IsAmountGreater;
                    //var sdgfg = @(ViewBag.IsAmountGreater);
                    ////var test =@(ViewBag.IsAmountGreater.ToString().ToLower());
                    //    if ('@ViewBag.IsAmountGreater')
                    //    { $scope.FinancialApprovalDiv = true };
                }
            }
            else {
                for (var n = 0; n < $scope.arrSecDetail.length; n++) {
                    $scope.arrIthNewDetail.push({ 'response': [] });
                    if ($scope.arrSecDetail[n].IsFlightCancel != true) {
                        $scope.arrIthNewDetail[n].response.push($scope.arrSecDetail[n]);
                        $scope.arrIthNewDetail[n].response.push({ 'DepartureDate': $scope.arrSecDetail[n].DepartureDate, 'PassengerID': $scope.arrSecDetail[n].PassengerID, 'DestinationPlace': $scope.arrSecDetail[n].DestinationPlace, 'OriginPlace': $scope.arrSecDetail[n].OriginPlace });
                        $scope.arrIthNewDetail[n].response.push({ 'DepartureDate': $scope.arrSecDetail[n].DepartureDate, 'PassengerID': $scope.arrSecDetail[n].PassengerID, 'DestinationPlace': $scope.arrSecDetail[n].DestinationPlace, 'OriginPlace': $scope.arrSecDetail[n].OriginPlace });
                        var DeptTime = $scope.arrSecDetail[n].DepartureTime.split('-')[0].trim();
                        var departureTime = DeptTime.split(":")[0] +" : "+ (DeptTime.split(":")[1]).trim().split(" ")[0];
                        $scope.timepickerInitialize(n, departureTime);
                    }
                }
            }
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
            alert("Oops! Something went wrong.");
            $('.loader').hide();
        });
    }

    $scope.timepickerRevertedData = function (id, departureTime) {
        setTimeout(function () {
            $('#' + id).wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false, now: departureTime
            }, 100);
        })
    }

    $scope.timepickerInitialize = function (n, departureTime) {
        setTimeout(function () {
            if (departureTime != "Any") {
                $('#departureTimePicker1' + n).wickedpicker({
                    twentyFour: false, title:
                        'Select Time', showSeconds: false, now: departureTime
                });
            } else {
                $('#departureTimePicker1' + n).wickedpicker({
                    twentyFour: false, title:
                        'Select Time', showSeconds: false
                });
            }

            $('#arrivalTimePicker1' + n).wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false
            });
            $('#departureTimePicker2' + n).wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false
            });
            $('#arrivalTimePicker2' + n).wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false
            });
            $('#departureTimePicker3' + n).wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false
            });
            $('#arrivalTimePicker3' + n).wickedpicker({
                twentyFour: false, title:
                    'Select Time', showSeconds: false
            });
        }, 100);
    }

    $scope.getangularDate = function (dval) {
        var length = dval.length - 2;
        return new Date(parseInt(dval.substring(6, length)));
    }

    $scope.getFileName = function (index) {
        var fileIndex = $('#getIndexValue' + index).text();
        var finalIndex = fileIndex == "" ? index : fileIndex;
        $scope.file = $('#file-upload' + finalIndex)[0].files[0];
        if ($scope.file.size > 1048576) {
            $('#fileName_' + finalIndex).hide();
            $('#fileSize_' + finalIndex).show();
            $('#fileSize_' + finalIndex).html("File size should be less than 1 MB.");
            return false;
        } else if (($scope.file.type.includes("image")) || ($scope.file.type.includes("pdf"))) {
            $('#fileSize_' + finalIndex).hide();
            $('#fileName_' + finalIndex).show();
            $('#fileName_' + finalIndex).html($scope.file.name);

        } else {
            $('#fileName_' + finalIndex).hide();
            $('#fileSize_' + finalIndex).show();
            $('#fileSize_' + finalIndex).html("File should be Image or PDF.");
            return false;
        }
        $scope.files.push($scope.file);
    }

    $scope.submitIthResponse = function (arrIthNewDetail) {
        $("#submitIthResponsebtn").prop('disabled', true);
        if (!$scope.forms.IthResponseForm.$valid) {
            $scope.submitIthForm = true;
            $("#submitIthResponsebtn").prop('disabled', false);
        }
        else {
            $('.loader').show();
            $scope.submitIthForm = true;
            //$scope.count = $scope.arrIthDetail.length;           
            $scope.jsonData = [];
            for (var i = 0; i < arrIthNewDetail.length; i++) {
                var countOfFile = 0;
                for (var j = 0; j < arrIthNewDetail[i].response.length; j++) {
                    arrIthNewDetail[i].response[j].OATRequestID = $scope.oatReqId;
                    countOfFile = arrIthNewDetail[i].response[j].ITHUploadRefFiles == undefined ? countOfFile : countOfFile + 1;
                    $scope.jsonData.push(arrIthNewDetail[i].response[j]);
                }
                if (countOfFile < 1 && arrIthNewDetail[i].response.length != 0) {
                    var msg = "";
                    $scope.arrSecDetail.length == 1 ? msg = "Please Upload supportive ducoment in valid format with valid size." : msg = "Please Upload supportive ducoment in valid format with valid size for each sector."
                    alert(msg);
                    $("#submitIthResponsebtn").prop('disabled', false);
                    $('.loader').hide();
                    return false;
                }
            }
            $('#submitIthResponsebtn').attr("disabled", true);

            $http({
                method: 'POST',
                url: "http://localhost:52809/PostFileAttachmentWithData",
                // url: "http://172.30.47.129/SpiceJetSODApplication4x/PostFileAttachmentWithData",
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    var formData = new FormData();
                    formData.append("model", angular.toJson(data));
                    for (var i = 0; i < data.files.length; i++) {
                        formData.append("file" + i, data.files[i]);
                    }
                    return formData;
                },
                data: { model: $scope.jsonData, files: $scope.files }
            }).
                success(function (data, status, headers, config) {
                    //alert(JSON.parse(data));
                    // $scope.successResponse = true;
                    $scope.successResponse = true;
                    $scope.SuccessMsg = JSON.parse(data);
                    $('#submitIthResponsebtn').attr("disabled", false);
                    //window.location.reload();
                    // $("#submitIthResponsebtn").prop('disabled', false);
                    $('.loader').hide();

                }).
                error(function (data, status, headers, config) {
                    $("#submitIthResponsebtn").prop('disabled', false);
                    $('.loader').hide();
                    alert(JSON.parse(data.message));
                });
        }
    };

    $scope.getSelectedIthResponse = function (list, index) {
        $scope.FinancialApprovalDiv = false;
        for (var i = 0; i < $scope.arrIthDetail.length; i++) {
            //list.ID = $scope.selectedId[i];
            if ($scope.arrIthDetail[i].ID == list.ID && list.Amount >= 8000) {
                $scope.FinancialApprovalDiv = true;
            }
            else {
                if ($scope.arrIthDetail[i].OriginPlace == list.OriginPlace && $scope.arrIthDetail[i].DestinationPlace == list.DestinationPlace)
                    $scope.arrIthDetail[i].selectedOption = 0;
            }
        }

        return $scope.FinancialApprovalDiv;

    }

    $scope.submitHodResponse = function (seletedList) {
        // var list = $scope.getIthResponseByHod;
        var list = [];
        var flist = [];
        var listTosend = [];
        if (types == 'ua' && !$scope.FinancialApprovalDiv && $scope.FinancialApprovalList.length > 0) {
            var response = $scope.getSelectedIthResponse(seletedList);
            if (response) return false;
        }
        for (var i = 0; i < seletedList.length; i++) {
            //list.ID = $scope.selectedId[i];
            if (seletedList[i].selectedOption == 1) {
                list.push(seletedList[i]);
            }
        }

        for (var i = 0; i < $scope.arrSecDetail.length; i++) {
            if (!$scope.arrSecDetail[i].IsFlightCancel) {
                listTosend.push($scope.arrSecDetail[i]);
            }
        }


        //list.ID = $scope.selectedId;
        if (list.count == 0) {
            alert("Please select your option.");
            return false;
        }
        else if (list.length != listTosend.length) {
            alert("Please select one option for each sector.");
            return false;
        }
        else if (types == 'ua') {
            for (var i = 0; i < $scope.FinancialApprovalList.length; i++) {
                if ($scope.FinancialApprovalList[i].selectedOption == 1) {
                    flist.push($scope.FinancialApprovalList[i]);
                }
            }
            if (flist.count == 0) {
                alert("Please select your option for financial approval.");
                return false;
            }
        }

        //else if (types == 'ua' || types == 'ha') {
        //    $scope.hodDiv = true;
        //}
        $('#Hodbtn').attr("disabled", true);
        var urlLink = (types == 'ua') ? "../OatDesk/SubmitUserResponse" : "../OatDesk/SubmitHodResponse";
        $http({
            method: "POST",
            url: urlLink,
            data: JSON.stringify({ 'SelectedResponse': list, 'FinancialApproval': flist }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('#Hodbtn').attr("disabled", true);
            $scope.successResponse = true;
            $scope.SuccessMsg = response.data;
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    }

    $scope.getDetailIthResponse();
});