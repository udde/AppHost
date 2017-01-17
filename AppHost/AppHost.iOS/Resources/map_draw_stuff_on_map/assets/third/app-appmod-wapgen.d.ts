interface External {
    notify(event: string): any;
}
interface Window {
    OutboundEvent: Function;
    InboundEvent: Function;
}
interface ServiceClientDefinition {
    get(objectType: string, take: number, skip: number, orderBy: string, filter: string): Windows.Foundation.IPromise<string>;
    getOne(objectType: string, id: string): Windows.Foundation.IPromise<string>;
    post(objectType: string, body: string): Windows.Foundation.IPromise<string>;
    put(objectType: string, id: string, body: string): Windows.Foundation.IPromise<string>;
    delete(objectType: string, id: string): Windows.Foundation.IPromise<string>;
}
interface ServiceDefinition {
    getService(name: string, key: string): ServiceClientDefinition;
}
declare var serviceHandler: ServiceDefinition;
interface GeoServiceDefinition {
    getLocation(): WapCom.Location;
}
declare var geoServiceHandler: GeoServiceDefinition;
interface ConfigurationDefinition {
    getConfigurations(): WapCom.Configuration[];
}
declare var configurationHandler: ConfigurationDefinition;
declare enum Environments {
    Node = 0,
    Standalone = 1,
    Proxy = 2,
}
declare namespace WapCom {
    class Environment {
        private static _testParameter;
        private static _instance;
        private _environment;
        constructor();
        CurrentEnvironment: Environments;
        static getInstance(): Environment;
        ResetEnvironment(): void;
        private SetEnvironment();
    }
}
declare namespace WapCom {
    interface IServiceClient {
        /**
         * Get
         * @param {string} objectType - objekttyp.
         * @param {number} take - filterargument.
         * @param {number} skip - filterargument.
         * @param {string} orderBy - filterargument.
         * @param {string} filter - filterargument.
         */
        Get(objectType: string, take: number, skip: number, orderBy: string, filter: string): Windows.Foundation.IPromise<string>;
        /**
        * Get by id
        * @param {string} objectType - objekttyp.
        * @param {string} id - Id argument.
        */
        GetOne(objectType: string, id: string): Windows.Foundation.IPromise<string>;
        /**
        * Post
        * @param {string} objectType - objekttyp.
        * @param {string} body - Body argument. Must be a json string.
        */
        Post(objectType: string, body: string): Windows.Foundation.IPromise<string>;
        /**
        * Put
        * @param {string} objectType - objekttyp.
        * @param {string} id  - Id argument.
        * @param {string} body  - Body argument. Must be a json string.
        */
        Put(objectType: string, id: string, body: string): Windows.Foundation.IPromise<string>;
        /**
        * Delete
        * @param {string} objectType - objekttyp.
        * @param {string} id  - Id argument.
        */
        Delete(objectType: string, id: string): Windows.Foundation.IPromise<string>;
    }
}
declare namespace WapCom {
    interface ITrafficManager {
        SendEvent(key: string, eventObject: any): void;
        RequestFocus(severity: number): void;
    }
}
declare namespace WapCom {
    interface ITrafficSettings {
        Configurations: string;
        onEventReceived?(event: any): void;
    }
}
declare namespace WapCom {
    interface ITrafficWorker {
        OutboundEvent(event: string): void;
        InboundEvent(event: any): void;
    }
}
declare namespace WapCom {
    class Path {
        static JoinOnSep(seperator: string, ...args: string[]): string;
        static Join(...args: string[]): string;
        static Query(args: {}): string;
    }
}
declare namespace WapCom {
    class PromiseAdapter<T> implements Windows.Foundation.IPromise<T> {
        private _state;
        private _value;
        private _handlers;
        constructor(callback: (resolve: any, reject: any) => void);
        then<U>(success?: (value: T) => Windows.Foundation.IPromise<U>, error?: (error: any) => Windows.Foundation.IPromise<U>, progress?: (progress: any) => void): Windows.Foundation.IPromise<U>;
        done<U>(success?: (value: any) => any, error?: (error: any) => any, progress?: (progress: any) => void): void;
        cancel(): void;
        private OnFullFilled(result);
        private OnRejected(error);
        private Resolve(result);
        private Handle(handler);
        private DoResolve(callback);
        private GetThen(value);
    }
    class PromiseSimulator<T> implements Windows.Foundation.IPromise<T> {
        private _simValue;
        constructor(_simValue: T);
        then<U>(success?: (value: T) => Windows.Foundation.IPromise<U>, error?: (error: any) => Windows.Foundation.IPromise<U>, progress?: (progress: any) => void): Windows.Foundation.IPromise<U>;
        done<U>(success?: (value: T) => any, error?: (error: any) => any, progress?: (progress: any) => void): void;
        cancel(): void;
    }
}
declare namespace WapCom {
    class Configuration {
        key: string;
        value: string;
        constructor(key: string, value: string);
    }
}
declare namespace WapCom {
    /**
     * Event
     */
    class Event {
        Key: string;
        EventObject: any;
        constructor(key: string, value: any);
    }
}
declare namespace WapCom {
    class Location {
        coordinates: Coordinates;
        structure: Structure;
        setLocation(lat: number, lng: number): void;
        setPosition(position: any): void;
    }
    class Coordinates {
        latitude: string;
        longitude: string;
        setLocation(lat: number, lng: number): void;
        setPosition(position: any): void;
    }
    class Structure {
        buildingId: string;
        stairwellId: string;
        level: string;
    }
}
declare namespace WapCom {
    class ConfigProvider {
        static GetConfigurations(): Windows.Foundation.IPromise<WapCom.Configuration[]>;
        private static standaloneGet();
        private static nodeGet();
    }
}
declare namespace WapCom {
    class GeoProvider {
        static GetLocation(lat?: number, lng?: number): Windows.Foundation.IPromise<WapCom.Location>;
        private static standaloneGetLocation(lat?, lng?);
        private static nodeGetLocation(lat?, lng?);
    }
}
declare namespace WapCom {
    abstract class ServiceClient implements IServiceClient {
        private _name;
        private _key;
        constructor(name: string, key: string);
        protected Key: string;
        protected Name: string;
        Get(objectType: string, take: number, skip: number, orderBy: string, filter: string): Windows.Foundation.IPromise<string>;
        GetOne(objectType: string, id: string): Windows.Foundation.IPromise<string>;
        Post(objectType: string, body: string): Windows.Foundation.IPromise<string>;
        Put(objectType: string, id: string, body: string): Windows.Foundation.IPromise<string>;
        Delete(objectType: string, id: string): Windows.Foundation.IPromise<string>;
        Validate(name: string, key: string): void;
    }
}
declare namespace WapCom {
    class NodeServiceClient extends ServiceClient implements IServiceClient {
        private _serviceProxy;
        constructor(name: string, key: string, keyToken: string);
        Get(objectType: string, take: number, skip: number, orderBy: string, filter: string): Windows.Foundation.IPromise<string>;
        GetOne(objectType: string, id: string): Windows.Foundation.IPromise<string>;
        Post(objectType: string, body: string): Windows.Foundation.IPromise<string>;
        Put(objectType: string, id: string, body: string): Windows.Foundation.IPromise<string>;
        Delete(objectType: string, id: string): Windows.Foundation.IPromise<string>;
        private TryToGetServiceHandler();
    }
}
declare namespace WapCom {
    class StandaloneServiceClient extends ServiceClient implements IServiceClient {
        _servicePrefix: string;
        _jsonContentType: string;
        _requestAsync: boolean;
        _basePath: string;
        _keyToken: string;
        constructor(name: string, key: string, apiUrl: string, keyToken: string);
        Get(objectType: string, take: number, skip: number, orderBy: string, filter: string): Windows.Foundation.IPromise<string>;
        GetOne(objectType: string, id: string): Windows.Foundation.IPromise<string>;
        Post(objectType: string, body: string): Windows.Foundation.IPromise<string>;
        Put(objectType: string, id: string, body: string): Windows.Foundation.IPromise<string>;
        Delete(objectType: string, id: string): Windows.Foundation.IPromise<string>;
        private OnXMLHttpRequestResponse(rc, resolve, reject);
        private IsHttpStatusValid(status);
        private CreateRequest();
        private CreateMethodRequest(method, url);
        private CreateMethodJsonRequest(method, url);
        private SetBasePath(url);
        private FilterUrl(url);
    }
}
declare namespace WapCom {
    class ServiceProvider {
        private static keyToken;
        /**
         * Get a Service
         * @param {string} name - The name of the service.
         * @param {string} key - The key for the service.
         * @param {string} apiUrl - The apiUrl for standalone service.
         */
        static GetService(name: string, key: string, apiUrl?: string): IServiceClient;
        static SetServiceKeyToken(keyToken: string): void;
    }
}
declare namespace WapCom {
    abstract class TrafficWorker implements ITrafficWorker {
        private _trafficSettings;
        constructor(trafficSettings: ITrafficSettings);
        abstract OutboundEvent(event: string): any;
        abstract InboundEvent(event: string): any;
        protected onEventReceived(event: any): void;
    }
}
declare namespace WapCom {
    class NodeTrafficWorker extends TrafficWorker implements ITrafficWorker {
        Configurations: any;
        constructor(trafficSettings: ITrafficSettings);
        OutboundEvent(event: string): void;
        InboundEvent(event: any): void;
    }
}
declare namespace WapCom {
    class StandaloneTrafficWorker extends TrafficWorker implements ITrafficWorker {
        constructor(trafficSettings: ITrafficSettings);
        OutboundEvent(event: string): void;
        InboundEvent(event: any): void;
    }
}
declare namespace WapCom {
    class TrafficManager implements ITrafficManager {
        private static _focusRequestParameter;
        private Worker;
        constructor(trafficSettings: ITrafficSettings);
        SendEvent(key: string, eventObject: any): void;
        RequestFocus(severity: number): void;
        private CreateEvent(key, eventObject);
        private SetEnvironmentWorker(trafficSettings);
    }
}
