var debug = true; //TODO release checklist this
            
var app = angular.module('ionicApp', ['ionic', 'ngCordova', 'ngMessages'])//,  , 'satellizer'

.config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider
    .state('tabs.account', {
        url: "/account",
        views: {
            'home-tab': {
                templateUrl: "../../content/_tm/tabs/account.html"
            }
        }
    })
    .state('tabs.settings', {
          url: "/settings",
          views: {
              'home-tab': {
                  templateUrl: "../../content/_tm/tabs/settings.html"
              }
          }
      })
        .state('tabs.profile', {
            url: "/profile",
            views: {
                'home-tab': {
                    templateUrl: "../../content/_tm/tabs/profile.html"
                    //,                    controller: 'UserProfileCtrl'
                }
            }
        })
        .state('tabs.signin', {
              url: "/signin",
              views: {
                  'home-tab': {
                      templateUrl: "../../content/_tm/tabs/signin.html"
                      , controller: 'UserSignInCtrl'
                  }
              }
          })
        .state('tabs.signup', {
             url: "/signup",
             views: {
                 'home-tab': {
                     templateUrl: "../../content/_tm/tabs/signup.html"
                     , controller: 'UserSignUpCtrl'
                 }
             }
         })
      .state('tabs', {
          url: "/tab",
          abstract: true
          ,templateUrl: "tabs.html"
      })
      .state('tabs.home', {
          url: "/home",
          views: {
              'home-tab': {
                  templateUrl: "../../content/_tm/tabs/home.html",
                  controller: 'HomeTabCtrl'
              }
          }
      })
        .state('tabs.facts', {
            url: "/facts",
            views: {
                'home-tab': {
                    templateUrl: "../../content/_tm/tabs/facts.html"
                }
            }
        })
        .state('tabs.facts2', {
            url: "/facts2",
            views: {
                'home-tab': {
                    templateUrl: "../../content/_tm/tabs/facts2.html"
                }
            }
        })
      .state('tabs.navstack', {
          url: "/navstack",
          views: {
              'about-tab': {
                  templateUrl: "../../content/_tm/tabs/nav-stack.html"
              }
          }
      })
      .state('tabs.logs', {
          url: "/logs",
          views: {
              'logs-tab': {
                  templateUrl: "../../content/_tm/tabs/logs.html"
              }
          }
      })
      .state('tabs.help', {
          url: "/help",
          views: {
              'help-tab': {
                  templateUrl: "../../content/_tm/tabs/help.html"
              }
          }
      })
        .state('tabs.about', {
            url: "/about",
            views: {
                'help-tab': { //NOTE: this is under what tab you want to display the content. So about will be under the 'help-tab'. if you don't match the tab to the view it will appear the link doesn't work.
                    templateUrl: "../../content/_tm/tabs/about.html"
                }
            }
        })
        .state('tabs.contact', {
            url: "/contact",
            views: {
                'help-tab': {
                    templateUrl: "../../content/_tm/tabs/contact.html"
                }
            }
        })
        .state('tabs.terms', {
            url: "/terms",
            views: {
                'help-tab': {
                    templateUrl: "../../content/_tm/tabs/terms.html"
                }
            }
        });

    $urlRouterProvider.otherwise("/tab/home");
})
//satellizer functions
//.config(function($authProvider) {
//    var commonConfig = {
//        popupOptions: {
//            location: 'no',
//            toolbar: 'yes',
//            width: window.screen.width,
//            height: window.screen.height
//        }
//    };
//    if (ionic.Platform.isIOS() || ionic.Platform.isAndroid()) {
//        commonConfig.redirectUri = 'http://localhost/';
//    }
//    $authProvider.facebook(angular.extend({}, commonConfig, {
//        clientId: 'YOUR_FACEBOOK_APP_ID',
//        url: 'http://localhost:3000/auth/facebook'
//    }));
//    $authProvider.twitter(angular.extend({}, commonConfig, {
//        url: 'http://localhost:3000/auth/twitter'
//    }));
//    $authProvider.google(angular.extend({}, commonConfig, {
//        clientId: 'YOUR_GOOGLE_CLIENT_ID',
//        url: 'http://localhost:3000/auth/google'
//    }));
//})
//  .run(function ($ionicPlatform) {
//      $ionicPlatform.ready(function () {
//          if (window.cordova && window.cordova.plugins.Keyboard) {
//              cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
//          }
//          if (window.StatusBar) {
//              StatusBar.styleDefault();
//          }
//      });
//  })
//END satellizer functions
.controller('NavCtrl', function ($scope, $ionicSideMenuDelegate) {
    $scope.showMenu = function () {
        $ionicSideMenuDelegate.toggleLeft();
    };
    $scope.showRightMenu = function () {
        $ionicSideMenuDelegate.toggleRight();
    };
})
.controller('UserProfileCtrl', function ($scope,  User) {
  
    $scope.Initialize = function () {
      //  User.Load();
    };

    $scope.IsLoggedIn = function () {
        console.log("TODO IMPLEMENT");
        return false;
    }

    $scope.SaveProfile = function (profile) {
        User.Save(profile);
    };

    $scope.userProfile = User; //make the user available to the view.

})
.controller('UserSignUpCtrl', function ($scope, $state,$http, $ionicNavBarDelegate, $ionicSideMenuDelegate,$ionicLoading, User, SERVER) {

        var model = this;
        model.Message = "";

        model.User = {
            UserName: "",
            UserEmail: "",
            UserPassword: "",
            UserPasswordConfirm: "",
            ClientType: "mobile.app"
        };

        $scope.show = function () {
            $ionicLoading.show({
                template: '<p>Loading...</p><ion-spinner></ion-spinner>'
            });
        };

        $scope.hide = function () {
            $ionicLoading.hide();
        };

        $ionicNavBarDelegate.showBackButton(false);
        $ionicSideMenuDelegate.canDragContent(false);

        $scope.$on('$ionicView.leave', function () { $ionicSideMenuDelegate.canDragContent(true) });

        $scope.Initialize = function () {

        };

        $scope.IsLoggedIn = function () {
            var checkSessionPromise = User.CheckSession();
            if (!isFunction(checkSessionPromise))
                return checkSessionPromise;


            checkSessionPromise.then(function (result) {
                if (result.data.Code != 200) {
                    return false;
                }
                return true;
            });
        };

        $scope.CreateAccount = function (isValidForm) {  //model.submit = function (isValid)

            $scope.show($ionicLoading);
            model.Message = "";

            if (!isValidForm) {
                model.Message = "There are still invalid fields below";
                return false;
            }

            var url = SERVER.url + 'api/Accounts/Register';

            $http.post(url, model.User )
              .success(function (response) {
                  if (response.Code != 200) {
                      model.Message = response.Message;
                      return false;
                  }
                  model.Message = "Account created.";
                  var loginResult = User.Login(model.User.UserName, model.User.UserPassword);
                  if (loginResult != true) {
                      model.Message = loginResult;
                     
                      //let user see message for a second.
                      setTimeout(function () {
                          // Whatever you want to do after the wait
                      }, 3300);//3000 = 3 seconds
                  }

                  $state.go('tabs.home');
                  return true;
              }).error(function (response) {
                  
                  model.Message = "An error occured while registering.<br/>Try again later.";

                  if (debug && response.Message) {
                      model.Message = response.Message;
                  }
                  else if (debug) {
                      model.Message = response.code;
                  }

                  return false;
              }).finally(function ($ionicLoading) {
                  // On both cases hide the loading
                  $scope.hide($ionicLoading);
              });
        };
})
.controller('UserSignInCtrl', function ($scope, $state, $ionicNavBarDelegate, $ionicSideMenuDelegate,$ionicLoading, User) { //$ionicPopup, $auth, $http,

    var model = this;
    model.Message = "";

    model.User = {
        UserName: "",
        Password: "",
    };

    $ionicNavBarDelegate.showBackButton(false);
    $ionicSideMenuDelegate.canDragContent(false);

    $scope.$on('$ionicView.leave', function () { $ionicSideMenuDelegate.canDragContent(true) });

    $scope.Initialize = function () {

    };

    $scope.IsLoggedIn = function () {
        var checkSessionPromise = User.CheckSession();
        if (!isFunction(checkSessionPromise))
            return checkSessionPromise;

        checkSessionPromise.then(function (result) {
            if (result.data.Code != 200) {
                return false;
            }
            return true;
        });
    };

    $scope.Authenticate = function (isValidForm) {  //model.submit = function (isValid)
        $scope.show($ionicLoading);
        model.Message = "";

        if (!isValidForm) {
            model.Message = "There are still invalid fields below";
            return false;
        }
        User.UserName = model.User.UserName;
        User.Email = model.User.Email;
        User.Password = model.User.Password;

        var loginPromise = User.Login(User.UserName, User.Password);
        loginPromise.then(function (result) {
        
            $scope.hide($ionicLoading);
          
            if (result.data.Code != 200) {
                model.Message = result.data.Message;
                return false;
            }
            $state.go('tabs.home');
            return true;
        });
        return true;
    };

    $scope.show = function () {
        $ionicLoading.show({
            template: '<p>Logging in...</p><ion-spinner></ion-spinner>'
        });
    };

    $scope.hide = function () {
        $ionicLoading.hide();
    };

    //==========================================================================================================
    //$scope.authenticate = function(provider) {
    //    $auth.authenticate(provider)
    //        .then(function() {
    //            $ionicPopup.alert({
    //                title: 'Success',
    //                content: 'You have successfully logged in!'
    //            })
    //        })
    //        .catch(function(error) {
    //            $ionicPopup.alert({
    //                title: 'Error',
    //                content: error.message || (error.data && error.data.message) || error
    //            });
    //        });
    //};
    //$scope.logout = function() {
    //        $auth.logout();
    //    };
    //$scope.isAuthenticated = function() {
    //        return $auth.isAuthenticated();
    //    };
})
.controller('HomeTabCtrl', function ($scope,  $location, User, tmApp) {
    var model = this;
    model.Message = "";

    $scope.Initialize = function () {
             if ( tmApp.RunCount() === 0 ) {
            $location.path('/tab/signup');
            return;
        }

        var checkSessionPromise = User.CheckSession();

        console.log('checkSessionPromise:' + checkSessionPromise);

        if (!isFunction(checkSessionPromise)) {
            if (checkSessionPromise === false) {
                $location.path('/tab/signin');
            }
            return checkSessionPromise;
        }

        checkSessionPromise.then(function (result) {
            if (result.data.Code != 200) {
                $location.path('/tab/signin');
            }
        });
    };

    $scope.IsLoggedIn = function () {
        var checkSessionPromise = User.CheckSession();
       
        if (!isFunction(checkSessionPromise))
            return checkSessionPromise;

        checkSessionPromise.then(function (result) {
            if (result.data.Code != 200) {
                return false;
            }
            return true;
        });
    };

    $scope.Logout = function () {
        User.Logout();
        $location.path('/tab/signin');
    }
   
})
.controller('AboutTabCtrl', function ($scope, $location, tmApp) {
    console.log('tmpAppName:' + tmApp.Name);

    $scope.mobileApp = tmApp; //make the tmApp available to the view.
})
.controller('ContactTabCtrl', function ($scope, $http,$state, $location, $ionicLoading, User,SERVER) {
        var form = this;
        form.Message = "";

        form.Mail = {
            Subject: "",
            Message: "",
            MessageType: "ContactAdmin"
        };

        $scope.Initialize = function () {
         
            var checkSessionPromise = User.CheckSession();

            console.log('checkSessionPromise:' + checkSessionPromise);

            if (!isFunction(checkSessionPromise)) {
                if (checkSessionPromise === false) {
                    $location.path('/tab/signin');
                }
                return checkSessionPromise;
            }

            checkSessionPromise.then(function (result) {
                if (result.data.Code != 200) {
                    $location.path('/tab/signin');
                }
            });
        };

        $scope.IsLoggedIn = function () {
            var checkSessionPromise = User.CheckSession();

            if (!isFunction(checkSessionPromise))
                return checkSessionPromise;

            checkSessionPromise.then(function (result) {
                if (result.data.Code != 200) {
                    return false;
                }
                return true;
            });
        };

        $scope.show = function () {
            $ionicLoading.show({
                template: '<p>Loading...</p><ion-spinner></ion-spinner>'
            });
        };

        $scope.hide = function () {
            $ionicLoading.hide();
        };

        $scope.SendMessage = function (isValidForm) {  //model.submit = function (isValid)
           
            form.Message = "";

            if (!isValidForm) {
                form.Message = "You must fill in all fields.";
                return false;
            }
            form.Mail.MessageType = 'ContactAdmin';

            $scope.show($ionicLoading);
            var url = SERVER.url + 'api/Users/SendMessageAsync/';
            
            $http.defaults.headers.common["Authorization"] = 'TreeMon ' + User.SessionId;

            $http.post(url, { Subject: form.Mail.Subject, Message: form.Mail.Message, MessageType: form.Mail.MessageType, ClientType: "mobile.app" })
              .success(function (response) {
                  if (response.Code != 200) {
                      form.Message = response.Message;
                      return false;
                  }
                  form.Message = "Message sent.";
                      //let user see message for a second.
                  setTimeout(function () {
                     form.Mail.Subject = '';
                     form.Mail.Message = '';
                     form.Message = '';
                     $state.go('tabs.home');
                  }, 3300);//3000 = 3 seconds

              }).error(function (data, status, headers, config) {
                  
                  form.Message = "An error occured while sending your message. Try again later.";

                  if (debug && status)
                      form.Message = status + form.Message;

                  return false;
              }).finally(function ($ionicLoading) {
                  // On both cases hide the loading
                  $scope.hide($ionicLoading);
              });
        };

    })
