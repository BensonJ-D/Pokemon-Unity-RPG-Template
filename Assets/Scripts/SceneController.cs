using System.Collections.Generic;
using UnityEngine;

public enum Scene { WorldView, SummaryView, PartyView, BattleView, InventoryView }

public class SceneController : MonoBehaviour
{
    #region Singleton setup
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else {
            Instance = this;
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
    private SceneState currentSceneState = new SceneState{
        Scene = Scene.WorldView,
        Position = Vector3.zero,
        RenderMode = RenderMode.WorldSpace
    };

    public void SetActiveScene(Scene newScene)
    {
        var oldView = GetSceneCanvas(currentSceneState.Scene);
        if (oldView != null)
        {
            oldView.renderMode = currentSceneState.RenderMode;
            oldView.transform.position = currentSceneState.Position;
        }

        var newView = GetSceneCanvas(newScene);
        
        currentSceneState = new SceneState{
            Scene = newScene,
            Position = newView.transform.position,
            RenderMode = newView.renderMode
        };
        
        if(newView != null) newView.renderMode = RenderMode.ScreenSpaceCamera;
    }

    private Canvas GetSceneCanvas(Scene scene) =>
        sceneCanvases.Find(view => view.name == $"{scene}_Canvas");
}