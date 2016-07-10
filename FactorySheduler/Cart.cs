﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    class Cart
    {
        private RestClient client;
        public string ip { get; private set; }
        public string errorMessage { get; private set; }

        public Cart(string ip) {
            this.ip = ip;
            
            // client.Authenticator = new HttpBasicAuthenticator(username, password);
        }

        public bool checkConnection() {
            client = new RestClient("http://" + ip + "/");
            RestRequest request = new RestRequest("arduino/{order}/", Method.GET);
            request.AddUrlSegment("order", "digital"); // replaces matching token in request.Resource

            // execute the request
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
