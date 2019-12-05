//Bulk Booking JS


var app = angular.module('bulkbooking', ['ui.bootstrap', 'ngSanitize']);

//Main Grid Filter
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


//Page level function : Fetch Data
app.controller("ctrblkbooking", function ($scope, $http) {
    function loaddata() {
        $http({
            method: "GET",
            url: "../bulk/GetCurrentUploadedList"
        }).then(function mySucces(response) {
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
    }

    loaddata();
});


//J-Query Function for Bulk Booking File
//get file size
function GetFileSize(fileid) {
    try {
        var fileSize = 0;
        //for IE
        if ($.browser.msie) {
            //before making an object of ActiveXObject,
            //please make sure ActiveX is enabled in your IE browser
            var objFSO = new ActiveXObject("Scripting.FileSystemObject"); var filePath = $("#" + fileid)[0].value;
            var objFile = objFSO.getFile(filePath);
            var fileSize = objFile.size; //size in kb
            fileSize = fileSize / 1048576; //size in mb
        }
            //for FF, Safari, Opeara and Others
        else {
            fileSize = $("#" + fileid)[0].files[0].size //size in kb
            fileSize = fileSize / 1048576; //size in mb
        }
        return fileSize;
    }
    catch (e) {
        alert("Error is :" + e);
    }
}

//get file path from client system
function getNameFromPath(strFilepath) {
    var objRE = new RegExp(/([^\/\\]+)$/);
    var strName = objRE.exec(strFilepath);

    if (strName == null) {
        return null;
    }
    else {
        return strName[0];
    }
}

//button submit click
$("#btnSubmit").live("click", function () {
    if ($('#fileToUpload').val() == "") {
        $("#spanfile").html("Please upload file");
        return false;
    }
    else {
        return checkfile();
    }
});


//check file
function checkfile() {
    var file = getNameFromPath($("#fileToUpload").val());
    if (file != null) {
        var extension = file.substr((file.lastIndexOf('.') + 1));
        // alert(extension);
        switch (extension) {
            case 'jpg':
            case 'png':
            case 'gif':
            case 'pdf':
                flag = true;
                break;
            default:
                flag = false;
        }
    }
    if (flag == false) {
        $("#spanfile").text("You can upload only jpg,png,gif,pdf extension file");
        return false;
    }
    else {
        var size = GetFileSize('fileToUpload');
        if (size > 3) {
            $("#spanfile").text("You can upload file up to 3 MB");
            return false;
        }
        else {
            $("#spanfile").text("");
        }
    }
}

//file upload change event
$(function () {
    $("#fileToUpload").change(function () {
        checkfile();
    });
});