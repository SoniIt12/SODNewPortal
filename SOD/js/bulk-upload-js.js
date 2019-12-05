
//Author: Satyam
//Created Date : 19-Dec-2016
//Modified Date :19-Dec-2016
//Purpose : Bulk Upload Excel File

function hides() {
    $("#dvtabledata").hide();
    bindAgency();
    //dePARTMENT RIGHTHKHJK
    bindDepartmentRight();
}

function shows() {
    $("#dvtabledata").show();
}

//Bulk Upload Function to upload file
function uploadFile() {
    getHodAccorToDepartment();
    var data = new FormData();
    var files = $("#file").get(0).files;
    //Validate Agency Code
    if ($("#ddlacType").val() == null || $("#ddlacType").val() == 'Undefined' || $("#ddlacType").val() == '') {
        alert('Warning : Private Fare Code can not be empty.Might be right has not been allocated.Please contact to SOD-Coordinator.');
        return false;
    }

    if (files.length > 0) {
        data.append("bulkfile", files[0]);
    }
    else {
        alert('Warning:Please select file to be uploaded.');
        return false;
    }

    var extension = $("#file").val().split('.').pop().toUpperCase();
    if (extension != "XLS" && extension != "XLSX") {
        alert('Warning:Invalid file format.');
        return false;
    }
    $.ajax({
        url: '../bulk/UploadFile?finid=' + $("#ddlacType option:selected").text(),
        type: "POST",
        processData: false,
        data: data,
        dataType: 'json',
        contentType: false,
        beforeSend: function () {
            $("#loaderMsg")[0].innerHTML = "Verifying Employee Code and Details";
            $('.loader').show();
        },
        complete: function () {
            $("#loaderMsg")[0].innerHTML = "";
            $("#loaderMsg")[0].innerHTML = "Verified Employee Code and Details";
            $('.loader').delay(100).hide(2000);
            //$('.loader').hide();
        },
        success: function (response) {
            if (response == 'empty') { alert('Incorrect excel file format.'); return; }
            if (response == "Nul") { alert('Empty file.'); return; }
            if (response != null || response != '') {
                binduploadData(response);
                bappright($("#hdnbbr").val());
            }
        },
        error: function (er) { }
    });
    return false;
}

