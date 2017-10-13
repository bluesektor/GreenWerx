/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
var app = {
    // Application Constructor
    initialize: function () {
        this.bindEvents();
    },
    // Bind Event Listeners
    //
    // Bind any events that are required on startup. Common events are:
    // 'load', 'deviceready', 'offline', and 'online'.
    bindEvents: function () {
        document.addEventListener('deviceready', this.onDeviceReady, false);
    },
    // deviceready Event Handler
    //
    // The scope of 'this' is the event. In order to call the 'receivedEvent'
    // function, we must explicitly call 'app.receivedEvent(...);'
    onDeviceReady: function () {
        console.log('deviceready');
        app.receivedEvent('deviceready');

        var db = window.sqlitePlugin.openDatabase({ name: "test.db" });
        db.executeSql("DROP TABLE IF EXISTS tt");
        db.executeSql("CREATE TABLE tt (data)");

        $.ajax({
            // THANKS: http://stackoverflow.com/a/8654078/1283667
            url: "https://api.github.com/users/litehelpers/repos",
            dataType: "json",

            success: function (res) {
                console.log('Got AJAX response: ' + JSON.stringify(res));
                //alert('Got AJAX response');
                db.transaction(function (tx) {
                    // http://stackoverflow.com/questions/33240009/jquery-json-cordova-issue
                    $.each(res, function (i, item) {
                        console.log('item: ' + JSON.stringify(item));
                        tx.executeSql("INSERT INTO tt values (?)", JSON.stringify(item));
                    });
                }, function (e) {
                    console.log('Transaction error: ' + e.message);
                    alert('Transaction error: ' + e.message);
                }, function () {
                    db.executeSql('SELECT COUNT(*) FROM tt', [], function (res) {
                        console.log('Check SELECT result: ' + JSON.stringify(res.rows.item(0)));
                        alert('Transaction finished, check record count: ' + JSON.stringify(res.rows.item(0)));
                    });
                });
            },
            error: function (e) {
                console.log('ajax error: ' + JSON.stringify(e));
                alert('ajax error: ' + JSON.stringify(e));
            }
        });
        console.log('sent ajax');
    },
    // Update DOM on a Received Event
    receivedEvent: function (id) {
        var parentElement = document.getElementById(id);
        var listeningElement = parentElement.querySelector('.listening');
        var receivedElement = parentElement.querySelector('.received');

        listeningElement.setAttribute('style', 'display:none;');
        receivedElement.setAttribute('style', 'display:block;');

        console.log('Received Event: ' + id);
    }
};

app.initialize();
