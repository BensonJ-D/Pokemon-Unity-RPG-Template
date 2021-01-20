using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private int lettersPerSecond;
    [SerializeField] private Text dialogText;
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            yield return new WaitForSeconds(1f / lettersPerSecond);
            dialogText.text += letter;
        }
    }
}
