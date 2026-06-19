using System.Text;

namespace HashUtil
{
    public class FNV1
    {
        private const uint FNV_OFFSET = 2166136261u;
        private const uint FNV_PRIME = 16777619u;
        public static int ComputeHash_24Bytes(string input)
        {
            if(string.IsNullOrEmpty(input))
                return 0;
            uint hash = FNV_OFFSET;
            foreach (byte b in Encoding.UTF8.GetBytes(input))
            {
                hash ^= b;
                hash *= FNV_PRIME;
            }

            int outHash = (int)(hash >> 8);
            return outHash;
        }
    }
}