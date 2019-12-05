/// <reference path="angular.js" />
/* 
Purpose : To Allocate CXO Dept 
Writer :Satyam
Create Date :17/08/2016
*/

var app = angular.module('cxodeptMapping', ['ui.bootstrap']);

app.filter('startFrom', function () {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    }
});


app.controller("deptMapping", function ($scope, $http, $timeout) {

    $scope.list_cxoname = {
        data: [
            { name: 'CMO' },
            { name: 'CFO' },
            { name: 'CAO' },
            { name: 'CPO' },
            { name: 'CSRO' },
            { name: 'CRO' }]
    }

    function loaddata(val) {
        $http({
            method: "GET",
            url: "../admin/GetcxoDeptMappingList?cxo=" + val
        }).then(function mySucces(response)
        {
            $scope.trlist = response.data;//fill cxo dept mappingg details data
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 5; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
            $scope.totalItems = $scope.trlist.length;
        }, function myError(response)
        {
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
     }

    //loaddata();
     $scope.filterdept = function (val) {
         ;
         $timeout(function () {
             loaddata($scope.searchDept.name);
         }, 1);
     };

    //Remove grid selected Item
     $scope.removeList = function (val) {
         var result = confirm("Are you sure! to remove this department right?");
         if (result) {
             $http({
                 method: "Get",
                 url: "../admin/RemoveCxoDeptMappingList?dept=" + val
             }).then(function mySucces(response) {
                 alert('Selected department has been removed successfully.');
                 loaddata($scope.searchDept.name);
                 return false;
             }, function myError(response) {
                 $scope.myWelcome = response.statusText;
             });
         };
     };

    //Allocate cxo dept mapping
     $scope.allocateList = function (val) {
         ;
         var strval = $scope.searchDept.name+","+val;
             $http({
                 method: "Post",
                 url: "../admin/AllocateCxoDeptMappingList?strval=" + strval
             }).then(function mySucces(response) {
                 alert('Selected department has been allocated successfully.');
                 loaddata($scope.searchDept.name);
                 return false;
             }, function myError(response) {
                 $scope.myWelcome = response.statusText;
             });
     };
});

