var app = angular.module("VendorApprovalApp", ['ui.bootstrap', 'ngSanitize']);

app.filter('startFrom', function ($timeout) {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    };
});

app.filter('nlfilter', function (obj) {
    return function (text) {
        text = String(text).trim();
        return (text.length > 0 ? '<p>' + text.replace(/[\r\n]+/g, '</p><p>') + '</p>' : null);
    };
});

app.controller("VendorApprovalCtrl", function ($scope, $http, $timeout, $interval, $window) {
    $scope.addedPassanger = [];
    $scope.totalSelected = 0;
    $scope.noOfPax = 0;
    $('.loader').show();
    function loaddata() {
        $http({
            method: "GET",
            url: "../nsvdapp/GetVendorApprovalList"
        }).then(function mySucces(response) {
            $scope.VendorList = response.data;
            $('.loader').hide();
            $scope.hideButton = false;
            for (var i = 0; i < response.data.length; i++) {
                $scope.VendorList[i].AddDate = new Date(parseInt((response.data[i].AddDate).substring(6)));
                if (!$scope.VendorList[i].IsApproved) {
                    $scope.hideButton = true;
                }
                else {
                    $scope.hidebutton = true;
                }
            }
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.VendorList.length; //Initially for no filter  
            $scope.totalItems = $scope.VendorList.length;
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    }

    $scope.GetAprrovedList = function () {
        $http({
            method: 'GET',
            url: "../nsvdflightbooking/GetApprovedList",
        }).then(function mySucces(response) {
            $scope.ApprovedVendorList = response.data;
            $scope.addedPassanger = $scope.ApprovedVendorList;
            for (var i = 0; i < $scope.addedPassanger.length; i++) {
                $scope.addedPassanger[i].AddDate = new Date(parseInt((response.data[i].AddDate).substring(6)));
            }
        })
    }, function myError(response) {
        $scope.myWelcome = response.statusText;
    };

    $scope.filterValue = function ($event) {
        if (isNaN(String.fromCharCode($event.keyCode))) {
            $event.preventDefault();
        }
    };

    $scope.updateTotal = function (list) {
        console.log(list);
        if (list) {
            $scope.totalSelected++;
        }
        else {
            $scope.totalSelected--;
        }
    }
    $scope.IsDisabled = true;
    $scope.AddPassengers = function (list) {
        var obj = new Array();
        for (var i = 0; i < list.length; i++) {
            if (list[i].Ischecked) {
                obj.push(list[i]);
                $scope.addedPassanger[i].selected = true;
                $scope.IsDisabled = false;
            }
            $scope.noOfPax = $scope.totalSelected;
        }
        if (obj.length == 0) {
            $scope.IsDisabled = true;
            alert("Please select minimum one vendor");
            return false;
        }
        $http({
            method: 'POST',
            url: "../VDBookingDetail/AddPassengers",
            data: JSON.stringify({ addlist: obj }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $timeout(function () {
                $("#viewdetailModal .close").click()
            }, 1000);
            alert("Passengers added successfully");
            console.log(obj);
        }), function myError(response) {
            alert(response.data);
        }
    }
    $scope.getPassenger = function (passenegerList) {
        $http({
            method: 'GET',
            url: "../VDBookingDetail/getPassengerList",
        }).then(function mySucces(response) {
            $scope.getPassenger = response.data;
        }), function myError(response) {
            alert(response.data);
        }
    }

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.hidebuttons = true;
    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered.length;
            for (var i = 0; i <= $scope.filtered.length; i++) {
                if ($scope.filteredItems == 0 || $scope.filtered[i].IsApproved == 1) {
                    $scope.hidebuttons = false;
                    if ($scope.search.length == "") {
                        $scope.hidebuttons = true;
                        return false;
                    }
                }
                else {
                    $scope.hidebuttons = true;
                    return false;
                }
            }
        }, 1);
    };

    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };

    $scope.Response = function (list, status) {
        $('.loader').show();
        console.log($scope.loading);
        var obj = new Array();
        for (var i = 0; i < list.length; i++) {
            if (list[i].Ischecked) {
                obj.push(list[i]);
            }
        }
        if (obj.length == 0) {
            $('.loader').hide();
            alert("Please select atleast one vendor from the list");
            return false;
        }
        if (status == "approved") {
            var result = confirm("Are you sure to approve the vendor");
            if (result) {
                $http({
                    method: "POST",
                    url: "../nsvdapp/ApproveAndRejectVendor",
                    async: false,
                    data: { 'mailerlist': obj, 'Status': status }
                }).then(function mySucces(response) {
                    $('.loader').hide();
                    if (response.data != "") {
                        alert(JSON.parse(response.data));
                    }
                }, function myError(response) {
                    $('.loader').hide();
                    alert(response.data);
                })
                $window.location.reload();
            }
            else
                $('.loader').hide();
        }
        else
            var result = confirm("Are you sure to reject the vendor");
        if (result) {
            $http({
                method: "POST",
                url: "../nsvdapp/ApproveAndRejectVendor",
                async: false,
                data: { 'mailerlist': obj, 'Status': status }
            }).then(function mySucces(response) {
                $('.loader').hide();
                if (response.data != "") {
                    alert(JSON.parse(response.data));
                }
            }, function myError(response) {
                $('.loader').hide();
                alert(response.data);
            })
            $window.location.reload();
        }
        else
            $('.loader').hide();
    }

    loaddata();
    $scope.GetAprrovedList();
    $scope.getPassenger();
});
  