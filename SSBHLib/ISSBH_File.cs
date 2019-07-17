using SSBHLib.IO;

namespace SSBHLib
{
    public abstract class ISSBH_File
    {
        /// <summary>
        /// This pretty much exists for MATL's only
        /// </summary>
        /// <param name="parser"></param>
        public virtual void PostProcess(SSBHParser parser)
        {

        }

        /// <summary>
        /// This pretty much exists for MATL's only
        /// </summary>
        /// <param name="exporter"></param>
        public virtual void PostWrite(SSBHExporter exporter)
        {

        }
    }
}
