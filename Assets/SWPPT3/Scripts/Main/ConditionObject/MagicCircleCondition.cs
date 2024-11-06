using UnityEngine.Assertions;

namespace SWPPT3.Main.ConditionObject
{
    public class MagicCircleCondition : ConditionObjectBase
    {
        private int _progress;

        protected override void IsSatisfied()
        {

        }

        private void ProgressUpdate(int addProgress)
        {
            _progress += addProgress;
        }
    }
}
