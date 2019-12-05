//Employee Help Desk Module :Passenger Booking Linst Info
var app = angular.module('FinanceChangeRequest', ['ui.bootstrap', 'ngSanitize']);

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

app.controller("FinanceReqHistory", function ($scope, $http, $timeout) {
    function loaddata() {
        $http({
            method: "GET",
            url: '../ChangeReq/FinanceRightsData'
        }).then(function mySucces(response) {
            $scope.trlist = response.data;//fill employee booking details data
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 5; //max no of items to display in a page
            $scope.filteredItems = $scope.trlist.length; //Initially for no filter  
            $scope.totalItems = $scope.trlist.length;
            //$scope.deptlist = response.data.deptList;//fill department list data
            response.data.length > 0 ? $('#finSearch').css('display', 'block') : $('#finSearch').css('display', 'none');
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
      
        $http({
            method: "GET",
            url: '../ChangeReq/FinanceRightsDataHistory'
        }).then(function mySucces(response) {
            $scope.trlistfin = response.data;//fill employee booking details data
            $scope.currentPagefin = 1; //current page
            $scope.entryLimitfin = 5; //max no of items to display in a page
            $scope.filteredItemsfin = $scope.trlistfin.length; //Initially for no filter  
            $scope.totalItemsfin = $scope.trlistfin.length;
            //$scope.deptlist = response.data.deptList;//fill department list data
            //response.data.length > 0 ? $('#finSearch').css('display', 'block') : $('#finSearch').css('display', 'none');
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });

        $scope.setPage = function (pageNo) {
            $scope.currentPage = pageNo;
        };
        $scope.setPagefin = function (pageNo) {
            $scope.currentPagefin = pageNo;
        };

        $scope.filter = function () {
            $timeout(function () {
                $scope.filteredItems = $scope.filtered.length;
            }, 1);
        };

        $scope.filterfin = function () {
            $timeout(function () {
                $scope.filteredItemsfin = $scope.filteredfin.length;
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

        $('#btnsave').removeClass("disabledLink");
        $scope.Mymodal = function (ReqId, Status) {
                $('#btnsave').on('click', function () {
                $('#btnsave').addClass("disabledLink");
                if ($scope.validation() == true) {
                    $http({
                        method: "post",
                        url: '../ChangeReq/ConfirmationMailtoUser',
                        data: { ReqId: ReqId, status: Status, remarks: $('#finremarks').val() },
                    }).then(function mysuccess(response) {
                        document.getElementById('remarksdiv').style.display = "none";
                        document.getElementById('remarksmsgdiv').style.display = "block";
                        document.getElementById('remarksmsg').textContent = response.data;
                        $('#btnsave').removeClass("disabledLink");
                    }, function myerror(response) {
                    });
                }
            });
        };
        $scope.validation = function () {
            var status = true;
            if (angular.element('#finremarks').val() == "") {
                status = false;
                document.getElementById('finremarks').style.border = "1px Solid red";
            }
            return status;
        };
    }

    angular.element(document).ready(function () {
          $('#confirmationHRRemarks').on('hidden.bs.modal', function (e) {
                $(this)
                  .find("textarea")
                     .val('')
                     .end()
               .find("textarea")
                     .css('border','')
                     .end()
                  .find("p").html('').end()

              $('#remarksdiv').css('display', 'block');
              document.getElementById('remarksmsgdiv').style.display = "none";
              document.getElementById('remarksmsg').textContent = "";
              loaddata();
          });
    });
    
    loaddata();
});
 