using UnityEngine;
using UnityEngine.UI;

namespace Menus.Move
{
    public class MoveDetails : MonoBehaviour
    {
        [SerializeField] private Text description;
        [SerializeField] private Text type;
        [SerializeField] private Text maximumPp;
        [SerializeField] private Text currentPp;

        public void SetMoveDetails(PokemonScripts.Moves.Move move)
        {
            if (description != null) description.text = move.Base.Description;
            
            type.text = move == null ? "-" : move.Base.Type.ToString();
            maximumPp.text = move == null ? "-" : move.Base.Pp.ToString();
            currentPp.text = move == null ? "-" : move.Pp.ToString();
        }
    }
}