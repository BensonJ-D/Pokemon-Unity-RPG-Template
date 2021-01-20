using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;

    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;
}