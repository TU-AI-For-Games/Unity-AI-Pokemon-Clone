using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    private bool m_isInitialized;

    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            var instances = Resources.FindObjectsOfTypeAll<T>();
            if (instances == null || instances.Length == 0)
            {
                return null;
            }

            instance = instances.FirstOrDefault(i => i.gameObject.scene.buildIndex != -1);
            if (Application.isPlaying)
            {
                instance?.Init();
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null || !instance || !instance.gameObject)
        {
            instance = (T)this;
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
        }
        instance.Init();
    }

    public void Init()
    {
        if (m_isInitialized)
        {
            return;
        }

        SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

        InternalInit();
        m_isInitialized = true;
    }

    private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
    {
        // Sanity
        if (!Instance || gameObject == null)
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            instance = null;
            return;
        }

        SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        instance = null;
    }

    protected abstract void InternalInit();

}