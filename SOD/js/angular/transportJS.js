//Employee Help Desk Module :Passenger Booking Linst Info
var app = angular.module('TransHistory', ['ui.bootstrap', 'ngSanitize']);

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
app.controller("TransController", function ($scope,  $http, $timeout) {

    function loaddata() {
        $http({
            method: "GET",
            url: "../trns/GetEmpCabBookingInfo?EmpId=0"
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
        };

        $scope.sort_by = function (predicate) {
            $scope.predicate = predicate;
            $scope.reverse = !$scope.reverse;
        };

        $scope.showstatus = function (TravelRequestId) {
            $http({
                method: "GET",
                url: "../trns/GetCabStatus?reqId=" + TravelRequestId
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
                url: "../trns/GetBookingandCabInfo?trId=" + TravelRequestId
            }).then(function mySucces(response) {
                $scope.arrbooking = response.data["cabinfo"];
                $scope.txtremarks = response.data["cabinfo"][0].Remarks_Status;
                $scope.arrFlight = response.data["flightInfo"];
                $scope.arrpass = response.data["passInfo"];
                $scope.arrpassod = response.data["bookingInfo"];
                $scope.trequestId = TravelRequestId;
                $scope.cabdata = response.data["cabinfo"];
                $scope.flightData=response.data["flightInfo"];
            }, function myError(response) {
          });
        };


        //Approved Cab info
        $scope.appcabinfo = function (TravelRequestId)
        {
            if ($scope.txtremarks == 'undefined' || $scope.txtremarks == '' || $scope.txtremarks == null)
            {
                alert('Please enter approval remarks');
                $("#txtremarks").focus();
                return false;
            }
            var result = confirm("Are you sure! to approve this cab request ?");
            if (result) {
                $scope.printloading = true;
                var req = TravelRequestId + '|' + $scope.txtremarks;
                $http({
                    method: "POST",
                    url: "../trns/AppCabRequest?req=" + req
                }).then(function mySucces(response) {
                    alert('Cab Request has been approved successfully.');
                    $scope.printloading = false;
                    $scope.aaptext = "Approved";
                    $('#btnreject').hide();
                    $scope.emailNotification();
                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                });
            }
        }

        //Reject Cab Info
        $scope.rejectcabinfo = function (TravelRequestId) {
            if ($scope.txtremarks == 'undefined' || $scope.txtremarks == '' || $scope.txtremarks == null) {
                alert('Please enter rejection remarks.');
                $("#txtremarks").focus();
                return false;
            }
            var result = confirm("Are you sure! to reject this cab request ?");
            if (result) {
                $scope.rejectloading = true;
                var req = TravelRequestId + '|' + $scope.txtremarks;
                $http({
                    method: "POST",
                    url: "../trns/RejectCabRequest?req=" + req
                }).then(function mySucces(response) {
                    alert('Cab Request has been rejected successfully.');
                    $scope.rejectloading = false;
                    $scope.rejtext = "Rejected";
                    $('#btnsubmit').hide();
                    $scope.emailNotification();
                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                });
            }
        }
        
        //Send Email Notification 
        $scope.emailNotification = function () {
            $.ajax({
                url: "../trns/sendEmailNotification",
                success: function (result) {
                }
            });
        }

        //Convert date script date format for display
        $scope.ToJavaScriptDate = function (value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
        };
    }

    loaddata();
    $scope.btnsubmit = true;
    $scope.btnreject = true;
    $scope.aaptext = "Approve";
    $scope.rejtext = "Reject";
});
    

//Allocate Meal info
function allocateMeal(pscount,passinfo, mealdata, flightData,bookinginfo) {
    $("#tblviewPassInfo").empty();
    var trmeal = "";
    if (bookinginfo.SodBookingTypeId == 1) {
        trmeal = $("<tr class='popheadcolor' style='font-weight:bold;'/>");
        trmeal.append("<td>Traveler Name</td><td>Designation</td><td>Department</td>");
        $('#tblviewPassInfo').append(trmeal);
        trmeal = "";
        trmeal = $('<tr/>');
        trmeal.append("<td>" + passinfo[0].Title + " " + passinfo[0].TravelerFirstName + " " + passinfo[0].TravelerLastName + "</td>");
        trmeal.append("<td>" + bookinginfo.RequestedEmpDesignation + "</td>");
        trmeal.append("<td>" + bookinginfo.RequestedEmpDept + "</td>");
        $('#tblviewPassInfo').append(trmeal);
    }
    else if (bookinginfo.SodBookingTypeId == 2) {
        trmeal = $("<tr class='popheadcolor' style='font-weight:bold;'/>");
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

//Vendor Allocation & View
app.controller("TransVendorCtrl", function ($scope, $filter, $http, $timeout) {
    function loadvenderList() {
        $http({
            method: "GET",
            url: "../trns/GetVendorList?vCode=0"
        }).then(function mySucces(response) {
            debugger;
            $scope.trlist = response.data;
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 5; //max no of items to display in a page
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

        $scope.filterdept = function (val) {
            $timeout(function () {
                $scope.search = $scope.searchState.StateCode;
                $scope.filteredItems = $scope.searchState.StateCode.length;
            }, 1);
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
                url: "../trns/GetBookingandCabInfo?trId=" + TravelRequestId
            }).then(function mySucces(response) {
                $scope.arrbooking = response.data["cabinfo"];
                $scope.arrFlight = response.data["flightInfo"];
                $scope.arrpass = response.data["passInfo"];
                $scope.trequestId = TravelRequestId;
                $scope.pscount = response.data["bookingInfo"][0].Passengers;
                $scope.cabdata = response.data["cabinfo"];
                $scope.flightData = response.data["flightInfo"];
            }, function myError(response) {

            });
        };
    }

    loadvenderList();
});

//Uniq Filter
app.filter('unique', function () {
    return function (collection, keyname) {
        var output = [],
            keys = []
        found = [];

        if (!keyname) {
            angular.forEach(collection, function (row) {
                var is_found = false;
                angular.forEach(found, function (foundRow) {
                    if (foundRow == row) {
                        is_found = true;
                    }
                });
                if (is_found) { return; }
                found.push(row);
                output.push(row);

            });
        }
        else {
            angular.forEach(collection, function (row) {
                var item = row[keyname];
                if (item === null || item === undefined) return;
                if (keys.indexOf(item) === -1) {
                    keys.push(item);
                    output.push(row);
                }
            });
        }
        return output;
    };
});

