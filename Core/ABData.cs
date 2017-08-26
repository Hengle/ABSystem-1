namespace ABSystem.Inner.Date
{
    public class ABVersion
    {
        public string Version;
    }

    public class ABInfo
    {
        public string Name;
        public string Hash;

        public bool HasNewVersion(ABInfo abinfo)
        {
            return Name == abinfo.Name && Hash != abinfo.Hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ABInfo other = obj as ABInfo;
            if (obj == null) return false;
            return Name.Equals(other.Name) && Hash.Equals(other.Hash);
        }

        public override int GetHashCode()
        {
            // 重写Equals时一般也要重写GetHashCode, 且要保证Equals成立时, HashCode也相等, 反之不然.
            // 这里是模仿jdk5.0中, String类的相应实现.
            int prime = 31; // 一般取一个质数
            int result = 1;
            result = prime * result + (Name == null ? 0 : Name.GetHashCode());
            result = prime * result + (Hash == null ? 0 : Hash.GetHashCode());
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Hash);
        }
    }

    public class ABDownloadItem : ABInfo
    {
        public long TotalBytesToReceive; // 总大小
        public long BytesReceived;  // 已经接收的大小
        public long ProgressPercentage; // 进度
        public string Version;  // 要下载的版本
    }


}
