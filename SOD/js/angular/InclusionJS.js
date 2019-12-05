var app = angular.module("InclusionApp", ['ui.bootstrap', 'ngSanitize']);

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

app.controller("InclusionCtrl", function ($scope, $http, $timeout, $interval) {

    function loaddata() {
        $http({
            method: "GET",
            url: "../InclusionMaster/GetHotelInclusionList"
        }).then(function mySucces(response) {
            $scope.incList = response.data;
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.incList.length; //Initially for no filter  
            $scope.totalItems = $scope.incList.length;
            
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
        if ($('#txtaccom').val() == '' || $('#txtaccom').val() == null ||
                $('#foodtxt').val() == '' || $('#foodtxt').val() == null ||
                $('#txtair').val() == '' || $('#txtair').val() == null ||
               $('#txtroomServ').val() == '' || $('#txtroomServ').val() == null ||
                $('#txtbuffetTym').val() == '' || $('#txtbuffetTym').val() == null ||
               $('#txtlau').val() == '' || $('#txtlau').val() == null ||
            $('#txtamn').val() == '' || $('#txtamn').val() == null ||
            $('#txtwf').val() == '' || $('#txtwf').val() == null ||
            $('#txtwater').val() == '' || $('#txtwater').val() == null ||
            $('#txtteamaker').val() == '' || $('#txtteamaker').val() == null ||
            $('#txtnews').val() == '' || $('#txtnews').val() == null ||
            $('#txtdis').val() == '' || $('#txtdis').val() == null ||
            $('#txtchckinout').val() == '' || $('#txtchckinout').val() == null ||
            $('#txtret').val() == '' || $('#txtret').val() == null ||
            $('#txtspouse').val() == '' || $('#txtspouse').val() == null) {
            alert('Please enter all the details.');
            return false;
        }

        var result = confirm("Are you sure to add inclusion list ?");
        if (result) {
            var arr = new Array();
            var obj = new Object();
            obj.Location = $('#txtcitycode :selected').text();
            obj.HotelName = $('#txthotelname :selected').text();
            obj.Accomodation = $('#txtaccom').val();
            obj.Food = $('#foodtxt').val();
            obj.AirportTransfers = $('#txtair').val();
            obj.RoomService = $('#txtroomServ').val();
            obj.BuffetTime = $('#txtbuffetTym').val();
            obj.Laundry = $('#txtlau').val();
            obj.Amenities = $('#txtamn').val();
            obj.WiFi = $('#txtwf').val();
            obj.DrinkingWater = $('#txtwater').val();
            obj.TeaMaker = $('#txtteamaker').val();
            obj.Newspaper = $('#txtnews').val();
            obj.Discount = $('#txtdis').val();
            obj.EntitlementType = 1;
            obj.CheckinOutHours = $('#txtchckinout').val();
            obj.RetentionCancellation = $('#txtret').val();
            obj.SpouseStay = $('#txtspouse').val();
            arr.push(obj);

            $http({
                method: "POST",
                url: "../InclusionMaster/AddHotelInclusion",
                data: JSON.stringify({ elist: arr }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                alert('Data has been successfully saved.');
                $('#viewdetailModalIncHotel').modal('toggle');
                loaddata();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    };

    $scope.updateRow = function (Id) {
        var result = confirm("Are you sure to update inclusion list ?");
        if (result) {
            var arr = new Array();
            var obj = new Object();
            obj.Id = Id;
            obj.Location = $('#txtlocation').val();
            obj.HotelName = $('#txtname').val();
            obj.Accomodation = $('#txtacc').val();
            obj.Food = $('#txtfood').val();
            obj.AirportTransfers = $('#txtairtransfer').val();
            obj.RoomService = $('#txtroomservice').val();
            obj.BuffetTime = $('#txtbuffet').val();
            obj.Laundry = $('#txtlaundry').val();
            obj.Amenities = $('#txtamnities').val();
            obj.WiFi = $('#txtwifi').val();
            obj.DrinkingWater = $('#txtdrinkingwater').val();
            obj.TeaMaker = $('#txttea').val();
            obj.Newspaper = $('#txtnewsppr').val();
            obj.Discount = $('#txtdiscount').val();
            obj.CheckinOutHours = $('#txtcheckinouthrs').val();
            obj.RetentionCancellation = $('#txtretention').val();
            obj.SpouseStay = $('#txtspousestay').val();
            arr.push(obj);

            $http({
                method: "POST",
                url: "../InclusionMaster/UpdateHotelInclusion",
                data: JSON.stringify({ elist: arr }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                alert('Data has been successfully updated.');
                $('#viewdetailModal').modal('toggle');
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
        $("#txtHeader3").hide();
        $("#txtRow3").show();
        $("#txtHeader4").hide();
        $("#txtRow4").show();
        $("#txtHeader5").hide();
        $("#txtRow5").show();
        $("#editbtntd").hide();
        $("#updatebtntd").show();
    };

    $scope.viewdetail = function (Id) {
        $http({
            method: "GET",
            url: "../InclusionMaster/GetInclusionInfoById?Id=" + Id
        }).then(function mySucces(response) {
            $scope.hotelIncList = response.data;
        }, function myError(response) {
        });

        $("#editbtntd").show();
        $("#updatebtntd").hide();
    };

    $scope.ShowDetail = function (Id) {
        var code= $('#txtcitycode :selected').text();
        var name = $('#txthotelname :selected').text();
        
        $http({
            method: "POST",
            url: "../InclusionMaster/findHotelInclusions",
            data: JSON.stringify({ hotelcity:code, hotelname:name }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(res) {
            if (res.data.length > 0) {
                alert("Inclusions for " + name + " and location " + code + " already exist.");
                $('#viewdetailModalIncHotel').modal('toggle');
                loaddata();
            } else {
                $http({
                    method: "POST",
                    url: "../InclusionMaster/FindHotelListByNameAndCode",
                    data: JSON.stringify({ hotelcity: code, hotelname: name }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(resp) {
                    if (resp.data.length > 0) {
                        $scope.address = resp.data[0].Address;
                        $scope.phone = resp.data[0].Phone;
                        $scope.emailid = resp.data[0].PrimaryEmail;
                        $("#hoteldetailsOnSelection").show();
                        $("#addinclusionbutton").show();
                    } else {
                        alert("Hotel " + name + " for location " + code + " does not exist in Hotel Master.");
                        $('#viewdetailModalIncHotel').modal('toggle');
                        loaddata();
                    }
                    
                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                });
                
            }
        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    };


    $scope.hoteldetailsFill = function () {
        var hoteldrop = $("#txthotelname");
        hoteldrop.empty();
        $("#hoteldetailsOnSelection").hide();
        $("#txtaccom").val("");
        $("#foodtxt").val("");
        $("#txtair").val("");
        $("#txtroomServ").val("");
        $("#txtbuffetTym").val("");
        $("#txtlau").val("");
        $("#txtamn").val("");
        $("#txtwf").val("");
        $("#txtwater").val("");
        $("#txtteamaker").val("");
        $("#txtnews").val("");
        $("#txtdis").val("");
        $("#addinclusionbutton").hide();
        $("#txtchckinout").val("");
        $("#txtret").val("");
        $("#txtspouse").val("");

        //hoteldrop.empty().append('<option selected="selected" value="0" disabled = "disabled">Loading.....</option>');
        $.ajax({
            type: "POST",
            url: "../InclusionMaster/HotelListData",
            data: '{}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var dup = [];
                $.each(response, function () {
                    if (dup.indexOf(this['Text']) == -1) {
                        hoteldrop.append($("<option></option>").val(this['Value']).html(this['Text']));
                        dup.push(this['Text']);
                    }
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });

        var cityDrop = $("#txtcitycode");
        cityDrop.empty();
        $.ajax({
            type: "POST",
            url: "../InclusionMaster/DropDownCityCodeData",
            data: '{}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var duplicate = [];
                $.each(response, function () {
                    if (duplicate.indexOf(this['Value']) == -1) {
                        cityDrop.append($("<option></option>").val(this['Value']).html(this['Text']));
                        duplicate.push(this['Value']);
                    }
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    };

    $('#btnExport').click(function () {
        var result = confirm("Are you sure to download all inclusions in Excel ?");
        if (result) {
            window.location = '../InclusionMaster/ExportListFromTable' ;
        }
    });

    loaddata();
});


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
