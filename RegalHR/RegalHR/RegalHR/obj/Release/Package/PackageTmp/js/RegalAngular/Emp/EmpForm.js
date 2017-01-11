var AngularApp = angular.module('EmpForm', []);

AngularApp.controller('Ctrl', function ($scope) {

    
    $scope.Emp = angular.fromJson(Emp);

    $scope.AlarmMsg = "";
    $scope.CheckResult = false;


    $scope.BackBtn = function () {
        location.href = "./?action=BACK";
    }



    
    $scope.CheckForm = function () {
        //檢查表單

        $scope.AlarmMsg = "";



        if ($scope.Emp.CardNo != null && $scope.Emp.CardNo != "") {
            //代表要檢驗日期是否需要填寫
            if ($scope.Emp.CardEffectiveDate == null || $scope.Emp.CardEffectiveDate == "") {
                $scope.AlarmMsg = "請輸入卡片生效日!";
            } else if ($scope.Emp.CardExpiryDate == null || $scope.Emp.CardExpiryDate == "") {
                $scope.AlarmMsg = "請輸入卡片失效日!";
            }
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
            url: "../Emp/EmpFormAdd",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    //SysShowAlert("新增成功!", "alert-success");
                    //var Temp = angular.toJson(NewEmp);
                    //$scope.Emp = angular.fromJson(Temp);

                    document.location.href = "../Emp/?action=BACK";
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
            url: "../Emp/EmpFormEdit",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    //SysShowAlert("編輯成功!", "alert-success");
                    //$scope.Emp.EditPK = $scope.Emp.EmployeeNo;

                    document.location.href = "../Emp/?action=BACK";
                } else {
                    SysShowAlert("編輯失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });

    }



    $scope.CardDelMsgBtn = function () {
        $('#CardDelMsgModal').modal('show');
    }




    $scope.CardDelBtn = function () {




        //編輯
        $.ajax({
            type: "POST",
            url: "../Emp/DeleteCardNo",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    $('#CardDelMsgModal').modal('hide');

                    location.href = "./EmpForm";

                } else {
                    $('#CardDelMsgModal').modal('hide');
                    SysShowAlert("註銷失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                $('#CardDelMsgModal').modal('hide');
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });


    }
    $scope.CardAddMsgBtn = function () {

        $scope.NewCardNo = "";
        $('#CardAddMsgModal').modal('show');
    }

    
    $scope.SetExpiryDateBtn = function () {
        Emp.CardExpiryDate = "9999-12-31";
    }

    $scope.CardAddBtn = function () {


        //編輯
        $.ajax({
            type: "POST",
            url: "../Emp/InsertCardNo",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp) + "&NewCardNo=" + $scope.NewCardNo,
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    $('#CardAddMsgModal').modal('hide');
                    location.href = "./EmpForm";

                } else {
                    $('#CardAddMsgModal').modal('hide');
                    SysShowAlert("新增卡號失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                $('#CardAddMsgModal').modal('hide');
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });


    }

});