//Pull Uploaded data from DB and wrap up in table
function binduploadData(json) {
    var tr;
    var agencyCode = $("#ddlacType option:selected").text();
    var counter = 0;
    $('#tblUploaddata  tr:not(:first)').remove();
    //Append each row to html table
    for (var i = 0; i < json.length; i++) {
        counter = counter + 1;
        tr = $('<tr />');
        tr.append("<td class='rowcolor' >" + counter + "</td>");
        tr.append("<td class='rowcolor' >" + json[i].EmpCode + "<span id=sts><img  src='../img/rejected.png' style='margin-bottom:4px;margin-left:4px;width:16px;height:16px;' alt='Not-Verified' /></span></td>");
        tr.append("<td class='rowcolor'>" + json[i].Title + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].FirstName + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].LastName + "</td>");
        //tr.append("<td class='rowcolor'>" + json[i].Designation + "</td>");
        //tr.append("<td class='rowcolor'>" + json[i].Department + "</td>");
        if (json[i].MobileNo == "" || json[i].MobileNo == "") {
            tr.append("<td class='rowcolor'>" + "<input type='text' id='updMobileNo'  maxlength='10' minlength='10' style='width:99%'>" + "</td>")
        }
        else {
            tr.append("<td class='rowcolor'>" + json[i].MobileNo + "</td>");
        }
        if (json[i].EmailId == "" || json[i].EmailId == "") {
            tr.append("<td class='rowcolor'>" + "<input type='text' id='updEmailId' style='width:99%'>" + "</td>")
        }
        else {
            tr.append("<td class='rowcolor'>" + json[i].EmailId + "</td>");
        }
        tr.append("<td class='rowcolor'>" + json[i].TravelDate + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].FlightNo + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].Sector + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].Purpose + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].Meal + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].Beverage + "</td>");
        tr.append("<td class='rowcolor'>" + "<span id='spnr'></span>" + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].BookingType + "</td>");
        if (json[i].IsHotelRequired == true) {
            tr.append("<td class='rowcolor'>" + "Yes" + "</td>");
            var length = json[i].CheckInDate.length;
            var travelDate = new Date(ConvertMMddyyyy(json[i].TravelDate));
            var checkinDate = new Date(parseInt(json[i].CheckInDate.substring(6, (length - 2))));
            var checkOutDate = new Date(parseInt(json[i].CheckOutDate.substring(6, (length - 2))));
            var checkDate = new Date(checkinDate);
            checkDate = checkDate.setDate(checkDate.getDate() + 30);
            var a = new Date(parseInt(checkDate));
            //var checkdate = new Date(checkinDate + 30);
            //var dd = checkinDate.getDate();
            //var mm = checkinDate.getMonth() + 1;
            //var y = checkinDate.getFullYear();
            //var someFormattedDate = new Date(mm + '/' + dd + '/' + y);
            tr.append("<td class='rowcolor'>" + ConvertMMddyyyy(checkinDate.toLocaleDateString()) + "</td>");
            tr.append("<td class='rowcolor'>" + ConvertMMddyyyy(checkOutDate.toLocaleDateString()) + "</td>");
            if (checkinDate < travelDate) {
                tr.append("<td class='rowcolor' style='border-right:1px solid #c2c2c2'> <span id='spChk' style='color:red'>Invalid Check-In Date</span></td>");
            }
            else if (checkOutDate < checkinDate) {
                tr.append("<td class='rowcolor' style='border-right:1px solid #c2c2c2'> <span id='spChk' style='color:red'>Invalid Check-Out Date</span></td>");

            }
            else if (checkOutDate > a) {
                tr.append("<td class='rowcolor' style='border-right:1px solid #c2c2c2'> <span id='spChk' style='color:red'>Check-Out Date should be one month of check-in date</span></td>");

            }
            else {
                tr.append("<td class='rowcolor' style='border-right:1px solid #c2c2c2'> <span id='spchk'></span> <input type='hidden' id='hdndupl' value='" + json[i].IsDuplicate + "'/> </td>");
            }
        } else {
            tr.append("<td class='rowcolor'>" + "No" + "</td>");
            tr.append("<td class='rowcolor'>" + "NA" + "</td>");
            tr.append("<td class='rowcolor'>" + "NA" + "</td>");
            tr.append("<td class='rowcolor' style='border-right:1px solid #c2c2c2'> <span id='spchk'></span> <input type='hidden' id='hdndupl' value='" + json[i].IsDuplicate + "'/> </td>");

        }
        $('#tblUploaddata').append(tr);
    }
    //Validate Employee ID
    $.getJSON("../bulk/ValidateEmployeeList",
        function (ejson) {
            if (ejson.length == 0) { hides(); return; } else { shows(); }
            var counter = 0;
            var vcount = 0;
            for (var i = 0; i < ejson.length; i++) {
                var status = false;
                $('#tblUploaddata tr').each(function () {
                    if (counter > 0) {
                        var eid = ($(this).closest('tr').find('td')[1].innerText).trim();
                        var bookingType = $(this).closest('tr').find('td')[14].innerText;
                        if (eid == ejson[i]) {
                            var th = $(this);
                            th.find("#sts").replaceWith("<img  src='../img/right.png' style='margin-bottom:4px;margin-left:4px;width:18px;height:18px;' alt='Verified' />");
                            th.find("#spchk").replaceWith("<input type='checkbox'   id='chkRow' class='chktbl' />");
                            vcount++;

                            //Validate Confirm Booking for AgencyCode
                            if (agencyCode == "SDFINANCED" && bookingType.toLowerCase() == "confirm")
                                th.find("input:checkbox").replaceWith("<span id='spchk'>Not Applicable</span>");
                            return;
                        }
                    }
                    counter++;
                });
            }

            //Validate Duplicate Booking
            if (validateDuplicateBooking() == vcount) {
                alert('Attention : This booking is an existance of duplicate booking.Please view booking history.');
                $('#btnGeneratePNR').hide();
                $('#btnsendRqtoHOD').hide();
            }
            validatePastDateBooking();
        });
}
var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
//Generate PNR function
function generatePNR() {
    var flag = false;
    var counter = 0;
    var i = 0;
    var elists = new Array();
    var bUpdatedlists = new Array();
    var slist = $("#tblUploaddata input[type=checkbox]");
    slist.each(function () {
        if (counter > 0) {
            if ($(this).is(':checked')) {
                var srno = ($(this).closest('tr').find('td')[0].innerText).trim();
                var eid = ($(this).closest('tr').find('td')[1].innerText).trim();
                var MobileNo = ($(this).closest('tr').find('#updMobileNo').val());
                if (MobileNo == "") {
                    alert("Please enter valid mobile no.");
                    $(this).closest('tr').find('#updMobileNo').focus();
                    flag = true;
                    return;
                }
                if (($(this).closest('tr').find('td')[6].innerText).trim() == "") {
                    var emailaddress = ($('#updEmailId').val());
                    if (!emailReg.test(emailaddress) || emailaddress == "") {
                        alert("Please enter valid Email Id.");
                        $(this).closest('tr').find("#updEmailId").focus();
                        flag = true;
                        return;
                    }
                }
                var EmailId = ($(this).closest('tr').find('#updEmailId').val());
                var obj = new Object();
                obj.SrNo = srno;
                obj.EmpCode = eid;
                obj.PNRStatus = "";
                elists.push(obj);
                var obj1 = new Object();
                obj1.EmpCode = eid;
                obj1.MobileNo = MobileNo;
                obj1.EmailId = EmailId;
                bUpdatedlists.push(obj1);
                i++;
            }
        }
        counter++;
    });
    if (!flag) {
        if (i == 0) {
            alert('Please check the row.');
            return;
        }
        var diffhod = $("#spnHODEMail").text();
        var elist = JSON.stringify({ elist: elists, BUpdateList: bUpdatedlists });
        $.ajax({
            url: '../bulk/GeneratePNR?finid=' + $("#ddlacType option:selected").text() + "&diffhod=" + diffhod,
            type: "POST",
            processData: false,
            data: elist,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                $("#loaderMsg")[0].innerHTML = "";
                $("#loaderMsg")[0].innerHTML = "Generating PNR";
                $('.loader').show();
            },
            complete: function () {
                $("#loaderMsg")[0].innerHTML = "";
                $("#loaderMsg")[0].innerHTML = "PNR generation has been completed...";
                $('.loader').delay(100).hide(2000);
            },
            success: function (response) {
                if (response != null || response != '')
                    updatePNRStatus(response);
                else if (response != null || response != '') {
                    alert('Please click on Refresh Button and upload file again.');
                    return;
                }
                else {
                    $("#loaderMsg")[0].innerHTML = " PNR has been already generated ...";
                    $('.loader').delay(1000).hide(2000);
                    return;
                }
            },
            error: function (er) { }
        });
    }
}

