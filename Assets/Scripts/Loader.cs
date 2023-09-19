using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene
    {
        MainMenuScene = 0,
        GameScene = 1,
        LoadingScene = 2,
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene((int)Scene.LoadingScene);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene((int)targetScene);
    }

}
