var AngularApp = angular.module('Manage', []);


AngularApp.controller('Ctrl', function ($scope) {
   
    $scope.CompanyList = angular.fromJson(CompanyList);
    $scope.DepartMentList = angular.fromJson(DepartMentList);



    $scope.GetEmpList = function () {

        $.ajax({
            type: "POST",
            url: "../Emp/GetEmpList",
            dataType: 'Json',
            data: 'FulltimeFlag=&company=' + $scope.UserFreeStyle.SearchCompany + "&DepartMentNo=" + $scope.UserFreeStyle.SearchDepartMentNo + "&status=" + $scope.UserFreeStyle.SearchEmpStatus + "&Sex=",
            async: false,
            success: function (data) {
                $scope.EmpDropDownList = data;
            }
        });
    }



    $scope.update = function (Url, PageNum) {
        
            $.ajax({
                type: "POST",
                url: Url,
                dataType: 'Json',
                data: 'PageNum=' + PageNum,
                async: false,
                success: function (data) {
                    $scope.UserFreeStyle = angular.fromJson(UserFreeStyle);

                    $scope.UserFreeStyle.PageNum = data.Page;




                    $scope.DataList = data.DataList;
                    $scope.Page = data.Page;
                    $scope.PageTotal = data.PageTotal-1;
                    $scope.PageTotalDisplay = data.PageTotal;
                    $scope.PageLimit = data.PageLimit;
                    $scope.PageBegin = $scope.Page - 5;
                    $scope.SignId = $scope.UserFreeStyle.SignId;//標記點選過的
                    $scope.Serial = (data.Page * data.PageLimit) + 1;//序號起始
                    




                    if ($scope.PageBegin < 0)
                        $scope.PageBegin = 0;


                    var PaginationList = [];


                    for(var i=0;i<data.PageTotal;i++)
                    {
                        var Obj={};
                        Obj.PageNum=i;
                        Obj.PageDisplay=i+1;
                        Obj.Url=data.TargetURL;
                        PaginationList.push(Obj);
                    }

                    

                    $scope.PaginationList = PaginationList;



                    $.ajax({
                        type: "POST",
                        url: "../Outgoing/GetEmpList",
                        data: 'DepartMentNo=' + $scope.UserFreeStyle.SearchText,
                        async: false,
                        success: function (data) {

                            $scope.EmpList = angular.fromJson(data);

                        }
                    })



                },
                error: function (err) {
                    alert('Error!');
                }
            });
    };


    
    $scope.update("../Outgoing/GridViewer", UserFreeStyle.PageNum);
    $scope.GetEmpList();


    $scope.Orderby = function (id, OrderField) {

        $('#DataGridView th span').html("");//CLEAR


        
   
        if ($scope.UserFreeStyle.OrderField == OrderField) {
            if ($scope.UserFreeStyle.OrderType == "ASC") {
                $scope.UserFreeStyle.OrderType = "DESC";
                $('#DataGridView th[data-id="' + id + '"] span').html("▲");
            } else {
                $scope.UserFreeStyle.OrderType = "ASC";
                $('#DataGridView th[data-id="' + id + '"] span').html("▼");
            }
        } else {
            $scope.UserFreeStyle.OrderField = OrderField;
            $scope.UserFreeStyle.OrderType = "ASC";
            $('#DataGridView th[data-id="' + id + '"] span').html("▼");
        }


        

        $.ajax({
            type: "POST",
            url: "../Outgoing/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {

                $scope.update("../Outgoing/GridViewer", 0);

            },
            error: function () {
                alert("Error !");
            }
        })
    }

    $scope.Search = function () {
        $.ajax({
            type: "POST",
            url: "../Outgoing/SaveUserFreeStyle",
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                $scope.update("../Outgoing/GridViewer", 0);
            },
            error: function () {
                alert("Error !");
            }
        });
    }






    $scope.Link = function (RecordID) {

        $scope.UserFreeStyle.SignId = RecordID;
 

        $.ajax({
            type: "POST",
            url: "../Outgoing/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                document.location.href = "../Outgoing/ManageOutMan";
            },
            error: function () {
                alert("Error !");
            }
        })
    }




    
});


