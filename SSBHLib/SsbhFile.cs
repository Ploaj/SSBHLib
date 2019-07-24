using SSBHLib.IO;

namespace SSBHLib
{
    public abstract class SsbhFile
    {
        /// <summary>
        /// This pretty much exists for MATL's only
        /// </summary>
        /// <param name="parser"></param>
        public virtual void PostProcess(SsbhParser parser)
        {

        }

        /// <summary>
        /// This pretty much exists for MATL's only
        /// </summary>
        /// <param name="exporter"></param>
        public virtual void PostWrite(SsbhExporter exporter)
        {

        }
    }
}
