var AngularApp = angular.module('GroupForm', []);

AngularApp.controller('Ctrl', function ($scope) {


    $scope.Group = angular.fromJson(Group);
    $scope.ViewLevelList = angular.fromJson(ViewLevelList);
    


    $scope.AlarmMsg = "";
    $scope.CheckResult = false;
    $scope.BackBtn = function () {
        location.href = "./?action=BACK";
    }


    $scope.CheckForm = function () {
        //檢查表單

        $scope.AlarmMsg = "";


        if ($scope.Group.GroupID == null || $scope.Group.GroupID == "") {
            
            $scope.AlarmMsg = "請輸入群組代碼!";
        } else if ($scope.Group.GroupName == null || $scope.Group.GroupName == "") {
            $scope.AlarmMsg = "請輸入群組名稱!";
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
            url: "../Group/GroupFormAdd",
            dataType: 'Json',
            data: 'GroupJson=' + angular.toJson($scope.Group),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    //SysShowAlert("新增成功!", "alert-success");
                    //var Temp = angular.toJson(NewProgram);
                    //$scope.Program = angular.fromJson(Temp);
                    document.location.href = "../Group/?action=BACK";
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
            url: "../Group/GroupFormEdit",
            dataType: 'Json',
            data: 'GroupJson=' + angular.toJson($scope.Group),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    //SysShowAlert("編輯成功!", "alert-success");
                    //$scope.Group.EditPK = $scope.Group.GroupID;
                    document.location.href = "../Group/?action=BACK";
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


