// Write your JavaScript code.

var app = angular.module('iGEMForm', []);

app.controller('TheForm', function ($scope) {

    $scope.formData = {};

    $scope.whetherResearch = function ($scope) {
        return this.formData.IsResearch == "Yes";
    }
});

app.controller('InitInfo', function ($scope, $http) {

    $scope.initData = {};

});

app.controller('BriefInfo', function ($scope) {

    $scope.briefInfo = {};

});

