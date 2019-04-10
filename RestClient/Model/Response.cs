using System;
using System.Collections.Generic;
using System.Text;

namespace RestClient.Model
{
    public class Response
    {
        public string Status { get; set; }
        public object Result { get; set; }
        public string[] Mensaje { get; set; }
    }
}
