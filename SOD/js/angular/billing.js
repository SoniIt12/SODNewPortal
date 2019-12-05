//Module Name : Biling
//********Angular Code *********************************************************************
var app = angular.module('appbilling', ['ui.bootstrap', 'ngSanitize']);
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

//New Line Filter
app.filter('nlfilter', function () {
    return function (text) {
        text = String(text).trim();
        return (text.length > 0 ? '<p>' + text.replace(/[\r\n]+/g, '</p><p>') + '</p>' : null);
    }
});

//Billing Controller
app.controller("billingCtrl", function ($scope, $http, $timeout, $filter, $window) {
    function loaddata(prm) {
        var date =ConvertMMddyyyy($("#txtfromdate").val()) + "," + ConvertMMddyyyy($("#txttodate").val()) + "," + prm;
        $http({
            method: "GET",
            url: "../billing/GetHotelBillingList?prm=" + date
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
            $scope.pscount = response.data["bookingInfo"][0].Passengers;
            $scope.flightData = response.data["flightInfo"];
            $scope.arrHotel = response.data["hotelInfo"];
        }, function myError(response) {
       });
    };


    //show popup bulk view details
    $scope.viewdetailbkList = function (TRId) {
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
    $scope.getbillingList = function () {
        var isdt = $('#rdoind').is(':checked');
        var isbulk = $('#rdoblk').is(':checked');
        if (isdt) { loaddata(1); }
        if (isbulk) { loaddata(2);} 
    }

    $scope.downloadexcelBlist = function (prm) {
        window.location = '../billing/ExportBulkListFromTsv?prm=' + prm;
    };

    //Get HOD(s) Approval Status
    $scope.showstatus = function (TravelRequestId) {
        $http({
            method: "GET",
            url: "../billing/GetApprovalStatus?prm=" + TravelRequestId,
            async: false
        }).then(function mySucces(response) {
            var val = '';
            var s = response.data.toString().split(',');
            for (var i = 0; i < s.length; i++) {
                if (i == 0)
                    val = s[i] + ' \n';
                else
                    val = val + s[i] + '\n';
            }
            $scope.bstatus = val;
        }, function myError(response) {

        });
    };
});

//OatBlist Controller
 
app.controller("oatblistCtrl", function ($scope, $http, $timeout, $filter, $window) {
    function loaddata(prm) {
        var date = ConvertMMddyyyy($("#txtfromdate").val()) + "," + ConvertMMddyyyy($("#txttodate").val()) + "," + prm;
        $http({
            method: "GET",
            url: "../billing/GetFlightBillingList?prm=" + date
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
            $scope.pscount = response.data["bookingInfo"][0].Passengers;
            $scope.flightData = response.data["flightInfo"];
            $scope.arrHotel = response.data["hotelInfo"];
        }, function myError(response) {
        });
    };


    //show popup bulk view details
    $scope.viewdetailbkList = function (TRId) {
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


    //Hideshow Table
    $scope.hideTable = function () {
        $scope.trlists = null;
        $scope.trlist = null;
        $scope.currentPage = null;
        $scope.entryLimit = null;
        $scope.filteredItems = null;
        $scope.totalItems = null;
    };

    //Load Data Function 
    $scope.getoatbList = function () {
        var isdt = $('#rdoind').is(':checked');
        var isbulk = $('#rdoblk').is(':checked');
        if (isdt) { loaddata(1); }
        if (isbulk) { loaddata(2); }
    }
    $scope.downloadexcelBlist = function (prm) {
        window.location = '../billing/ExportBulkListFromTsv?prm=' + prm;
    };
    $scope.getAngularDate = function (dval) {
        return new Date(parseInt(dval.substring(6, 19)));
    }
    //Get HOD(s) Approval Status
    $scope.showstatus = function (TravelRequestId) {
        $http({
            method: "GET",
            url: "../billing/GetApprovalStatus?prm=" + TravelRequestId,
            async: false
        }).then(function mySucces(response) {
            var val = '';
            var s = response.data.toString().split(',');
            for (var i = 0; i < s.length; i++) {
                if (i == 0)
                    val = s[i] + ' \n';
                else
                    val = val + s[i] + '\n';
            }
            $scope.bstatus = val;
        }, function myError(response) {

        });
    };
});
 