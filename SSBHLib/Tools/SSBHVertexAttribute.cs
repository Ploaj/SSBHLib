namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a generic vector4 attribute
    /// Not all values will be used for every type
    /// </summary>
    public struct SsbhVertexAttribute
    {
        public static readonly SsbhVertexAttribute Zero = new SsbhVertexAttribute(0.0f, 0.0f, 0.0f, 0.0f);

        public static readonly SsbhVertexAttribute One = new SsbhVertexAttribute(1.0f, 1.0f, 1.0f, 1.0f);

        public float X;

        public float Y;

        public float Z;

        public float W;

        public SsbhVertexAttribute(float x, float y, float z, float w = 0)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new System.IndexOutOfRangeException($"Index {index} must be greater or equal to 0 and less than 4.");
                }
            }
            set 
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException($"Index {index} must be greater or equal to 0 and less than 4.");
                }
            }
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z},{W}";
        }
    }
}
