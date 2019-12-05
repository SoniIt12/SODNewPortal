//Employee Help Desk Module :Passenger Booking Linst Info
var app = angular.module('EmpViewDetails', ['ui.bootstrap', 'ngSanitize']);

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

//Get Employee Details
app.controller("EmpView", function ($scope, $http, $timeout) {
    $scope.loaddata = function (search) {        
        $http({
            method: "GET",
            url: "../hd/GetEmployeeViewDetails?EmpId="+search
        }).then(function mySucces(response) {
            //$scope.totalRecords();
            $scope.trlist = response.data.emplist;//fill employee details data
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Filtered no. of records 
            
            $scope.totalList = response.data.totalEmpList;//total records in table
            $scope.totalItems = $scope.totalList;
            

        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });

        $scope.setPage = function (pageNo) {
            $scope.currentPage = pageNo;
        };

        //$scope.filter = function () {
        //    $timeout(function () {
        //       $scope.filteredItems = $scope.filtered.length;
        //    }, 1);
        //};

        $scope.sort_by = function (predicate) {
            $scope.predicate = predicate;
            $scope.reverse = !$scope.reverse;
        };
 
    }
    
    $scope.empty = function () {
        alert("Enter keyword to search!");
    }

    $scope.refresh = function () {
        location.reload();
    }

    $("#search_textbox").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#btnSearch_blkList").click();
        }
    });
});


