// Write your JavaScript code.

var app = angular.module('iGEMForm', ['ngFileUpload']);

//app.controller('TheForm', ['$scope', 'Upload', function ($scope, $http, Upload) {
//    // upload later on form submit or something similar
//    $scope.submit = function () {
//        if (form.file.$valid && $scope.file) {
//            $scope.upload($scope.file);
//        }
//    };

//    $scope.upload = function (file) {
//        Upload.upload({
//            url: 'upload/url',
//            data: { file: file, 'username': $scope.username }
//        }).then(function (resp) {
//            console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.data);
//        }, function (resp) {
//            console.log('Error status: ' + resp.status);
//        }, function (evt) {
//            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
//            console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
//        });
//    };

//    $scope.formData = {};
//    $scope.lastSavedTime = '';

//    $scope.whetherResearch = function ($scope) {
//        return this.formData.isResearch == "Yes";
//    }

//    $scope.submit = function () {
//        $http.post('/Apply/SubmitForm/', $scope.formData)
//            .then(function (result) {
//                window.location.href = '/Apply/SubmitFormSucceeded/';
//            });
//    }

//    $scope.save = function () {
//        $http.post('/Apply/SaveForm/', $scope.formData)
//            .then(function (result) {
//                $scope.lastSavedTime = result.data;
//            });
//    }

//    $scope.getSaved = function () {
//        $http.get('/Apply/GetSavedForm/')
//            .then(function (result) {
//                if (result.data != '') {
//                    $scope.formData = angular.fromJson(result.data);
//                    console.log(result.data);
//                } else {
//                    $http.get('/Apply/Clear/').then();
//                }
//            });
//    }

//    $scope.getExistForm = function (isExist, hashValue) {
//        if (isExist == 'Yes') {
//            $http.get('/Apply/GetExistForm', { params: hashValue })
//                .then(function (result) {
//                    if (result.data != '') {
//                        $scope.formData = angular.fromJson(result.data);
//                        console.log(result.data);
//                    } else {
//                        window.location.href = '/Apply/Error?errCode=1';
//                    }
//                });
//        }
//    }

//    $scope.init = function (isExistSaved, inputName, inputId) {
//        console.log(isExistSaved + ', got it!');
//        $scope.lastSavedTime = '';
//        if (isExistSaved == 'Yes') {
//            console.log('Got the data!');
//            $scope.getSaved();
//        } else {
//            $scope.formData = { 'isResearch': 'Yes', 'engType': 'highSchool', 'gender': 'M', 'name': inputName, 'birthDate': '1996-11', 'phone': '12345678900', 'email': 'a@c.com', 'grade': '2017', 'stuID': inputId, 'college': 'LSC', 'major': 'Major', 'stuFrom': 'CD', 'engGrade': '100', 'stuUnionText': 'StuUnion', 'researchText': 'Research', 'prizeText': 'Prize', 'introText': 'Intro' }
//        }
//    }

//    $scope.getGenderText = function (gender) {
//        if (gender == 'M') {
//            return '男';
//        } else {
//            return '女';
//        }
//    }

//    $scope.getEnglishTypeText = function (engType) {
//        if (engType == 'cet4') {
//            return '大学英语四级';
//        }
//        if (engType == 'cet6') {
//            return '大学英语六级';
//        }
//        if (engType == 'highSchool') {
//            return '高考英语';
//        }
//        if (engType == 'toeils') {
//            return '托福/雅思';
//        }
//    }

//}]);

