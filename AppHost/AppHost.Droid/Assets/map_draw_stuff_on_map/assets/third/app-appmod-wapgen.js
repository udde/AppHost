/*
*   Declaration file
*   Declares extension injected from Shell
*/
var Environments;
(function (Environments) {
    Environments[Environments["Node"] = 0] = "Node";
    Environments[Environments["Standalone"] = 1] = "Standalone";
    Environments[Environments["Proxy"] = 2] = "Proxy";
})(Environments || (Environments = {}));
/// <reference path="Models/Enums.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var Environment = (function () {
        function Environment() {
            this._environment = Environments.Standalone;
            if (Environment._instance) {
                throw new Error("Error: Instantiation failed: Use Environment.getInstance() instead of new.");
            }
            this.ResetEnvironment();
            Environment._instance = this;
        }
        Object.defineProperty(Environment.prototype, "CurrentEnvironment", {
            get: function () {
                return this._environment;
            },
            enumerable: true,
            configurable: true
        });
        Environment.getInstance = function () {
            return Environment._instance;
        };
        Environment.prototype.ResetEnvironment = function () {
            this._environment = this.SetEnvironment();
        };
        Environment.prototype.SetEnvironment = function () {
            // If window.external.notify throws an exception it indicates
            // that we are not in a shell, i.e standalone
            try {
                window.external.notify(Environment._testParameter);
                return Environments.Node;
            }
            catch (err) {
                return Environments.Standalone;
            }
        };
        Environment._testParameter = "Test";
        Environment._instance = new Environment();
        return Environment;
    }());
    WapCom.Environment = Environment;
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
    var Path = (function () {
        function Path() {
        }
        Path.JoinOnSep = function (seperator) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            return args.join(seperator);
        };
        Path.Join = function () {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i - 0] = arguments[_i];
            }
            return args.join('');
        };
        Path.Query = function (args) {
            var query = '?';
            var argcount = 0;
            for (var key in args) {
                if (args.hasOwnProperty(key)) {
                    if (argcount++) {
                        query += '&';
                    }
                    query += encodeURIComponent(key) + '=' + encodeURIComponent(args[key]);
                }
            }
            return query;
        };
        return Path;
    }());
    WapCom.Path = Path;
})(WapCom || (WapCom = {}));
/// <reference path="Path.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var PromiseStatus;
    (function (PromiseStatus) {
        PromiseStatus[PromiseStatus["Pending"] = 0] = "Pending";
        PromiseStatus[PromiseStatus["Resolved"] = 1] = "Resolved";
        PromiseStatus[PromiseStatus["Rejected"] = 2] = "Rejected";
    })(PromiseStatus || (PromiseStatus = {}));
    var PromiseAdapter = (function () {
        function PromiseAdapter(callback) {
            this._state = PromiseStatus.Pending;
            this._value = null;
            this._handlers = [];
            this.DoResolve(callback);
        }
        PromiseAdapter.prototype.then = function (success, error, progress) {
            var _this = this;
            // Adding a new promise Impl and connect the last one with done
            return new PromiseAdapter(function (resolve, reject) {
                return _this.done(function (response) {
                    if (typeof success === 'function') {
                        try {
                            return resolve(success(response));
                        }
                        catch (ex) {
                            return reject(ex);
                        }
                    }
                    else {
                        return resolve(response);
                    }
                }, function (errorMessage) {
                    if (typeof error === 'function') {
                        try {
                            return resolve(error(errorMessage));
                        }
                        catch (ex) {
                            return reject(ex);
                        }
                    }
                    else {
                        return reject(errorMessage);
                    }
                });
            });
        };
        PromiseAdapter.prototype.done = function (success, error, progress) {
            var _this = this;
            setTimeout(function () {
                _this.Handle({
                    onResolved: success,
                    onRejected: error
                });
            }, 0);
        };
        ;
        PromiseAdapter.prototype.cancel = function () {
        };
        PromiseAdapter.prototype.OnFullFilled = function (result) {
            var _this = this;
            this._state = PromiseStatus.Resolved;
            this._value = result;
            this._handlers.forEach(function (handler) {
                _this.Handle(handler);
            });
            this._handlers = null;
        };
        PromiseAdapter.prototype.OnRejected = function (error) {
            var _this = this;
            this._state = PromiseStatus.Rejected;
            this._value = error;
            this._handlers.forEach(function (handler) {
                _this.Handle(handler);
            });
            this._handlers = null;
        };
        PromiseAdapter.prototype.Resolve = function (result) {
            try {
                var then = this.GetThen(result);
                if (then) {
                    this.DoResolve(then.bind(result));
                    return;
                }
                this.OnFullFilled(result);
            }
            catch (e) {
                this.OnRejected(e);
            }
        };
        PromiseAdapter.prototype.Handle = function (handler) {
            if (this._state === PromiseStatus.Pending) {
                this._handlers.push(handler);
            }
            else {
                if (this._state === PromiseStatus.Resolved &&
                    typeof handler.onResolved === 'function') {
                    handler.onResolved(this._value);
                }
                if (this._state === PromiseStatus.Rejected &&
                    typeof handler.onRejected === 'function') {
                    handler.onRejected(this._value);
                }
            }
        };
        PromiseAdapter.prototype.DoResolve = function (callback) {
            var _this = this;
            var hasBeenResolved = false;
            try {
                callback(function (value) {
                    if (hasBeenResolved)
                        return;
                    hasBeenResolved = true;
                    _this.Resolve(value);
                }, function (reason) {
                    if (hasBeenResolved)
                        return;
                    hasBeenResolved = true;
                    _this.OnRejected(reason);
                });
            }
            catch (ex) {
                if (hasBeenResolved)
                    return;
                hasBeenResolved = true;
                this.OnRejected(ex);
            }
        };
        PromiseAdapter.prototype.GetThen = function (value) {
            var t = typeof value;
            if (value && (t === 'object' || t === 'function')) {
                var then = value.then;
                if (typeof then === 'function') {
                    return then;
                }
            }
            return null;
        };
        return PromiseAdapter;
    }());
    WapCom.PromiseAdapter = PromiseAdapter;
    var PromiseSimulator = (function () {
        function PromiseSimulator(_simValue) {
            this._simValue = _simValue;
        }
        PromiseSimulator.prototype.then = function (success, error, progress) {
            success(this._simValue);
            return null;
        };
        PromiseSimulator.prototype.done = function (success, error, progress) {
            success(this._simValue);
            return null;
        };
        PromiseSimulator.prototype.cancel = function () {
        };
        return PromiseSimulator;
    }());
    WapCom.PromiseSimulator = PromiseSimulator;
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
    var Configuration = (function () {
        function Configuration(key, value) {
            this.key = key;
            this.value = value;
        }
        return Configuration;
    }());
    WapCom.Configuration = Configuration;
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
    /**
     * Event
     */
    var Event = (function () {
        function Event(key, value) {
            this.Key = key;
            this.EventObject = value;
        }
        return Event;
    }());
    WapCom.Event = Event;
})(WapCom || (WapCom = {}));
var WapCom;
(function (WapCom) {
    'use strict';
    var Location = (function () {
        function Location() {
        }
        Location.prototype.setLocation = function (lat, lng) {
            if (!this.coordinates) {
                this.coordinates = new Coordinates();
            }
            this.coordinates.setLocation(lat, lng);
        };
        Location.prototype.setPosition = function (position) {
            if (!this.coordinates) {
                this.coordinates = new Coordinates();
            }
            this.coordinates.setPosition(position);
        };
        return Location;
    }());
    WapCom.Location = Location;
    var Coordinates = (function () {
        function Coordinates() {
        }
        Coordinates.prototype.setLocation = function (lat, lng) {
            this.latitude = (lat) ? JSON.stringify(lat) : "0";
            this.longitude = (lng) ? JSON.stringify(lng) : "0";
        };
        Coordinates.prototype.setPosition = function (position) {
            this.latitude = "0";
            this.longitude = "0";
            if (position && position.coords) {
                this.latitude = position.coords.latitude;
                this.longitude = position.coords.longitude;
            }
        };
        return Coordinates;
    }());
    WapCom.Coordinates = Coordinates;
    var Structure = (function () {
        function Structure() {
        }
        return Structure;
    }());
    WapCom.Structure = Structure;
})(WapCom || (WapCom = {}));
/// <reference path="../Environment.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var ConfigProvider = (function () {
        function ConfigProvider() {
        }
        ConfigProvider.GetConfigurations = function () {
            return (WapCom.Environment.getInstance().CurrentEnvironment == Environments.Node) ?
                ConfigProvider.nodeGet() : ConfigProvider.standaloneGet();
        };
        ConfigProvider.standaloneGet = function () {
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                var configs = [];
                var query = window.location.search.substr(1);
                var vars = query.split("&");
                for (var i = 0; i < vars.length; i++) {
                    var pair = vars[i].split("=");
                    configs.push(new WapCom.Configuration(pair[0], pair[1]));
                }
                resolve(configs);
            });
        };
        ConfigProvider.nodeGet = function () {
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                try {
                    if (!configurationHandler) {
                        throw new Error("Unable to get configurations, due to configurationHandler is null.");
                    }
                    var configs = configurationHandler.getConfigurations();
                    resolve(configs);
                }
                catch (err) {
                    reject(err);
                }
            });
        };
        return ConfigProvider;
    }());
    WapCom.ConfigProvider = ConfigProvider;
})(WapCom || (WapCom = {}));
/// <reference path="../Environment.ts" />
/// <reference path="../Models/Location.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var GeoProvider = (function () {
        function GeoProvider() {
        }
        GeoProvider.GetLocation = function (lat, lng) {
            if (lat === void 0) { lat = 15.64126; }
            if (lng === void 0) { lng = 58.41659; }
            return (WapCom.Environment.getInstance().CurrentEnvironment == Environments.Node) ?
                GeoProvider.nodeGetLocation(lat, lng) : GeoProvider.standaloneGetLocation(lat, lng);
        };
        GeoProvider.standaloneGetLocation = function (lat, lng) {
            if (lat === void 0) { lat = 15.64126; }
            if (lng === void 0) { lng = 58.41659; }
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                var location = new WapCom.Location();
                location.setLocation(lat, lng);
                // In Chrome for instace, only support location with https
                // http://caniuse.com/#search=location
                if (navigator && navigator.geolocation) {
                    navigator
                        .geolocation
                        .getCurrentPosition(function (position) {
                        location.setPosition(position);
                        resolve(location);
                    }, function (error) {
                        switch (error.code) {
                            case error.PERMISSION_DENIED:
                                resolve(location);
                            case error.POSITION_UNAVAILABLE:
                                reject("Location information is unavailable.");
                            case error.TIMEOUT:
                                reject("The request to get user location timed out.");
                            default:
                                reject("An unknown error occurred.");
                        }
                    });
                }
                else {
                    resolve(location);
                }
            });
        };
        GeoProvider.nodeGetLocation = function (lat, lng) {
            if (lat === void 0) { lat = 15.64126; }
            if (lng === void 0) { lng = 58.41659; }
            if (!geoServiceHandler) {
                throw new Error("Unable to get location, due to geoServiceHandler is null.");
            }
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                try {
                    var location = geoServiceHandler.getLocation();
                    resolve(location);
                }
                catch (err) {
                    reject(err);
                }
            });
        };
        return GeoProvider;
    }());
    WapCom.GeoProvider = GeoProvider;
})(WapCom || (WapCom = {}));
/// <reference path="../../Contract/IServiceClient.ts" />
/// <reference path="../../Helpers/PromiseImplementations.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var ServiceClient = (function () {
        function ServiceClient(name, key) {
            this.Validate(name, key);
            this._key = key;
            this._name = name;
        }
        Object.defineProperty(ServiceClient.prototype, "Key", {
            get: function () {
                return this._key;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(ServiceClient.prototype, "Name", {
            get: function () {
                return this._name;
            },
            enumerable: true,
            configurable: true
        });
        ServiceClient.prototype.Get = function (objectType, take, skip, orderBy, filter) {
            return new WapCom.PromiseSimulator("Get success");
        };
        ServiceClient.prototype.GetOne = function (objectType, id) {
            return new WapCom.PromiseSimulator("GetOne success");
        };
        ServiceClient.prototype.Post = function (objectType, body) {
            return new WapCom.PromiseSimulator("Post success");
        };
        ServiceClient.prototype.Put = function (objectType, id, body) {
            return new WapCom.PromiseSimulator("Put success");
        };
        ServiceClient.prototype.Delete = function (objectType, id) {
            return new WapCom.PromiseSimulator("true");
        };
        ServiceClient.prototype.Validate = function (name, key) {
            if (!name || name == "") {
                throw new Error("Invalid name, please fill out correct name");
            }
            if (!key || key == "") {
                throw new Error("Invalid key, please fill out correct key");
            }
        };
        return ServiceClient;
    }());
    WapCom.ServiceClient = ServiceClient;
})(WapCom || (WapCom = {}));
/// <reference path="../../Contract/IServiceClient.ts" />
/// <reference path="ServiceClient.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var WapCom;
(function (WapCom) {
    'use strict';
    var NodeServiceClient = (function (_super) {
        __extends(NodeServiceClient, _super);
        function NodeServiceClient(name, key, keyToken) {
            _super.call(this, name, key);
            var serviceHandler = this.TryToGetServiceHandler();
            this._serviceProxy = serviceHandler.getService(this.Name, this.Key);
        }
        NodeServiceClient.prototype.Get = function (objectType, take, skip, orderBy, filter) {
            return this._serviceProxy.get(objectType, take, skip, orderBy, filter);
        };
        NodeServiceClient.prototype.GetOne = function (objectType, id) {
            return this._serviceProxy.getOne(objectType, id);
        };
        NodeServiceClient.prototype.Post = function (objectType, body) {
            return this._serviceProxy.post(objectType, body);
        };
        NodeServiceClient.prototype.Put = function (objectType, id, body) {
            return this._serviceProxy.put(objectType, id, body);
        };
        NodeServiceClient.prototype.Delete = function (objectType, id) {
            return this._serviceProxy.delete(objectType, id);
        };
        NodeServiceClient.prototype.TryToGetServiceHandler = function () {
            if (!serviceHandler) {
                throw new Error("Unable to create ServiceClient, due to servicehandler is null");
            }
            return serviceHandler;
        };
        return NodeServiceClient;
    }(WapCom.ServiceClient));
    WapCom.NodeServiceClient = NodeServiceClient;
})(WapCom || (WapCom = {}));
/// <reference path="../../Contract/IServiceClient.ts" />
/// <reference path="../../Helpers/Path.ts" />
/// <reference path="../../Helpers/PromiseImplementations.ts" />
/// <reference path="ServiceClient.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var StandaloneServiceClient = (function (_super) {
        __extends(StandaloneServiceClient, _super);
        function StandaloneServiceClient(name, key, apiUrl, keyToken) {
            if (apiUrl === void 0) { apiUrl = ""; }
            _super.call(this, name, key);
            this._servicePrefix = "services";
            this._jsonContentType = 'application/json';
            this._requestAsync = true;
            this.SetBasePath(apiUrl);
            this._keyToken = keyToken;
        }
        StandaloneServiceClient.prototype.Get = function (objectType, take, skip, orderBy, filter) {
            var _this = this;
            var base = WapCom.Path.JoinOnSep('/', this._basePath, 'types', objectType);
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                var rc = _this.CreateMethodRequest('GET', WapCom.Path.Join(base, WapCom.Path.Query({
                    'take': take,
                    'skip': skip,
                    'orderBy': orderBy,
                    'filter': filter
                })));
                rc.send();
                _this.OnXMLHttpRequestResponse(rc, resolve, reject);
            });
        };
        StandaloneServiceClient.prototype.GetOne = function (objectType, id) {
            var _this = this;
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                var rc = _this.CreateMethodRequest('GET', WapCom.Path.JoinOnSep('/', _this._basePath, 'types', objectType, id));
                rc.send();
                _this.OnXMLHttpRequestResponse(rc, resolve, reject);
            });
        };
        StandaloneServiceClient.prototype.Post = function (objectType, body) {
            var _this = this;
            var base = WapCom.Path.JoinOnSep('/', this._basePath, 'types', objectType);
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                var rc = _this.CreateMethodJsonRequest('POST', base);
                rc.send(body);
                _this.OnXMLHttpRequestResponse(rc, resolve, reject);
            });
        };
        StandaloneServiceClient.prototype.Put = function (objectType, id, body) {
            var _this = this;
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                var rc = _this.CreateMethodJsonRequest('PUT', WapCom.Path.JoinOnSep('/', _this._basePath, 'types', objectType, id));
                rc.send(body);
                _this.OnXMLHttpRequestResponse(rc, resolve, reject);
            });
        };
        StandaloneServiceClient.prototype.Delete = function (objectType, id) {
            var _this = this;
            return new WapCom.PromiseAdapter(function (resolve, reject) {
                var rc = _this.CreateMethodRequest('DELETE', WapCom.Path.JoinOnSep('/', _this._basePath, 'types', objectType, id));
                rc.send();
                _this.OnXMLHttpRequestResponse(rc, resolve, reject);
            });
        };
        StandaloneServiceClient.prototype.OnXMLHttpRequestResponse = function (rc, resolve, reject) {
            var isHttpStatusValid = this.IsHttpStatusValid;
            rc.onload = function () {
                if (isHttpStatusValid(this.status)) {
                    resolve(this.response);
                }
                else {
                    reject(this.statusText);
                }
            };
            rc.onerror = function () {
                reject(this.statusText);
            };
        };
        StandaloneServiceClient.prototype.IsHttpStatusValid = function (status) {
            return (status >= 200 && status < 300);
        };
        StandaloneServiceClient.prototype.CreateRequest = function () {
            return new XMLHttpRequest();
        };
        StandaloneServiceClient.prototype.CreateMethodRequest = function (method, url) {
            var rc = this.CreateRequest();
            rc.open(method, url, this._requestAsync);
            rc.setRequestHeader("X-Key", this.Key);
            rc.setRequestHeader("Key-Token", this._keyToken);
            return rc;
        };
        StandaloneServiceClient.prototype.CreateMethodJsonRequest = function (method, url) {
            var rc = this.CreateMethodRequest(method, url);
            rc.setRequestHeader('Content-type', this._jsonContentType);
            return rc;
        };
        StandaloneServiceClient.prototype.SetBasePath = function (url) {
            if (url) {
                url = this.FilterUrl(url);
                this._basePath = (url.indexOf(this._servicePrefix) === -1) ? WapCom.Path.JoinOnSep('/', url, this._servicePrefix, this.Name) : WapCom.Path.JoinOnSep('/', url, this.Name);
            }
        };
        StandaloneServiceClient.prototype.FilterUrl = function (url) {
            return url.replace(/\/$/, '').toLowerCase();
        };
        return StandaloneServiceClient;
    }(WapCom.ServiceClient));
    WapCom.StandaloneServiceClient = StandaloneServiceClient;
})(WapCom || (WapCom = {}));
/// <reference path="../Environment.ts" />
/// <reference path="../Contract/IServiceClient.ts" />
/// <reference path="ServiceClients/NodeServiceClient.ts" />
/// <reference path="ServiceClients/StandaloneServiceClient.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var ServiceProvider = (function () {
        function ServiceProvider() {
        }
        /**
         * Get a Service
         * @param {string} name - The name of the service.
         * @param {string} key - The key for the service.
         * @param {string} apiUrl - The apiUrl for standalone service.
         */
        ServiceProvider.GetService = function (name, key, apiUrl) {
            if (apiUrl === void 0) { apiUrl = null; }
            if (this.keyToken == null) {
                this.keyToken = ""; // try get from local storage: ms-appdata:///local/keyToken.jwt                
            }
            return (WapCom.Environment.getInstance().CurrentEnvironment == Environments.Node) ?
                new WapCom.NodeServiceClient(name, key, this.keyToken) :
                new WapCom.StandaloneServiceClient(name, key, apiUrl, this.keyToken);
        };
        ServiceProvider.SetServiceKeyToken = function (keyToken) {
            this.keyToken = keyToken;
        };
        return ServiceProvider;
    }());
    WapCom.ServiceProvider = ServiceProvider;
})(WapCom || (WapCom = {}));
/// <reference path="../../Contract/ITrafficWorker.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var TrafficWorker = (function () {
        function TrafficWorker(trafficSettings) {
            this._trafficSettings = trafficSettings;
        }
        TrafficWorker.prototype.onEventReceived = function (event) {
            if (this._trafficSettings.onEventReceived && Object.prototype.toString.call(this._trafficSettings.onEventReceived) === "[object Function]") {
                this._trafficSettings.onEventReceived.call(this, event);
            }
        };
        return TrafficWorker;
    }());
    WapCom.TrafficWorker = TrafficWorker;
})(WapCom || (WapCom = {}));
/// <reference path="../../Contract/ITrafficWorker.ts" />
/// <reference path="Trafficworker.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var NodeTrafficWorker = (function (_super) {
        __extends(NodeTrafficWorker, _super);
        function NodeTrafficWorker(trafficSettings) {
            _super.call(this, trafficSettings);
            window.OutboundEvent = this.OutboundEvent.bind(this);
            window.InboundEvent = this.InboundEvent.bind(this);
        }
        NodeTrafficWorker.prototype.OutboundEvent = function (event) {
            window.external.notify(event);
        };
        NodeTrafficWorker.prototype.InboundEvent = function (event) {
            var parsedEvent = JSON.parse(event);
            _super.prototype.onEventReceived.call(this, parsedEvent);
        };
        return NodeTrafficWorker;
    }(WapCom.TrafficWorker));
    WapCom.NodeTrafficWorker = NodeTrafficWorker;
})(WapCom || (WapCom = {}));
/// <reference path="../../Contract/ITrafficWorker.ts" />
/// <reference path="Trafficworker.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var StandaloneTrafficWorker = (function (_super) {
        __extends(StandaloneTrafficWorker, _super);
        function StandaloneTrafficWorker(trafficSettings) {
            _super.call(this, trafficSettings);
        }
        StandaloneTrafficWorker.prototype.OutboundEvent = function (event) {
        };
        StandaloneTrafficWorker.prototype.InboundEvent = function (event) {
        };
        return StandaloneTrafficWorker;
    }(WapCom.TrafficWorker));
    WapCom.StandaloneTrafficWorker = StandaloneTrafficWorker;
})(WapCom || (WapCom = {}));
/// <reference path="../../Environment.ts" />
/// <reference path="../../Models/Enums.ts" />
/// <reference path="../trafficworkers/nodetrafficworker.ts" />
/// <reference path="../trafficworkers/standalonetrafficworker.ts" />
var WapCom;
(function (WapCom) {
    'use strict';
    var TrafficManager = (function () {
        function TrafficManager(trafficSettings) {
            this.Worker = this.SetEnvironmentWorker(trafficSettings);
        }
        TrafficManager.prototype.SendEvent = function (key, eventObject) {
            var event = this.CreateEvent(key, eventObject);
            this.Worker.OutboundEvent(event);
        };
        TrafficManager.prototype.RequestFocus = function (severity) {
            var event = this.CreateEvent(TrafficManager._focusRequestParameter, severity);
            this.Worker.OutboundEvent(event);
        };
        TrafficManager.prototype.CreateEvent = function (key, eventObject) {
            var event = new WapCom.Event(key, eventObject);
            return JSON.stringify(event);
        };
        TrafficManager.prototype.SetEnvironmentWorker = function (trafficSettings) {
            return (WapCom.Environment.getInstance().CurrentEnvironment == Environments.Node) ?
                new WapCom.NodeTrafficWorker(trafficSettings) :
                new WapCom.StandaloneTrafficWorker(trafficSettings);
        };
        TrafficManager._focusRequestParameter = "FocusRequest";
        return TrafficManager;
    }());
    WapCom.TrafficManager = TrafficManager;
})(WapCom || (WapCom = {}));
