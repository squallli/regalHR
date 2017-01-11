var AngularApp = angular.module('ProgramForm', []);

AngularApp.controller('Ctrl', function ($scope) {


    $scope.Program = angular.fromJson(Program);


    $scope.AlarmMsg = "";
    $scope.CheckResult = false;
    $scope.BackBtn = function () {
        location.href = "./?action=BACK";
    }



    
    $scope.CheckForm = function () {
        //檢查表單
        
        $scope.AlarmMsg = "";

        if ($scope.Program.ProgID <1 )
        {
            $scope.AlarmMsg = "請輸入作業代碼!";

        } else if (!angular.isNumber($scope.Program.Power)) {
            $scope.AlarmMsg = "Power輸入不正確!";
        }





        if ($scope.AlarmMsg == "") {
            $scope.CheckResult = true;
            return true;//通過檢驗
        } else {
            $scope.CheckResult = false;
            return false;//未通過檢驗
        }

    }





    $scope.AddBtn = function () {
        //新增

        $.ajax({
            type: "POST",
            url: "../Program/ProgramFormAdd",
            dataType: 'Json',
            data: 'ProgramJson=' + angular.toJson($scope.Program),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    //SysShowAlert("新增成功!", "alert-success");
                    //var Temp = angular.toJson(NewProgram);
                    //$scope.Program = angular.fromJson(Temp);
                    document.location.href = "../Program/?action=BACK";
                } else {
                    SysShowAlert("新增失敗!" + data, "alert-danger");
                }
            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });

    }




    $scope.EditBtn = function () {
        //編輯
        $.ajax({
            type: "POST",
            url: "../Program/ProgramFormEdit",
            dataType: 'Json',
            data: 'ProgramJson=' + angular.toJson($scope.Program),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    SysShowAlert("編輯成功!", "alert-success");
                    $scope.Program.EditPK = $scope.Program.ProgID;
                } else {
                    SysShowAlert("編輯失敗!" + data, "alert-danger");
                }
            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });

    }



});