.controller('AccountTabCtrl', function ($scope, $http, $location, User, tmApp) {
    var model = this;
    model.Message = "";

    $scope.Initialize = function () {
             if ( tmApp.RunCount() === 0 ) {
            $location.path('/tab/signup');
            return;
        }

        var checkSessionPromise = User.CheckSession();

        console.log('checkSessionPromise:' + checkSessionPromise);

        if (!isFunction(checkSessionPromise)) {
            if (checkSessionPromise === false) {
                $location.path('/tab/signin');
            }
            return checkSessionPromise;
        }

        checkSessionPromise.then(function (result) {
            if (result.data.Code != 200) {
                $location.path('/tab/signin');
            }
        });
    };

    $scope.IsLoggedIn = function () {
        var checkSessionPromise = User.CheckSession();
       
        if (!isFunction(checkSessionPromise))
            return checkSessionPromise;

        checkSessionPromise.then(function (result) {
            if (result.data.Code != 200) {
                return false;
            }
            return true;
        });
    };

    $scope.Logout = function () {
        User.Logout();
        $location.path('/tab/signin');
    }
   
})
.controller('ImagePickerController', function ($scope, $cordovaImagePicker, $ionicPlatform) {//, $cordovaContacts) {

    $scope.collection = {
        selectedImage: ''
    };

    $ionicPlatform.ready(function () {

        $scope.getImageSaveContact = function () {
            // Image picker will load images according to these settings
            var options = {
                maximumImagesCount: 1, // Max number of selected images, I'm using only one for this example
                width: 800,
                height: 800,
                quality: 80            // Higher is better
            };

            $cordovaImagePicker.getPictures(options).then(function (results) {
                // Loop through acquired images
                for (var i = 0; i < results.length; i++) {
                    $scope.collection.selectedImage = results[i];   // We loading only one image so we can use it like this

                    window.plugins.Base64.encodeFile($scope.collection.selectedImage, function (base64) {  // Encode URI to Base64 needed for contacts plugin
                        $scope.collection.selectedImage = base64;
                        $scope.addContact();    // Save contact
                    });
                }
            }, function (error) {
                console.log('Error: ' + JSON.stringify(error));    // In case of error
            });
        };

    });

    //    $scope.contact = {     // We will use it to save a contact
    //        "displayName": "Gajotres",
    //        "name": {
    //            "givenName": "Dragannn",
    //            "familyName": "Gaiccc",
    //            "formatted": "Dragannn Gaiccc"
    //        },
    //        "nickname": 'Gajotres',
    //        "phoneNumbers": [
    //            {
    //                "value": "+385959052082",
    //                "type": "mobile"
    //            },
    //            {
    //                "value": "+385914600731",
    //                "type": "phone"
    //            }
    //        ],
    //        "emails": [
    //            {
    //                "value": "dragan.gaic@gmail.com",
    //                "type": "home"
    //            }
    //        ],
    //        "addresses": [
    //            {
    //                "type": "home",
    //                "formatted": "Some Address",
    //                "streetAddress": "Some Address",
    //                "locality": "Zagreb",
    //                "region": "Zagreb",
    //                "postalCode": "10000",
    //                "country": "Croatia"
    //            }
    //        ],
    //        "ims": null,
    //        "organizations": [
    //            {
    //                "type": "Company",
    //                "name": "Generali",
    //                "department": "IT",
    //                "title": "Senior Java Developer"
    //            }
    //        ],
    //        "birthday": Date("08/01/1980"),
    //        "note": "",
    //        "photos": [
    //            {
    //                "type": "base64",
    //                "value": $scope.collection.selectedImage
    //            }
    //        ],
    //        "categories": null,
    //        "urls": null
    //    }
    //    $scope.addContact = function () {
    //        $cordovaContacts.save($scope.contact).then(function (result) {
    //            console.log('Contact Saved!');
    //        }, function (err) {
    //            console.log('An error has occured while saving contact data!');
    //        });
    //    };

})
.factory('$localstorage', ['$window', function ($window) {
    return {
        set: function (key, value) {
            $window.localStorage[key] = value;
        },
        get: function (key, defaultValue) {
            return $window.localStorage[key] || defaultValue;
        },
        setObject: function (key, value) {
            $window.localStorage[key] = JSON.stringify(value);
        },
        getObject: function (key) {
            return JSON.parse($window.localStorage[key] || '{}');
        }
    }
}])
.factory('User', function ($q, $http, $localstorage, SERVER) {

    var o = {
        UserId : -1,
        UserName: false,
        AccountId: '',
        SessionId: false,
        Email: '',
        Gender: '',
        Height: '',
        Weight: '',
        BodyFat: '',
        BirthDate: '',
        favorites: [],
        newFavorites: 0
    }

    o.CheckSession = function () {

        // Session is already initialized.
        if (o.SessionId) { return true; }

        // check if there's a session in localstorage from previous use.
        var user = $localstorage.getObject('user');

        if ( !user.SessionId )
            return false;
    
        if (user.SessionId) {//todo set a flag or date to check..
            o.UserId = user.UserId;
            o.UserName = user.UserName;
            o.AccountId = user.AccountId;
            o.SessionId = user.SessionId;
            return true;
        }

        var url = SERVER.url + 'api/Sessions/Status/' + user.SessionId;

        return $http.get(url)
            .success(function (response) {
                if (response.Code != 200) {
                    console.log('LoginAsync.response.Message:' + response.Message);
                    return response.Message;
                }
                o.UserId = user.UserId;
                o.UserName = user.UserName;
                o.AccountId = user.AccountId;
                o.SessionId = user.SessionId;
                //o.populateFavorites().then(function () { defer.resolve(true);});
                return true;
            }).error(function (response) {
                console.log('CheckSession.error:' + response.Message);
                return response.Message;
            });
    }

    //Local only
    o.SetSession = function (userName, userId, accountId, authorizationToken) {

        if (!userName || !userId || !authorizationToken)
            return "Invalid session parameter.";

        o.UserName = userName;
        o.UserId = userId;
        o.AccountId = accountId;
        o.SessionId = authorizationToken;
     
        // set data in localstorage object
        $localstorage.setObject('user', { UserName: userName, UserId: userId, AccountId: accountId, SessionId: authorizationToken });
        return true;
    }
  
    // wipe out our session data
    o.Logout = function () {
        var url = SERVER.url + 'api/Sessions/Delete/';

        return $http.post(url, { UserId: o.UserId, SessionId: o.SessionId, ClientType: "mobile.app" })
            .success(function (response) {
                if (response.Code != 200) {
                    console.log('LoginAsync.response.Message:' + response.Message);
                }
                $localstorage.setObject('user', {});
                o.UserName = false;
                o.SessionId = false;
                o.AccountId = "";
                o.UserId = "";
                o.Email       = "";
                o.Gender      = "";
                o.Height      = "";
                o.Weight      = "";
                o.BodyFat     = "";
                o.BirthDate   = "";
                // o.favorites = [];
                // o.newFavorites = 0;
            }).error(function (response) {
                console.log('CheckSession.error:' + response.Message);
            });
    }

    o.Login = function (userName, password) {

      var url = SERVER.url + 'api/Accounts/LoginAsync';
   
      return $http.post(url, { UserName: userName, Password: password, ClientType: "mobile.app" })
          .success(function (response) {
              if (response.Code != 200) {
                  console.log('LoginAsync.response.Message:' + response.Message);
                  return response.Message;
              }
              var userData = JSON.parse(response.Message);
              var setSessionRes = o.SetSession(userName, userData.UserUUID, userData.AccountUUID, userData.Authorization);
              console.log('setSessionRes' + setSessionRes);
              return setSessionRes;

          }).error(function (response) {
              console.log('LoginAsync.error:' + response.Message);
              return response.Message;
          });
    }

    o.Save = function (userData) {
        if (!userData)
        { return false; }

        console.log('User.Save:' + JSON.stringify(userData));
        $localstorage.setObject('userProfile', userData);

        //todo Save userData
        //has session?
        //is name already registered
        //is email already registered
        //save local
        //settings.hasAcount
        //tmApp use Services post to server
        //    console.log("UserProfileCtrl.SaveUser:" + JSON.stringify(profile));
        //  save to service.        // $http.post('http://posttestserver.com/post.php?dir=jsfiddle', JSON.stringify(data)).success(function () {/*success callback*/ });

        //don't think we need this cause angulars 2 way binding.
        o.UserId = userData.UserId;
        o.UserName   = userData.UserName;
        o.SessionId = userData.SessionId;
        o.AccountId = userData.AccountId;
        o.Email     = userData.Email;
        o.Gender     = userData.Gender;
        o.Height        = userData.Height;
        o.Weight     = userData.Weight;
        o.BodyFat   = userData.BodyFat;
        o.BirthDate = userData.BirthDate;
    }

    o.GetProfile = function () {
          console.log('GetProfile');
        var profile = $localstorage.getObject('userprofile');

        if (profile.UserName) {
            console.log("getting profile:" + profile.UserName);
            return profile;
        }
        console.log('GetProfile:false');
        return false;
    }

    o.Load = function () {
        var userData = $localstorage.getObject('userProfile');
        o.UserName = userData.UserName;
        o.SessionId = userData.SessionId;
        o.Email = userData.Email;
        o.Gender = userData.Gender;
        o.Height = userData.Height;
        o.Weight = userData.Weight;
        o.BodyFat = userData.BodyFat;
        o.BirthDate = userData.BirthDate;
        return o;
    }

    // gets the entire list of this user's favs from server
    o.populateFavorites = function () {
        return $http({
            method: 'GET',
            url: SERVER.url + '/favorites',
            params: { SessionId: o.SessionId }
        }).success(function (data) {
            // merge data into the queue
            o.favorites = data;
        });
    }

    o.addSongToFavorites = function (song) {
        // make sure there's a song to add
        if (!song) return false;

        // add to favorites array
        o.favorites.unshift(song);
        o.newFavorites++;

        // persist this to the server
        return $http.post(SERVER.url + '/favorites', { SessionId: o.SessionId, song_id: song.song_id });
    }

    o.removeSongFromFavorites = function (song, index) {
        // make sure there's a song to add
        if (!song) return false;

        // add to favorites array
        o.favorites.splice(index, 1);

        // persist this to the server
        return $http({
            method: 'DELETE',
            url: SERVER.url + '/favorites',
            params: { SessionId: o.SessionId, song_id: song.song_id }
        });

    }

    o.favoriteCount = function () {
        return o.newFavorites;
    }

    //function requireLogin() {
    //    return $q(function (resolve, reject) {
    //        someCheckAuthenticationMethod().then(function (data) {
    //            // authenticated
    //            resolve(data); // this will allow the state to change
    //        }).catch(function () {
    //            // unauthenticated - initiate login
    //            reject(); // thanks to this the state will not be changed
    //            console.log('Return url: ' + $location.path());
    //            $ionicHistory.nextViewOptions({
    //                disableBack: true,
    //                historyRoot: true
    //            });
    //            // go to the login screen and provide return url (important for views others than the dashboard)
    //            $state.go('app.login', { returnUrl: $location.path() });
    //        });
    //    });
    //}
    return o;
})
.factory('tmApp', function ($q, $ionicHistory, $state, $location, $localstorage) {
    var app = {
        Name: 'TM App',
        Version: 'alpha',
        CopyRight: 'wut 2016'
    }
    var localDb = $localstorage;
  
    app.RunCount = function () {
        var count = parseInt(localDb.getObject('hasRun'));

        if (count && count > 0) {
            count = count + 1; console.log("app has ran:" + count);
            localDb.setObject('hasRun', count);
        } else {
            localDb.setObject('hasRun', 1);
            console.log("first time app launch");
            count = 0;
        }
        return count;
    }
    return app;
})
.constant('SERVER', {
    url: 'http://localhost:65340/'
    //url: ''
})
.directive("compareTo", function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
});

function isFunction(x) {
    return Object.prototype.toString.call(x) == '[object Function]';
}