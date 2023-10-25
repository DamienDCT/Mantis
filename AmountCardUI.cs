using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmountCardUI : MonoBehaviour
{
    [SerializeField] private GameObject numberGameObject;
    [SerializeField] private TextMeshProUGUI nbCardValueTxt;

    public void Hide()
    {
        numberGameObject.SetActive(false);
    }

    public void UpdateText(int nbCard)
    {
        Show();
        nbCardValueTxt.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        nbCardValueTxt.text = "" + nbCard;
    }

    public void Show()
    {
        numberGameObject.SetActive(true);
    }
}
