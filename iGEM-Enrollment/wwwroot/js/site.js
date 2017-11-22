// Write your JavaScript code.

var app = angular.module('iGEMForm', []);

app.controller('TheForm', function ($scope, $http) {

    $scope.formData = {};

    $scope.whetherResearch = function ($scope) {
        return this.formData.isResearch == "Yes";
    }

    $scope.submit = function() {
        $http.post('/Apply/SubmitForm/', $scope.formData).then();
    }
});

app.controller('InitInfo', function ($scope, $http) {

    $scope.initData = {};

});

app.controller('BriefInfo', function ($scope) {

    $scope.briefInfo = {};

});