//Update PNR Status
function updatePNRStatus(response) {
    for (var i = 0; i < response.length; i++) {
        var counter = 0;
        $('#tblUploaddata tr').each(function () {
            if (counter > 0) {
                var srno = $(this).closest('tr').find('td')[0].innerText;
                var eid = $(this).closest('tr').find('td')[1].innerText;
                var chkbox = $(this).closest('tr').find('input:checkbox').eq(0);
                if (srno == response[i].SrNo && eid == response[i].EmpCode && response[i].PNRStatus.split('|')[0].trim() != "ERR001") {
                    var th = $(this);
                    th.find("#spnr").replaceWith(response[i].PNRStatus.split('|')[0] + " " + "<img  src='../img/right.png' style='margin-bottom:4px;margin-left:4px;width:10px;height:10px;' alt='PNR has been generated successfully.' />");
                    $(this).closest('tr').find('input:checkbox').attr('disabled', 'disabled');
                    return;
                }
                else if (srno == response[i].SrNo && eid == response[i].EmpCode && response[i].PNRStatus.split('|')[0].trim() == "ERR001") {
                    var th = $(this);
                    th.find("#spnr").replaceWith(response[i].PNRStatus.split('|')[0] + " " + "<img  src='../img/rejected1.png' style='margin-bottom:4px;margin-left:4px;width:10px;height:10px;' alt='PNR ERROR' />");
                    $(this).closest('tr').find('input:checkbox').attr('disabled', 'disabled');
                    return;
                }
            }
            counter++;
        });
    }
}

