namespace CrystalCore.Graphics.Core
{
    public interface IPreRenderable : IRenderable
    {

        public void PreDraw(IBatchRenderer renderer);
    }
}
