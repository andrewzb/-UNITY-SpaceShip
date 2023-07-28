using System.Collections;
using SpaceShip.Managers;
using SpaceShip.Services;
using UnityEngine;

namespace SpaceShip.UI.Controllers
{
  public class AuthorizationController : MonoBehaviour
  {
    private IEnumerator Start()
    {
      yield return new WaitUntil(() => SceneLoader.Instance != null);
      yield return Authentication.Login();
      SceneLoader.Instance.LoadScene(SceneName.Menu, false);
    }
  }
}