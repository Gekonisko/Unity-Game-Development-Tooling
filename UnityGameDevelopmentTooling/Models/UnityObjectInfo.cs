namespace UnityGameDevelopmentTooling.Models
{
    public class UnityObjectInfo : IEquatable<UnityObjectInfo>
    {
        public int ClassId { get; private set; }
        public long FileId { get; private set; }
        public bool IsStripped { get; private set; }

        public UnityObjectInfo(int classId, long fileId, bool isStripped = false)
        {
            ClassId = classId;
            FileId = fileId;
            IsStripped = isStripped;
        }

        public bool Equals(UnityObjectInfo other)
        {
            if (other is null)
                return false;

            return ClassId == other.ClassId && FileId == other.FileId;
        }

        public override bool Equals(object obj) => Equals(obj as UnityObjectInfo);

        public override int GetHashCode()
        {
            return HashCode.Combine(ClassId, FileId);
        }

        public override string ToString()
        {
            return $"ClassId:{ClassId} FileId:{FileId}";
        }
    }
}