var app = angular.module('OatViewDetails', []);

app.controller("IthRensonse", function ($scope, $http, $timeout, $filter) {
    $scope.files = [];
    $scope.successResponse = false;
    $scope.StartPage = false;

    $scope.getDetailIthResponse = function (oatreqId) {
        $('.loader').show();
        var name = window.location.href;
        var params = new URL(name).searchParams;
        var reqId = params.get('str');
        types = params.get('type');
        $scope.oatReqId = reqId.split(',')[0];
        var ithTransactionID = reqId.split(',')[1];
        $http({
            method: "POST",
            url: "../OatDesk/GetDataForFinalBookingForm",
            data: JSON.stringify({ 'oatReqId': $scope.oatReqId }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('.loader').hide();
            //$scope.arrPassenger = response.data["PassengerDetail"];
            //$scope.arrIthDetail = response.data["IthDetails"];
            $scope.StartPage = true;            
            $scope.flightStatus = response.data["flightStatus"];
            $scope.arrSecDetail = response.data["SectorDetail"];
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
            alert("Oops! Something went wrong.");
            $('.loader').hide();
        });
    }

    $scope.getangularDate = function (dval) {
        var length = dval.length - 2;
        return new Date(parseInt(dval.substring(6, length)));
    }

    $scope.getFileName = function (index) {
        // var fileIndex = $('#getIndexValue' + index).text();
        var finalIndex = index;//fileIndex == "" ? index : fileIndex;
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

    $scope.SendItenaryToTravelDesk = function (list) {        
        $('.loader').show();       
        $scope.jsonData = [];
        for (var i = 0; i < list.length; i++) {
            if (list[i].PNR == "" || list[i].PNR == null) {
                alert("Please give pnr for each passenger.");
                $('.loader').hide();
                return false;
            }
            list[i].Remarks = $("#IthComment").val();
        }
       // list[0].EntryDate = $("#IthComment").val();
        if ($scope.files.length != list.length) {
            alert("Please Upload all document in valid format with valid size.");
            // $("#submitIthResponsebtn").prop('disabled', false);
            $('.loader').hide();
            return false;
        }
        $('#sendItenaryBtn').attr("disabled", true);
        
        $http({
            method: 'POST',
            url: "http://localhost:52809/PostItenarybooking",
            headers: { 'Content-Type': undefined },
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data));
                for (var i = 0; i < data.files.length; i++) {
                    formData.append("file" + i, data.files[i]);
                }
                return formData;
            },
            data: { model: list, files: $scope.files }
        }).
            success(function (data, status, headers, config) {
                alert(JSON.parse(data));
                $scope.SuccessMsg = JSON.parse(data);
                $scope.successResponse = true;
                $("#submitIthResponsebtn").prop('disabled', false);
                $('#sendItenaryBtn').attr("disabled", true);
                $('.loader').hide();

            }).
            error(function (data, status, headers, config) {
                alert(JSON.parse(data));
                $("#submitIthResponsebtn").prop('disabled', false);
                $('.loader').hide();
            });
        //}
    };
    //$scope.getDetailIthResponse = function () {
    //    $('.loader').show();
    //    var name = window.location.href;
    //    var params = new URL(name).searchParams;
    //    var reqId = params.get('str');
    //    types = params.get('type');
    //    $scope.oatReqId = reqId.split(',')[0];
    //    var ithTransactionID = reqId.split(',')[1];
    //    $http({
    //        method: "POST",
    //        url: "../OatDesk/GetDetailForIthResponse",
    //        data: JSON.stringify({ 'oatReqId': $scope.oatReqId, 'ithTransactionID': ithTransactionID, 'type': types }),
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8'
    //    }).then(function mySucces(response) {
    //        $('.loader').hide();
           
    //    }, function myError(response) {
    //        $scope.myWelcome = response.statusText;
    //        alert("Oops! Something went wrong.");
    //        $('.loader').hide();
    //    });
    //}
    //$scope.getDetailIthResponse();
    $scope.getDetailIthResponse();
});