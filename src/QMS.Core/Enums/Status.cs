using System.Runtime.Serialization;

namespace QMS.Core.Enums
{
    [DataContract]
    public enum Status
    {
        [EnumMember]
        Process = 0,//Đang thực hiện
        [EnumMember]
        Wait = 1, //Đang chờ
        [EnumMember]
        Done = 2,//Đã thực hiện xong
        [EnumMember]
        Miss = 3,//Goi nhỡ
        [EnumMember]
        Remove = 4,//Bỏ qua khi gọi nhỡ N lần
        [EnumMember]
        Cancel = -1,//huỷ số tt
        [EnumMember]
        None = 100,//Null

        [EnumMember]
        Process_Step = 5,//Null
        [EnumMember]
        Wait_Step = 6,//Null
        [EnumMember]
        Process_Step_2 = 7,//Null
        [EnumMember]
        Wait_Step_2 = 8//Null
    }
}
