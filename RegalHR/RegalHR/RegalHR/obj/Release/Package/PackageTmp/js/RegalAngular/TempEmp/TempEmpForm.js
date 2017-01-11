var AngularApp = angular.module('TempEmpForm', []);

AngularApp.controller('Ctrl', function ($scope) {

    
    $scope.Emp = angular.fromJson(Emp);

    $scope.DepartMentList = angular.fromJson(DepartMentList);

    $scope.CompanyList = angular.fromJson(CompanyList);
    $scope.TurnEmpList = [];


    $scope.AlarmMsg = "";
    $scope.CheckResult = false;


    $scope.BackBtn = function () {
        location.href = "./?action=BACK";
    }



    $scope.CheckTurnEmpNo = function () {
        if ($scope.TurnEmpNo == null) {
            return false;
        } else {
            return true;
        }
    }
    

    $scope.SetExpiryDateBtn = function () {
        Emp.CardExpiryDate = "9999-12-31";
    }
    
    $scope.CheckForm = function () {
        //檢查表單

        $scope.AlarmMsg = "";



        if ($scope.Emp.EmployeeNo == null || $scope.Emp.EmployeeNo == "")
        {
            $scope.AlarmMsg = "請輸入員工編號!";
        } else if ($scope.Emp.EmployeeName == null || $scope.Emp.EmployeeName == "") {
            $scope.AlarmMsg = "請輸入姓名!";
        } else if ($scope.Emp.EmployeeEName == null || $scope.Emp.EmployeeEName == "") {
            $scope.AlarmMsg = "請輸入英文姓名!";
        } else if ($scope.Emp.CardNo != null && $scope.Emp.CardNo != "") {
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
            url: "../TempEmp/TempEmpFormAdd",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    //SysShowAlert("新增成功!", "alert-success");
                    //var Temp = angular.toJson(NewEmp);
                    //$scope.Emp = angular.fromJson(Temp);

                    document.location.href = "../TempEmp/?action=BACK";
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
            url: "../TempEmp/TempEmpFormEdit",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    $scope.BackBtn();//返回
                    //SysShowAlert("編輯成功!", "alert-success");
                    //$scope.Emp.EditPK = $scope.Emp.EmployeeNo;

                } else {
                    SysShowAlert("編輯失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });

    }




    //轉正式員工訊息
    $scope.TurnEmpMsgBtn = function () {
        $('#TurnEmpMsgModal').modal('show');

        $.ajax({
            type: "POST",
            url: "../TempEmp/GetTurnEmpList",
            data: "EmpJson="+angular.toJson($scope.Emp),
            dataType: 'Json',
            async: false,
            timeout: 10000,
            success: function (data) {
                $scope.TurnEmpList = data;
            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });


    }


    $scope.TurnEmpBtn = function (EmpNo) {
        

        //編輯
        $.ajax({
            type: "POST",
            url: "../TempEmp/TempEmpFormTurn",
            dataType: 'Json',
            data: 'TurnEmpNo=' + EmpNo + '&EmpJson=' + angular.toJson($scope.Emp),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {

                    location.href = "../TempEmp/";
                } else {
                    SysShowAlert("轉正式失敗!" + data, "alert-danger");
                    $('#TurnEmpMsgModal').modal('hide');
                }

            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
                $('#TurnEmpMsgModal').modal('hide');
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
            url: "../TempEmp/DeleteCardNo",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    $('#CardDelMsgModal').modal('hide');
                    //SysShowAlert("註銷成功!", "alert-success");

                    //$scope.Emp.CardStatus = "0";
                    //$scope.Emp.CardNo=null;
                    location.href = "./TempEmpForm";

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

    

    $scope.CardAddBtn = function () {

        if ($scope.NewCardNo == null || $scope.NewCardNo=="") {
            alert("請輸入卡號");
            return;
        }

        //編輯
        $.ajax({
            type: "POST",
            url: "../TempEmp/InsertCardNo",
            dataType: 'Json',
            data: 'EmpJson=' + angular.toJson($scope.Emp) + "&NewCardNo=" + $scope.NewCardNo,
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    $('#CardAddMsgModal').modal('hide');
                    //SysShowAlert("新增卡號成功!", "alert-success");

                    //$scope.Emp.CardStatus = "1";
                    //$scope.Emp.CardNo = $scope.NewCardNo;
                    location.href = "./TempEmpForm";

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