app.controller('TheForm', ['$scope', '$http', 'Upload', function ($scope, $http, Upload) {
    $scope.submit = function () {
        if ($scope.form.file.$valid && $scope.file) {
            $scope.upload($scope.file);
        }
    };

    $scope.upload = function (file, type) {
        Upload.upload({
            url: '/Apply/UploadFileToCache/',
            data: { file: file }
        }).then(function (resp) {
            console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.data);
            if (type == 'photo') {
                $scope.formData.photoFileName = resp.data;
                console.log('Photo uploaded.');
            } else if (type == 'appendix') {
                $scope.formData.appendixFileName = resp.data;
                console.log('Appendix uploaded.');
            }
        }, function (resp) {
            console.log('Error status: ' + resp.status);
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
        });
    };

    $scope.formData = {};
    $scope.lastSavedTime = '';

    $scope.whetherResearch = function ($scope) {
        return this.formData.isResearch == "Yes";
    }

    $scope.submitForm = function () {

        $scope.errList = '';
        $scope.isValid = true;

        var regexId = /[0-9]{13}/;
        var regexBirth = /^[0-9]{4}\-[0-9]{1,2}$/;
        var regexGrade = /^(201[0-9]{1})$/;
        var regexEngGrade = /^([0-9]{1,3})$/;
        var regexEmail = /^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$/;

        if ($scope.formData.name == '') {
            $scope.errList = $scope.errList + '姓名空。\n';
            console.log($scope.errList);
            $scope.isValid = false;
        }
        if ($scope.formData.stuID == '') {
            console.log($scope.errList);
            $scope.errList = $scope.errList + '学号空。\n';
            $scope.isValid = false;
        } else if (!regexId.test($scope.formData.stuID)) {
            $scope.errList = $scope.errList + '学号不合法。\n';
            $scope.isValid = false;
        }
        if ($scope.formData.birthDate == '') {
            console.log($scope.errList);
            $scope.errList = $scope.errList + '出生日期未输入。\n';
            $scope.isValid = false;
        } else if (!regexBirth.test($scope.formData.birthDate)) {
            $scope.errList = $scope.errList + '出生日期不合法。\n';
            $scope.isValid = false;
        }
        if ($scope.formData.email == '') {
            console.log($scope.errList);
            $scope.errList = $scope.errList + '电子邮件未输入。\n';
            $scope.isValid = false;
        } else if (!regexEmail.test($scope.formData.email)) {
            $scope.errList = $scope.errList + '电子邮件不合法。\n';
            $scope.isValid = false;
        }
        if ($scope.formData.grade == '') {
            console.log($scope.errList);
            $scope.errList = $scope.errList + '年级未输入。\n';
            $scope.isValid = false;
        } else if (!regexGrade.test($scope.formData.grade)) {
            $scope.errList = $scope.errList + '年级不合法。\n';
            $scope.isValid = false;
        }
        if ($scope.formData.engGrade == '') {
            console.log($scope.errList);
            $scope.errList = $scope.errList + '英语成绩未输入。\n';
            $scope.isValid = false;
        } else if (!regexEngGrade.test($scope.formData.engGrade)) {
            $scope.errList = $scope.errList + '英语成绩不合法。\n';
            $scope.isValid = false;
        }
        if ($scope.formData.photoFileName == '') {
            $scope.errList = $scope.errList + '未上传照片。\n';
            console.log($scope.errList);
            $scope.isValid = false;
        }
        if ($scope.formData.phone == '') {
            $scope.errList = $scope.errList + '电话未输入。\n';
            console.log($scope.errList);
            $scope.isValid = false;
        }

        if ($scope.isValid) {
            $http.post('/Apply/SubmitForm/', $scope.formData)
                .then(function (result) {
                    window.location.href = '/Apply/SubmitFormSucceeded/';
                });
        }
    }

    $scope.save = function () {
        $http.post('/Apply/SaveForm/', $scope.formData)
            .then(function (result) {
                $scope.lastSavedTime = result.data;
            });
    }

    $scope.getSaved = function () {
        $http.get('/Apply/GetSavedForm/')
            .then(function (result) {
                if (result.data != '') {
                    $scope.formData = angular.fromJson(result.data);
                    console.log(result.data);
                } else {
                    $http.get('/Apply/Clear/').then();
                }
            });
    }

    $scope.getExistForm = function (isExist, hashValue) {
        if (isExist == 'Yes') {
            $http.get('/Apply/GetExistForm', { params: hashValue })
                .then(function (result) {
                    if (result.data != '') {
                        $scope.formData = angular.fromJson(result.data);
                        console.log(result.data);
                    } else {
                        window.location.href = '/Apply/Error?errCode=1';
                    }
                });
        }
    }

    $scope.init = function (isExistSaved, inputName, inputId) {
        console.log(isExistSaved + ', got it!');
        $scope.lastSavedTime = '';
        if (isExistSaved == 'Yes') {
            console.log('Got the data!');
            $scope.getSaved();
        } else {
            $scope.formData = { 'appendixFileName': '', 'photoFileName': '', 'isResearch': 'Yes', 'engType': 'highSchool', 'gender': 'M', 'name': inputName, 'birthDate': '1996-11', 'phone': '12345678900', 'email': 'a@c.com', 'grade': '2017', 'stuID': inputId, 'college': 'LSC', 'major': 'Major', 'stuFrom': 'CD', 'engGrade': '100', 'stuUnionText': 'StuUnion', 'researchText': 'Research', 'prizeText': 'Prize', 'introText': 'Intro' }
        }
    }

    $scope.getGenderText = function (gender) {
        if (gender == 'M') {
            return '男';
        } else {
            return '女';
        }
    }

    $scope.getEnglishTypeText = function (engType) {
        if (engType == 'cet4') {
            return '大学英语四级';
        }
        if (engType == 'cet6') {
            return '大学英语六级';
        }
        if (engType == 'highSchool') {
            return '高考英语';
        }
        if (engType == 'toeils') {
            return '托福/雅思';
        }
    }

    $scope.getFilePath = function (type) {
        if (type == 'photo') {
            return '/uploads/photos/' + $scope.formData.photoFileName;
        }
        if (type == 'appendix') {
            return '/uploads/appendices/' + $scope.formData.appendixFileName;
        }
    }
}]);

app.controller('InitInfo', function ($scope, $http) {

    $scope.initData = {};
    $scope.isValid = true;
    $scope.errList = '';

    $scope.submitData = function () {
        $scope.errList = '';
        $scope.isValid = true;
        var regexId = /[0-9]{13}/;
        if ($scope.initData.Name == '') {
            $scope.errList = $scope.errList + '请输入姓名。\n';
            console.log($scope.errList);
            $scope.isValid = false;
        }
        if ($scope.initData.StuID == '') {
            console.log($scope.errList);
            $scope.errList = $scope.errList + '请输入学号。\n';
            $scope.isValid = false;
        } else if (!regexId.test($scope.initData.StuID)) {
            $scope.errList = $scope.errList + '请输入合法的学号。\n';
            $scope.isValid = false;
        }

        if ($scope.isValid) {
            console.log('/Apply/Welcome?name=' + $scope.initData.Name + '&stuId=' + $scope.initData.StuID);
            window.location.href = '/Apply/Welcome?name=' + $scope.initData.Name + '&stuId=' + $scope.initData.StuID;
        }

        $scope.initData = { 'Name': '', 'StuID': '' }; $scope.test = '';
    }

});

app.controller('BriefInfo', function ($scope) {

    $scope.briefInfo = {};

});

app.controller('Welcome', function ($scope) {

    $scope.briefInfo = {};

});


