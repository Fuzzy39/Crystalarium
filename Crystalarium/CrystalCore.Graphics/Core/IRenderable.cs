namespace CrystalCore.Graphics.Core
{
    public interface IRenderable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns>whether the object was rendered.</returns>
        public bool Draw(IRenderer renderer);

    }
}
