using System.Collections.Generic;
using UnityEngine;

public enum Scene { WorldView, SummaryView, PartyView, BattleView }

public class SceneController : MonoBehaviour
{
    #region Singleton setup
    private static SceneController _instance;
    public static SceneController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion
    
    private struct SceneState
    {
        public Scene Scene;
        public Vector3 Position;
        public RenderMode RenderMode;
    }
    
    [SerializeField] private List<Canvas> sceneCanvases;
    private SceneState _activeScene = new SceneState{
        Scene = Scene.WorldView,
        Position = Vector3.zero,
        RenderMode = RenderMode.WorldSpace
    };

    public void SetActiveScene(Scene newScene)
    {
        var oldView = sceneCanvases.Find(view => view.name == $"{_activeScene.Scene}_Canvas");
        if (oldView != null)
        {
            oldView.renderMode = _activeScene.RenderMode;
            oldView.transform.position = _activeScene.Position;
        }

        var newView = sceneCanvases.Find(view => view.name == $"{newScene}_Canvas");
        
        _activeScene = new SceneState{
            Scene = newScene,
            Position = newView.transform.position,
            RenderMode = newView.renderMode
        };
        
        if(newView != null) newView.renderMode = RenderMode.ScreenSpaceCamera;
    }
}