namespace CrystalCore.View.Core
{
    internal interface IPreRenderable : IRenderable
    {

        public void PreDraw(IBatchRenderer renderer);
    }
}
