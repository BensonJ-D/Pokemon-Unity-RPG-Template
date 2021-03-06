
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private BattleDialogBox battleDialog;

    public IEnumerator TypeBattleDialog(string dialog)
    {
        yield return battleDialog.TypeDialog(dialog);
    }
    
    public IEnumerator TypeBattleDialog(IEnumerable<string> messages)
    {
        return messages.Select(message => battleDialog.TypeDialog(message)).GetEnumerator();
    }
}