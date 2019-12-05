var app = angular.module("MasterApp", ['ui.bootstrap', 'ngSanitize']);

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

app.controller("cityMasterCtrl", function ($scope, $http, $timeout, $interval) {
 
    function loaddata() {
        $http({
            method: "GET",
            url: "../city/GetCityListData"
        }).then(function mySucces(response) {
            $scope.cityList = response.data;
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.cityList.length; //Initially for no filter  
            $scope.totalItems = $scope.cityList.length;

        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    };

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

 
    $scope.addRow = function () {
        if ($('#txtcitycode').val() == '') {
            alert('Please enter city code.');
            $('#txtcitycode').focus();
            return false;
        }
        if ($('#txtcityname').val() == '') {
            alert('Please enter city name.');
            $('#txtcityname').focus();
            return false;
        }
        if ($('#txtcitytype').val() == '') {
            alert('Please enter city type.');
            $('#txtcitytype').focus();
            return false;
        }
        for (var i = 0; i < $scope.cityList.length; i++) {
            if ($scope.cityList[i].CityCode.toLowerCase() == $('#txtcitycode').val().toLowerCase()) {
                alert('This city code - ' + $('#txtcitycode').val() +' has already exists.' );
                $('#txtcitycode').val('');
                $('#txtcityname').val('');
                $('#txtcitytype').val('');
                return false;
            }
        }
       
        var result = confirm("Are you sure to add this new city ?");
        if (result) {
            var arr = new Array();
            var obj = new Object();
            obj.cityCode = $('#txtcitycode').val();
            obj.cityName = $('#txtcityname').val();
           
            obj.Type = $('#txtcitytype').val();           
            arr.push(obj); 

            $http({
                method: "POST",
                url: "../city/AddNewCity",
                data: JSON.stringify({ elist: arr}),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {               
                alert('Data has been successfully saved.'); 
                $('#txtcitycode').val('');
                $('#txtcityname').val('');
                $('#txtcitytype').val('');
                $('#viewdetailModalofCity').modal('toggle');
                loaddata();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    };

    $scope.updateRow = function (City) {
        var result = confirm("Are you sure to update this City list ?");
        if (result) {
            //console.log(City)
            var arr = new Array();
            var obj = new Object();
            obj.Id = City.Id;
            obj.cityCode = City.CityCode;
            obj.cityName = City.CityName;
            obj.Type = City.Type ;            
           arr.push(obj);

            $http({
                method: "POST",
                url: "../city/UpdateCityList",
                data: JSON.stringify({ elist: arr }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('#viewdetailModal').modal('toggle');
                alert('Data has been successfully updated.');
                
                loaddata();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    };

    $scope.editRow = function () {
        $("#txtHeader1").hide();
        $("#txtRow1").show();
        $("#txtHeader2").hide();
        $("#txtRow2").show();
        $(".txtHeader3").hide();  
        $("#editbtntd").hide();
        $("#updatebtntd").show();
    };

    $scope.viewdetail = function (Id) {
        $("#txtHeader1").show();
        $("#txtRow1").hide();
        $http({
            method: "GET",
            url: "../city/GetCityInfoById?Id=" + Id
        }).then(function mySucces(response) {
            $scope.viewCityDetail = response.data.cityPopupDetails;            
        }, function myError(response) {
        });

        $("#editbtntd").show();
        $("#updatebtntd").hide();

    };  
    loaddata();
});

