var app = angular.module("SjSisBooking", []);

app.controller("ctrlSjSisBooking", function ($scope, $http) {
    $scope.submitted = false;
    $scope.SignUpsubmitted = false;
    $scope.GetVerticals = function () {
        $http({
            method: "Get",
            url: "../SjSisBooking/GetVerticals",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {
            //console.log(response);
            $scope.VList = response.data;
        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    }
   
    $scope.register = function (reg) {
        $scope.specificVerical = true;
        if (reg.SJSCVerticalID == "5" || reg.SJSCVerticalID == "6") {
            $scope.specificVerical = false;           
        }
        if ($scope.RegForm.$valid && (reg.Pwd == reg.ConfirmPwd)) {
            var result = confirm("Please be ensure, all the provided details should be correct, before submit the details ?");
            if (result) {
                $http({
                    method: "POST",
                    url: "../SjSisBooking/UserRegister",
                    data: JSON.stringify({ UserData: reg }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                }).then(function mySucces(response) {
                    $scope.sendEmailNotification();
                    $('#myModal').modal('hide');
                    alert(JSON.parse(response.data));
                    $scope.resetValidateUSerIdForm();
                }, function myError(response) {
                    alert('Error: Invalid request processing...');
                });
            }
        }
        else
        {
            $scope.submitted = true;
            return;
        }
        $scope.submitted = false;
    };

    $scope.SignUp = function (user) {
        $scope.SignUpsubmitted = true;
        $scope.invalidCredentials = false;
        if ($scope.signUpForm.$valid) {
            $http({
                method: "POST",
                url: "../SjSisBooking/Loginregister",
                data: JSON.stringify({ UserData: user }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                if (JSON.parse(response.data) == "Invalid") {
                    $scope.invalidCredentials = true;
                    $scope.ErrorText = "Invalid userId or Password";
                }
                else if (JSON.parse(response.data) == "Not varified") {
                    $scope.invalidCredentials = true;
                    $scope.ErrorText = "Your Email-ID is not verified . Please verify your Email Id.";
                }
                else {
                    window.location.href = "../sjsisBooking/flight";
                }
            }, function myError(response) {
                alert('Error: Invalid request processing...');
            });
        }
        else {
            return;
        }
        $scope.SignUpsubmitted = false;
    };

    $scope.validateUserId = function (reg) {
        $('.loader').show();
        $scope.forgotPasswordResponseText = "";
        $scope.submitted = true;
        $scope.successColor = false;
        if ($scope.validateUSerIdForm.$valid) {
            $http({
                method: "POST",
                url: "../SjSisBooking/validateUserIdforForgetPassword",
                data: JSON.stringify({ UserData: reg }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then(function mySucces(response) {
                $scope.sendEmailNotification();
                $('.loader').hide();
                if (JSON.parse(response.data) == "Error") {
                    $scope.forgotPasswordResponseText = "Oops! something went wrong";
                } else if (JSON.parse(response.data) == "Invalid") {
                    $scope.forgotPasswordResponseText = "Please Enter Correct User Email Id.";
                } else {
                    $scope.successColor = true;
                    $scope.forgotPasswordResponseText = "Account Recovery Email has been sent to Your Registerd Email ID Succesfully, You can reset your Password from there.";
                }
            }, function myError(response) {
                alert('Error: Invalid request processing...');
                $('.loader').hide();
            });
        }
        else
        {
            $('.loader').hide();
            return;            
        }
        $scope.submitted = false;
    };

    $scope.resetValidateUSerIdForm = function () {
        $scope.reg = {};
        $scope.reg.Title = 'Mr';
        $scope.reg.HodTitle = 'Mr';
        $scope.reg.EmailID = "";
        $scope.reg.MobileNo =  "";
        $scope.reg.HodEmailID = "";
        $scope.User = {};
        $scope.User.EmailID = "";
        $scope.forgotPasswordResponseText = "";
        $scope.submitted = false;
    }

    $scope.sendEmailNotification = function () {
        $http({
            method: "POST",
            url: "../bulk/sendEmailNotification",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).then(function mySucces(response) {

        }, function myError(response) {
            alert('Error: Invalid request processing...');
        });
    }
    $scope.GetVerticals();
});
//Send Rejection Email Notification to User :Non Selective
sendRejectionEmailNotification = function () {
    $.ajax({
        url: "../bulk/sendEmailNotification",
        success: function (result) {
        }
    });
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
