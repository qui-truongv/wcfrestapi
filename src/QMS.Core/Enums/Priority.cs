using System.Runtime.Serialization;
namespace QMS.Core.Enums;

[DataContract]
public enum Priority
{
    [EnumMember]
    No = 0,//Không ưu tiên
    [EnumMember]
    Lever1 = 1, //Uu tiên lever 1
    [EnumMember]
    Lever2 = 2,// Ưu tiên lever 2
    [EnumMember]
    Lever3 = 3,//Ưu tiên lever 3
    [EnumMember]
    Lever4 = 4,//Ưu tiên lever 4

}