function getmasterbulkList() {
    var date = ConvertMMddyyyy($("#txtfromdate").val()) + "," + ConvertMMddyyyy($("#txttodate").val());//local
   // var date = $("#txtfromdate").val() + "," + $("#txttodate").val(); //Prod
    $.ajax({
        url: '../bulk/GetMasterBulkList?prm=' + date + "&BookingType=bulk",
        type: "GET",
        processData: false,
        dataType: 'json',
        contentType: false,
        beforeSend: function () {
            $("#loaderMsg")[0].innerHTML = "Please wait....";
            $('.loader').show();
        },
        complete: function () {
            $("#loaderMsg")[0].innerHTML = "";
            $("#loaderMsg")[0].innerHTML = "Fetching bulk booking data...";
            $('.loader').delay(100).hide(2000);
        },
        success: function (response) {
            if (response != null || response != '')
                bindMasterTable(response);
        },
        error: function (er) { }
    });
}

function bindMasterTable(json) {
    var tr;
    var counter = 0;
    $('#tblMaster  tr:not(:first)').remove();
    //Append each row to html table
    for (var i = 0; i < json.length; i++) {
        counter = counter + 1;
        tr = $('<tr />');
        tr.append("<td class='rowcolor' >" + counter + "</td>");
        tr.append("<td class='rowcolor'>" + json[i].TRId + "</td>");
        tr.append("<td class='rowcolor'>" + ToJavaScriptDate(json[i].TransactionDate) + "</td>");
        tr.append("<td class='rowcolor'><a href='#' onclick='downloadExcel()'>" + json[i].FileName + "</a></td>");
        tr.append("<td class='rowcolor'>" + json[i].CreatedById + " - " + json[i].CreatedByName + "</td>");
        tr.append("<td class='rowcolor' style='border-right:1px solid #c2c2c2'> <span id='spchk'></span></td>");
        $('#tblMaster').append(tr);
    }

    //Paging
    var totalRows = $('#tblMaster').find('tbody tr:has(td)').length;
    var recordPerPage = 20;
    var totalPages = Math.ceil(totalRows / recordPerPage);
    var $pages = $('<div id="pages"></div>');
    for (i = 0; i < totalPages; i++) {
        $('<span >&nbsp;' + (i + 1) + '</span>').appendTo($pages);
    }
    $pages.appendTo('#tblMaster');
    $('.pageNumber').hover(
        function () { $(this).addClass('focus'); },
        function () { $(this).removeClass('focus'); }
    );
    $('table').find('tbody tr:has(td)').hide();
    var tr = $('table tbody tr:has(td)');
    for (var i = 0; i <= recordPerPage - 1; i++) {
        $(tr[i]).show();
    }

    $('span').click(function (event) {
        $('#tblMaster').find('tbody tr:has(td)').hide();
        var nBegin = ($(this).text() - 1) * recordPerPage;
        var nEnd = $(this).text() * recordPerPage - 1;
        for (var i = nBegin; i <= nEnd; i++) {
            $(tr[i]).show();
        }
    });
}

