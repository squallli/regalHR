var AngularApp = angular.module('EmpGroupForm',[]);

AngularApp.controller('Ctrl', function ($scope) {

    $scope.EmpGroup = angular.fromJson(EmpGroup);

    $scope.InGroup = function (Group, Idx) {
        $scope.EmpGroup.NoGroupList.splice(Idx, 1);
        $scope.EmpGroup.GroupList.push(Group);
        blur();
    }




    $scope.OutGroup = function (Group, Idx) {

        $scope.EmpGroup.GroupList.splice(Idx, 1);
        $scope.EmpGroup.NoGroupList.push(Group);
        blur();
    }


    /*
    $scope.UpdateGroup = function () {
 
        for (i = 1; i < $scope.EmpGroupList.length; i++) {
            
            if ($scope.EmpGroupList[i].GroupID == $scope.DropdownGroup) {
                $scope.GroupList = $scope.EmpGroupList[i];
            }
        }

    }
    */


    $scope.SaveBtn = function () {

        //編輯
        $.ajax({
            type: "POST",
            url: "../EmpGroup/EmpGroupSave",
            dataType: 'Json',
            data: 'EmpGroupJson=' + angular.toJson($scope.EmpGroup),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {
                    //SysShowAlert("編輯成功!", "alert-success");
                    document.location.href = "../EmpGroup/?action=BACK";
                    
                } else {
                    SysShowAlert("編輯失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });


    }



    $scope.BackBtn = function () {
        location.href = "./?action=BACK";
    }


    
});


