using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Collection.Common
{
    [Serializable]
    [DataContract]
    public class MTTable : List<MTKeyValue>, ICloneable
    {
        public MTKeyValue this[string key]
        {
            get
            {
                var model = (from li in this where li.Key == key select li).FirstOrDefault();
                return model;
            }
        }

        public List<string> Keys
        {
            get { return (from li in this select li.Key).ToList(); }
        }

        /// <summary>
        ///     创建一个新的对象，是当前实例副本。
        /// </summary>
        object ICloneable.Clone()
        {
            return Clone();
        }

        public MTTable Remove(string key)
        {
            var model = this.Where(p => p.Key == key).FirstOrDefault();


            if (model != null)
            {
                Remove(model);
            }

            return this;
        }

        public MTTable Clone()
        {
            try
            {
                var stream = new MemoryStream();
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                var obj = formatter.Deserialize(stream);
                return (MTTable) obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    [Serializable]
    [DataContract]
    public class MTKeyValue
    {
        private int mAlarmCount;
        private int mCount;
        private bool mIsAlarm;
        private string mKey;
        private string mMsg;
        private int mNoDataCount;
        private DateTime mStartTime;
        private int mStatus;
        private object mTag;
        private object mValue;

        [DataMember]
        public string Key
        {
            get { return mKey; }
            set { mKey = value; }
        }

        [DataMember]
        public object Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        ///     开始计数时间
        /// </summary>
        [DataMember]
        public DateTime StartTime
        {
            get { return mStartTime; }
            set { mStartTime = value; }
        }

        /// <summary>
        ///     计数
        /// </summary>
        [DataMember]
        public int Count
        {
            get { return mCount; }
            set { mCount = value; }
        }

        /// <summary>
        ///     报警计数
        /// </summary>
        [DataMember]
        public int AlarmCount
        {
            get { return mAlarmCount; }
            set { mAlarmCount = value; }
        }

        /// <summary>
        ///     无数据计数
        /// </summary>
        [DataMember]
        public int NoDataCount
        {
            get { return mNoDataCount; }
            set { mNoDataCount = value; }
        }

        /// <summary>
        ///     信号状态
        /// </summary>
        [DataMember]
        public int Status
        {
            get { return mStatus; }
            set { mStatus = value; }
        }

        /// <summary>
        ///     无数据计数
        /// </summary>
        [DataMember]
        public object Tag
        {
            get { return mTag; }
            set { mTag = value; }
        }

        /// <summary>
        ///     报警信息
        /// </summary>
        [DataMember]
        public string Msg
        {
            get { return mMsg; }
            set { mMsg = value; }
        }

        /// <summary>
        ///     报警是否存储了
        /// </summary>
        [DataMember]
        public bool IsAlarm
        {
            get { return mIsAlarm; }
            set { mIsAlarm = value; }
        }
    }
}