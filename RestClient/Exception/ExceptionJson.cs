using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RestClient.Exception
{

    [Serializable]
    public class ExceptionJson : System.Exception
    {
        HttpStatusCode _httpStatusCode;

        HttpResponseMessage _httpResponseMessage;

        public ExceptionJson(){}

        public ExceptionJson(HttpStatusCode httpStatusCode, HttpResponseMessage httpResponseMessage)
        {
            _httpResponseMessage = httpResponseMessage;
            _httpStatusCode = httpStatusCode;
        }
        public ExceptionJson(HttpStatusCode httpStatusCode, HttpResponseMessage httpResponseMessage,string message, System.Exception inner) : base(message, inner)
        {
            _httpResponseMessage = httpResponseMessage;
            _httpStatusCode = httpStatusCode;
        }

        public ExceptionJson(HttpStatusCode httpStatusCode,string message,HttpResponseMessage httpResponseMessage) { }

        public ExceptionJson(string message) : base(message) { }

        public ExceptionJson(string message, System.Exception inner) : base(message, inner) { }

        protected ExceptionJson(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
