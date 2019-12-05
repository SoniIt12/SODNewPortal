//Employee Help Desk Module :Passenger Booking Linst Info
var app = angular.module('HRChangeRequest', ['ui.bootstrap','ngSanitize']);

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

app.controller("HRReqHistory", function ($scope, $http, $timeout) {
    $scope.historyHR = function () {
    }

    function loaddata() {
        $http({
            method: "GET",
            url: "../ChangeReq/HRRightsData"
        }).then(function mySucces(response) {
            $scope.trlist = response.data;//fill employee booking details data
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 5; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
            $scope.totalItems = $scope.trlist.length;
            response.data.length > 0 ? $('#HrSearch').css('display', 'block') : $('#HrSearch').css('display', 'none');
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });


        $http({
            method: "GET",
            url: "../ChangeReq/HRRightsDataHistory"
        }).then(function mySucces(response) {
            $scope.trlisthr = response.data;//fill employee booking details data
            $scope.currentPagehr = 1; //current page
            $scope.entryLimithr = 5; //max no of items to display in a page
            $scope.filteredItemshr = $scope.trlisthr.length; //Initially for no filter  
            $scope.totalItemshr = $scope.trlisthr.length;
          
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
        $scope.setPage = function (pageNo) {
            $scope.currentPage = pageNo;
        };
        $scope.setPagehr = function (pageNo) {
            $scope.currentPagehr = pageNo;
        };

        $scope.filter = function () {
            $timeout(function () {
                $scope.filteredItems = $scope.filtered.length;
            }, 1);
        };
        $scope.filterhr = function () {
            $timeout(function () {
                $scope.filteredItemshr = $scope.filteredhr.length;
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

        //Convert date script date formt for display
        $scope.ToJavaScriptDate = function (value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
        };

        //new function added
        $scope.InformtoFinance = function (ReqId, status)
        {
            $.confirm({
                title: status,
                content: 'Do you want to continue!!',
                theme: 'bootstrap', 
                buttons: {
                    Ok: function () {
                         $http({
                             method: 'post',
                             url: "../ChangeReq/SendChangeRequestToFinance",
                             data: { ReqId: ReqId, status: status }
                                }).then(function (response) {
                                    $.alert({
                                        title: 'Email sent Confirmation',
                                        content: status
                                    });

                                   loaddata();
                                 }, function (response) {
                                });
                    },
                    Cancel: function () {
                    }

                }
            });
           
        };

    }

    loaddata();
});
 