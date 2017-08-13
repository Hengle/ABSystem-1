namespace ABSystem
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
            return base.GetHashCode();
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
        public string Version;
    }

}
