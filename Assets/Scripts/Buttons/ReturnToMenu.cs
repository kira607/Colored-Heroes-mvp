using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttons
{
    public class ReturnToMenu : MonoBehaviour
    {
        public void Return()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
