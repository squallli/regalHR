var AngularApp = angular.module('index2', []);


AngularApp.controller('Ctrl', function ($scope, $timeout) {

    $scope.Data;
    $scope.tmp = false;
    $scope.GetData = function () {
        var Model = [];


        for (i = 0; i < 10;i++)
        {
            var Obj = JSON.parse('{"No":""}');

            Obj.No = "No. " + i;
            Obj.Ans1 = Math.floor(Math.random() * 1000);
            Obj.Ans2 = Math.floor(Math.random() * 1000);
            Obj.Ans3 = Math.floor(Math.random() * 1000);
            Obj.Ans4 = Math.floor(Math.random() * 1000);
            Obj.Ans5 = Math.floor(Math.random() * 1000);
            Obj.Ans6 = Math.floor(Math.random() * 1000);
            Obj.Ans7 = Math.floor(Math.random() * 1000);
            Obj.Ans8 = Math.floor(Math.random() * 1000);
            Obj.Ans9 = Math.floor(Math.random() * 1000);
            Obj.Ans10 = Math.floor(Math.random() * 1000);


            Model.push(Obj);
        }

        $scope.Data = Model;
        $timeout($scope.GetData, 1);
    }

    $scope.GetData();




    $scope.expression = function () {
        $scope.tmp = !$scope.tmp;
    }
});


AngularApp.directive("test1", function () {
    return {
        restrict: "E",
        templateUrl: "../../../js/RegalAngular/test/tmp.html"
        ,
        //template: "<div>### {{Data[0].No}} ### {{Data[0].Ans1}}</div> ",
        replace:true
    };
});


AngularApp.directive("test2", function () {
    return {
        restrict: "E",
        template: "<div>### Hello ###</div> ",
        replace: true
    };
});



