using UnityEngine;
using UnityEngine.SceneManagement;

namespace SignalUtil
{
    public class SignalUnloadPauseMenu
    {
        private bool isActive;
        private int targetScenceCount;
        public void TriggerSignal(int targetScenceCount)
        {
            isActive = true;
            this.targetScenceCount = targetScenceCount;
        }
        public bool Valid()
        {
            if (!isActive)
                return false;

            return targetScenceCount != SceneManager.sceneCount;
        }
        public void Reset()
        {
            isActive = false;
        }
    }
}