//Convert date script date formt for display
function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getDate()) + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
};

//convert MM/dd/yyyy
function ConvertMMddyyyy(dval) {
    var mval = (dval.split('/')[1] + '/' + dval.split('/')[0] + '/' + dval.split('/')[2]);
    return mval;
}

//Download excelsheet
function downloadExcel() {
    // alert('Hi');
}

//Bind Agency Code
function bindAgency() {
    $.ajax({
        url: '../bulk/GetAgencyCode',
        type: "GET",
        processData: false,
        dataType: 'json',
        contentType: false,
        success: function (response) {
            if (response != null || response != '')
                $.each(response, function (key, value) {
                    $("#ddlacType").append($("<option></option>").val(value).html(value));
                });
        },
        error: function (er) { }
    });
}

//Bind Agency Code
function bindDepartmentRight() {
    $.ajax({
        url: '../bulk/GetDepartment',
        type: "GET",
        processData: false,
        dataType: 'json',
        contentType: false,
        success: function (response) {
            if (response != null || response != '')
                $.each(response, function (key, value) {
                    var dep = value.DeptId + '|' + value.VerticalCode;
                    var depName = value.DeptName + ' (' + value.VerticalName + ')';
                    $("#ddeptType").append($("<option></option>").val(dep).html(depName));
                });
        },
        error: function (er) { }
    });
}

//passing bulk booking rights
function bappright(bright) {
    if (bright == 1) {
        $('#btnGeneratePNR').show(); $('#btnsendRqtoHOD').hide();
        $('#dvuser').show(); $('#dvuser').hide();
    }
    else {
        $('#btnGeneratePNR').hide(); $('#btnsendRqtoHOD').show();
        $('#dvuser').hide(); $('#dvuser').show();
    }
}

//Send Request to respected HOD
function sendReqtoHOD() {
    var flag = false;
    var counter = 0;
    var i = 0;
    var blists = new Array();
    var bUpdatedlists = new Array();
    var slist = $("#tblUploaddata input[type=checkbox]");
    slist.each(function () {
        if (counter > 0) {
            if ($(this).is(':checked')) {
                var srno = ($(this).closest('tr').find('td')[0].innerText).trim();
                var eid = ($(this).closest('tr').find('td')[1].innerText).trim();
                var MobileNo = ($(this).closest('tr').find('#updMobileNo').val());
                if (MobileNo == "") {
                    alert("Please enter valid mobile no.");
                    $(this).closest('tr').find('#updMobileNo').focus();
                    flag = true;
                    return;
                }
                if (($(this).closest('tr').find('td')[6].innerText).trim() == "") {
                    var emailaddress = ($('#updEmailId').val());
                    if (!emailReg.test(emailaddress) || emailaddress == "") {
                        alert("Please enter valid Email Id.");
                        $(this).closest('tr').find("#updEmailId").focus();
                        flag = true;
                        return;
                    }
                }
                var EmailId = ($(this).closest('tr').find('#updEmailId').val());
                var obj = new Object();
                obj.SrNo = srno;
                obj.EmpCode = eid;
                obj.PNRStatus = "";
                blists.push(obj);
                var obj1 = new Object();
                obj1.EmpCode = eid;
                obj1.MobileNo = MobileNo;
                obj1.EmailId = EmailId;
                bUpdatedlists.push(obj1);
                i++;
            }
        }
        counter++;
    });
    if (!flag) {
        if (i == 0) {
            alert('Please check the row.');
            return;
        }
        var blist = JSON.stringify({ blist: blists, BUpdateList: bUpdatedlists });
        // var diffhod = ($("#chkalternateHODemailId").is(':checked')) ? $("#txtalternateHODemailId").val() : ""; 
        var diffhod = $("#spnHODEMail").text();
        $.ajax({
            url: '../bulk/SendRequesttoHOD?finid=' + $("#ddlacType option:selected").text() + "&diffhod=" + diffhod,
            type: "POST",
            processData: false,
            data: blist,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                $("#loaderMsg")[0].innerHTML = "";
                $("#loaderMsg")[0].innerHTML = "SOD Bulk Booking Request is sending to HOD...";
                $('.loader').show();
            },
            complete: function () {
                $("#loaderMsg")[0].innerHTML = "";
                $("#loaderMsg")[0].innerHTML = "SOD Bulk Booking Request has been sent successfylly...";
                $('.loader').delay(100).hide(2000);
                updateResponse();
            },
            success: function (response) {
                if (response != null || response != '')
                    alert('Request has been sent successfully.');
                else {
                    $("#loaderMsg")[0].innerHTML = "SOD Bulk Booking Request has been sent already ..";
                    $('.loader').delay(1000).hide(2000);
                    return;
                }
            },
            error: function (er) { }
        });
    }
}

