using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ERICAPI.Utils
{
    //[JsonObject(MemberSerialization.Fields)]
    [DataContract]
    public class ServerResponse<T>
    {
        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public string Msg { get; set; }

        [DataMember]
        public T Data { get; set; }

        public ServerResponse(int status)
        {
            this.Status = status;
        }

        public ServerResponse(int status, T data) : this(status)
        {
            this.Data = data;
        }

        public ServerResponse(int status, string msg) : this(status)
        {
            this.Msg = msg;
        }

        public ServerResponse(int status, string msg, T data) : this(status, msg)
        {
            this.Data = data;
        }

        public bool IsSuccess()
        {
            return this.Status == (int)ResponseCode.SUCCESS;
        }

        public static ServerResponse<T> CreateBySuccessMessage(String msg)
        {
            return new ServerResponse<T>((int)ResponseCode.SUCCESS, msg);
        }

        public static ServerResponse<T> CreateBySuccess(T data)
        {
            return new ServerResponse<T>((int)ResponseCode.SUCCESS, data);
        }

        public static ServerResponse<T> CreateBySuccess(String msg, T data)
        {
            return new ServerResponse<T>((int)ResponseCode.SUCCESS, msg, data);
        }


        public static ServerResponse<T> CreateByError()
        {
            return new ServerResponse<T>((int)ResponseCode.ERROR, ResponseCode.ERROR.ToString());
        }


        public static ServerResponse<T> CreateByErrorMessage(String errorMessage)
        {
            return new ServerResponse<T>((int)ResponseCode.ERROR, errorMessage);
        }

        public static ServerResponse<T> CreateByErrorCodeMessage(int errorCode, String errorMessage)
        {
            return new ServerResponse<T>(errorCode, errorMessage);
        }

    }

}
