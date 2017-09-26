using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace IDReaderActiveX
{
    public class PostData
    {
        #region 字段
        private string _name;
        private string _sex;
        private string _nation;
        private string _birthday;
        private string _number;
        private string _address;
        private string _issue;
        private string _enable_time;
        private string _photoPath;

        #endregion

        #region 属性

        public string name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string sex
        {
            get
            {
                return _sex;
            }

            set
            {
                _sex = value;
            }
        }

        public string nation
        {
            get
            {
                return _nation;
            }

            set
            {
                _nation = value;
            }
        }

        public string birthday
        {
            get
            {
                return _birthday;
            }

            set
            {
                _birthday = value;
            }
        }

        public string number
        {
            get
            {
                return _number;
            }

            set
            {
                _number = value;
            }
        }

        public string address
        {
            get
            {
                return _address;
            }

            set
            {
                _address = value;
            }
        }

        public string issue
        {
            get
            {
                return _issue;
            }

            set
            {
                _issue = value;
            }
        }

        public string enable_time
        {
            get
            {
                return _enable_time;
            }

            set
            {
                _enable_time = value;
            }
        }

        public string photoPath
        {
            get
            {
                return _photoPath;
            }

            set
            {
                _photoPath = value;
            }
        }

        #endregion

        #region 方法

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
