using UnityEngine;

namespace Utils
{
    [System.Serializable]
    public class SceneObject
    {
        [SerializeField] private string m_SceneName;

        public static implicit operator string(SceneObject sceneObject)
        {
            return sceneObject.m_SceneName;
        }

        public static implicit operator SceneObject(string sceneName)
        {
            return new SceneObject() { m_SceneName = sceneName };
        }
    }
}
