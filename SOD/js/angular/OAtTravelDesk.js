var app = angular.module('OatViewDetails', ['ui.bootstrap', 'ngSanitize']);
app.filter('startFrom', function () {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    }
});

app.controller("OatTravelDesk", function ($scope, $http, $timeout) {
    $scope.OatMasterList = [];
    $scope.openDiv = 'false';
    $scope.openCancelDocument = 'false';
    $scope.openCancelDocumentDiv = 'false';
    $scope.ithrevertButton = 'true';
    $scope.submitIthInfo = false;
    $scope.successRes = false;
    $scope.getIthResponseByTravelDesk = [];
    $scope.isFullrefundcancellationSent = false;
    $scope.IsVisible = false;
    var OATreqList = [];
    $('.spinner').hide();

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };

    $scope.getMasterOAtList = function () {
        $('.loader').show();
        $http({
            method: "GET",
            url: "../OatDesk/getListOfOatBooking",
        }).then(function mySucces(response) {
            $scope.OatMasterList = response.data;
            //for (var i = 0; i < $scope.OatMasterList.length; i++) {
            //    var length = response.data[i].RequestDate.length;
            //    $scope.OatMasterList[i].RequestDate = (response.data[i].RequestDate).substring(6, (length - 2));
            //}            
            $scope.currentPage = 1;
            $scope.entryLimit = 10;
            $scope.filteredItems = $scope.OatMasterList.length;
            $scope.totalItems = $scope.OatMasterList.length;
            $('.loader').hide();
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    }
    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered.length;
        }, 1);
    };
    $scope.SaveManageNoShow = function (list, date) {
        $('.loader').show();
        var date1 = JSON.stringify(date);
        date1 = date1.slice(1, 11);
        $scope.manageShowList = list;
        $scope.date = date1;
        var result = confirm("Are you sure to save  Manage NoShow record in database ?");
        if (result) {
            $http({
                method: "POST",
                url: "../OatDesk/getSaveManageNoShow",
                data: JSON.stringify({ 'mangeShowList': $scope.manageShowList, 'date': $scope.date }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySuccess(response) {
                if (response.data == "") {
                    alert("Your record failed to save in database")
                }               
                else {
                    alert("Your record has been successfully save in database");
                    var isvisible = "isvisible" + list.OriginPlace + list.DestinationPlace;
                    $scope[isvisible] = true;
                    
                }
                $('.loader').hide();
            }, function myError(response) {

                $('.loader').hide();
            });
        }
        else {
            $('.loader').hide();
        }

    }
    $scope.viewdetail = function (reqId) {
        $('.loader').show();
        $http({
            method: "POST",
            url: "../OatDesk/getOatDetailsPerReqId",
            data: JSON.stringify({ reqId: reqId }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $scope.OatDetailList = response.data["oatMasterList"];
            $scope.FltDetailList = response.data["oatFlightList"];
            $scope.psgrDetailList = response.data["oatPassangerList"];
            //$scope.currentPage = 1;
            //$scope.entryLimit = 10;
            //$scope.filteredItems = $scope.OatMasterList.length;
            //$scope.totalItems = $scope.OatMasterList.length;
            $('.loader').hide();
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }


    $scope.getListToSendReqToITH = function (Ilist) {
        if (Ilist.Selected) {
            var obj = new Object();
            obj.oatReqId = Ilist.OATRequestID;
            obj.empCode = Ilist.RequestedEmpCode;
            obj.EmpId = Ilist.RequestedEmpId;
            OATreqList.push(obj);
        }
        else {
            if (OATreqList.findIndex(x => x.oatReqId == Ilist.OATRequestID) != -1) {
                var index = OATreqList.findIndex(x => x.oatReqId == Ilist.OATRequestID);
                OATreqList.splice(index, 1);
            }
        }
    }


    $scope.GetIthNameList = function () {
        if (OATreqList.length == 0) {
            alert('Please select atleast one request for sending to service provider.');
            $('#viewdetailModalSendToITh').modal('hide');
            return false;
        } else {
            $('#viewdetailModalSendToITh').modal('show');
            $('.loader').show();
            $http({
                method: "GET",
                url: "../OatDesk/getITHlistName",
            }).then(function mySucces(response) {
                $('.loader').hide();
                $scope.ithNameList = response.data;
            }, function myError(response) {
                $('.loader').hide();
                $scope.myWelcome = response.statusText;
            });
        }

    }

    $scope.ITHfilldropdown = function (ithName) {
        $('.loader').show();
        $http({
            method: "POST",
            url: "../OatDesk/getIthDetailPerName",
            data: JSON.stringify({ ithName: ithName }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $('.loader').hide();
            $scope.ithDetaillst = response.data[0];
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }


    $scope.sendReqToIth = function (ithList) {
        $("#SendReqToIthBtn").prop('disabled', true);
        if (!$scope.IthDetailsForm.$valid) {
            $scope.submitIthInfo = true;
            $("#SendReqToIthBtn").prop('disabled', false);
        }
        else {
            $('.loader').show();

            $http({
                method: "POST",
                url: "../OatDesk/sendReqToIth",
                data: JSON.stringify({ 'ithDetaillst': ithList, 'OATReqList': OATreqList }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                alert(JSON.parse(response.data));
                $("#SendReqToIthBtn").prop('disabled', false);
                window.location.reload();
                $('.loader').show();
            }, function myError(response) {
                $scope.myWelcome = response.statusText;
                $("#SendReqToIthBtn").prop('disabled', false);
                $('.loader').hide();
            });
        }
    }

    $scope.ViewIthDetail = function (oatreqId) {
        $(".loader").show();
        $http({
            method: "POST",
            url: "../OatDesk/ViewIthDetailAsPerReqID",
            data: JSON.stringify({ 'OatReqID': oatreqId }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            $scope.ReqID = oatreqId;
            $scope.arrFlight = response.data["FlightDetail"];
            $scope.arrIthDetail = response.data["IthDetails"];
            $scope.arrhistIthDetail = response.data["IthDetailLog"];
            $scope.arrIthVenderDetail = response.data["IthVenderDetail"];
            $scope.ithMasterList = response.data["ithMasterList"];
            $scope.uniqueSectorFlight = response.data["uniqueSectorFlight"];
            $scope.transactionDetail = response.data["transactionDetail"];

            $('.loader').hide();
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
            $('.loader').hide();
        });
    }
    $scope.viewIthAttachedSrc = function (TrnId, ID, criteria) {
        $('.loader').show();
        $http({
            method: "POST",
            url: "../OatDesk/viewIthAttachedSrc",
            data: JSON.stringify({ 'ID': ID, 'criteria': criteria }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            var url = '../OatDesk/ViewAttachment';
            window.open(url, '_blank');
            $('.loader').hide();
        });
    }
    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        dataAsFormData.append('RejectionSrc', data);
        //for (var i = 0; data.response.length; i++)
        //    angular.forEach(data, function (value, key) {

        //    });
        return dataAsFormData;
    };

    $scope.viewSrc = function (imgsrc) {
        viewwin = window.open(imgsrc, 'viewwin', 'width=600,height=300');
    }

    $scope.openHistoryDiv = function () {
        $scope.openDiv == 'false' ? $scope.openDiv = 'true' : $scope.openDiv = 'false';
    }
    $scope.ithopenHistoryDiv = function () {
        $scope.openviewDiv == 'false' ? $scope.openviewDiv = 'true' : $scope.openviewDiv = 'false';
    }

    $scope.travelopenHistoryDiv = function () {
        $scope.travelopenviewDiv == 'false' ? $scope.travelopenviewDiv = 'true' : $scope.travelopenviewDiv = 'false';
    }

    $scope.controlopenHistoryDiv = function () {
        $scope.controlopenviewDiv == 'false' ? $scope.controlopenviewDiv = 'true' : $scope.controlopenviewDiv = 'false';
    }
    $scope.hideShowCancelDocumentDiv = function (type, index) {
        $scope.cancelType = type;
        $scope.openCancelDocument = 'true'; /*'false' ? $scope.openCancelDocument = 'true' : $scope.openCancelDocument = 'false'; */
        var documentDiv = 'openCancelDocumentDiv' + index;
        $scope[documentDiv] == 'true' ? $scope[documentDiv] = 'false' : $scope[documentDiv] = 'true';
    }

    $scope.changeActionButtonIth = function (index) {
        var openDiv = 'ithrevertButton' + index;
        $("#ithOptionCheckBox" + index).is(":checked") ? $scope[openDiv] = 'true' : $scope[openDiv] = 'false';
        $scope.disableConfirmButton = 'false';
        for (var i = 0; i < $scope.uniqueSectorFlight.length; i++) {
            if ($("#ithOptionCheckBox" + i).is(":checked")) {
                $scope.disableConfirmButton = 'true';
                // $("#ithOptionCheckBox" + index).is(":checked") ? $scope.disableConfirmButton = 'true' : $scope.disableConfirmButton = 'false';
            }
        }

    }

    $scope.cancellationRequestToITh = function (index, PassengerID, originPlace, DestinationPlace) {
        $scope.successRes = false;
        $('.spinner').show();
        var type = $scope.cancelType;
        var cancelReason = $("#CancelReason" + index).val();
        var cancelSrc = $('#cancellationSrc' + index)[0].files[0];
        if (cancelReason == "") {
            alert("Please write your reason for cancellation.");
            $("#CancelReason" + index).focus();
            $('.spinner').hide();
            return false;
        }
        if (type != "c") {
            if (cancelSrc == undefined) {
                alert("Please select any proof for reason.");
                $('.spinner').hide();
                return;
            }
            if (cancelSrc.size > 1048576) {
                alert("File size should be less than 1 MB.");
                $('.spinner').hide();
                return;
            }
            if ((!cancelSrc.type.includes("image")) && (!cancelSrc.type.includes("pdf"))) {
                alert("File should be Image or PDF.");
                $('.spinner').hide();
                return;
            }
        }
        var bookingDetail = PassengerID + "," + originPlace + "," + DestinationPlace + "," + "By OAT Desk" + "," + $scope.ReqID;
        $http({
            method: "POST",
            url: "../OatDesk/cancellationRequestToIth?cancelReason=" + cancelReason + "&BookingDetail=" + bookingDetail + "&cancelType=" + type,
            data: getModelAsFormData(cancelSrc),
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined, 'Cache-Control': 'no-cache , no-store' }
        }).then(function mySucces(response) {
            //$('.loader').hide();
            // 
            //var ResponseDiv = 'successRes' + index;
            //$scope[ResponseDiv] = 'true';
            //$('successResMsg' + index).html(JSON.parse(response.data));           
            //alert(response.data);
            alert(JSON.parse(response.data));
            $scope.successRes = true;
            $('.spinner').hide();
        }, function myError(response) {
            alert("Oops! Something went wrong.");
            $('.spinner').hide();
            $scope.myWelcome = response.statusText;
        });
    }

    $scope.cancellationRequestToWholeBookingRequest = function (OatReqId) {
        $('.loader').show();
        var type = $scope.cancelType;
        var cancelReason = $("#CancelReason").val();
        var cancelSrc = $('#cancellationSrc')[0].files[0];
        if (cancelReason == "") {
            alert("Please write your reason for cancellation.");
            $("#CancelReason" + index).focus();
            return false;
        }
        if (type != "c") {
            if (cancelSrc == undefined) {
                alert("Please select any proof for reason.");
                $('.loader').hide();
                return;
            }
            if (cancelSrc.size > 1048576) {
                alert("File size should be less than 1 MB.");
                $('.loader').hide();
                return;
            }
            if ((!cancelSrc.type.includes("image")) && (!cancelSrc.type.includes("pdf"))) {
                alert("File should be Image or PDF.");
                $('.loader').hide();
                return;
            }
        }
        var bookingDetail = OatReqId + "," + "By OAT Desk";
        $http({
            method: "POST",
            url: "../OatDesk/cancellationRequestToWholeBookingRequest?cancelReason=" + cancelReason + "&BookingDetail=" + bookingDetail + "&cancelType=" + type,
            data: getModelAsFormData(cancelSrc),
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined, 'Cache-Control': 'no-cache , no-store' }
        }).then(function mySucces(response) {
            $('.loader').hide();
            alert(JSON.parse(response.data));
            window.location.reload();
            // $scope.openCancelDocument = false;
            //$scope.isFullrefundcancellationSent = true; 
            //$scope.successRes = false;
            //var ResponseDiv = 'successRes' + index;
            //$scope[ResponseDiv] = 'true';
            //$('successResMsg' + index).html(JSON.parse(response.data));

        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }

    $scope.RevertToIth = function (list, index) {
        $('.loader').show();
        var rejectionRemark = $("#RejectionRemark" + index).val();
        var RejectionSrc = $('#RejectionSrc' + index)[0].files[0];
        if (rejectionRemark == "" || RejectionSrc == undefined) {
            alert("Please enter all details.");
            $('.loader').hide();
            return;
        }
        if (RejectionSrc.size > 1048576) {
            alert("File size should be less than 1 MB.");
            $('.loader').hide();
            return;
        }
        if ((!RejectionSrc.type.includes("image")) && (!RejectionSrc.type.includes("pdf"))) {
            alert("File should be Image or PDF.");
            $('.loader').hide();
            return;
        }


        var idDetail = list.PassengerID + "-" + list.TrnId + "-" + list.ID + "-" + list.OATRequestID + "-" + list.OriginPlace + "-" + list.DestinationPlace;
        $http({
            method: "POST",
            url: "../OatDesk/RevertToITh?rejectionRemark=" + rejectionRemark + "&idDetail=" + idDetail,
            data: getModelAsFormData(RejectionSrc),
            transformRequest: angular.identity,
            //dataType: 'json',
            headers: { 'Content-Type': undefined, 'Cache-Control': 'no-cache , no-store' }
        }).then(function mySucces(response) {
            $('.loader').hide();
            alert(JSON.parse(response.data));
            window.location.reload();
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }
    $scope.getSelectedIthResponse = function (list, index) {
        for (var i = 0; i < list.length; i++) {
            if (list[i].ID == index) {
                list[i].SelectedOption == "true";
                if ($scope.getIthResponseByTravelDesk.findIndex(x => x.originPlace == list[i].originPlace && x.DestinationPlace == list[i].DestinationPlace) != -1) {
                    var ind = $scope.getIthResponseByTravelDesk.findIndex(x => x.originPlace == list[i].originPlace && x.DestinationPlace == list[i].DestinationPlace);
                    $scope.getIthResponseByTravelDesk.splice(ind, 1);
                    //$scope.getIthResponseByTravelDesk.push(list[i]);
                }
                $scope.getIthResponseByTravelDesk.push(list[i]);
            }                
            else
                list[i].SelectedOption == "false";
        }
        
    }
    $scope.ConfirmIthResponse = function (getList) {
        $('.loader').show();
        //var list = [];// $scope.getIthResponseByTravelDesk; 
        var listwithnocancellation = [];
        //for (var i = 0; i < getList.length; i++) {
        //    for (var j = 0; j < getList[i].newIthDetail.length; j++) {
        //        if (getList[i].newIthDetail[j].SelectedOption == "true") {
        //            list.push(getList[i].newIthDetail[j]);
        //        }
        //    }
        //}
        //list.push($scope.getIthResponseByTravelDesk);
        for (var i = 0; i < $scope.transactionDetail.length; i++) {
            if (!$scope.transactionDetail[i].newIthDetail[0].IsFlightCancel) {
                listwithnocancellation.push($scope.transactionDetail[i]);
            }
        }
        if ($scope.getIthResponseByTravelDesk.length != listwithnocancellation.length) {
            $('.loader').hide();
            alert("Please confirm atleast one option for reach sector.");
            return false;
        }
        $http({
            method: "POST",
            url: "../OatDesk/ConfirmIThResponse",
            data: JSON.stringify({ 'SelectedResponse': $scope.getIthResponseByTravelDesk }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            alert(JSON.parse(response.data));
            window.location.reload();
            $('.loader').hide();
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    }

    $scope.sendHoldRequest = function () {
        var ReqID = $scope.ReqID;
        var holdBy = "by OAT Desk";
        var result = confirm("Are you sure to hold this request ?");
        if (result) {
            $('.loader').show();
            $http({
                method: "POST",
                url: "../OatDesk/sendHoldRequestToIth",
                data: JSON.stringify({ 'oatReqId': ReqID, 'holdBy': holdBy }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert(JSON.parse(response.data));
                window.location.reload();
            }, function myError(response) {
                $('.loader').hide();
                $scope.myWelcome = response.statusText;
            });
        }
    }

    $scope.undoHoldRequest = function () {
        var ReqID = $scope.ReqID;
        var holdBy = "by OAT Desk";
        var result = confirm("Are you sure to unhold this request ?");
        if (result) {
            $('.loader').show();
            $http({
                method: "POST",
                url: "../OatDesk/undoHoldRequest",
                data: JSON.stringify({ 'oatReqId': ReqID, 'holdBy': holdBy }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $('.loader').hide();
                alert(JSON.parse(response.data));
                window.location.reload();
            }, function myError(response) {
                $('.loader').hide();
                $scope.myWelcome = response.statusText;
            });
        }
    }



    $scope.sendMailToIthForBooking = function (oatReqId) {
        $('.loader').show();
        $http({
            method: "POST",
            url: "../OatDesk/sendMailToIthForBooking",
            data: JSON.stringify({ 'oatReqId': oatReqId }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            alert(JSON.parse(response.data));
            window.location.reload();
            $('.loader').hide();
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }
    $scope.sendFinalConfirmationToUser = function (OATRequestID) {
        $('.loader').show();
        $http({
            method: "POST",
            url: "../OatDesk/sendFinalConfirmationToUser",
            data: JSON.stringify({ 'oatReqId': OATRequestID }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            alert(response.data);
            // window.location.reload();
            $('.loader').hide();
        }, function myError(response) {
            $('.loader').hide();
            $scope.myWelcome = response.statusText;
        });
    }



    $scope.getangularDate = function (dval) {
        return new Date(parseInt(dval.substring(6, 19)));
    }
    $scope.getMasterOAtList();

});
app.controller("OatFlightNoShow", function ($scope, $http, $timeout) {
    $scope.oatflightNoShowList = [];
    $scope.openDiv = 'false';
    $scope.openCancelDocument = 'false';
    $scope.openCancelDocumentDiv = 'false';
    $scope.ithrevertButton = 'true';
    $scope.submitIthInfo = false;
    $scope.successRes = false;
    $scope.getIthResponseByTravelDesk = [];
    $scope.isFullrefundcancellationSent = false;
    $('.spinner').hide();

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };

    $scope.getFlightNoShowList = function () {
        $('.loader').show();
        $http({
            method: "GET",
            url: "../OatDesk/getListOfOatFlightNoShow",
        }).then(function mySucces(response) {
            $scope.oatflightNoShowList = response.data;
            //for (var i = 0; i < $scope.OatMasterList.length; i++) {
            //    var length = response.data[i].RequestDate.length;
            //    $scope.OatMasterList[i].RequestDate = (response.data[i].RequestDate).substring(6, (length - 2));
            //}            
            $scope.currentPage = 1;
            $scope.entryLimit = 5;
            $scope.filteredItems = $scope.oatflightNoShowList.length;
            $scope.totalItems = $scope.oatflightNoShowList.length;
            $('.loader').hide();
        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    }
    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered.length;
        }, 1);
    };
    $scope.getAngularDate = function (dval) {
        return new Date(parseInt(dval.substring(6, 19)));
    }
    $scope.getFlightNoShowList();
});


