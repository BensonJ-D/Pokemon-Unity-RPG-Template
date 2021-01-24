using UnityEngine;

public enum GameState { Start, Moving, Talking, Battle, Menu }
public static class Game
{
    public static GameState State { get; set; } = GameState.Moving;
}