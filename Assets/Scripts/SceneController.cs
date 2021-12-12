using System.Collections.Generic;
using UnityEngine;

public enum Scene { WorldView, SummaryView, PartyView, BattleView, InventoryView }

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
    private SceneState _currentSceneState = new SceneState{
        Scene = Scene.WorldView,
        Position = Vector3.zero,
        RenderMode = RenderMode.WorldSpace
    };

    public void SetActiveScene(Scene newScene)
    {
        var oldView = GetSceneCanvas(_currentSceneState.Scene);
        if (oldView != null)
        {
            oldView.renderMode = _currentSceneState.RenderMode;
            oldView.transform.position = _currentSceneState.Position;
        }

        var newView = GetSceneCanvas(newScene);
        
        _currentSceneState = new SceneState{
            Scene = newScene,
            Position = newView.transform.position,
            RenderMode = newView.renderMode
        };
        
        if(newView != null) newView.renderMode = RenderMode.ScreenSpaceCamera;
    }

    private Canvas GetSceneCanvas(Scene scene) =>
        sceneCanvases.Find(view => view.name == $"{scene}_Canvas");
}