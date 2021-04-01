using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttons
{
    public class PlayButton : MonoBehaviour
    {
        public void ChangeScene()
        {
            SceneManager.LoadScene("Battle", LoadSceneMode.Single);
        }
    }
}
