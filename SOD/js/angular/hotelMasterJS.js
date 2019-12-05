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

app.controller("MasterCtrl", function ($scope, $http,$timeout, $interval) {

    function loaddata() {
        $http({
            method: "GET",
            url: "../InclusionMaster/GetHotelListData"
        }).then(function mySucces(response) {
            $scope.hotelList = response.data;
            $scope.currentPage = 1; //current page
            $scope.entryLimit = 10; //max no of items to display in a page
            $scope.filteredItems = $scope.hotelList.length; //Initially for no filter  
            $scope.totalItems = $scope.hotelList.length;
            
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

    $scope.clearFields = function () {
        //$('#txthotelcode').val("");
        //$('#txthotelname').val("");
        //$('#txtcitycode').val("");
        //$('#txtcityname').val("");
        //$('#txtaddress').val("");
        //$('#txtphone').val("");
        //$('#txtprimary').val("");
        //$('#txtsecondary').val("");
        //$('#txtstartdate').val("");
        //$('#txtenddate').val("");
        //$('#txtsingleprice').val("");
        //$('#txtdoubleprice').val("");
        //$('#txtgmname').val("");
       
    }

    function validateEmail(email) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }

    $scope.addRow = function () {
        if ( $('#txthotelcode').val().trim() == '' ){
            alert('Please enter hotel code.');
            $('#txthotelcode').focus();
            return false;
        }
        if($('#txthotelname').val().trim() == ''){
            alert('Please enter hotel name.');
            $('#txthotelname').focus();
            return false;
        }
        if($('#txtcitycode').val().trim() == ''){
            alert('Please enter city code.');
            $('#txtcitycode').focus();
            return false;
        }
        if ($('#txtcityname').val().trim() == '') {
            alert('Please enter city name.');
            $('#txtcityname').focus();
            return false;
        }
        if( $('#txtaddress').val().trim() == ''){
            alert('Please enter hotel address.');
            $('#txtaddress').focus();
            return false;
        }
        if( $('#txtphone').val().trim() == '' ){
            alert('Please enter hotel phone no.');
            $('#txtphone').focus();
            return false;
        }
        if($('#txtprimary').val().trim() == '' ){
            alert('Please enter primary email.');
            $('#txtprimary').focus();
            return false;
        }
        if (!validateEmail($('#txtprimary').val().trim())) {
            alert('Please enter valid primary email.');
            $('#txtprimary').focus();
            return false;
        }
        if ($('#selectAddCurrency').val().trim() == 'Null') {
            alert('Please select your currency code.');
            $('#selectAddCurrency').focus();
            return false;
        }
        if($('#txtsecondary').val().trim() == ''){
            alert('Please enter secondary email.');
            $('#txtsecondary').focus();
            return false;
        }

        var Smail = $('#txtsecondary').val().trim().split(',');
        for (var i = 0; i < Smail.length; i++) {
            if (!validateEmail(Smail[i].trim())) {
                alert('Please enter valid secondary email.');
                $('#txtsecondary').focus();
                return false;
            }
        }

        if($('#txtgmname').val().trim() == '' ){
            alert('Please enter GM Details.');
            $('#txtgmname').focus();
            return false;
        }
        if($('#txtstartdate').val().trim() == ''){
            alert('Please enter contract start date.');
            $('#txtstartdate').focus();
            return false;
        }    

        if($('#txtenddate').val().trim() == ''){
            alert('Please enter contract end date.');
            $('#txtenddate').focus();
            return false;
        }
        var startDate = new Date(ConvertMMddyyyy($('#txtstartdate').val()));
        var endDate = new Date(ConvertMMddyyyy($('#txtenddate').val()));
        var today = new Date();
        today = today.setHours(0, 0, 0, 0);
        startDate = startDate.setHours(0, 0, 0, 0);
        endDate = endDate.setHours(0, 0, 0, 0);


        //if (startDate < today) {
        //    alert('Please enter contract start date not less than current date.');
        //    return false;
        //}

        if (endDate < today) {
            alert('Please enter contract end date not less than current date.');
            return false;
        }


        if (startDate > endDate) {
            alert('Please enter contract end date not less than contract start date.');
            return false;
        }

        if (($('#apaiMeal').is(':checked')) == false && ($('#mapmeal').is(':checked')) == false && ($('#cpaimeal').is(':checked')) == false) {
            alert('Please select a meal type.');
            return false;
        }

        if (($('#apaiMeal').is(':checked'))) {
            if ($('#txtsingleprice1').val().trim() == '') {
                alert('Please enter single meal price.');
                $('#txtsingleprice1').focus();
                return false;
            }

            if ($('#txtdoubleprice1').val().trim() == '') {
                alert('Please enter double meal price.');
                $('#txtdoubleprice1').focus();
                return false;
            }

            if (isNaN($('#txtsingleprice1').val().trim())) {
                alert('Please enter only numbers for single price.');
                $('#txtsingleprice1').focus();
                return false;
            }

            if (isNaN($('#txtdoubleprice1').val().trim())) {
                alert('Please enter only numbers for double price.');
                $('#txtdoubleprice1').focus();
                return false;
            }
        }
        if (($('#mapmeal').is(':checked'))) {
            if ($('#txtsingleprice2').val().trim() == '') {
                alert('Please enter single meal price.');
                $('#txtsingleprice2').focus();
                return false;
            }

            if ($('#txtdoubleprice2').val().trim() == '') {
                alert('Please enter double meal price.');
                $('#txtdoubleprice2').focus();
                return false;
            }

            if (isNaN($('#txtsingleprice2').val().trim())) {
                alert('Please enter only numbers for single price.');
                $('#txtsingleprice2').focus();
                return false;
            }

            if (isNaN($('#txtdoubleprice2').val().trim())) {
                alert('Please enter only numbers for double price.');
                $('#txtdoubleprice2').focus();
                return false;
            }
        }
        if (($('#cpaimeal').is(':checked'))) {
            if ($('#txtsingleprice3').val().trim() == '') {
                alert('Please enter single meal price.');
                $('#txtsingleprice3').focus();
                return false;
            }

            if ($('#txtdoubleprice3').val().trim() == '') {
                alert('Please enter double meal price.');
                $('#txtdoubleprice3').focus();
                return false;
            }

            if (isNaN($('#txtsingleprice3').val().trim())) {
                alert('Please enter only numbers for single price.');
                $('#txtsingleprice3').focus();
                return false;
            }

            if (isNaN($('#txtdoubleprice3').val().trim())) {
                alert('Please enter only numbers for double price.');
                $('#txtdoubleprice3').focus();
                return false;
            }
        }


        var validateDate1 = $('#txtstartdate').val();
        if (!isValidDate(validateDate1)) {            
            alert("Invalid date format! Enter Contract Start Date in dd/MM/yyyy format.");
            $('#txtstartdate').focus();
            return;
        }

        var validateDate2 = $('#txtenddate').val();
        if (!isValidDate(validateDate2)) {
            alert("Invalid date format! Enter Contract End Date in dd/MM/yyyy format.");
            $('#txtenddate').focus();
            return;
        }
                
        var result = confirm("Are you sure to add hotel list ?");
        if (result) {
            var arr = new Array();
            var obj = new Object();
            obj.HotelCode = $('#txthotelcode').val();
            obj.HotelName = $('#txthotelname').val();
            obj.StationCode = $('#txtcitycode').val();
            obj.City = $('#txtcityname').val();
            obj.Address = $('#txtaddress').val();
            obj.Phone = $('#txtphone').val();
            obj.PrimaryEmail = $('#txtprimary').val();
            obj.SecondaryEmail = $('#txtsecondary').val();
            obj.Status = "Active";
            obj.GMname = $('#txtgmname').val();
            if ($('input[name="radio"]:checked').val() == 'yes') {
                obj.IsTaxIncluded = true;
            }
            else {
                obj.IsTaxIncluded = false;
            }            
            obj.ContractStartDate = document.getElementById("txtstartdate").value;// ConvertMMddyyyy(document.getElementById("txtstartdate").value);
            obj.ContractEndDate = document.getElementById("txtenddate").value;// ConvertMMddyyyy(document.getElementById("txtenddate").value);
            arr.push(obj);

            var arrPrice = new Array();
            if ($('#apaiMeal').is(':checked')) {
                var object = new Object();
                object.HotelCode = $('#txthotelcode').val();
                object.StationCode = $('#txtcitycode').val();
                object.SinglePrice = $('#txtsingleprice1').val();
                object.DoublePrice = $('#txtdoubleprice1').val();
                object.HotelCurrencyCode = $('#selectAddCurrency').val();
                object.TCId = 1;
                arrPrice.push(object);
            }
            if ($('#mapmeal').is(':checked')) {
                var object = new Object();
                object.HotelCode = $('#txthotelcode').val();
                object.StationCode = $('#txtcitycode').val();
                object.SinglePrice = $('#txtsingleprice2').val();
                object.DoublePrice = $('#txtdoubleprice2').val();
                object.HotelCurrencyCode = $('#selectAddCurrency').val();
                object.TCId = 2;
                arrPrice.push(object);
            }
            if ($('#cpaimeal').is(':checked')) {
                var object = new Object();
                object.HotelCode = $('#txthotelcode').val();
                object.StationCode = $('#txtcitycode').val();
                object.SinglePrice = $('#txtsingleprice3').val();
                object.DoublePrice = $('#txtdoubleprice3').val();
                object.HotelCurrencyCode = $('#selectAddCurrency').val();
                object.TCId = 3;
                arrPrice.push(object);
            }

            $http({
                method: "POST",
                url: "../InclusionMaster/AddNewHotel",
                data: JSON.stringify({ elist: arr, plist: arrPrice }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                var code = $('#txthotelcode').val();
                UploadContract(code);
                //alert('Data has been successfully saved.');                
                $('#viewdetailModalIncHotel').modal('toggle');
                loaddata();
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
    };

    $scope.updateRow = function (Id) {

        if ($('#txtcode').val().trim() == '') {
            alert('Please enter hotel code.');
            $('#txtcode').focus();
            return false;
        }
        if ($('#txtname').val().trim() == '') {
            alert('Please enter hotel name.');
            $('#txtname').focus();
            return false;
        }
        if ($('#txtcitycode').val().trim() == '') {
            alert('Please enter city code.');
            $('#txtcitycode').focus();
            return false;
        }
        if ($('#txtcity').val().trim() == '') {
            alert('Please enter city name.');
            $('#txtcity').focus();
            return false;
        }
        if ($('#txtadd').val().trim() == '') {
            alert('Please enter hotel address.');
            $('#txtadd').focus();
            return false;
        }
        if ($('#txtphn').val().trim() == '') {
            alert('Please enter hotel phone no.');
            $('#txtphn').focus();
            return false;
        }
        if ($('#txtpemail').val().trim() == '') {
            alert('Please enter primary email.');
            $('#txtpemail').focus();
            return false;
        }
        if (!validateEmail($('#txtpemail').val().trim())) {
            alert('Please enter valid primary email.');
            $('#txtpemail').focus();
            return false;
        }
        
        var Smail = $('#txtsemail').val().split(',');
        for (var i = 0; i < Smail.length; i++) {
            if (!validateEmail(Smail[i].trim())) {
                alert('Please enter valid secondary email.');
                $('#txtsemail').focus();
                return false;
            }
        }
 
        if ($('#txtgm').val().trim() == '') {
            alert('Please enter GM Details.');
            $('#txtgm').focus();
            return false;
        }

        if(($('#chkmeal1').is(':checked')))
        {
            if ($('#txtSingle1').val().trim() == '') {
            alert('Please enter single meal price.');
                $('#txtSingle1').focus();
            return false;
            }

            if ($('#txtDouble1').val().trim() == '') {
                alert('Please enter double meal price.');
                $('#txtDouble1').focus();
                return false;
            }

            if (isNaN($('#txtSingle1').val().trim())) {
                alert('Please enter only numbers for single price.');
                $('#txtSingle1').focus();
                return false;
            }

            if (isNaN($('#txtDouble1').val().trim())) {
                alert('Please enter only numbers for double price.');
                $('#txtDouble1').focus();
                return false;
            }
            if ($('#selecttxtCurrency1').val().trim() == 'Null') {
                alert('Please select currency code.');
                $('#selecttxtCurrency1').focus();
                return false;
            }
        }
        if (($('#chkmeal2').is(':checked'))) {

            if ($('#txtSingle2').val().trim() == '') {
                alert('Please enter single meal price.');
                $('#txtSingle2').focus();
                return false;
            }

            if ($('#txtDouble2').val().trim() == '') {
                alert('Please enter double meal price.');
                $('#txtDouble2').focus();
                return false;
            }

            if (isNaN($('#txtSingle2').val().trim())) {
                alert('Please enter only numbers for single price.');
                $('#txtSingle2').focus();
                return false;
            }

            if (isNaN($('#txtDouble2').val().trim())) {
                alert('Please enter only numbers for double price.');
                $('#txtDouble2').focus();
                return false;
            }
            if ($('#selecttxtCurrency2').val().trim() == 'Null') {
                alert('Please select currency code.');
                $('#selecttxtCurrency2').focus();
                return false;
            }
        }
        if (($('#chkmeal3').is(':checked'))) {
            if ($('#txtSingle3').val().trim() == '') {
                alert('Please enter single meal price.');
                $('#txtSingle3').focus();
                return false;
            }

            if ($('#txtDouble3').val().trim() == '') {
                alert('Please enter double meal price.');
                $('#txtDouble3').focus();
                return false;
            }

            if (isNaN($('#txtSingle3').val().trim())) {
                alert('Please enter only numbers for single price.');
                $('#txtSingle3').focus();
                return false;
            }

            if (isNaN($('#txtDouble3').val().trim())) {
                alert('Please enter only numbers for double price.');
                $('#txtDouble3').focus();
                return false;
            }
            if ($('#selecttxtCurrency3').val().trim() == 'Null') {
                alert('Please select currency code.');
                $('#selecttxtCurrency3').focus();
                return false;
            }
        }
        if ($('#chkmeal1').is(":not(:checked)") && $('#chkmeal2').is(":not(:checked)") && $('#chkmeal3').is(":not(:checked)")){
            alert('Please select atlest one type of meal.');
            $('#chkmeal1').focus();
            return false;
        }



        if ($('#txtstart').val().trim() == '') {
            alert('Please enter contract start date.');
            $('#txtstart').focus();
            return false;
        }

        if ($('#txtend').val().trim() == '') {
            alert('Please enter contract end date.');
            $('#txtend').focus();
            return false;
        }
        var startDate = new Date(ConvertMMddyyyy($('#txtstart').val()));
        var endDate = new Date(ConvertMMddyyyy($('#txtend').val()));
        var today = new Date();
        today = today.setHours(0, 0, 0, 0);
        startDate = startDate.setHours(0, 0, 0, 0);
        endDate = endDate.setHours(0, 0, 0, 0);


        //if (startDate < today) {
        //    alert('Please enter contract start date not less than current date.');
        //    return false;
        //}

        //if (endDate < today) {
        //    alert('Please enter contract end date not less than current date.');
        //    return false;
        //}

        var validateDate1 = $('#txtstart').val();
        if (!isValidDate(validateDate1)) {
            alert("Invalid date format ! Please enter Contract Start Date in dd/MM/yyyy format.");
            $('#txtstart').focus();
            return;
        }

        var validateDate2 = $('#txtend').val();
        if (!isValidDate(validateDate2)) {
            alert("Invalid date format ! Please enter Contract End Date in dd/MM/yyyy format.");
            $('#txtend').focus();
            return;
        }

        if (startDate > endDate) {
            alert('Please enter contract end date not less than contract start date.');
            return false;
        }

        if (($('#chkmeal1').is(':checked')) == false && ($('#chkmeal2').is(':checked')) == false && ($('#chkmeal3').is(':checked')) == false) {
            alert('Please select a meal type.');
            return false;
        }

        var validateDate1 = $('#txtstartdate').val();
        if (!isValidDate(validateDate1)) {
            alert("Invalid date format! Please enter Contract Start Date in dd/MM/yyyy format.");
            $('#txtstartdate').focus();
            return;
        }

        var validateDate2 = $('#txtenddate').val();
        if (!isValidDate(validateDate2)) {
            alert("Invalid date format! Please enter Contract End Date in dd/MM/yyyy format.");
            $('#txtenddate').focus();
            return;
        }


        var result = confirm("Are you sure to update hotel list ?");
        if (result) {
            var arr = new Array();
            var obj = new Object();
            obj.ID = Id;
            obj.HotelCode = $('#txtcode').val();
            obj.HotelName = $('#txtname').val();
            obj.StationCode = $('#txtcitycode').val();
            obj.City = $('#txtcity').val();
            obj.Address = $('#txtadd').val();
            obj.Phone = $('#txtphn').val();
            obj.PrimaryEmail = $('#txtpemail').val();
            obj.SecondaryEmail = $('#txtsemail').val();
            obj.GMname = $('#txtgm').val();
            if ($('input[name="radiobtn"]:checked').val() == 'yes') {
                obj.IsTaxIncluded = true;
            }
            else {
                obj.IsTaxIncluded = false;
            }
            obj.ContractStartDate = document.getElementById("txtstart").value;// ConvertMMddyyyy(document.getElementById("txtstart").value);
            obj.ContractEndDate = document.getElementById("txtend").value;// ConvertMMddyyyy(document.getElementById("txtend").value);
            arr.push(obj);

            var arrPrice = new Array();
            if ($("#chkmeal1").is(':checked')) {
                if ($('#selecttxtCurrency1').val().trim() == 'Null') {
                    alert('Please select currency code.');
                    $('#selecttxtCurrency1').focus();
                    return false;
                }
                var object = new Object();
                object.HotelId = Id;
                object.HotelCode = $('#txtcode').val();
                object.StationCode = $('#txtcitycode').val();
                object.SinglePrice = $('#txtSingle1').val();
                object.HotelCurrencyCode = $('#selecttxtCurrency1').val();
                object.DoublePrice = $('#txtDouble1').val();
                if ($('#mealprice1').text() == "APAI") {
                    object.TCId = 1;
                } else if ($('#mealprice1').text() == "MAP") {
                    object.TCId = 2;
                } else if ($('#mealprice1').text() == "CPAI") {
                    object.TCId = 3;
                }
                arrPrice.push(object);
            }
            if ($("#chkmeal2").is(':checked')) {
                if ($('#selecttxtCurrency2').val().trim() == 'Null') {
                    alert('Please select currency code.');
                    $('#selecttxtCurrency2').focus();
                    return false;
                }
                var object = new Object();
                object.HotelId = Id;
                object.HotelCode = $('#txtcode').val();
                object.StationCode = $('#txtcitycode').val();
                object.SinglePrice = $('#txtSingle2').val();
                object.DoublePrice = $('#txtDouble2').val();
                object.HotelCurrencyCode = $('#selecttxtCurrency2').val();
                if ($('#mealprice2').text() == "APAI") {
                    object.TCId = 1;
                } else if ($('#mealprice2').text() == "MAP") {
                    object.TCId = 2;
                } else if ($('#mealprice2').text() == "CPAI") {
                    object.TCId = 3;
                }
                arrPrice.push(object);
            }
            if ($("#chkmeal3").is(':checked')) {
                if ($('#selecttxtCurrency3').val().trim() == 'Null') {
                    alert('Please select currency code.');
                    $('#selecttxtCurrency3').focus();
                    return false;
                }
                var object = new Object();
                object.HotelId = Id;
                object.HotelCode = $('#txtcode').val();
                object.StationCode = $('#txtcitycode').val();
                object.SinglePrice = $('#txtSingle3').val();
                object.DoublePrice = $('#txtDouble3').val();
                object.HotelCurrencyCode = $('#selecttxtCurrency3').val();
                if ($('#mealprice3').text() == "APAI") {
                    object.TCId = 1;
                } else if ($('#mealprice3').text() == "MAP") {
                    object.TCId = 2;
                } else if ($('#mealprice3').text() == "CPAI") {
                    object.TCId = 3;
                }
                arrPrice.push(object);
            }

            $http({
                method: "POST",
                url: "../InclusionMaster/UpdateHotelList",
                data: JSON.stringify({ elist: arr, plist: arrPrice }),
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
        $(".txtHeader3").hide();
        $("#txtRow3a").show();
        $("#txtRow3b").show();
        $("#txtRow3c").show();
        $("#txtHeader4").hide();
        $("#txtRow4").show();
        $("#txtHeader5").hide();
        $("#txtRow5").show();
        $("#editbtntd").hide();
        $("#updatebtntd").show();
        if ($('#taxvalue').text() == "Yes") {
            $('input[name="radiobtn"][value="yes"]').prop('checked', true);
        } else {
            $('input[name="radiobtn"][value="no"]').prop('checked', true);
        }

        var len = $scope.hotelPriceList.length;
        if (len == 1) {      
            $('#chkmeal2').prop('checked', false);
            $('#chkmeal3').prop('checked', false);
            $('#chkmeal1').prop('checked', true);
            if ($scope.hotelPriceList[0].TCId == "1") {
                $("#mealprice1").text("APAI");
                $("#mealprice2").text("MAP");
                $("#mealprice3").text("CPAI");
            } else if ($scope.hotelPriceList[0].TCId == "2") {
                $("#mealprice1").text("MAP");
                $("#mealprice2").text("APAI");
                $("#mealprice3").text("CPAI");
            } else {
                $("#mealprice1").text("CPAI");
                $("#mealprice2").text("APAI");
                $("#mealprice3").text("MAP");
            }    
            $('#txtSingle1').val($scope.hotelPriceList[0].SinglePrice);
            $('#txtDouble1').val($scope.hotelPriceList[0].DoublePrice);
            if ($scope.hotelPriceList[0].HotelCurrencyCode == null) {
                $('#selecttxtCurrency1').val("INR");
            }
            else {
                $('#selecttxtCurrency1').val($scope.hotelPriceList[0].HotelCurrencyCode);
            }
            $('#txtSingle2').val("");
            $('#txtDouble2').val("");
            $('#txtSingle3').val("");
            $('#txtDouble3').val("");
            $('#selecttxtCurrency2').val("Null");
            $('#selecttxtCurrency3').val("Null");
        }
        else if (len == 2) {
            $('#chkmeal3').prop('checked', false);
            $('#chkmeal1').prop('checked', true);
            $('#chkmeal2').prop('checked', true);
            if ($scope.hotelPriceList[0].TCId == "1") {
                $("#mealprice1").text("APAI");
            } else if ($scope.hotelPriceList[0].TCId == "2") {
                $("#mealprice1").text("MAP");
            } else {
                $("#mealprice1").text("CPAI");
            }
            $('#txtSingle1').val($scope.hotelPriceList[0].SinglePrice);
            $('#txtDouble1').val($scope.hotelPriceList[0].DoublePrice);
            if ($scope.hotelPriceList[0].HotelCurrencyCode == null) {
                $('#selecttxtCurrency1').val("INR");
            }
            else {
                $('#selecttxtCurrency1').val($scope.hotelPriceList[0].HotelCurrencyCode);
            }

            if ($scope.hotelPriceList[1].TCId == "1") {
                $("#mealprice2").text("APAI");
            } else if ($scope.hotelPriceList[1].TCId == "2") {
                $("#mealprice2").text("MAP");
            } else {
                $("#mealprice2").text("CPAI");
            }
            $('#txtSingle2').val($scope.hotelPriceList[1].SinglePrice);
            $('#txtDouble2').val($scope.hotelPriceList[1].DoublePrice);
            if ($scope.hotelPriceList[1].HotelCurrencyCode == null) {
                $('#selecttxtCurrency2').val("INR");
            }
            else {
                $('#selecttxtCurrency2').val($scope.hotelPriceList[1].HotelCurrencyCode);
            }

            if ($scope.hotelPriceList[0].TCId != '1' && $scope.hotelPriceList[1].TCId != '1') {
                $("#mealprice3").text("APAI");
            }
            else if ($scope.hotelPriceList[0].TCId != '2' && $scope.hotelPriceList[1].TCId != '2') {
                $("#mealprice3").text("MAP");
            }
            else if ($scope.hotelPriceList[0].TCId != '3' && $scope.hotelPriceList[1].TCId != '3') {
                $("#mealprice3").text("CPAI");
            }
            $('#txtSingle3').val("");
            $('#txtDouble3').val("");
            $('#selecttxtCurrency3').val("Null");
        }
        else if (len == 3) {
            $('#chkmeal1').prop('checked', true);
            $('#chkmeal2').prop('checked', true);
            $('#chkmeal3').prop('checked', true);

            if ($scope.hotelPriceList[0].TCId == "1") {
                $("#mealprice1").text("APAI");
            } else if ($scope.hotelPriceList[0].TCId == "2") {
                $("#mealprice1").text("MAP");
            } else {
                $("#mealprice1").text("CPAI");
            }
            $('#txtSingle1').show();
            $('#txtDouble1').show();
            $('#selecttxtCurrency1').show();
            $('#txtSingle2').show();
            $('#txtDouble2').show();
            $('#txtSingle3').show();
            $('#txtDouble3').show();
            $('#selecttxtCurrency2').show();
            $('#selecttxtCurrency3').show();
            $('#txtSingle1').val($scope.hotelPriceList[0].SinglePrice);
            $('#txtDouble1').val($scope.hotelPriceList[0].DoublePrice);
            if ($scope.hotelPriceList[0].HotelCurrencyCode == null) {
                $('#selecttxtCurrency1').val("INR");
            }
            else {
                $('#selecttxtCurrency1').val($scope.hotelPriceList[0].HotelCurrencyCode);
            }

            if ($scope.hotelPriceList[1].TCId == "1") {
                $("#mealprice2").text("APAI");
            } else if ($scope.hotelPriceList[1].TCId == "2") {
                $("#mealprice2").text("MAP");
            } else {
                $("#mealprice2").text("CPAI");
            }
            $('#txtSingle2').val($scope.hotelPriceList[1].SinglePrice);
            $('#txtDouble2').val($scope.hotelPriceList[1].DoublePrice);
            if ($scope.hotelPriceList[1].HotelCurrencyCode == null) {
                $('#selecttxtCurrency2').val("INR");
            }
            else {
                $('#selecttxtCurrency2').val($scope.hotelPriceList[1].HotelCurrencyCode);
            }

            if ($scope.hotelPriceList[2].TCId == "1") {
                $("#mealprice3").text("APAI");
            } else if ($scope.hotelPriceList[2].TCId == "2") {
                $("#mealprice3").text("MAP");
            } else {
                $("#mealprice3").text("CPAI");
            }
            $('#txtSingle3').val($scope.hotelPriceList[2].SinglePrice);
            $('#txtDouble3').val($scope.hotelPriceList[2].DoublePrice);
            if ($scope.hotelPriceList[2].HotelCurrencyCode == null) {
                $('#selecttxtCurrency3').val("INR");
            }
            else {
                $('#selecttxtCurrency3').val($scope.hotelPriceList[2].HotelCurrencyCode);
            }
        }
        else {
            $('#chkmeal1').prop('checked', false);
            $('#chkmeal2').prop('checked', false);
            $('#chkmeal3').prop('checked', false);

             $("#mealprice1").text("APAI");
             $("#mealprice2").text("MAP");
             $("#mealprice3").text("CPAI");
            $('#txtSingle1').val("");
            $('#txtDouble1').val("");
            $('#selecttxtCurrency1').val("Null");
            $('#txtSingle2').val("");
            $('#txtDouble2').val("");
            $('#txtSingle3').val("");
            $('#txtDouble3').val("");
            $('#selecttxtCurrency2').val("Null");
            $('#selecttxtCurrency3').val("Null");
        }

    };

    $scope.viewdetail = function (Id) {
        $("#txtRow3a").hide();
        $("#txtRow3b").hide();
        $("#txtRow3c").hide();
        $http({
            method: "GET",
            url: "../InclusionMaster/GetHotelInfoById?Id=" + Id
        }).then(function mySucces(response) {
            $scope.hotelIncList = response.data.hotelPopupDetails;
            $scope.hotelPriceList = response.data.hotelPriceList;
            var len = $scope.hotelPriceList.length;
        }, function myError(response) {
        });

        $("#editbtntd").show();
        $("#updatebtntd").hide();
        
    };

    $scope.ToJavaScriptDate = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
    };

    $scope.uploadDivOpen = function (val) {
        $scope.trhotelcode = val;
    }

    $scope.UploadContractEdit = function (hotelCode) {
        // Checking whether FormData is available in browser
        if (window.FormData !== undefined) {
            var fileUpload = $("#FileUpload2").get(0);
            var files = fileUpload.files;
            // Create FormData object
            var fileData = new FormData();
            // Looping over all files and add it to FormData object
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            $.ajax({
                url: '../InclusionMaster/UploadFiles?HotelCode=' + hotelCode,
                type: "POST",
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: fileData,

                success: function (result) {
                    alert(result);
                    location.reload();
                },
                error: function (err) {
                    alert(err.statusText);
                    location.reload();
                }
            });
        } else {
            alert("Incorrect file format. Upload the file again against the hotel.");
        }
    }
     
    $('#btnExport').click(function () {
        var result = confirm("Are you sure to download all hotel data in Excel ?");
        if (result) {
            window.location = '../InclusionMaster/ExportHotelData';
        }
    });

    $scope.CurrencyList = function() {
        $http({
            method: "GET",
            url: "../InclusionMaster/GetCurrencyList"
        }).then(function mySucces(response) {
            $scope.CurrencyList = response.data;         

        }, function myError(response) {
            $scope.myWelcome = response.statusText;
        });
    };
    $scope.CurrencyList();
    loaddata();
});


function isValidDate(sText) {
    var reDate = /(?:0*[1-9]|[12][0-9]|3[01])\/(?:0*[1-9]|1[0-2])\/(?:19|20\d{2})/;
    return reDate.test(sText);
}

function changeMeal1() {
    if ($('#apaiMeal').is(':checked')) {
        $('#mealdiv1').show();
    } else {
        $('#mealdiv1').hide();
    }
}

function changeMeal2() {
    if ($('#mapmeal').is(':checked')) {
        $('#mealdiv2').show();
    } else {
        $('#mealdiv2').hide();
    }
}

function changeMeal3() {
    if ($('#cpaimeal').is(':checked')) {
        $('#mealdiv3').show();
    } else {
        $('#mealdiv3').hide();
    }
}

function clickcheckboxmeal1() {
    if ($('#chkmeal1').is(':checked')) {
        $('#txtSingle1').show();
        $('#txtDouble1').show();
        $('#selecttxtCurrency1').show();
    } else {
        $('#txtSingle1').hide();
        $('#txtDouble1').hide();
        $('#selecttxtCurrency1').hide();
    }   
}

function clickcheckboxmeal2() {
    if ($('#chkmeal2').is(':checked')) {
        $('#txtSingle2').show();
        $('#txtDouble2').show();
        $('#selecttxtCurrency2').show();
    } else {
        $('#txtSingle2').hide();
        $('#txtDouble2').hide();
        $('#selecttxtCurrency2').hide();
    }
}

function clickcheckboxmeal3() {
    if ($('#chkmeal3').is(':checked')) {
        $('#txtSingle3').show();
        $('#txtDouble3').show();
        $('#selecttxtCurrency3').show();
    } else {
        $('#txtSingle3').hide();
        $('#txtDouble3').hide();
        $('#selecttxtCurrency3').hide();
    }
}
function ConvertMMddyyyy(dval) {
    var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
    return mval;
}

function blockSpecialChar(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
}

function UploadContract(hotelCode) {
    // Checking whether FormData is available in browser
    if (window.FormData !== undefined) {
        var fileUpload = $("#FileUpload1").get(0);
        var files = fileUpload.files;
        // Create FormData object
        var fileData = new FormData();
        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        $.ajax({
            url: '../InclusionMaster/UploadFiles?HotelCode=' + hotelCode,
            type: "POST",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,

            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function (err) {
                alert(err.statusText);
                location.reload();
            }
        });
    } else {
        alert("Incorrect file format. Upload the file again against the hotel.");
    }
}

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

jQuery(document).ready(function ($) {   
    $(function () {
        $("#txtstart").datepicker({
            numberOfMonths: 1,
            defaultDate: new Date(),
            minDate: new Date(),
            maxDate: "+10M +00D",
            dateFormat: "dd/mm/yy"
        });
    });

    $(function () {
        $("#txtend").datepicker({
            numberOfMonths: 1,
            defaultDate: new Date(),
            minDate: new Date(),
            maxDate: "+10M +00D",
            dateFormat: "dd/mm/yy"
        });
    });

    $(function () {
        $("#txtstartdate").datepicker({
            numberOfMonths: 1,
            defaultDate: new Date(),
            //minDate: new Date(),
            maxDate: "+10M +00D",
            dateFormat: "dd/mm/yy"
        });
        $("#txtstartdate").datepicker('setDate', new Date());
    });

    $(function () {
        $("#txtenddate").datepicker({
            numberOfMonths: 1,
            defaultDate: new Date(),
            minDate: new Date(),
            maxDate: "+60M +00D",
            dateFormat: "dd/mm/yy"
        });
        $("#txtenddate").datepicker('setDate', new Date());
    });
});
