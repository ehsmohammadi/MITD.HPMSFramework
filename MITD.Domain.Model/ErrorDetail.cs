using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MITD.Domain.Model
{
    [DataContract]
    public class ErrorDetail 
    {
        public ErrorDetail(Dictionary<string,string> messages)
        {
            this.Messages = messages;
        }
        [DataMember]     
        public Dictionary<string,string> Messages {get;set;}
    }
}
