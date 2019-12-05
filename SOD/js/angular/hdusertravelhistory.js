//Employee Help Desk Module :Passenger Booking Linst Info
var app = angular.module('UserTrHistory', ['ui.bootstrap','ngSanitize']);

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
app.controller("TrHistory", function ($scope, $http, $timeout) {
    function loaddata() {
        $http({
            method: "GET",
            url: "../hd/GetEmployeeBookingHistory?EmpId=0"
        }).then(function mySucces(response) {
            $scope.trlist = response.data.bookingList;//fill employee booking details data
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 5; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
            $scope.totalItems = $scope.trlist.length;
            $scope.deptlist = response.data.deptList;//fill department list data
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

        $scope.filterdept = function (val) {
            $timeout(function () {
                $scope.search = $scope.searchDept.dept_name;
                $scope.filteredItems = $scope.searchDept.dept_name.length;
            }, 1);
            //$scope.search = "";
        };

        $scope.sort_by = function (predicate) {
            $scope.predicate = predicate;
            $scope.reverse = !$scope.reverse;
        };

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
                $scope.pscount = response.data["bookingInfo"][0].Passengers;
                $scope.mealdata = response.data["mealsInfo"];
                $scope.flightData=response.data["flightInfo"];
                $scope.customerpaid = response.data["bookingInfo"][0].IsAmountPaidByTraveller == true ? " -  Passenger Paid Booking" : "";
                $scope.arrHotel = response.data["hotelInfo"];
                $scope.arrCab = response.data["cabInfo"];
                allocateMeal($scope.pscount, $scope.arrpass, $scope.mealdata, $scope.flightData, $scope.arrbooking[0]);
            }, function myError(response) {
                 
            });
        };

        //Convert date script date formt for display
        $scope.ToJavaScriptDate = function (value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
        };
    }
     loaddata();
});
    

//Allocate Meal info
function allocateMeal(pscount,passinfo, mealdata, flightData,bookinginfo) {
    $("#tblviewPassInfo").empty();
    var trmeal = "";
    if (bookinginfo.SodBookingTypeId == 1) {
        trmeal = $("<tr class='popheadcolor_red' style='font-weight:bold;'/>");
        trmeal.append("<td>Traveller Name</td><td>Designation</td><td>Department</td>");
        $('#tblviewPassInfo').append(trmeal);
        trmeal = "";
        trmeal = $('<tr/>');
        trmeal.append("<td>" + bookinginfo.Title + " " + bookinginfo.RequestedEmpName + "</td>");
        trmeal.append("<td>" + bookinginfo.RequestedEmpDesignation + "</td>");
        trmeal.append("<td>" + bookinginfo.RequestedEmpDept + "</td>");
        $('#tblviewPassInfo').append(trmeal);
    }
    else if (bookinginfo.SodBookingTypeId == 2) {
        trmeal = $("<tr class='popheadcolor_red' style='font-weight:bold;'/>");
        trmeal.append("<td>Passenger No.</td>");

        for (var col = 0; col < flightData.length; col++) {
            trmeal.append("<td>" + flightData[col].OriginPlace + '-' + flightData[col].DestinationPlace + "</td>");
        }
        $('#tblviewPassInfo').append(trmeal);

        trmeal = "";
        var cols = $("#tblviewPassInfo").find("tr:first td");

        for (var i = 1; i <= pscount; i++) {
            trmeal = $('<tr/>');
            trmeal.append("<td>"+ passinfo[i - 1].Title +". " + passinfo[i - 1].TravelerFirstName + " " + passinfo[i - 1].TravelerLastName + "</td>");
            for (var t = 1; t < cols.length; t++) {
                var counter = 0;
                for (var j = 0; j < mealdata.length; j++) {
                    if (cols[t].innerText == mealdata[j].Sector && mealdata[j].PassengerId == i && counter==0) {
                        trmeal.append("<td>" + mealdata[j].MealType + "</td>");
                        counter++;
                    }
                    else if (cols[t].innerText == mealdata[j].Sector && mealdata[j].PassengerId == 0 && counter == 0) {
                        trmeal.append("<td>" + mealdata[j].MealType + "</td>");
                        counter++;
                    }
                }
            }
            $('#tblviewPassInfo').append(trmeal);
        }
    }
}


//Download data in an Excel Format
app.controller("hdController", function ($scope, $http, $timeout) {
    function loadfile() {
        $scope.list_sodname = {
            data: [
                { name: 'SOD' },
                { name: 'NON-SOD' }]
        }
        $scope.downloadData = function (EmpId) {
            $http({
                method: "GET",
                url: "../hd/GetEmployeeBookingHistory?EmpId=" + EmpId
            }).then(function mySucces(response) {
                window.location = '../hd/DownloadExl?file=' + response;
            }, function myError(response) {
                //$scope.myWelcome = response.statusText;
            });
        };
        //Test Download
        $scope.downloadData1 = function () {
            $http({
                method: "GET",
                url: "../hd/ExportListFromTsv/"
            });
        };
    }

    loadfile();
});

