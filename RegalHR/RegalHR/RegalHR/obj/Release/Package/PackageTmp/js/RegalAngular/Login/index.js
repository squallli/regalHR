var AngularApp = angular.module('index', []);

AngularApp.controller('Ctrl', function ($scope) {
   

    $scope.ErrMsg = "";
    $scope.LoginBtn = function (LoginId,LoginPwd) {
        $.ajax({
            type: "POST",
            url: "../Login/LoginSys",
            dataType: 'Json',
            data: 'LoginId=' + encodeURIComponent(LoginId) + "&LoginPwd=" + encodeURIComponent(LoginPwd),
            async: false,
            success: function (data) {
                if (data == "1") {
                    location.href = "../";//登入
                } else {
                    $scope.ErrMsg = "登入失敗! " + data;

                }
            }
        })
    }


    $scope.ClearErrMsg = function () {
        $scope.ErrMsg = "";
    }

});


