using SpaceShip.Utils;
using TMPro;
using UnityEngine;

namespace SpaceShip.Managers
{
    public class FinishManager: SingletonNetwork<FinishManager>
    {
        [SerializeField] private TextMeshProUGUI _coin;

        private void Start()
        {
            _coin.text = SceneLoader.Instance.Coins.ToString();
        }
        
        public void GoToMenu()
        {
            SceneLoader.Instance.LoadScene(SceneName.Menu, false);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}