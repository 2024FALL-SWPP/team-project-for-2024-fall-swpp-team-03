using UnityEngine.Assertions;

namespace SWPPT3.Main.SystemObject
{
    public class MagicCircleSystem : SystemObjectBase
    {
        private int _progress;

        protected override void IsSatisfied()
        {

        }

        private void ProgressUpdate(int update)
        {
            _progress += update;
            Assert.IsTrue(_progress <= 100 && _progress >= 0, "Progress update failed");
        }
    }
}
