var app = angular.module("VendorApp", ['ui.bootstrap', 'ngSanitize']);

app.filter('startFrom', function () {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    };
});

app.filter('nlfilter', function () {
    return function (text) {
        text = String(text).trim();
        return (text.length > 0 ? '<p>' + text.replace(/[\r\n]+/g, '</p><p>') + '</p>' : null);
    };
});

app.controller("VendorCtrl", function ($scope, $http, $timeout, $interval, $window) {
    $scope.hideButton = false;
    $scope.hidebuttons = true;
    function loaddata() {
        $('.loader').show();
        $http({
            method: "GET",
            url: "../nsvd/GetVendorList"
        }).then(function mySucces(response) {
            $scope.VendorList = response.data;
            $('.loader').hide();
            $scope.hideButton = false;
            $('#hidecheckbox').hide();
            for (var i = 0; i < response.data.length; i++) {
                $scope.VendorList[i].AddDate = new Date(parseInt((response.data[i].AddDate).substring(6)));
                if (!$scope.VendorList[i].IsApproved) {
                    $scope.hideButton = true;
                    $('#hidecheckbox').show()
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
    };

    $scope.userroledetails = function (EmpId) {
        $http({
            method: 'GET',
            url: "../nsvd/NonSODUserRoleDetails",
        }).then(function mySucces(response) {
            $scope.userroledetail = response.data;
        }), function myError(response) {
            alert(response.data); NonSODHODdetails
        }
    }

    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered.length;
            var count = 0;
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
    $scope.HODdetails = function (hodEmailID) {
        $http({
            method: 'GET',
            url: "../nsvd/NonSODHODdetails",
            params: { hodemailid: hodEmailID }
        }).then(function mySucces(response) {
            $scope.hODdetails = response.data;
        }), function myError(response) {
            alert(response.data);
        }
    }
    $scope.filterValue = function ($event) {
        if (isNaN(String.fromCharCode($event.keyCode))) {
            $event.preventDefault();
        }
    };

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };

    $scope.vendordelete = function (id) {
        if (confirm('Are you sure to delete this record!')) {
            $http({
                method: "POST",
                url: "../nsvd/DeleteVendor",
                data: { Id: id }
            }).then(function mySucces(response) {
                loaddata();
                alert('Record has been deleted successfully !');
            }, function myError(response) {
                $scope.myWelcome = response.statusText;
            });
        }
    };

    function validateEmail(email) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }

    $scope.addupdatevendor = function () {
        if ($('#firstName').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter First Name**');
            $("#firstName").addClass("highlight");
            return false;
        }
        else {
            $('#firstName').removeClass("highlight");
            $scope.IsVisible = false;
        }
        if ($('#lastName').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Last Name**');
            $("#lastName").addClass("highlight");
            return false;
        }
        else {
            $("#lastName").removeClass("highlight");
            $scope.IsVisible = false;
        }
        if ($('#email').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Email Id**');
            $("#email").addClass("highlight");
            return false;
        }
        else {
            $("#email").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if (!validateEmail($('#email').val().trim())) {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Valid Email Id**');
            return false;
        }
        else {
            $("#message").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#mobileNumber').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Phone No**');
            $("#mobileNumber").addClass('highlight');
            return false;
        }
        else {
            $("#mobileNumber").removeClass('highlight');
            $scope.IsVisible = false;
        }
        var ph_numbr = /^\+?\d{10}$/;
        var mobilenumber = $('#mobileNumber').val();
        var found = mobilenumber.match(ph_numbr);
        if (!found) {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Valid 10 digits Phone No**');
            $("#mobileNumber").addClass('highlight');
            return false;
        }
        else {
            $("#mobileNumber").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#Code').children("option:selected").val() == "") {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Company Name**');
            $("#Code").addClass('highlight');
            return false;
        }
        else {
            $("#Code").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#gender').children("option:selected").val() == "") {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Gender**');
            $("#gender").addClass('highlight');
            return false;
        }
        else {
            $("#gender").removeClass('highlight');
            $scope.IsVisible = false;
        }
        var status = true;
     
        if ($('#email').val().trim() != '' || $('#mobileNumber').val().trim() != '') {
            $http({
                method: "POST",
                url: "../nsvd/IsExistEmailOrMobile",
                async: false,
                data: { Email: $('#email').val(), Mobile: $('#mobileNumber').val(), Id: $('#hdnId').val() }
            }).then(function mySucces(response) {
                if (response != null && response.data != "") {
                    status = false;
                    alert(response.data);
                } else {
                    var obj = new Object();
                    obj.ID = $('#hdnId').val();
                    obj.FirstName = $('#firstName').val();
                    obj.LastName = $('#lastName').val();
                    obj.EmailId = $('#email').val();
                    obj.MobileNo = $('#mobileNumber').val();
                    obj.Gender = $('#gender').val();
                    obj.CompanyName = $('#Code').val();
                    obj.IsApproved = 0;
                    $http({
                        method: "POST",
                        url: "../nsvd/AddUpdateVendor",
                        data: JSON.stringify({ Model: obj }),
                        dataType: 'json',
                        async: false,
                        contentType: 'application/json; charset=utf-8'
                    }).then(function mySucces(response) {
                        if (response.data != "") {
                            alert(JSON.parse(response.data));
                        }
                        loaddata();
                    }, function myError(response) {
                        alert('Error: Invalid request processing...');
                    });
                    $scope.clearFields();
                }
            }, function myError(response) {
                $scope.myWelcome = response.statusText;
            });
        }
    };
    function ConvertMMddyyyy(dval) {
        var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
        return mval;
    }
    $scope.getdatewiseList = function (prm) {
        var date = ConvertMMddyyyy($("#txtfromdate").val()) + "," + ConvertMMddyyyy($("#txttodate").val()) + "," + prm;//dev
        //var date = $("#txtfromdate").val() + "," + $("#txttodate").val() + "," + prm; //prod
        $http({
            method: "GET",
            url: "../nsvd/getdatewiseList?prm=" + date
        }).then(function mySucces(response) {
            $scope.VendorList = response.data;
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
    $scope.addupdatevendorhd = function () {
        if ($('#firstName').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter First Name**');
            $("#firstName").addClass('highlight');
            return false;
        }
        else {
            $("#firstName").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#lastName').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Last Name**');
            $("#lastName").addClass('highlight');
            return false;
        }
        else {
            $("#lastName").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#email').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Email Id**');
            $("#email").addClass('highlight');
            return false;
        }
        else {
            $("#email").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if (!validateEmail($('#email').val().trim())) {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Valid Email Id**');
            return false;
        }
        else {
            $("#message").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#mobileNumber').val().trim() == '') {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Phone No**');
            $("#mobileNumber").addClass('highlight');
            return false;
        }
        else {
            $("#mobileNumber").removeClass('highlight');
            $scope.IsVisible = false;
        }
        var ph_numbr = /^\+?\d{10}$/;
        var mobilenumber = $('#mobileNumber').val();
        var found = mobilenumber.match(ph_numbr);
        if (!found) {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Valid 10 digits Phone No**');
            $("#mobileNumber").addClass('highlight');
            return false;
        }
        else {
            $("#mobileNumber").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#Code').children("option:selected").val() == "") {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Company Name**');
            $("#Code").addClass('highlight');
            return false;
        }
        else {
            $("#Code").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#gender').children("option:selected").val() == "") {
            $scope.IsVisible = true;
            $("#message").html('**Please Enter Gender**');
            $("#gender").addClass('highlight');
            return false;
        }
        else {
            $("#gender").removeClass('highlight');
            $scope.IsVisible = false;
        }
        if ($('#UserEmpID').children("option:selected").val() == "" || $('#UserEmpID').children("option:selected").val() == undefined) {
            $scope.IsVisible = true;
            $("#message").html('**Enter Requested Employee Name**');
            $("#UserEmpID").addClass('highlight');
            return false;
        }
        else {
            $("#UserEmpID").removeClass('highlight');
            $scope.IsVisible = false;
        }
        var status = true;
        if ($('#email').val().trim() != '' || $('#mobileNumber').val().trim() != '') {
            $http({
                method: "POST",
                url: "../nsvd/IsExistEmailOrMobile",
                async: false,
                data: { Email: $('#email').val(), Mobile: $('#mobileNumber').val(), Id: $('#hdnId').val() }
            }).then(function mySucces(response) {
                if (response != null && response.data != "") {
                    status = false;
                    alert(response.data);
                } else {
                    var obj = new Object();
                    obj.ID = $('#hdnId').val();
                    obj.FirstName = $('#firstName').val();
                    obj.LastName = $('#lastName').val();
                    obj.EmailId = $('#email').val();
                    obj.MobileNo = $('#mobileNumber').val();
                    obj.Gender = $('#gender').val();
                    obj.CompanyName = $('#Code').val();
                    obj.ReqEmpEmailID = $('#UserEmpID').children("option:selected").val();
                    obj.ApprovedbyEmpEmailID = null;
                    obj.IsApproved = 0;
                    obj.AddVendorOnBehalfof = null;
                    $http({
                        method: "POST",
                        url: "../nsvd/AddUpdateVendorhd",
                        data: JSON.stringify({ Model: obj }),
                        dataType: 'json',
                        async: false,
                        contentType: 'application/json; charset=utf-8'
                    }).then(function mySucces(response) {
                        if (response.data != "") {
                            alert(response.data);
                        }
                        loaddata();
                    }, function myError(response) {
                        alert('Error: Invalid request processing...');
                    });
                    $scope.clearFields();
                }
            }, function myError(response) {
                $scope.myWelcome = response.statusText;
            });
        }
    };
    //updated vendordetails
    $scope.vendorupdate = function (obj) {
        $('#firstName').val(obj.FirstName);
        $('#lastName').val(obj.LastName);
        $('#email').val(obj.EmailId);
        $('#mobileNumber').val(obj.MobileNo);
        $('#hdnId').val(obj.ID);
        $('#gender').val(obj.Gender).trigger('change');
        $('#Code').val(obj.CompanyName).trigger('change');
        $('#UserEmailID').val(obj.ReqEmpEmailID).trigger('change');
        $("#btnsubmit").attr('value', 'Update');
    };

    $scope.clearFields = function (obj) {
        $('#firstName').val("");
        $('#lastName').val("");
        $('#email').val("");
        $('#mobileNumber').val("");
        $('#hdnId').val("0");
        $('#gender').val("");
        $('#Code').val("");
        $('#UserEmpID').val("");
        $("#btnsubmit").attr('value', 'Add');
    };

    $scope.sendpendinglist = function (list) {
        //console.log($scope.loading);
        var obj = new Array();
        for (var i = 0; i < list.length; i++) {
            if (list[i].Ischecked) {
                obj.push(list[i]);
                $('.loader').show();
            }
        }
        if (obj.length == 0) {
            $('.loader').hide();
            alert("Please select atleast one vendor from the list");
            return false;
        }
        $http({
            method: "POST",
            url: "../nsvd/sendmailforApproval",
            async: false,
            data: { 'mailerlist': obj }
        }).then(function mySucces(response) {
            $('.loader').hide();
            if (response.data != "") {
                alert(JSON.parse(response.data));
            }
        }, function myError(response) {
            $('.loader').hide(); s
            alert(response.data);
        })
        window.location.reload();
    }

    //Check All
    $scope.checkAll = function () {
        if (!$scope.selectedAll) {
            $scope.selectedAll = true;
        } else {
            $scope.selectedAll = false;
        }
        angular.forEach($scope.VendorList, function (dlist) {
            if (dlist.IsApproved == 0) {
                dlist.Ischecked = $scope.selectedAll;
                            }            
        });
    };
    $('#btnExport').click(function () {
        var result = confirm("Are you sure to download all vendor data in Excel ?");
        if (result) {
            window.location = '../nsvd/ExportVendorData';
        }
    });
    loaddata();
    $scope.userroledetails();
})


