﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace SOD.FlightLoadCapacity {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="FlightLoadCapacitySoap", Namespace="http://tempuri.org/")]
    public partial class FlightLoadCapacity : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetFlightLoadCapacityOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetTodaysFlightLoadCapacityOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public FlightLoadCapacity() {
            this.Url = global::SOD.Properties.Settings.Default.SOD_FlightLoadCapacity_FlightLoadCapacity;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetFlightLoadCapacityCompletedEventHandler GetFlightLoadCapacityCompleted;
        
        /// <remarks/>
        public event GetTodaysFlightLoadCapacityCompletedEventHandler GetTodaysFlightLoadCapacityCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFlightLoadCapacity", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public LoadAndCapacity GetFlightLoadCapacity(string UserName, string Password, string FlightNumber, System.DateTime DepartureDate, string DepartureStation, string ArrivalStation) {
            object[] results = this.Invoke("GetFlightLoadCapacity", new object[] {
                        UserName,
                        Password,
                        FlightNumber,
                        DepartureDate,
                        DepartureStation,
                        ArrivalStation});
            return ((LoadAndCapacity)(results[0]));
        }
        
        /// <remarks/>
        public void GetFlightLoadCapacityAsync(string UserName, string Password, string FlightNumber, System.DateTime DepartureDate, string DepartureStation, string ArrivalStation) {
            this.GetFlightLoadCapacityAsync(UserName, Password, FlightNumber, DepartureDate, DepartureStation, ArrivalStation, null);
        }
        
        /// <remarks/>
        public void GetFlightLoadCapacityAsync(string UserName, string Password, string FlightNumber, System.DateTime DepartureDate, string DepartureStation, string ArrivalStation, object userState) {
            if ((this.GetFlightLoadCapacityOperationCompleted == null)) {
                this.GetFlightLoadCapacityOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFlightLoadCapacityOperationCompleted);
            }
            this.InvokeAsync("GetFlightLoadCapacity", new object[] {
                        UserName,
                        Password,
                        FlightNumber,
                        DepartureDate,
                        DepartureStation,
                        ArrivalStation}, this.GetFlightLoadCapacityOperationCompleted, userState);
        }
        
        private void OnGetFlightLoadCapacityOperationCompleted(object arg) {
            if ((this.GetFlightLoadCapacityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFlightLoadCapacityCompleted(this, new GetFlightLoadCapacityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetTodaysFlightLoadCapacity", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public LoadAndCapacity[] GetTodaysFlightLoadCapacity(string UserName, string Password) {
            object[] results = this.Invoke("GetTodaysFlightLoadCapacity", new object[] {
                        UserName,
                        Password});
            return ((LoadAndCapacity[])(results[0]));
        }
        
        /// <remarks/>
        public void GetTodaysFlightLoadCapacityAsync(string UserName, string Password) {
            this.GetTodaysFlightLoadCapacityAsync(UserName, Password, null);
        }
        
        /// <remarks/>
        public void GetTodaysFlightLoadCapacityAsync(string UserName, string Password, object userState) {
            if ((this.GetTodaysFlightLoadCapacityOperationCompleted == null)) {
                this.GetTodaysFlightLoadCapacityOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTodaysFlightLoadCapacityOperationCompleted);
            }
            this.InvokeAsync("GetTodaysFlightLoadCapacity", new object[] {
                        UserName,
                        Password}, this.GetTodaysFlightLoadCapacityOperationCompleted, userState);
        }
        
        private void OnGetTodaysFlightLoadCapacityOperationCompleted(object arg) {
            if ((this.GetTodaysFlightLoadCapacityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTodaysFlightLoadCapacityCompleted(this, new GetTodaysFlightLoadCapacityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class LoadAndCapacity {
        
        private string flightNumberField;
        
        private System.DateTime departureDateField;
        
        private string departureField;
        
        private string arrivalField;
        
        private int loadField;
        
        private int capacityField;
        
        /// <remarks/>
        public string FlightNumber {
            get {
                return this.flightNumberField;
            }
            set {
                this.flightNumberField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime DepartureDate {
            get {
                return this.departureDateField;
            }
            set {
                this.departureDateField = value;
            }
        }
        
        /// <remarks/>
        public string Departure {
            get {
                return this.departureField;
            }
            set {
                this.departureField = value;
            }
        }
        
        /// <remarks/>
        public string Arrival {
            get {
                return this.arrivalField;
            }
            set {
                this.arrivalField = value;
            }
        }
        
        /// <remarks/>
        public int Load {
            get {
                return this.loadField;
            }
            set {
                this.loadField = value;
            }
        }
        
        /// <remarks/>
        public int Capacity {
            get {
                return this.capacityField;
            }
            set {
                this.capacityField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void GetFlightLoadCapacityCompletedEventHandler(object sender, GetFlightLoadCapacityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFlightLoadCapacityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetFlightLoadCapacityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public LoadAndCapacity Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((LoadAndCapacity)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void GetTodaysFlightLoadCapacityCompletedEventHandler(object sender, GetTodaysFlightLoadCapacityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetTodaysFlightLoadCapacityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTodaysFlightLoadCapacityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public LoadAndCapacity[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((LoadAndCapacity[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591