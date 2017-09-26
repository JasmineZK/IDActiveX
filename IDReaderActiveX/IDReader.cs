using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace IDReaderActiveX
{
    [Guid("615840C7-84BA-4727-B4C8-00F04BCB74D6")]
    public class ActiveXIDReader : IObjectSafety
    {
        #region IObjectSafety 成员

        private const string _IID_IDispatch = "{00020400-0000-0000-C000-000000000046}";
        private const string _IID_IDispatchEx = "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}";
        private const string _IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}";
        private const string _IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}";
        private const string _IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}";

        private const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;
        private const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;
        private const int S_OK = 0;
        private const int E_FAIL = unchecked((int)0x80004005);
        private const int E_NOINTERFACE = unchecked((int)0x80004002);

        private bool _fSafeForScripting = true;
        private bool _fSafeForInitializing = true;


        public int GetInterfaceSafetyOptions(ref Guid riid, ref int pdwSupportedOptions, ref int pdwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA;
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForScripting == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForInitializing == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }

        public int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_CALLER) &&
                            (_fSafeForScripting == true))
                        Rslt = S_OK;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_DATA) &&
                            (_fSafeForInitializing == true))
                        Rslt = S_OK;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }

        #endregion

        #region 变量

        private int intReadBaseInfosPhotoRet = 1;

        private int iPort = 1001;

        private string info = null;

        public string path = @"C:\HeadShot"; // 图片存放路径

        #endregion

        #region 动态链接库调用

        [DllImport("Sdtapi.dll")]
        //初始化端口
        private static extern int InitComm(int iPort);

        [DllImport("Sdtapi.dll")]
        //关闭端口
        private static extern int CloseComm();

        [DllImport("Sdtapi.dll")]
        //卡认证
        private static extern int Authenticate();

        [DllImport("Sdtapi.dll")]
        //信息读取
        private static extern int ReadBaseInfos(StringBuilder Name, StringBuilder Gender, StringBuilder Folk, StringBuilder BirthDay, StringBuilder Code, StringBuilder Address, StringBuilder Agency, StringBuilder ExpireStart, StringBuilder ExpireEnd);

        [DllImport("Sdtapi.dll")]
        //判断身份证是否在设备上，只能在读取完身份证信息后使用
        private static extern int CardOn();

        //信息读取并指定保存图片的位置
        [DllImport("Sdtapi.dll")]
        private static extern int ReadBaseInfosPhoto(StringBuilder Name, StringBuilder Gender, StringBuilder Folk, StringBuilder BirthDay, StringBuilder Code, StringBuilder Address, StringBuilder Agency, StringBuilder ExpireStart, StringBuilder ExpireEnd, string Direct);


        #endregion

        #region 读取身份证

        /// <summary>
        /// 读取身份证信息操作
        /// </summary>
        /// <param name="obj"></param>
        public string readID()
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                InitComm(iPort);
                Authenticate();
                info = read();
            }
            catch
            {

            }

            return info;
        }

        /// <summary>
        /// 读取身份证信息
        /// </summary>
        public string read()
        {

            #region 变量

            StringBuilder Name = new StringBuilder(31);
            StringBuilder Gender = new StringBuilder(3);
            StringBuilder Folk = new StringBuilder(10);
            StringBuilder Birth = new StringBuilder(9);
            StringBuilder ID = new StringBuilder(19);
            StringBuilder Address = new StringBuilder(71);
            StringBuilder Agency = new StringBuilder(31);
            StringBuilder Validity_Start = new StringBuilder(9);
            StringBuilder Validity_End = new StringBuilder(9);

            string name, gender, folk, birth, id, address, agency, validity_start, validity_end;

            #endregion

            #region 卡信息读取

            try
            {
                intReadBaseInfosPhotoRet = ReadBaseInfosPhoto(Name, Gender, Folk, Birth, ID, Address, Agency, Validity_Start, Validity_End, path);
            }
            catch
            {

            }

            if (intReadBaseInfosPhotoRet != 1)
            {
                return "CardNotFound";
            }

            #endregion

            #region 用 - 分隔日期

            Birth = Birth.Insert(4, "-");
            Birth = Birth.Insert(7, "-");

            Validity_Start = Validity_Start.Insert(4, "-");
            Validity_Start = Validity_Start.Insert(7, "-");

            Validity_End = Validity_End.Insert(4, "-");
            Validity_End = Validity_End.Insert(7, "-");

            #endregion

            #region 读取到的身份证信息

            id = ID.ToString();

            name = Name.ToString();

            gender = Gender.ToString();

            folk = Folk.ToString();

            birth = Birth.ToString();

            address = Address.ToString();

            agency = Agency.ToString();

            validity_start = Validity_Start.ToString();

            validity_end = Validity_End.ToString();

            #endregion

            #region 信息返回给前端处理

            PostData p = new PostData();

            p.name = name.Trim();

            p.sex = gender.Trim();

            p.nation = folk.Trim();

            p.birthday = birth.Trim();

            p.number = id.Trim();

            p.address = address.Trim();

            p.issue = agency.Trim();

            p.enable_time = validity_start.Trim() + " - " + validity_end.Trim();

            p.photoPath = path;

            return p.ToJsonString();

            #endregion
        }

        #endregion
    }
}