//Update User Response
function updateResponse() {
    window.location.href = '../bulk/bulkresponse';
}

//Validate Duplicate Booking
function validateDuplicateBooking() {
    var counter = 0;
    var flag = 0;
    $('#tblUploaddata tr').each(function () {
        if (counter > 0) {
            var IsDuplicate = $(this).closest('tr').find('input:hidden').eq(0);
            if (IsDuplicate.val() == "true") {
                var th = $(this);
                var chkbox = th.closest('tr').find('input:checkbox').eq(0);
                if (chkbox != null || chkbox != "undefined")
                    th.find("#chkRow").replaceWith("<span id='spchk' style='color:red'>Duplicate Booking</span>");
                flag++;
            }
        }
        counter++;
    });
    return flag;
}

//Alternate HOD visibility
function diffHODVisibility(chk) {
    if ($(chk).is(':checked')) {
        $("#txtalternateHODemailId").show();
        $("#txtalternateHODemailId").focus();
        getHodAccorToDepartment();
    }
    else
        $("#txtalternateHODemailId").hide();
}

//gethod according to department 

function getHodAccorToDepartment() {
    var deptID = $("#ddeptType option:selected").val();
    $.ajax({
        url: '../bulk/GetHodOfDepartment',
        type: "POST",
        processData: false,
        data: JSON.stringify({ dept: deptID }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            if (response != null || response != '')
                $("#spnHODEMail").text(response);
        },
        error: function (er) { }
    });
}

//Validate past date Booking
function validatePastDateBooking() {
    var counter = 0;
    var flag = 0;
    var today = new Date();
    var day = today.getDate();
    var month = today.getMonth() + 1;
    var year = today.getFullYear();
    var cdate = month + '/' + day + '/' + year;

    $('#tblUploaddata tr').each(function () {
        if (counter > 0) {
            var arr = $(this).closest('tr').find('td')[7].innerText.split('/');
            var pdate = arr[1] + '/' + arr[0] + '/' + arr[2];
            if (Date.parse(pdate) < Date.parse(cdate)) {
                var th = $(this);
                var chkbox = th.closest('tr').find('input:checkbox').eq(0);
                if (chkbox != null || chkbox != "undefined")
                    th.find("#chkRow").replaceWith("<span id='spchk' style='color:red'>Invalid Date</span>");
                flag++;
            }
        }
        counter++;
    });
    return flag;
}

function validateNumber(evt) {
    var e = evt || window.event;
    var key = e.keyCode || e.which;

    if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
        // numbers   
        key >= 48 && key <= 57 ||
        // Numeric keypad
        key >= 96 && key <= 105 ||
        // Backspace and Tab and Enter
        key == 8 || key == 9 || key == 13 ||
        // Home and End
        key == 35 || key == 36 ||
        // left and right arrows
        key == 37 || key == 39 ||
        // Del and Ins
        key == 46 || key == 45) {
        // input is VALID
    }
    else {
        // input is INVALID
        e.returnValue = false;
        if (e.preventDefault) e.preventDefault();
    }
}
