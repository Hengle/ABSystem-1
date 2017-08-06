namespace ABSystem
{
    public class VersionInfo
    {
        public string Version;
    }

    public class AssetBundleInfo
    {
        public string Name;
        public string Hash;

        public bool HasNewVersion(AssetBundleInfo abinfo)
        {
            if(this.Name == abinfo.Name && this.Hash != abinfo.Hash)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            AssetBundleInfo other = (AssetBundleInfo)obj;
            if (this.Name.Equals(other.Name))
            {
                return true;
            }
            else
            {
                return false;
            }
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
}
